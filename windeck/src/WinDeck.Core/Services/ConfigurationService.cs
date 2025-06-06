using System.IO;
using Newtonsoft.Json;
using WinDeck.Core.Interfaces;
using WinDeck.Core.Models;

namespace WinDeck.Core.Services;

/// <summary>
/// Configuration management service for WinDeck
/// </summary>
public class ConfigurationService : IConfigurationService
{
    private readonly IWinDeckLogger _logger;
    private readonly string _defaultConfigPath;
    private readonly string _backupDirectory;
    private WinDeckConfiguration _current;
    private bool _disposed = false;

    public ConfigurationService(IWinDeckLogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Set up default paths
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var winDeckPath = Path.Combine(appDataPath, "WinDeck");
        _defaultConfigPath = Path.Combine(winDeckPath, "config.json");
        _backupDirectory = Path.Combine(winDeckPath, "Backups");

        // Ensure directories exist
        Directory.CreateDirectory(winDeckPath);
        Directory.CreateDirectory(_backupDirectory);

        // Initialize with default configuration
        _current = CreateDefaultConfiguration();

        _logger.LogInfo("ConfigurationService initialized - Default path: {0}", _defaultConfigPath);
    }

    /// <summary>
    /// Event fired when configuration changes
    /// </summary>
    public event EventHandler<ConfigurationChangedEventArgs>? ConfigurationChanged;

    /// <summary>
    /// Current configuration
    /// </summary>
    public WinDeckConfiguration Current => _current;

