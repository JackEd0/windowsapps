using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WinDeck.Core.Interfaces;
using WinDeck.Core.Models;

namespace WinDeck.UI.ViewModels;

/// <summary>
/// View model for the overlay window
/// </summary>
public class OverlayViewModel : INotifyPropertyChanged
{
    private readonly IWinDeckLogger _logger;
    private bool _isVisible;
    private string _searchText = string.Empty;

    public OverlayViewModel(IWinDeckLogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Initialize shortcut buttons (0-9)
        ShortcutButtons = new ObservableCollection<ShortcutButtonViewModel>();
        for (int i = 0; i <= 9; i++)
        {
            ShortcutButtons.Add(new ShortcutButtonViewModel(i, $"Action {i}", null));
        }

        _logger.LogDebug("OverlayViewModel initialized with {0} shortcut buttons", ShortcutButtons.Count);
    }

    /// <summary>
    /// Collection of shortcut buttons (0-9)
    /// </summary>
    public ObservableCollection<ShortcutButtonViewModel> ShortcutButtons { get; }

    /// <summary>
    /// Whether the overlay is currently visible
    /// </summary>
    public bool IsVisible
    {
        get => _isVisible;
        set
        {
            if (SetProperty(ref _isVisible, value))
            {
                _logger.LogDebug("Overlay visibility changed to: {0}", value);
                VisibilityChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    /// <summary>
    /// Search text for filtering actions
    /// </summary>
    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                _logger.LogDebug("Search text changed to: '{0}'", value);
                FilterShortcuts();
            }
        }
    }

    /// <summary>
    /// Event fired when overlay visibility changes
    /// </summary>
    public event EventHandler? VisibilityChanged;

    /// <summary>
    /// Event fired when a shortcut button is activated
    /// </summary>
    public event EventHandler<ShortcutActivatedEventArgs>? ShortcutActivated;

    /// <summary>
    /// Show the overlay
    /// </summary>
    public void Show()
    {
        if (!IsVisible)
        {
            IsVisible = true;
            SearchText = string.Empty;
            _logger.LogInfo("Overlay shown");
        }
    }

    /// <summary>
    /// Hide the overlay
    /// </summary>
    public void Hide()
    {
        if (IsVisible)
        {
            IsVisible = false;
            SearchText = string.Empty;
            _logger.LogInfo("Overlay hidden");
        }
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
    /// Activate shortcut by number key (0-9)
    /// </summary>
    /// <param name="number">Number key pressed (0-9)</param>
    public void ActivateShortcut(int number)
    {
        if (number < 0 || number > 9)
        {
            _logger.LogWarning("Invalid shortcut number: {0}", number);
            return;
        }

        var shortcut = ShortcutButtons.FirstOrDefault(s => s.Number == number && s.IsVisible);
        if (shortcut != null)
        {
            _logger.LogInfo("Activating shortcut {0}: {1}", number, shortcut.Name);
            var eventArgs = new ShortcutActivatedEventArgs(number, shortcut.Name, shortcut.Action);
            ShortcutActivated?.Invoke(this, eventArgs);

            // Hide overlay after activation
            Hide();
        }
        else
        {
            _logger.LogWarning("No visible shortcut found for number: {0}", number);
        }
    }

    /// <summary>
    /// Update shortcut configuration
    /// </summary>
    /// <param name="shortcuts">New shortcut configurations</param>
    public void UpdateShortcuts(IEnumerable<ShortcutConfiguration> shortcuts)
    {
        _logger.LogInfo("Updating shortcut configurations");

        // Reset all buttons to default
        for (int i = 0; i <= 9; i++)
        {
            var button = ShortcutButtons.First(b => b.Number == i);
            button.Name = $"Empty Slot {i}";
            button.Action = null;
            button.IsEnabled = false;
        }

        // Apply new configurations
        foreach (var shortcut in shortcuts.Take(10)) // Limit to 10 shortcuts (0-9)
        {
            if (shortcut.Number >= 0 && shortcut.Number <= 9)
            {
                var button = ShortcutButtons.First(b => b.Number == shortcut.Number);
                button.Name = shortcut.Name;
                button.Action = shortcut;
                button.IsEnabled = true;
            }
        }

        FilterShortcuts();
    }

    private void FilterShortcuts()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            // Show all enabled shortcuts
            foreach (var button in ShortcutButtons)
            {
                button.IsVisible = button.IsEnabled;
            }
        }
        else
        {
            // Filter by search text
            var searchLower = SearchText.ToLowerInvariant();
            foreach (var button in ShortcutButtons)
            {
                button.IsVisible = button.IsEnabled &&
                    button.Name.ToLowerInvariant().Contains(searchLower);
            }
        }
    }

    #region INotifyPropertyChanged Implementation

    public event PropertyChangedEventHandler? PropertyChanged;

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }

    #endregion
}

/// <summary>
/// View model for individual shortcut buttons
/// </summary>
public class ShortcutButtonViewModel : INotifyPropertyChanged
{
    private string _name;
    private bool _isEnabled;
    private bool _isVisible = true;

    public ShortcutButtonViewModel(int number, string name, ShortcutConfiguration? action)
    {
        Number = number;
        _name = name;
        Action = action;
        _isEnabled = action != null;
    }

    /// <summary>
    /// Shortcut number (0-9)
    /// </summary>
    public int Number { get; }

    /// <summary>
    /// Display name of the shortcut
    /// </summary>
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    /// <summary>
    /// Whether the shortcut is enabled
    /// </summary>
    public bool IsEnabled
    {
        get => _isEnabled;
        set => SetProperty(ref _isEnabled, value);
    }

    /// <summary>
    /// Whether the shortcut is visible (after filtering)
    /// </summary>
    public bool IsVisible
    {
        get => _isVisible;
        set => SetProperty(ref _isVisible, value);
    }

    /// <summary>
    /// Associated shortcut action
    /// </summary>
    public ShortcutConfiguration? Action { get; set; }

    #region INotifyPropertyChanged Implementation

    public event PropertyChangedEventHandler? PropertyChanged;

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }

    #endregion
}

/// <summary>
/// Event arguments for shortcut activation
/// </summary>
public class ShortcutActivatedEventArgs : EventArgs
{
    public int Number { get; }
    public string Name { get; }
    public ShortcutConfiguration? Action { get; }

    public ShortcutActivatedEventArgs(int number, string name, ShortcutConfiguration? action)
    {
        Number = number;
        Name = name;
        Action = action;
    }
}
