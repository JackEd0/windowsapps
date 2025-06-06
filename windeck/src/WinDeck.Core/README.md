# WinDeck Logging Service

## Overview

The WinDeck logging service provides a configurable, production-ready logging solution with the ability to toggle debug logging on/off. It's designed specifically for WinDeck but can be used in any .NET application.

## Features

- ✅ **Configurable Debug Mode** - Enable/disable debug logging at runtime
- ✅ **Multiple Log Levels** - Debug, Info, Warning, Error, Critical
- ✅ **Scoped Logging** - Create component-specific loggers
- ✅ **Production Ready** - Debug logging disabled in Release builds by default
- ✅ **Exception Handling** - Safe logging that won't crash your app
- ✅ **Flexible Configuration** - JSON-based configuration
- ✅ **Console & Debug Output** - Multiple output targets

## Quick Start

### 1. Create Logging Service

```csharp
using WinDeck.Core.Services;
using WinDeck.Core.Models;

// Create with default configuration
var loggingService = new LoggingService();

// Or with custom configuration
var config = new LoggingConfiguration
{
    IsDebugEnabled = true,
    MinimumLevel = "Information"
};
var loggingService = new LoggingService(config);
```

### 2. Create Loggers

```csharp
// Create logger for a specific component
var logger = loggingService.CreateLogger("MyComponent");

// Create logger for a type
var logger = loggingService.CreateLogger<MyClass>();

// Create scoped logger
var scopedLogger = logger.CreateScoped("SubComponent");
```

### 3. Log Messages

```csharp
// Different log levels
logger.LogDebug("Debug info: {0}", someValue);
logger.LogInfo("Application started");
logger.LogWarning("Configuration file not found, using defaults");
logger.LogError("Failed to save file", exception);
logger.LogCritical("System is in critical state", exception);

// Toggle debug logging
logger.SetDebugEnabled(false); // Disables debug messages
logger.SetDebugEnabled(true);  // Re-enables debug messages
```

## Configuration

### LoggingConfiguration Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `IsDebugEnabled` | bool | true (Debug) / false (Release) | Enable/disable debug logging |
| `MinimumLevel` | string | "Information" | Minimum log level for console output |
| `LogToFile` | bool | true | Whether to log to file |
| `LogFilePath` | string | "logs/windeck.log" | Log file path |
| `MaxLogFileSizeMB` | int | 10 | Max log file size before rotation |
| `MaxLogFiles` | int | 5 | Number of log files to keep |
| `IncludeTimestamp` | bool | true | Include timestamps in log messages |
| `IncludeScopes` | bool | true | Include scope information |

### JSON Configuration

```json
{
  "logging": {
    "isDebugEnabled": true,
    "minimumLevel": "Debug",
    "logToFile": true,
    "logFilePath": "logs/windeck.log",
    "maxLogFileSizeMB": 10,
    "maxLogFiles": 5,
    "includeTimestamp": true,
    "includeScopes": true
  }
}
```

## Production vs Development

### Debug Build

- Debug logging is **enabled** by default
- Console and debug output providers are active
- All log levels are captured

### Release Build

- Debug logging is **disabled** by default
- Console output only (no debug provider)
- Optimized for performance

### Runtime Control

```csharp
// Check if debug is enabled
if (logger.IsDebugEnabled)
{
    // Perform expensive logging operations
}

// Toggle debug at runtime (useful for troubleshooting)
logger.SetDebugEnabled(true);  // Enable debug logging
logger.SetDebugEnabled(false); // Disable debug logging
```

## Best Practices

### 1. Use Scoped Loggers

```csharp
public class HotkeyService
{
    private readonly IWinDeckLogger _logger;

    public HotkeyService(IWinDeckLogger parentLogger)
    {
        _logger = parentLogger.CreateScoped(nameof(HotkeyService));
    }

    public void RegisterHotkey()
    {
        _logger.LogDebug("Registering hotkey: F10");
        // Implementation...
        _logger.LogInfo("Hotkey registered successfully");
    }
}
```

### 2. Safe Exception Logging

```csharp
try
{
    // Risky operation
}
catch (Exception ex)
{
    logger.LogError("Operation failed", ex);
    // Handle gracefully
}
```

### 3. Conditional Debug Logging

```csharp
// For expensive operations
if (logger.IsDebugEnabled)
{
    var expensiveDebugInfo = GenerateDebugInfo();
    logger.LogDebug("Debug info: {0}", expensiveDebugInfo);
}
```

### 4. Parameterized Messages

```csharp
// Good - uses parameters
logger.LogInfo("Processing {0} items in {1} seconds", count, elapsed);

// Avoid - string concatenation
logger.LogInfo("Processing " + count + " items in " + elapsed + " seconds");
```

## Testing

Run the test application to see the logging service in action:

```bash
cd src/WinDeck.App
dotnet run
```

This will demonstrate:

- Different log levels
- Scoped logging
- Debug toggle functionality
- Component-specific logging

## Integration with WinDeck

The logging service is designed to integrate seamlessly with all WinDeck components:

- **HotkeyService** - Log hotkey registration and events
- **ActionExecutor** - Log action execution and results
- **ConfigManager** - Log configuration loading and changes
- **OverlayWindow** - Log UI events and state changes
- **SystemTray** - Log system tray interactions

Each component can create its own scoped logger for organized, traceable logging throughout the application.
