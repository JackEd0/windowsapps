using System.Windows.Forms;
using WinDeck.Core.Models;
using WinDeck.Core.Services;

namespace WinDeck.App;

/// <summary>
/// Main entry point for WinDeck application
/// </summary>
class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        // Check if user wants console test mode
        if (args.Length > 0 && args[0].Equals("--console", StringComparison.OrdinalIgnoreCase))
        {
            RunConsoleTests();
            return;
        }

        // Run WPF application
        Console.WriteLine("Starting WinDeck WPF Application...");
        var app = new WinDeckTestApp();
        app.Run();
    }

    static void RunConsoleTests()
    {
        Console.WriteLine("=== WinDeck Logging Service Test ===\n");        // Create logging configuration
        var loggingConfig = new LoggingConfiguration
        {
            IsDebugEnabled = true, // Enable for testing
            MinimumLevel = "Debug",
            IncludeTimestamp = true,
            IncludeScopes = true
        };

        // Create logging service
        using var loggingService = new LoggingService(loggingConfig);

        // Create main application logger
        var mainLogger = loggingService.CreateLogger("Program");

        // Test different log levels
        TestLoggingLevels(mainLogger);

        // Test scoped logging
        TestScopedLogging(mainLogger);

        // Test debug toggle
        TestDebugToggle(mainLogger);        // Test component-specific logging
        TestComponentLogging(loggingService);

        // Test hotkey service
        TestHotkeyService(loggingService);

        Console.WriteLine("\n=== Logging Test Complete ===");
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    static void TestLoggingLevels(WinDeck.Core.Interfaces.IWinDeckLogger logger)
    {
        Console.WriteLine("1. Testing different log levels:");

        logger.LogDebug("This is a debug message with parameter: {0}", "test-value");
        logger.LogInfo("Application started successfully");
        logger.LogWarning("This is a warning message");
        logger.LogError("This is an error message");

        try
        {
            throw new InvalidOperationException("Test exception");
        }
        catch (Exception ex)
        {
            logger.LogError("Caught an exception during testing", ex);
        }

        logger.LogCritical("This is a critical error message");
        Console.WriteLine();
    }

    static void TestScopedLogging(WinDeck.Core.Interfaces.IWinDeckLogger mainLogger)
    {
        Console.WriteLine("2. Testing scoped logging:");

        var hotkeyLogger = mainLogger.CreateScoped("HotkeyService");
        hotkeyLogger.LogInfo("Hotkey service initialized");
        hotkeyLogger.LogDebug("Registering hotkey: F10");

        var actionLogger = mainLogger.CreateScoped("ActionExecutor");
        actionLogger.LogInfo("Action executor ready");
        actionLogger.LogDebug("Loaded {0} actions from configuration", 5);

        Console.WriteLine();
    }

    static void TestDebugToggle(WinDeck.Core.Interfaces.IWinDeckLogger logger)
    {
        Console.WriteLine("3. Testing debug toggle:");

        logger.LogDebug("Debug message 1 (should be visible)");

        logger.SetDebugEnabled(false);
        logger.LogDebug("Debug message 2 (should be hidden)");
        logger.LogInfo("Info message (should still be visible)");

        logger.SetDebugEnabled(true);
        logger.LogDebug("Debug message 3 (should be visible again)");

        Console.WriteLine();
    }

    static void TestComponentLogging(LoggingService loggingService)
    {
        Console.WriteLine("4. Testing component-specific logging:");

        var configLogger = loggingService.CreateLogger<ConfigurationService>();
        configLogger.LogInfo("Configuration service starting");
        configLogger.LogDebug("Loading configuration from: config/settings.json");

        var overlayLogger = loggingService.CreateLogger("OverlayWindow");
        overlayLogger.LogInfo("Overlay window created");
        overlayLogger.LogDebug("Window position: {0}, {1}", 100, 200);        Console.WriteLine();
    }

    static void TestHotkeyService(LoggingService loggingService)
    {
        Console.WriteLine("5. Testing hotkey service:");

        var hotkeyLogger = loggingService.CreateLogger("HotkeyService");

        // Create hotkey configuration
        var hotkeyConfig = new HotkeyConfiguration
        {
            PrimaryHotkey = Keys.F10,
            PrimaryModifiers = ModifierKeys.None,
            SecondaryHotkey = Keys.Space,
            SecondaryModifiers = ModifierKeys.Control,
            IsEnabled = true
        };

        // Create and test hotkey service
        using var hotkeyService = new HotkeyService(hotkeyLogger);

        // Subscribe to hotkey events
        hotkeyService.HotkeyPressed += (sender, e) =>
        {
            Console.WriteLine($"   -> Hotkey pressed: {e.Modifiers}+{e.Key} at {e.Timestamp:HH:mm:ss.fff}");
        };

        // Register hotkeys
        bool registered = hotkeyService.RegisterHotkeys(hotkeyConfig);
        Console.WriteLine($"   -> Hotkeys registered: {registered}");

        if (registered)
        {
            Console.WriteLine($"   -> Registered hotkey IDs: [{string.Join(", ", hotkeyService.GetRegisteredHotkeyIds())}]");
            Console.WriteLine("   -> Press F10 or Ctrl+Space to test hotkeys (or any other key to continue)...");

            // Wait for a few seconds to allow testing
            var startTime = DateTime.Now;
            while ((DateTime.Now - startTime).TotalSeconds < 5)
            {
                Application.DoEvents();
                Thread.Sleep(100);

                // Check if user pressed any key to continue
                if (Console.KeyAvailable)
                {
                    Console.ReadKey(true);
                    break;
                }
            }
        }

        Console.WriteLine();
    }
}

// Dummy class for testing generic logger creation
public class ConfigurationService
{
    // This is just for testing the CreateLogger<T>() method
}
