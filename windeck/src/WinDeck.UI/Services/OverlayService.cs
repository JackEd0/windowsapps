using System.Windows;
using System.Windows.Threading;
using WinDeck.Core.Interfaces;
using WinDeck.Core.Models;
using WinDeck.UI.Windows;

namespace WinDeck.UI.Services;

/// <summary>
/// WPF implementation of the overlay service
/// </summary>
public class OverlayService : IOverlayService
{
    private readonly IWinDeckLogger _logger;
    private OverlayWindow? _overlayWindow;
    private OverlayConfiguration? _config;
    private IEnumerable<ShortcutConfiguration>? _shortcuts;
    private bool _disposed = false;

    public OverlayService(IWinDeckLogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logger.LogInfo("OverlayService initialized");
    }

    /// <summary>
    /// Event fired when a shortcut is activated from the overlay
    /// </summary>
    public event EventHandler<ShortcutActivatedEventArgs>? ShortcutActivated;

    /// <summary>
    /// Event fired when the overlay visibility changes
    /// </summary>
    public event EventHandler<OverlayVisibilityChangedEventArgs>? VisibilityChanged;

    /// <summary>
    /// Whether the overlay is currently visible
    /// </summary>
    public bool IsVisible => _overlayWindow?.IsVisible == true;

    /// <summary>
    /// Initialize the overlay service
    /// </summary>
    public void Initialize(OverlayConfiguration overlayConfig, IEnumerable<ShortcutConfiguration> shortcuts)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(OverlayService));

        _config = overlayConfig ?? throw new ArgumentNullException(nameof(overlayConfig));
        _shortcuts = shortcuts ?? throw new ArgumentNullException(nameof(shortcuts));

        // Create overlay window on UI thread
        Application.Current.Dispatcher.Invoke(() =>
        {
            CreateOverlayWindow();
        });

        _logger.LogInfo("OverlayService initialized with {0} shortcuts", _shortcuts.Count());
    }

    /// <summary>
    /// Show the overlay
    /// </summary>
    public void Show()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(OverlayService));

        if (_overlayWindow == null)
        {
            _logger.LogWarning("Cannot show overlay - not initialized");
            return;
        }

        Application.Current.Dispatcher.Invoke(() =>
        {
            try
            {
                _overlayWindow.ShowOverlay();
                _logger.LogInfo("Overlay shown");
                VisibilityChanged?.Invoke(this, new OverlayVisibilityChangedEventArgs(true));
            }
            catch (Exception ex)
            {
                _logger.LogError("Error showing overlay: {0}", ex.Message);
            }
        });
    }

    /// <summary>
    /// Hide the overlay
    /// </summary>
    public void Hide()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(OverlayService));

        if (_overlayWindow == null)
            return;

        Application.Current.Dispatcher.Invoke(() =>
        {
            try
            {
                _overlayWindow.HideOverlay();
                _logger.LogInfo("Overlay hidden");
                VisibilityChanged?.Invoke(this, new OverlayVisibilityChangedEventArgs(false));
            }
            catch (Exception ex)
            {
                _logger.LogError("Error hiding overlay: {0}", ex.Message);
            }
        });
    }

    /// <summary>
    /// Toggle overlay visibility
    /// </summary>
    public void Toggle()
    {
        if (IsVisible)
            Hide();
        else
            Show();
    }

    /// <summary>
    /// Update the overlay configuration
    /// </summary>
    public void UpdateConfiguration(OverlayConfiguration config)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(OverlayService));

        _config = config ?? throw new ArgumentNullException(nameof(config));

        if (_overlayWindow != null)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                // Recreate window with new configuration
                var wasVisible = IsVisible;
                _overlayWindow.Close();
                CreateOverlayWindow();

                if (wasVisible)
                {
                    _overlayWindow.ShowOverlay();
                }
            });
        }

        _logger.LogInfo("Overlay configuration updated");
    }

    /// <summary>
    /// Update the shortcuts displayed in the overlay
    /// </summary>
    public void UpdateShortcuts(IEnumerable<ShortcutConfiguration> shortcuts)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(OverlayService));

        _shortcuts = shortcuts ?? throw new ArgumentNullException(nameof(shortcuts));

        if (_overlayWindow != null)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _overlayWindow.UpdateShortcuts(_shortcuts);
            });
        }

        _logger.LogInfo("Overlay shortcuts updated - {0} shortcuts", _shortcuts.Count());
    }

    #region Private Methods

    private void CreateOverlayWindow()
    {
        if (_config == null)
        {
            _logger.LogError("Cannot create overlay window - configuration is null");
            return;
        }

        try
        {
            // Dispose existing window
            if (_overlayWindow != null)
            {
                _overlayWindow.ShortcutActivated -= OnShortcutActivated;
                _overlayWindow.Close();
            }

            // Create new window
            _overlayWindow = new OverlayWindow(_logger, _config);
            _overlayWindow.ShortcutActivated += OnShortcutActivated;

            // Apply shortcuts if available
            if (_shortcuts != null)
            {
                _overlayWindow.UpdateShortcuts(_shortcuts);
            }

            _logger.LogDebug("Overlay window created");
        }
        catch (Exception ex)
        {
            _logger.LogError("Error creating overlay window: {0}", ex.Message);
        }
    }

    private void OnShortcutActivated(object? sender, UI.ViewModels.ShortcutActivatedEventArgs e)
    {
        _logger.LogDebug("Shortcut activated in overlay: {0} - {1}", e.Number, e.Name);

        // Convert UI event args to Core event args
        var coreEventArgs = new ShortcutActivatedEventArgs(e.Number, e.Name, e.Action);
        ShortcutActivated?.Invoke(this, coreEventArgs);
    }

    #endregion

    #region IDisposable Implementation

    public void Dispose()
    {
        if (!_disposed)
        {
            Application.Current?.Dispatcher.Invoke(() =>
            {
                if (_overlayWindow != null)
                {
                    _overlayWindow.ShortcutActivated -= OnShortcutActivated;
                    _overlayWindow.Close();
                    _overlayWindow = null;
                }
            });

            _disposed = true;
            _logger.LogInfo("OverlayService disposed");
        }
    }

    #endregion
}
