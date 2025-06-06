using Newtonsoft.Json;

namespace WinDeck.Core.Models;

/// <summary>
/// Main configuration class for WinDeck application
/// </summary>
public class WinDeckConfiguration
{
    /// <summary>
    /// Logging configuration
    /// </summary>
    public LoggingConfiguration Logging { get; set; } = new LoggingConfiguration();

    /// <summary>
    /// Hotkey configuration
    /// </summary>
    public HotkeyConfiguration Hotkeys { get; set; } = new HotkeyConfiguration();

    /// <summary>
    /// Overlay appearance configuration
    /// </summary>
    public OverlayConfiguration Overlay { get; set; } = new OverlayConfiguration();

    /// <summary>
    /// Application settings
    /// </summary>
    public ApplicationConfiguration Application { get; set; } = new ApplicationConfiguration();

    /// <summary>
    /// Shortcut configurations
    /// </summary>
    public List<ShortcutConfiguration> Shortcuts { get; set; } = new List<ShortcutConfiguration>();

    /// <summary>
    /// Configuration file version for migration support
    /// </summary>
    public string Version { get; set; } = "1.0.0";

    /// <summary>
    /// When the configuration was last modified
    /// </summary>
    public DateTime LastModified { get; set; } = DateTime.Now;
}

/// <summary>
/// General application configuration
/// </summary>
public class ApplicationConfiguration
{
    /// <summary>
    /// Whether to start with Windows
    /// </summary>
    public bool StartWithWindows { get; set; } = false;

    /// <summary>
    /// Whether to start minimized to system tray
    /// </summary>
    public bool StartMinimized { get; set; } = true;

    /// <summary>
    /// Whether to show notifications
    /// </summary>
    public bool ShowNotifications { get; set; } = true;

    /// <summary>
    /// Whether to check for updates automatically
    /// </summary>
    public bool CheckForUpdates { get; set; } = true;

    /// <summary>
    /// Language/locale setting
    /// </summary>
    public string Language { get; set; } = "en-US";

    /// <summary>
    /// Theme setting (Light, Dark, System)
    /// </summary>
    public string Theme { get; set; } = "Dark";

    /// <summary>
    /// Whether to collect anonymous usage statistics
    /// </summary>
    public bool CollectAnalytics { get; set; } = false;

    /// <summary>
    /// Backup settings automatically
    /// </summary>
    public bool AutoBackupSettings { get; set; } = true;

    /// <summary>
    /// Maximum number of backup files to keep
    /// </summary>
    public int MaxBackupFiles { get; set; } = 5;
}