    /// <summary>
    /// Load configuration from file
    /// </summary>
    public async Task<bool> LoadAsync(string? filePath = null)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(ConfigurationService));

        var path = filePath ?? _defaultConfigPath;

        try
        {
            if (!File.Exists(path))
            {
                _logger.LogInfo("Configuration file not found at {0}, using defaults", path);
                return await SaveAsync(path); // Save default configuration
            }

            var json = await File.ReadAllTextAsync(path);
            var loadedConfig = JsonConvert.DeserializeObject<WinDeckConfiguration>(json);

            if (loadedConfig == null)
            {
                _logger.LogError("Failed to deserialize configuration from {0}", path);
                return false;
            }

            // Validate loaded configuration
            var validationResult = ValidateConfiguration(loadedConfig);
            if (!validationResult.IsValid)
            {
                _logger.LogError("Loaded configuration is invalid: {0}", string.Join(", ", validationResult.Errors));
                return false;
            }

            if (validationResult.HasWarnings)
            {
                _logger.LogWarning("Configuration has warnings: {0}", string.Join(", ", validationResult.Warnings));
            }

            var oldConfig = _current;
            _current = loadedConfig;

            _logger.LogInfo("Configuration loaded successfully from {0}", path);
            ConfigurationChanged?.Invoke(this, new ConfigurationChangedEventArgs(oldConfig, _current));

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error loading configuration from {0}: {1}", path, ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Save configuration to file
    /// </summary>
    public async Task<bool> SaveAsync(string? filePath = null)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(ConfigurationService));

        var path = filePath ?? _defaultConfigPath;

        try
        {
            // Update last modified timestamp
            _current.LastModified = DateTime.Now;

            // Serialize configuration
            var json = JsonConvert.SerializeObject(_current, Formatting.Indented);

            // Ensure directory exists
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Write to file
            await File.WriteAllTextAsync(path, json);

            _logger.LogInfo("Configuration saved successfully to {0}", path);

            // Auto-backup if enabled
            if (_current.Application.AutoBackupSettings)
            {
                await CreateBackupAsync();
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error saving configuration to {0}: {1}", path, ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Update configuration
    /// </summary>
    public async Task UpdateAsync(WinDeckConfiguration config, bool saveImmediately = true)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(ConfigurationService));

        if (config == null)
            throw new ArgumentNullException(nameof(config));

        var oldConfig = _current;
        _current = config;

        _logger.LogInfo("Configuration updated");
        ConfigurationChanged?.Invoke(this, new ConfigurationChangedEventArgs(oldConfig, _current));

        if (saveImmediately)
        {
            await SaveAsync();
        }
    }

    /// <summary>
    /// Reset configuration to defaults
    /// </summary>
    public async Task ResetToDefaultsAsync(bool saveImmediately = true)
    {
        var oldConfig = _current;
        _current = CreateDefaultConfiguration();

        _logger.LogInfo("Configuration reset to defaults");
        ConfigurationChanged?.Invoke(this, new ConfigurationChangedEventArgs(oldConfig, _current));

        if (saveImmediately)
        {
            await SaveAsync();
        }
    }

    /// <summary>
    /// Create a backup of current configuration
    /// </summary>
    public async Task<string> CreateBackupAsync()
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var backupFileName = $"config_backup_{timestamp}.json";
        var backupPath = Path.Combine(_backupDirectory, backupFileName);

        await SaveAsync(backupPath);

        // Clean up old backups if needed
        await CleanupOldBackupsAsync();

        _logger.LogInfo("Configuration backup created: {0}", backupPath);
        return backupPath;
    }

    /// <summary>
    /// Restore configuration from backup
    /// </summary>
    public async Task<bool> RestoreFromBackupAsync(string backupFilePath)
    {
        if (!File.Exists(backupFilePath))
        {
            _logger.LogError("Backup file not found: {0}", backupFilePath);
            return false;
        }

        var success = await LoadAsync(backupFilePath);
        if (success)
        {
            _logger.LogInfo("Configuration restored from backup: {0}", backupFilePath);
            await SaveAsync(); // Save to main config file
        }

        return success;
    }

    /// <summary>
    /// Get all available backup files
    /// </summary>
    public async Task<List<string>> GetBackupFilesAsync()
    {
        return await Task.Run(() =>
        {
            if (!Directory.Exists(_backupDirectory))
                return new List<string>();

            return Directory.GetFiles(_backupDirectory, "config_backup_*.json")
                           .OrderByDescending(f => File.GetCreationTime(f))
                           .ToList();
        });
    }

    /// <summary>
    /// Validate configuration integrity
    /// </summary>
    public ConfigurationValidationResult ValidateConfiguration(WinDeckConfiguration? config = null)
    {
        var result = new ConfigurationValidationResult { IsValid = true };
        var configToValidate = config ?? _current;

        try
        {
            // Validate shortcuts
            var shortcutNumbers = new HashSet<int>();
            foreach (var shortcut in configToValidate.Shortcuts)
            {
                if (shortcut.Number < 0 || shortcut.Number > 9)
                {
                    result.Errors.Add($"Shortcut number {shortcut.Number} is invalid (must be 0-9)");
                    result.IsValid = false;
                }

                if (shortcutNumbers.Contains(shortcut.Number))
                {
                    result.Errors.Add($"Duplicate shortcut number: {shortcut.Number}");
                    result.IsValid = false;
                }
                shortcutNumbers.Add(shortcut.Number);

                if (string.IsNullOrWhiteSpace(shortcut.Name))
                {
                    result.Warnings.Add($"Shortcut {shortcut.Number} has no name");
                }

                if (string.IsNullOrWhiteSpace(shortcut.Target))
                {
                    result.Warnings.Add($"Shortcut {shortcut.Number} has no target");
                }
            }

            // Validate overlay configuration
            if (configToValidate.Overlay.Width <= 0 || configToValidate.Overlay.Height <= 0)
            {
                result.Errors.Add("Overlay dimensions must be positive");
                result.IsValid = false;
            }

            if (configToValidate.Overlay.Opacity < 0 || configToValidate.Overlay.Opacity > 1)
            {
                result.Errors.Add("Overlay opacity must be between 0 and 1");
                result.IsValid = false;
            }

            // Validate hotkey configuration
            if (!Enum.IsDefined(typeof(System.Windows.Forms.Keys), configToValidate.Hotkeys.PrimaryHotkey))
            {
                result.Errors.Add($"Invalid primary hotkey: {configToValidate.Hotkeys.PrimaryHotkey}");
                result.IsValid = false;
            }

            _logger.LogDebug("Configuration validation completed - Valid: {0}, Errors: {1}, Warnings: {2}",
                result.IsValid, result.Errors.Count, result.Warnings.Count);
        }
        catch (Exception ex)
        {
            result.Errors.Add($"Validation error: {ex.Message}");
            result.IsValid = false;
            _logger.LogError("Configuration validation failed: {0}", ex.Message);
        }

        return result;
    }

    /// <summary>
    /// Get default configuration file path
    /// </summary>
    public string GetDefaultConfigPath() => _defaultConfigPath;

    #region Private Methods

    private static WinDeckConfiguration CreateDefaultConfiguration()
    {
        var config = new WinDeckConfiguration();

        // Add some default shortcuts
        config.Shortcuts.AddRange(new[]
        {
            new ShortcutConfiguration { Number = 1, Name = "Calculator", ActionType = ShortcutActionType.Application, Target = "calc.exe", IsEnabled = true },
            new ShortcutConfiguration { Number = 2, Name = "Notepad", ActionType = ShortcutActionType.Application, Target = "notepad.exe", IsEnabled = true },
            new ShortcutConfiguration { Number = 3, Name = "File Explorer", ActionType = ShortcutActionType.Application, Target = "explorer.exe", IsEnabled = true }
        });

        return config;
    }

    private async Task CleanupOldBackupsAsync()
    {
        try
        {
            var backupFiles = await GetBackupFilesAsync();
            var maxFiles = _current.Application.MaxBackupFiles;

            if (backupFiles.Count > maxFiles)
            {
                var filesToDelete = backupFiles.Skip(maxFiles);
                foreach (var file in filesToDelete)
                {
                    File.Delete(file);
                    _logger.LogDebug("Deleted old backup: {0}", file);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Error cleaning up old backups: {0}", ex.Message);
        }
    }

    #endregion

    #region IDisposable Implementation

    public void Dispose()
    {
        if (!_disposed)
        {
            // Save configuration on dispose
            try
            {
                SaveAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error saving configuration during dispose: {0}", ex.Message);
            }

            _disposed = true;
            _logger.LogInfo("ConfigurationService disposed");
        }
    }

    #endregion
}
