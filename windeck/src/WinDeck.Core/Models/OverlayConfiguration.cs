namespace WinDeck.Core.Models;

/// <summary>
/// Configuration for the overlay window appearance and behavior
/// </summary>
public class OverlayConfiguration
{
    /// <summary>
    /// Width of the overlay window in pixels (default: 300)
    /// </summary>
    public int Width { get; set; } = 300;

    /// <summary>
    /// Height of the overlay window in pixels (default: 400)
    /// </summary>
    public int Height { get; set; } = 400;

    /// <summary>
    /// Position of the overlay window (default: Center)
    /// </summary>
    public OverlayPosition Position { get; set; } = OverlayPosition.Center;

    /// <summary>
    /// Custom X position (used when Position is Custom)
    /// </summary>
    public int CustomX { get; set; } = 0;

    /// <summary>
    /// Custom Y position (used when Position is Custom)
    /// </summary>
    public int CustomY { get; set; } = 0;

    /// <summary>
    /// Opacity of the overlay window (0.0 to 1.0, default: 0.95)
    /// </summary>
    public double Opacity { get; set; } = 0.95;

    /// <summary>
    /// Background color in hex format (default: #2D2D30)
    /// </summary>
    public string BackgroundColor { get; set; } = "#2D2D30";

    /// <summary>
    /// Border color in hex format (default: #007ACC)
    /// </summary>
    public string BorderColor { get; set; } = "#007ACC";

    /// <summary>
    /// Border thickness in pixels (default: 2)
    /// </summary>
    public int BorderThickness { get; set; } = 2;

    /// <summary>
    /// Corner radius for rounded corners (default: 8)
    /// </summary>
    public int CornerRadius { get; set; } = 8;

    /// <summary>
    /// Animation duration in milliseconds (default: 200)
    /// </summary>
    public int AnimationDuration { get; set; } = 200;

    /// <summary>
    /// Whether to show animation when opening/closing (default: true)
    /// </summary>
    public bool ShowAnimation { get; set; } = true;

    /// <summary>
    /// Auto-hide timeout in milliseconds (0 = no auto-hide, default: 5000)
    /// </summary>
    public int AutoHideTimeout { get; set; } = 5000;

    /// <summary>
    /// Whether to hide when clicking outside the overlay (default: true)
    /// </summary>
    public bool HideOnClickOutside { get; set; } = true;

    /// <summary>
    /// Whether to hide when pressing Escape key (default: true)
    /// </summary>
    public bool HideOnEscape { get; set; } = true;

    /// <summary>
    /// Font family for text (default: Segoe UI)
    /// </summary>
    public string FontFamily { get; set; } = "Segoe UI";

    /// <summary>
    /// Font size for text (default: 14)
    /// </summary>
    public int FontSize { get; set; } = 14;

    /// <summary>
    /// Text color in hex format (default: #FFFFFF)
    /// </summary>
    public string TextColor { get; set; } = "#FFFFFF";
}

/// <summary>
/// Position options for the overlay window
/// </summary>
public enum OverlayPosition
{
    Center,
    TopLeft,
    TopCenter,
    TopRight,
    CenterLeft,
    CenterRight,
    BottomLeft,
    BottomCenter,
    BottomRight,
    Custom
}
