using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WinDeck.Core.Interfaces;
using WinDeck.Core.Models;

namespace WinDeck.Core.Services;

/// <summary>
/// Windows-specific implementation of global hotkey service
/// </summary>
public class HotkeyService : IHotkeyService
{
    #region Windows API Imports

    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    [DllImport("kernel32.dll")]
    private static extern uint GetLastError();

    // Windows constants for hotkey modifiers
    private const uint MOD_ALT = 0x0001;
    private const uint MOD_CONTROL = 0x0002;
    private const uint MOD_SHIFT = 0x0004;
    private const uint MOD_WIN = 0x0008;
    private const uint MOD_NOREPEAT = 0x4000;

    // Windows message for hotkey
    private const int WM_HOTKEY = 0x0312;

    #endregion

    private readonly IWinDeckLogger _logger;
    private readonly ConcurrentDictionary<int, HotkeyInfo> _registeredHotkeys;
    private readonly MessageWindow _messageWindow;
    private bool _disposed = false;

    /// <summary>
    /// Event fired when a registered hotkey is pressed
    /// </summary>
    public event EventHandler<HotkeyEventArgs>? HotkeyPressed;

    public HotkeyService(IWinDeckLogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _registeredHotkeys = new ConcurrentDictionary<int, HotkeyInfo>();
        _messageWindow = new MessageWindow(OnHotkeyMessage);

        _logger.LogInfo("HotkeyService initialized");
    }

