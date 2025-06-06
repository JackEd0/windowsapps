# 🔨 Development

## Project Structure

```
src/
├── WinDeck.Core/          # Core business logic and services
│   ├── Interfaces/        # Service contracts
│   ├── Models/           # Data models and configuration
│   └── Services/         # Implementation classes
├── WinDeck.UI/           # WPF user interface
│   ├── Windows/          # XAML windows and controls
│   ├── ViewModels/       # MVVM view models
│   └── Services/         # UI-specific services
└── WinDeck.App/          # Application entry point
```

### Building the Solution

#### Quick Build Commands

```bash
# Clean previous builds
dotnet clean

# Restore NuGet packages
dotnet restore

# Build in Debug mode (with symbols)
dotnet build --configuration Debug

# Build in Release mode (optimized)
dotnet build --configuration Release

# Run all tests (when available)
dotnet test

# Create self-contained executable
dotnet publish src/WinDeck.App --configuration Release --output ./dist --self-contained true --runtime win-x64
```

#### Advanced Build Options

```bash
# Build specific project only
dotnet build src/WinDeck.Core --configuration Release

# Build with verbose output
dotnet build --verbosity detailed

# Build and run immediately
dotnet run --project src/WinDeck.App --configuration Release

# Build for multiple targets
dotnet build --configuration Release --framework net8.0-windows
```

#### Continuous Development

```bash
# Watch for changes and auto-rebuild
dotnet watch --project src/WinDeck.App run

# Run with specific arguments
dotnet run --project src/WinDeck.App -- --console --debug

# Hot reload during development
dotnet watch --project src/WinDeck.App run --launch-profile "Development"
```

### Development Commands

```bash
# Run with console logging
dotnet run --project src/WinDeck.App -- --console

# Run full WPF application
dotnet run --project src/WinDeck.App

# Build and watch for changes
dotnet watch --project src/WinDeck.App run
```

### Debugging

1. **Enable Debug Logging**: Set `"isDebugEnabled": true` in config.json
2. **Console Mode**: Run with `--console` argument to see detailed logs
3. **Log Files**: Check `%APPDATA%\WinDeck\logs\` for log files
4. **VS Code**: Use the provided launch configurations in `.vscode/`

## 🎛️ Architecture

### Core Components

- **🔑 HotkeyService** - Global keyboard hook management
- **🖼️ OverlayService** - UI overlay window management
- **⚙️ ConfigurationService** - JSON configuration management
- **📝 LoggingService** - Comprehensive logging system
- **🎯 ActionExecutor** - Shortcut action execution engine

### Key Design Patterns

- **Dependency Injection** - Services are injected via constructor
- **MVVM** - UI follows Model-View-ViewModel pattern
- **Observer Pattern** - Event-driven communication between services
- **Strategy Pattern** - Different action types with common interface

## 🛠️ API Reference

### Core Interfaces

```csharp
// Overlay Management
IOverlayService.Show();                    // Show overlay window
IOverlayService.Hide();                    // Hide overlay window
IOverlayService.Toggle();                  // Toggle overlay visibility
IOverlayService.IsVisible { get; }         // Check if overlay is visible

// Configuration Management
await IConfigurationService.LoadAsync();             // Load config from file
await IConfigurationService.SaveAsync();             // Save config to file
await IConfigurationService.ResetToDefaultAsync();   // Reset to defaults
IConfigurationService.Configuration { get; }         // Current configuration

// Hotkey Management
IHotkeyService.RegisterHotkey(id, key, modifiers);   // Register global hotkey
IHotkeyService.UnregisterHotkey(id);                 // Unregister hotkey
IHotkeyService.UnregisterAllHotkeys();               // Clear all hotkeys

// Logging
IWinDeckLogger.LogInfo(message);           // Information logging
IWinDeckLogger.LogError(message, ex);      // Error logging with exception
IWinDeckLogger.LogDebug(message);          // Debug logging (when enabled)
```

### Event Handling

```csharp
// Subscribe to overlay events
overlayService.OverlayShown += OnOverlayShown;
overlayService.OverlayHidden += OnOverlayHidden;

// Subscribe to hotkey events
hotkeyService.HotkeyPressed += OnHotkeyPressed;

