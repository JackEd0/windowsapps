using WinDeck.Core.Models;

namespace WinDeck.Core.Interfaces;

/// <summary>
/// Service for managing WinDeck configuration
/// </summary>
public interface IConfigurationService : IDisposable
{
    /// <summary>
    /// Event fired when configuration changes
    /// </summary>
    event EventHandler<ConfigurationChangedEventArgs>? ConfigurationChanged;

    /// <summary>
    /// Current configuration
    /// </summary>
    WinDeckConfiguration Current { get; }

    /// <summary>
    /// Load configuration from file
    /// </summary>
    /// <param name="filePath">Path to configuration file (optional, uses default if null)</param>
    /// <returns>True if loaded successfully</returns>
    Task<bool> LoadAsync(string? filePath = null);

    /// <summary>
    /// Save configuration to file
    /// </summary>
    /// <param name="filePath">Path to configuration file (optional, uses default if null)</param>
    /// <returns>True if saved successfully</returns>
    Task<bool> SaveAsync(string? filePath = null);

    /// <summary>
    /// Update configuration
    /// </summary>
    /// <param name="config">New configuration</param>
    /// <param name="saveImmediately">Whether to save to file immediately</param>
    Task UpdateAsync(WinDeckConfiguration config, bool saveImmediately = true);

    /// <summary>
    /// Reset configuration to defaults
    /// </summary>
    /// <param name="saveImmediately">Whether to save to file immediately</param>
    Task ResetToDefaultsAsync(bool saveImmediately = true);

    /// <summary>
    /// Create a backup of current configuration
    /// </summary>
    /// <returns>Path to backup file</returns>
    Task<string> CreateBackupAsync();

    /// <summary>
    /// Restore configuration from backup
    /// </summary>
    /// <param name="backupFilePath">Path to backup file</param>
    /// <returns>True if restored successfully</returns>
    Task<bool> RestoreFromBackupAsync(string backupFilePath);

    /// <summary>
    /// Get all available backup files
    /// </summary>
    /// <returns>List of backup file paths</returns>
    Task<List<string>> GetBackupFilesAsync();

    /// <summary>
    /// Validate configuration integrity
    /// </summary>
    /// <param name="config">Configuration to validate (uses current if null)</param>
    /// <returns>Validation result</returns>
    ConfigurationValidationResult ValidateConfiguration(WinDeckConfiguration? config = null);

    /// <summary>
    /// Get default configuration file path
    /// </summary>
    string GetDefaultConfigPath();
}

/// <summary>
/// Event arguments for configuration changes
/// </summary>
public class ConfigurationChangedEventArgs : EventArgs
{
    public WinDeckConfiguration OldConfiguration { get; }
    public WinDeckConfiguration NewConfiguration { get; }
    public DateTime Timestamp { get; }

    public ConfigurationChangedEventArgs(WinDeckConfiguration oldConfig, WinDeckConfiguration newConfig)
    {
        OldConfiguration = oldConfig;
        NewConfiguration = newConfig;
        Timestamp = DateTime.Now;
    }
}

/// <summary>
/// Configuration validation result
/// </summary>
public class ConfigurationValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
    public List<string> Warnings { get; set; } = new List<string>();

    public bool HasErrors => Errors.Count > 0;
    public bool HasWarnings => Warnings.Count > 0;
}
