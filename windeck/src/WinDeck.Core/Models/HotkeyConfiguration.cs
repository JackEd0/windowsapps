using System.Windows.Forms;

namespace WinDeck.Core.Models;

/// <summary>
/// Configuration for global hotkeys
/// </summary>
public class HotkeyConfiguration
{
    /// <summary>
    /// Primary hotkey to show/hide WinDeck overlay (default: F10)
    /// </summary>
    public Keys PrimaryHotkey { get; set; } = Keys.F10;

    /// <summary>
    /// Modifier keys for primary hotkey (default: None)
    /// </summary>
    public ModifierKeys PrimaryModifiers { get; set; } = ModifierKeys.None;

    /// <summary>
    /// Secondary hotkey for quick access (default: Ctrl+Space)
    /// </summary>
    public Keys SecondaryHotkey { get; set; } = Keys.Space;

    /// <summary>
    /// Modifier keys for secondary hotkey (default: Control)
    /// </summary>
    public ModifierKeys SecondaryModifiers { get; set; } = ModifierKeys.Control;

    /// <summary>
    /// Whether hotkeys are enabled
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Whether to suppress hotkey when other applications have focus
    /// </summary>
    public bool SuppressInFullscreen { get; set; } = true;
}

/// <summary>
/// Modifier keys for hotkey combinations
/// </summary>
[Flags]
public enum ModifierKeys
{
    None = 0,
    Alt = 1,
    Control = 2,
    Shift = 4,
    Windows = 8
}

/// <summary>
/// Hotkey event arguments
/// </summary>
public class HotkeyEventArgs : EventArgs
{
    public Keys Key { get; }
    public ModifierKeys Modifiers { get; }
    public DateTime Timestamp { get; }

    public HotkeyEventArgs(Keys key, ModifierKeys modifiers)
    {
        Key = key;
        Modifiers = modifiers;
        Timestamp = DateTime.Now;
    }
}
