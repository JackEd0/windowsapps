using System.Windows.Forms;
using WinDeck.Core.Models;

namespace WinDeck.Core.Interfaces;

/// <summary>
/// Service for managing global hotkeys
/// </summary>
public interface IHotkeyService : IDisposable
{
    /// <summary>
    /// Event fired when a registered hotkey is pressed
    /// </summary>
    event EventHandler<HotkeyEventArgs>? HotkeyPressed;

    /// <summary>
    /// Register a hotkey with the system
    /// </summary>
    /// <param name="id">Unique identifier for the hotkey</param>
    /// <param name="key">Key to register</param>
    /// <param name="modifiers">Modifier keys</param>
    /// <returns>True if registration was successful</returns>
    bool RegisterHotkey(int id, Keys key, ModifierKeys modifiers);

    /// <summary>
    /// Unregister a hotkey
    /// </summary>
    /// <param name="id">Hotkey identifier to unregister</param>
    /// <returns>True if unregistration was successful</returns>
    bool UnregisterHotkey(int id);

    /// <summary>
    /// Register hotkeys from configuration
    /// </summary>
    /// <param name="config">Hotkey configuration</param>
    /// <returns>True if all hotkeys were registered successfully</returns>
    bool RegisterHotkeys(HotkeyConfiguration config);

    /// <summary>
    /// Unregister all hotkeys
    /// </summary>
    void UnregisterAllHotkeys();

    /// <summary>
    /// Check if a hotkey is currently registered
    /// </summary>
    /// <param name="id">Hotkey identifier</param>
    /// <returns>True if the hotkey is registered</returns>
    bool IsHotkeyRegistered(int id);

    /// <summary>
    /// Get all registered hotkey IDs
    /// </summary>
    /// <returns>Array of registered hotkey IDs</returns>
    int[] GetRegisteredHotkeyIds();
}
