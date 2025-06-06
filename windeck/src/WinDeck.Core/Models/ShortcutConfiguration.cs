namespace WinDeck.Core.Models;

/// <summary>
/// Configuration for a single shortcut action
/// </summary>
public class ShortcutConfiguration
{
    /// <summary>
    /// Shortcut number (0-9)
    /// </summary>
    public int Number { get; set; }

    /// <summary>
    /// Display name of the shortcut
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of what the shortcut does
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Type of action to perform
    /// </summary>
    public ShortcutActionType ActionType { get; set; } = ShortcutActionType.Application;

    /// <summary>
    /// Target for the action (file path, URL, command, etc.)
    /// </summary>
    public string Target { get; set; } = string.Empty;

    /// <summary>
    /// Arguments to pass to the target (for applications and scripts)
    /// </summary>
    public string Arguments { get; set; } = string.Empty;

    /// <summary>
    /// Working directory for the action
    /// </summary>
    public string WorkingDirectory { get; set; } = string.Empty;

    /// <summary>
    /// Icon path or name for the shortcut
    /// </summary>
    public string Icon { get; set; } = string.Empty;

    /// <summary>
    /// Whether the shortcut is enabled
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Whether to run the action as administrator
    /// </summary>
    public bool RunAsAdmin { get; set; } = false;

    /// <summary>
    /// Window state for launched applications
    /// </summary>
    public WindowState WindowState { get; set; } = WindowState.Normal;

    /// <summary>
    /// Background color for the shortcut button (hex format)
    /// </summary>
    public string BackgroundColor { get; set; } = "#404040";

    /// <summary>
    /// Text color for the shortcut button (hex format)
    /// </summary>
    public string TextColor { get; set; } = "#FFFFFF";

    /// <summary>
    /// Tags for organizing shortcuts
    /// </summary>
    public List<string> Tags { get; set; } = new List<string>();

    /// <summary>
    /// Custom hotkey for this specific shortcut (optional)
    /// </summary>
    public string? CustomHotkey { get; set; }

    /// <summary>
    /// Last used timestamp
    /// </summary>
    public DateTime? LastUsed { get; set; }

    /// <summary>
    /// Usage count for analytics
    /// </summary>
    public int UsageCount { get; set; } = 0;
}

/// <summary>
/// Types of actions that can be performed by shortcuts
/// </summary>
public enum ShortcutActionType
{
    /// <summary>
    /// Launch an application
    /// </summary>
    Application,

    /// <summary>
    /// Open a file or folder
    /// </summary>
    File,

    /// <summary>
    /// Open a URL in default browser
    /// </summary>
    Url,

    /// <summary>
    /// Execute a PowerShell script
    /// </summary>
    PowerShell,

    /// <summary>
    /// Execute a batch/command script
    /// </summary>
    Batch,

    /// <summary>
    /// Send keyboard shortcut to active window
    /// </summary>
    Hotkey,

    /// <summary>
    /// Custom system action (shutdown, restart, etc.)
    /// </summary>
    System,

    /// <summary>
    /// Text snippet insertion
    /// </summary>
    Text
}

/// <summary>
/// Window states for launched applications
/// </summary>
public enum WindowState
{
    Normal,
    Minimized,
    Maximized,
    Hidden
}
