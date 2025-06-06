using WinDeck.Core.Models;

namespace WinDeck.Core.Interfaces;

/// <summary>
/// Service for managing the overlay window
/// </summary>
public interface IOverlayService : IDisposable
{
    /// <summary>
    /// Event fired when a shortcut is activated from the overlay
    /// </summary>
    event EventHandler<ShortcutActivatedEventArgs>? ShortcutActivated;

    /// <summary>
    /// Event fired when the overlay visibility changes
    /// </summary>
    event EventHandler<OverlayVisibilityChangedEventArgs>? VisibilityChanged;

    /// <summary>
    /// Whether the overlay is currently visible
    /// </summary>
    bool IsVisible { get; }

    /// <summary>
    /// Show the overlay
    /// </summary>
    void Show();

    /// <summary>
    /// Hide the overlay
    /// </summary>
    void Hide();

    /// <summary>
    /// Toggle overlay visibility
    /// </summary>
    void Toggle();

    /// <summary>
    /// Update the overlay configuration
    /// </summary>
    /// <param name="config">New overlay configuration</param>
    void UpdateConfiguration(OverlayConfiguration config);

    /// <summary>
    /// Update the shortcuts displayed in the overlay
    /// </summary>
    /// <param name="shortcuts">New shortcut configurations</param>
    void UpdateShortcuts(IEnumerable<ShortcutConfiguration> shortcuts);

    /// <summary>
    /// Initialize the overlay service
    /// </summary>
    /// <param name="overlayConfig">Overlay configuration</param>
    /// <param name="shortcuts">Initial shortcuts</param>
    void Initialize(OverlayConfiguration overlayConfig, IEnumerable<ShortcutConfiguration> shortcuts);
}

/// <summary>
/// Event arguments for shortcut activation
/// </summary>
public class ShortcutActivatedEventArgs : EventArgs
{
    public int Number { get; }
    public string Name { get; }
    public ShortcutConfiguration? Action { get; }
    public DateTime Timestamp { get; }

    public ShortcutActivatedEventArgs(int number, string name, ShortcutConfiguration? action)
    {
        Number = number;
        Name = name;
        Action = action;
        Timestamp = DateTime.Now;
    }
}

/// <summary>
/// Event arguments for overlay visibility changes
/// </summary>
public class OverlayVisibilityChangedEventArgs : EventArgs
{
    public bool IsVisible { get; }
    public DateTime Timestamp { get; }

    public OverlayVisibilityChangedEventArgs(bool isVisible)
    {
        IsVisible = isVisible;
        Timestamp = DateTime.Now;
    }
}
