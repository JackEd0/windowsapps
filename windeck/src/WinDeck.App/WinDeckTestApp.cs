using System.Windows;
using System.Windows.Forms;
using WinDeck.Core.Models;
using WinDeck.Core.Services;
using WinDeck.UI.Services;
using Application = System.Windows.Application;

namespace WinDeck.App;

/// <summary>
/// Test WPF application to demonstrate the complete WinDeck system
/// </summary>
public class WinDeckTestApp : Application
{
    private LoggingService? _loggingService;
    private HotkeyService? _hotkeyService;
    private OverlayService? _overlayService;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            Console.WriteLine("=== WinDeck Integration Test ===");

            // Initialize logging
            var loggingConfig = new LoggingConfiguration
            {
                IsDebugEnabled = true,
                MinimumLevel = "Debug"
            };
            _loggingService = new LoggingService(loggingConfig);
            var logger = _loggingService.CreateLogger("WinDeckTestApp");

            logger.LogInfo("Starting WinDeck test application");

            // Initialize hotkey service
            var hotkeyLogger = _loggingService.CreateLogger("HotkeyService");
            _hotkeyService = new HotkeyService(hotkeyLogger);

            // Initialize overlay service
            var overlayLogger = _loggingService.CreateLogger("OverlayService");
            _overlayService = new OverlayService(overlayLogger);

            // Setup test configuration
            var overlayConfig = new OverlayConfiguration
            {
                Width = 350,
                Height = 450,
                Position = OverlayPosition.Center,
                ShowAnimation = true,
                AutoHideTimeout = 10000 // 10 seconds for testing
            };

            var testShortcuts = CreateTestShortcuts();

            // Initialize overlay
            _overlayService.Initialize(overlayConfig, testShortcuts);

            // Subscribe to overlay events
            _overlayService.ShortcutActivated += OnShortcutActivated;
            _overlayService.VisibilityChanged += OnVisibilityChanged;

            // Setup hotkeys
            var hotkeyConfig = new HotkeyConfiguration
            {
                PrimaryHotkey = Keys.F10,
                PrimaryModifiers = ModifierKeys.None,
                SecondaryHotkey = Keys.Space,
                SecondaryModifiers = ModifierKeys.Control,
                IsEnabled = true
            };

            // Subscribe to hotkey events
            _hotkeyService.HotkeyPressed += OnHotkeyPressed;

            // Register hotkeys
            if (_hotkeyService.RegisterHotkeys(hotkeyConfig))
            {
                logger.LogInfo("Hotkeys registered successfully");
                logger.LogInfo("Press F10 or Ctrl+Space to show overlay");

                Console.WriteLine("\n=== WinDeck Ready ===");
                Console.WriteLine("• Press F10 or Ctrl+Space to show/hide overlay");
                Console.WriteLine("• Use number keys (0-9) to activate shortcuts");
                Console.WriteLine("• Press ESC or click outside to hide overlay");
                Console.WriteLine("• Close this console to exit");
                Console.WriteLine();
            }
            else
            {
                logger.LogError("Failed to register hotkeys");
                Console.WriteLine("ERROR: Failed to register hotkeys. Application may not work correctly.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR starting application: {ex.Message}");
            Shutdown(1);
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        // Cleanup resources
        _overlayService?.Dispose();
        _hotkeyService?.Dispose();
        _loggingService?.Dispose();

        Console.WriteLine("WinDeck test application closed.");
        base.OnExit(e);
    }

    private void OnHotkeyPressed(object? sender, HotkeyEventArgs e)
    {
        Console.WriteLine($"Hotkey pressed: {e.Modifiers}+{e.Key} at {e.Timestamp:HH:mm:ss.fff}");
        _overlayService?.Toggle();
    }

    private void OnShortcutActivated(object? sender, Core.Interfaces.ShortcutActivatedEventArgs e)
    {
        Console.WriteLine($"Shortcut activated: [{e.Number}] {e.Name}");

        if (e.Action != null)
        {
            ExecuteShortcut(e.Action);
        }
    }

    private void OnVisibilityChanged(object? sender, Core.Interfaces.OverlayVisibilityChangedEventArgs e)
    {
        Console.WriteLine($"Overlay {(e.IsVisible ? "shown" : "hidden")} at {e.Timestamp:HH:mm:ss.fff}");
    }

    private void ExecuteShortcut(ShortcutConfiguration shortcut)
    {
        try
        {
            Console.WriteLine($"Executing: {shortcut.ActionType} - {shortcut.Target}");

            switch (shortcut.ActionType)
            {
                case ShortcutActionType.Application:
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = shortcut.Target,
                        Arguments = shortcut.Arguments,
                        WorkingDirectory = shortcut.WorkingDirectory,
                        UseShellExecute = true
                    });
                    break;

                case ShortcutActionType.Url:
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = shortcut.Target,
                        UseShellExecute = true
                    });
                    break;

                case ShortcutActionType.File:
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "explorer.exe",
                        Arguments = $"\"{shortcut.Target}\"",
                        UseShellExecute = true
                    });
                    break;

                default:
                    Console.WriteLine($"Action type {shortcut.ActionType} not implemented in test");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error executing shortcut: {ex.Message}");
        }
    }

    private static List<ShortcutConfiguration> CreateTestShortcuts()
    {
        return new List<ShortcutConfiguration>
        {
            new() { Number = 1, Name = "Calculator", ActionType = ShortcutActionType.Application, Target = "calc.exe", IsEnabled = true },
            new() { Number = 2, Name = "Notepad", ActionType = ShortcutActionType.Application, Target = "notepad.exe", IsEnabled = true },
            new() { Number = 3, Name = "File Explorer", ActionType = ShortcutActionType.Application, Target = "explorer.exe", IsEnabled = true },
            new() { Number = 4, Name = "Control Panel", ActionType = ShortcutActionType.Application, Target = "control.exe", IsEnabled = true },
            new() { Number = 5, Name = "Task Manager", ActionType = ShortcutActionType.Application, Target = "taskmgr.exe", IsEnabled = true },
            new() { Number = 6, Name = "Google", ActionType = ShortcutActionType.Url, Target = "https://www.google.com", IsEnabled = true },
            new() { Number = 7, Name = "GitHub", ActionType = ShortcutActionType.Url, Target = "https://www.github.com", IsEnabled = true },
            new() { Number = 8, Name = "Documents", ActionType = ShortcutActionType.File, Target = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), IsEnabled = true },
            new() { Number = 9, Name = "Downloads", ActionType = ShortcutActionType.File, Target = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads"), IsEnabled = true },
            new() { Number = 0, Name = "System Info", ActionType = ShortcutActionType.Application, Target = "msinfo32.exe", IsEnabled = true }
        };
    }
}