// Subscribe to configuration changes
configService.ConfigurationChanged += OnConfigChanged;
```

## 🐛 Troubleshooting

### Common Issues

#### "Application won't start"

- **Check .NET Runtime**: Ensure .NET 8.0 is installed
- **Run as Administrator**: Try running with elevated permissions once
- **Check Windows Version**: Requires Windows 10 1809+ or Windows 11
- **Antivirus**: Add WinDeck to antivirus exclusions

#### "Hotkeys not working"

- **Other Applications**: Check if other apps are using F10/Ctrl+Space
- **Gaming Software**: Disable gaming overlays temporarily
- **Configuration**: Verify hotkey settings in config.json
- **Restart**: Restart WinDeck to re-register hotkeys

#### "Overlay not appearing"

- **Multiple Monitors**: Check if overlay appears on different screen
- **Windows Display Settings**: Verify DPI scaling settings
- **Focus Issues**: Try pressing hotkey when desktop has focus
- **Graphics Issues**: Update graphics drivers

#### "Configuration not saving"

- **File Permissions**: Check %APPDATA%\WinDeck\ folder permissions
- **Disk Space**: Ensure sufficient disk space available
- **File Locks**: Close other applications that might lock config files
- **JSON Syntax**: Validate JSON syntax in configuration file

### Debug Mode

Enable detailed logging by setting in config.json:

```json
{
  "logging": {
    "isDebugEnabled": true,
    "minimumLevel": "Debug"
  }
}
```

### Log File Locations

- **Application Logs**: `%APPDATA%\WinDeck\logs\`
- **Configuration**: `%APPDATA%\WinDeck\config.json`
- **Backups**: `%APPDATA%\WinDeck\backups\`

## 🤝 Contributing

### Getting Started

1. **Fork** the repository on GitHub
2. **Clone** your fork locally: `git clone https://github.com/yourusername/windeck.git`
3. **Create** a feature branch: `git checkout -b feature/amazing-feature`
4. **Setup** development environment (see Development section)
5. **Make** your changes following the guidelines below
6. **Test** your changes thoroughly
7. **Commit** with clear messages: `git commit -m 'Add amazing feature'`
8. **Push** to your branch: `git push origin feature/amazing-feature`
9. **Open** a Pull Request with detailed description

### Development Guidelines

#### Code Standards

- Follow Microsoft C# coding conventions
- Use meaningful variable and method names
- Add XML documentation comments for public APIs
- Maintain consistent code formatting
- Write unit tests for new features
- Update documentation for API changes

#### Architecture Guidelines

- Follow SOLID principles
- Use dependency injection for services
- Implement proper error handling and logging
- Maintain separation of concerns (Core/UI/App layers)
- Use async/await for I/O operations
- Follow MVVM pattern in UI code

#### Pull Request Process

- **Description**: Provide clear description of changes
- **Testing**: Include test results and screenshots if applicable
- **Documentation**: Update README/docs if needed
- **Changelog**: Add entry to version history
- **Review**: Address all review feedback
- **Squash**: Clean up commit history before merge

### Areas for Contribution

#### High Priority

- 🏗️ System tray integration with context menu
- ⚙️ Settings GUI for configuration management
- 📦 MSI installer creation and signing
- 🧪 Unit tests and integration tests
- 🎨 Custom icon support for shortcuts

#### Medium Priority

- 🔌 Plugin system architecture
- 🎭 Theme system and customization
- 📱 Multiple overlay layouts
- 🌐 Import/export configuration profiles
- 📊 Performance optimizations

#### Low Priority

- 🗣️ Voice command integration
- ☁️ Cloud sync capabilities
- 🖥️ Multi-monitor enhancements
- 🎮 Gaming integration features
- 🌍 Internationalization support

### Reporting Issues

#### Bug Reports

Please include:

- Windows version and build number
- WinDeck version
- Steps to reproduce the issue
- Expected vs actual behavior
- Screenshots or logs if applicable
- System configuration details

#### Feature Requests

Please provide:

- Clear description of the feature
- Use cases and benefits
- Proposed implementation approach
- Mockups or examples if applicable

### Community Guidelines

- Be respectful and inclusive
- Help others learn and grow
- Share knowledge and best practices
- Follow the project's code of conduct
- Focus on constructive feedback
