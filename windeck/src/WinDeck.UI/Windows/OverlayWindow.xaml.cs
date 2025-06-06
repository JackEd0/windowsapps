using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using WinDeck.Core.Interfaces;
using WinDeck.Core.Models;
using WinDeck.UI.ViewModels;

namespace WinDeck.UI.Windows;

/// <summary>
/// WinDeck overlay window - calculator-style shortcuts grid
/// </summary>
public partial class OverlayWindow : Window
{
    private readonly IWinDeckLogger _logger;
    private readonly OverlayViewModel _viewModel;
    private readonly OverlayConfiguration _config;
    private System.Timers.Timer? _autoHideTimer;

    public OverlayWindow(IWinDeckLogger logger, OverlayConfiguration config)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _config = config ?? throw new ArgumentNullException(nameof(config));

        InitializeComponent();

        // Create view model
        _viewModel = new OverlayViewModel(_logger);
        DataContext = _viewModel;

        // Subscribe to view model events
        _viewModel.VisibilityChanged += OnVisibilityChanged;
        _viewModel.ShortcutActivated += OnShortcutActivated;

        // Apply configuration
        ApplyConfiguration();

        // Setup auto-hide timer
        if (_config.AutoHideTimeout > 0)
        {
            _autoHideTimer = new System.Timers.Timer(_config.AutoHideTimeout);
            _autoHideTimer.Elapsed += (s, e) => Dispatcher.Invoke(HideOverlay);
            _autoHideTimer.AutoReset = false;
        }