    /// <summary>
    /// Register a hotkey with the system
    /// </summary>
    public bool RegisterHotkey(int id, Keys key, ModifierKeys modifiers)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(HotkeyService));

        try
        {
            // Check if hotkey is already registered
            if (_registeredHotkeys.ContainsKey(id))
            {
                _logger.LogWarning("Hotkey ID {0} is already registered", id);
                return false;
            }

            // Convert modifiers to Windows API format
            uint winModifiers = ConvertModifiers(modifiers);

            // Add no-repeat flag to prevent spam
            winModifiers |= MOD_NOREPEAT;

            // Register with Windows
            bool success = RegisterHotKey(_messageWindow.Handle, id, winModifiers, (uint)key);

            if (success)
            {
                var hotkeyInfo = new HotkeyInfo(id, key, modifiers, DateTime.Now);
                _registeredHotkeys.TryAdd(id, hotkeyInfo);

                _logger.LogInfo("Successfully registered hotkey: {0}+{1} (ID: {2})",
                    modifiers, key, id);
                return true;
            }
            else
            {
                uint error = GetLastError();                _logger.LogError("Failed to register hotkey {Modifiers}+{Key} (ID: {Id}). Error: {Error}",
                    null, modifiers, key, id, error);
                return false;
            }
        }
        catch (Exception ex)
        {            _logger.LogError("Exception registering hotkey {Modifiers}+{Key} (ID: {Id}): {Message}",
                ex, modifiers, key, id, ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Unregister a hotkey
    /// </summary>
    public bool UnregisterHotkey(int id)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(HotkeyService));

        try
        {
            if (!_registeredHotkeys.TryGetValue(id, out var hotkeyInfo))
            {
                _logger.LogWarning("Hotkey ID {0} is not registered", id);
                return false;
            }

            bool success = UnregisterHotKey(_messageWindow.Handle, id);

            if (success)
            {
                _registeredHotkeys.TryRemove(id, out _);
                _logger.LogInfo("Successfully unregistered hotkey: {0}+{1} (ID: {2})",
                    hotkeyInfo.Modifiers, hotkeyInfo.Key, id);
                return true;
            }
            else
            {
                uint error = GetLastError();
                _logger.LogError("Failed to unregister hotkey ID {Id}. Error: {Error}", null, id, error);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Exception unregistering hotkey ID {Id}: {Message}", ex, id, ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Register hotkeys from configuration
    /// </summary>
    public bool RegisterHotkeys(HotkeyConfiguration config)
    {
        if (config == null)
            throw new ArgumentNullException(nameof(config));

        if (!config.IsEnabled)
        {
            _logger.LogInfo("Hotkeys are disabled in configuration");
            return true;
        }

        bool allSuccess = true;

        // Register primary hotkey (ID: 1)
        if (!RegisterHotkey(1, config.PrimaryHotkey, config.PrimaryModifiers))
        {
            allSuccess = false;
        }

        // Register secondary hotkey (ID: 2)
        if (!RegisterHotkey(2, config.SecondaryHotkey, config.SecondaryModifiers))
        {
            allSuccess = false;
        }

        return allSuccess;
    }

    /// <summary>
    /// Unregister all hotkeys
    /// </summary>
    public void UnregisterAllHotkeys()
    {
        var hotkeyIds = _registeredHotkeys.Keys.ToArray();
        foreach (var id in hotkeyIds)
        {
            UnregisterHotkey(id);
        }

        _logger.LogInfo("All hotkeys unregistered");
    }

    /// <summary>
    /// Check if a hotkey is currently registered
    /// </summary>
    public bool IsHotkeyRegistered(int id)
    {
        return _registeredHotkeys.ContainsKey(id);
    }

    /// <summary>
    /// Get all registered hotkey IDs
    /// </summary>
    public int[] GetRegisteredHotkeyIds()
    {
        return _registeredHotkeys.Keys.ToArray();
    }

    #region Private Methods

    private uint ConvertModifiers(ModifierKeys modifiers)
    {
        uint result = 0;

        if (modifiers.HasFlag(ModifierKeys.Alt))
            result |= MOD_ALT;
        if (modifiers.HasFlag(ModifierKeys.Control))
            result |= MOD_CONTROL;
        if (modifiers.HasFlag(ModifierKeys.Shift))
            result |= MOD_SHIFT;
        if (modifiers.HasFlag(ModifierKeys.Windows))
            result |= MOD_WIN;

        return result;
    }

    private void OnHotkeyMessage(int hotkeyId)
    {
        try
        {
            if (_registeredHotkeys.TryGetValue(hotkeyId, out var hotkeyInfo))
            {
                _logger.LogDebug("Hotkey pressed: {0}+{1} (ID: {2})",
                    hotkeyInfo.Modifiers, hotkeyInfo.Key, hotkeyId);

                var eventArgs = new HotkeyEventArgs(hotkeyInfo.Key, hotkeyInfo.Modifiers);
                HotkeyPressed?.Invoke(this, eventArgs);
            }
            else
            {
                _logger.LogWarning("Received hotkey message for unregistered ID: {0}", hotkeyId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Exception handling hotkey message: {Message}", ex, ex.Message);
        }
    }

    #endregion

    #region IDisposable Implementation

    public void Dispose()
    {
        if (!_disposed)
        {
            UnregisterAllHotkeys();
            _messageWindow?.Dispose();
            _disposed = true;
            _logger.LogInfo("HotkeyService disposed");
        }
    }

    #endregion

    #region Helper Classes

    /// <summary>
    /// Information about a registered hotkey
    /// </summary>
    private class HotkeyInfo
    {
        public int Id { get; }
        public Keys Key { get; }
        public ModifierKeys Modifiers { get; }
        public DateTime RegisteredAt { get; }

        public HotkeyInfo(int id, Keys key, ModifierKeys modifiers, DateTime registeredAt)
        {
            Id = id;
            Key = key;
            Modifiers = modifiers;
            RegisteredAt = registeredAt;
        }
    }

    /// <summary>
    /// Hidden window for receiving Windows messages
    /// </summary>
    private class MessageWindow : NativeWindow, IDisposable
    {
        private readonly Action<int> _hotkeyCallback;

        public MessageWindow(Action<int> hotkeyCallback)
        {
            _hotkeyCallback = hotkeyCallback ?? throw new ArgumentNullException(nameof(hotkeyCallback));
            CreateHandle(new CreateParams());
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY)
            {
                int hotkeyId = m.WParam.ToInt32();
                _hotkeyCallback(hotkeyId);
            }

            base.WndProc(ref m);
        }

        public void Dispose()
        {
            DestroyHandle();
        }
    }

    #endregion
}
