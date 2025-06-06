# WinDeck - Windows Shortcuts Deck Application

## Project Overview

A Windows desktop application that provides a calculator-like GUI for executing custom shortcuts and scripts. The app stays hidden in the system tray and appears as an overlay when triggered by a global hotkey.

## Core Features

### 1. Global Hotkey System

- **Primary Trigger**: F10 (configurable)
- **Secondary Triggers**: Configurable key combinations
- Works system-wide, even when app is not in focus
- Registers low-level keyboard hooks

### 2. GUI Design

- **Layout**: Calculator-style grid interface
- **Appearance**: Modern, translucent overlay window
- **Positioning**: Appears at cursor position or screen center
- **Behavior**:
  - Auto-hide after action execution
  - Click outside to dismiss
  - ESC key to close

### 3. Action System

- **Number Keys (1-9, 0)**: Execute predefined shortcuts
- **Action Types**:
  - Run applications/executables
  - Execute PowerShell/Batch scripts
  - Send keystrokes/hotkeys
  - Open files/folders
  - Web URLs
  - System commands (shutdown, sleep, etc.)

### 4. Configuration Management

- **Settings File**: JSON-based configuration
- **Customizable Elements**:
  - Button labels and icons
  - Action assignments
  - Hotkey mappings
  - UI appearance/theme
  - Window behavior

## Technical Architecture

### Technology Stack Recommended: C# WPF (.NET 6/8)

Best balance of performance, Windows integration, and development ease.

## Project Structure

```
WinDeck/
├── src/
│   ├── WinDeck.Core/          # Core logic and models
│   │   ├── Models/
│   │   │   ├── ShortcutAction.cs
│   │   │   ├── Configuration.cs
│   │   │   └── HotkeyConfig.cs
│   │   ├── Services/
│   │   │   ├── HotkeyService.cs
│   │   │   ├── ActionExecutor.cs
│   │   │   ├── ConfigManager.cs
│   │   │   └── ScriptRunner.cs
│   │   └── Interfaces/
│   ├── WinDeck.UI/            # WPF User Interface
│   │   ├── Views/
│   │   │   ├── MainWindow.xaml
│   │   │   ├── SettingsWindow.xaml
│   │   │   └── OverlayWindow.xaml
│   │   ├── ViewModels/
│   │   ├── Converters/
│   │   └── Resources/
│   └── WinDeck.App/           # Application entry point
├── config/
│   ├── default-config.json
│   └── user-config.json
├── scripts/                   # User custom scripts
└── assets/
    └── icons/
```

## Implementation Phases

### Phase 1: Core Infrastructure

- [ ] Set up project structure
- [ ] Implement global hotkey registration
- [ ] Create basic overlay window
- [ ] Build configuration system
- [ ] System tray integration

### Phase 2: Action System

- [ ] Action executor framework
- [ ] Application launcher
- [ ] Script execution (PowerShell/Batch)
- [ ] Keystroke simulation
- [ ] File/URL operations

### Phase 3: User Interface

- [ ] Calculator-style grid layout
- [ ] Button customization
- [ ] Themes and styling
- [ ] Icons and visual feedback
- [ ] Animations and transitions

### Phase 4: Configuration & Settings

- [ ] Settings GUI
- [ ] Hotkey customization
- [ ] Action configuration interface
- [ ] Import/Export settings
- [ ] Profile management

### Phase 5: Polish & Distribution

- [ ] Error handling and logging
- [ ] Auto-updater
- [ ] Installer creation
- [ ] Documentation
- [ ] Testing and debugging

## Key Technical Considerations

### Global Hotkey Implementation

```csharp
// Low-level keyboard hook for system-wide hotkeys
[DllImport("user32.dll")]
private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
```

### Window Behavior

- **Topmost**: Always on top when visible
- **ShowInTaskbar**: False
- **WindowStyle**: None (borderless)
- **AllowsTransparency**: True
- **Background**: Semi-transparent

### Action Execution Security

- Sandbox script execution
- Validate file paths
- User permission prompts for sensitive operations
- Whitelist allowed operations

### Performance Optimization

- Lazy loading of UI components
- Efficient hotkey handling
- Minimal memory footprint when hidden
- Fast show/hide animations

## Configuration File Example

```json
{
  "hotkeys": {
    "primary": "F10",
    "modifiers": []
  },
  "appearance": {
    "theme": "dark",
    "opacity": 0.9,
    "position": "cursor"
  },
  "shortcuts": {
    "1": {
      "label": "Calculator",
      "icon": "calculator.png",
      "action": {
        "type": "application",
        "path": "calc.exe"
      }
    },
    "2": {
      "label": "Notepad",
      "icon": "notepad.png",
      "action": {
        "type": "application",
        "path": "notepad.exe"
      }
    },
    "3": {
      "label": "Screenshot",
      "icon": "camera.png",
      "action": {
        "type": "keystroke",
        "keys": "Win+Shift+S"
      }
    }
  }
}
```