        _logger.LogInfo("OverlayWindow initialized");
    }

    /// <summary>
    /// Event fired when a shortcut is activated
    /// </summary>
    public event EventHandler<ShortcutActivatedEventArgs>? ShortcutActivated;

    /// <summary>
    /// Show the overlay with animation
    /// </summary>
    public void ShowOverlay()
    {
        if (_viewModel.IsVisible)
            return;

        _logger.LogDebug("Showing overlay");

        // Position window
        PositionWindow();

        // Show window
        Show();
        Activate();

        // Focus search box
        SearchTextBox.Focus();

        // Update view model
        _viewModel.Show();

        // Start auto-hide timer
        RestartAutoHideTimer();

        // Show animation
        if (_config.ShowAnimation)
        {
            AnimateShow();
        }
        else
        {
            Opacity = _config.Opacity;
        }
    }

    /// <summary>
    /// Hide the overlay with animation
    /// </summary>
    public void HideOverlay()
    {
        if (!_viewModel.IsVisible)
            return;

        _logger.LogDebug("Hiding overlay");

        // Stop auto-hide timer
        _autoHideTimer?.Stop();

        // Update view model
        _viewModel.Hide();

        // Hide animation
        if (_config.ShowAnimation)
        {
            AnimateHide();
        }
        else
        {
            Hide();
        }
    }

    /// <summary>
    /// Toggle overlay visibility
    /// </summary>
    public void ToggleOverlay()
    {
        if (_viewModel.IsVisible)
            HideOverlay();
        else
            ShowOverlay();
    }

    /// <summary>
    /// Update shortcut configurations
    /// </summary>
    public void UpdateShortcuts(IEnumerable<ShortcutConfiguration> shortcuts)
    {
        _viewModel.UpdateShortcuts(shortcuts);
        _logger.LogInfo("Shortcuts updated");
    }

    #region Event Handlers

    private void OnVisibilityChanged(object? sender, EventArgs e)
    {
        _logger.LogDebug("ViewModel visibility changed to: {0}", _viewModel.IsVisible);
    }

    private void OnShortcutActivated(object? sender, ShortcutActivatedEventArgs e)
    {
        _logger.LogInfo("Shortcut activated: {0} - {1}", e.Number, e.Name);
        ShortcutActivated?.Invoke(this, e);
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        RestartAutoHideTimer();

        // Handle number keys (0-9)
        if (e.Key >= Key.D0 && e.Key <= Key.D9)
        {
            int number = e.Key - Key.D0;
            _viewModel.ActivateShortcut(number);
            e.Handled = true;
        }
        // Handle numpad keys (0-9)
        else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
        {
            int number = e.Key - Key.NumPad0;
            _viewModel.ActivateShortcut(number);
            e.Handled = true;
        }
        // Handle Escape key
        else if (e.Key == Key.Escape && _config.HideOnEscape)
        {
            HideOverlay();
            e.Handled = true;
        }
        // Handle Enter key (activate first visible shortcut)
        else if (e.Key == Key.Enter)
        {
            var firstVisible = _viewModel.ShortcutButtons.FirstOrDefault(b => b.IsVisible && b.IsEnabled);
            if (firstVisible != null)
            {
                _viewModel.ActivateShortcut(firstVisible.Number);
            }
            e.Handled = true;
        }
    }

    private void Window_Deactivated(object sender, EventArgs e)
    {
        if (_config.HideOnClickOutside && _viewModel.IsVisible)
        {
            HideOverlay();
        }
    }

    private void ShortcutButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is int number)
        {
            _viewModel.ActivateShortcut(number);
        }
    }

    #endregion

    #region Private Methods

    private void ApplyConfiguration()
    {
        // Window size
        Width = _config.Width;
        Height = _config.Height;

        // Apply colors to the border (would need to be done via code or binding)
        _logger.LogDebug("Applied configuration - Size: {0}x{1}, Position: {2}",
            _config.Width, _config.Height, _config.Position);
    }

    private void PositionWindow()
    {
        var workingArea = SystemParameters.WorkArea;

        switch (_config.Position)
        {
            case OverlayPosition.Center:
                Left = (workingArea.Width - Width) / 2;
                Top = (workingArea.Height - Height) / 2;
                break;

            case OverlayPosition.TopLeft:
                Left = 20;
                Top = 20;
                break;

            case OverlayPosition.TopCenter:
                Left = (workingArea.Width - Width) / 2;
                Top = 20;
                break;

            case OverlayPosition.TopRight:
                Left = workingArea.Width - Width - 20;
                Top = 20;
                break;

            case OverlayPosition.CenterLeft:
                Left = 20;
                Top = (workingArea.Height - Height) / 2;
                break;

            case OverlayPosition.CenterRight:
                Left = workingArea.Width - Width - 20;
                Top = (workingArea.Height - Height) / 2;
                break;

            case OverlayPosition.BottomLeft:
                Left = 20;
                Top = workingArea.Height - Height - 20;
                break;

            case OverlayPosition.BottomCenter:
                Left = (workingArea.Width - Width) / 2;
                Top = workingArea.Height - Height - 20;
                break;

            case OverlayPosition.BottomRight:
                Left = workingArea.Width - Width - 20;
                Top = workingArea.Height - Height - 20;
                break;

            case OverlayPosition.Custom:
                Left = _config.CustomX;
                Top = _config.CustomY;
                break;
        }

        _logger.LogDebug("Positioned window at: {0}, {1}", Left, Top);
    }

    private void AnimateShow()
    {
        var animation = new DoubleAnimation
        {
            From = 0.0,
            To = _config.Opacity,
            Duration = TimeSpan.FromMilliseconds(_config.AnimationDuration),
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
        };

        BeginAnimation(OpacityProperty, animation);
    }

    private void AnimateHide()
    {
        var animation = new DoubleAnimation
        {
            From = Opacity,
            To = 0.0,
            Duration = TimeSpan.FromMilliseconds(_config.AnimationDuration),
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
        };

        animation.Completed += (s, e) => Hide();
        BeginAnimation(OpacityProperty, animation);
    }

    private void RestartAutoHideTimer()
    {
        if (_autoHideTimer != null)
        {
            _autoHideTimer.Stop();
            _autoHideTimer.Start();
        }
    }

    #endregion

    protected override void OnClosed(EventArgs e)
    {
        // Cleanup
        _autoHideTimer?.Dispose();
        _viewModel.VisibilityChanged -= OnVisibilityChanged;
        _viewModel.ShortcutActivated -= OnShortcutActivated;

        base.OnClosed(e);

        _logger.LogInfo("OverlayWindow closed");
    }
}
