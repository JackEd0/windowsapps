using Microsoft.Extensions.Logging;
using WinDeck.Core.Interfaces;
using WinDeck.Core.Models;
using WinDeck.Core.Services;

namespace WinDeck.Core.Services;

/// <summary>
/// Factory for creating WinDeck loggers with consistent configuration
/// </summary>
public class LoggingService : IDisposable
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly LoggingConfiguration _config;
    private bool _disposed = false;

    public LoggingService(LoggingConfiguration? config = null)
    {
        _config = config ?? new LoggingConfiguration();
        _loggerFactory = CreateLoggerFactory();
    }

    /// <summary>
    /// Create a logger for the specified category/scope
    /// </summary>
    /// <param name="categoryName">Category name (typically class name)</param>
    /// <returns>WinDeck logger instance</returns>
    public IWinDeckLogger CreateLogger(string categoryName)
    {
        var logger = _loggerFactory.CreateLogger(categoryName);
        return new WinDeckLogger(logger, _config, categoryName);
    }

    /// <summary>
    /// Create a logger for the specified type
    /// </summary>
    /// <typeparam name="T">Type to create logger for</typeparam>
    /// <returns>WinDeck logger instance</returns>
    public IWinDeckLogger CreateLogger<T>()
    {
        return CreateLogger(typeof(T).Name);
    }

    /// <summary>
    /// Get the current logging configuration
    /// </summary>
    public LoggingConfiguration Configuration => _config;

    /// <summary>
    /// Update logging configuration at runtime
    /// </summary>
    /// <param name="config">New configuration</param>
    public void UpdateConfiguration(LoggingConfiguration config)
    {
        if (config == null) throw new ArgumentNullException(nameof(config));

        // Update existing configuration
        _config.IsDebugEnabled = config.IsDebugEnabled;
        _config.MinimumLevel = config.MinimumLevel;
        _config.LogToFile = config.LogToFile;
        _config.LogFilePath = config.LogFilePath;
        _config.MaxLogFileSizeMB = config.MaxLogFileSizeMB;
        _config.MaxLogFiles = config.MaxLogFiles;
        _config.IncludeTimestamp = config.IncludeTimestamp;
        _config.IncludeScopes = config.IncludeScopes;
    }

    private ILoggerFactory CreateLoggerFactory()
    {
        var factory = LoggerFactory.Create(builder =>
        {
            // Configure console logging
            builder.AddConsole(options =>
            {
                options.IncludeScopes = _config.IncludeScopes;
                options.TimestampFormat = _config.IncludeTimestamp ? "[yyyy-MM-dd HH:mm:ss] " : null;
            });

            // Add debug output for development
#if DEBUG
            builder.AddDebug();
#endif

            // Set minimum log level
            if (Enum.TryParse<LogLevel>(_config.MinimumLevel, out var minLevel))
            {
                builder.SetMinimumLevel(minLevel);
            }

            // Configure filtering
            builder.AddFilter((category, level) =>
            {
                // Always allow warnings and above
                if (level >= LogLevel.Warning) return true;

                // For debug messages, check if debug is enabled
                if (level == LogLevel.Debug) return _config.IsDebugEnabled;

                // For info and trace, allow based on minimum level
                return level >= (Enum.TryParse<LogLevel>(_config.MinimumLevel, out var min) ? min : LogLevel.Information);
            });
        });

        return factory;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _loggerFactory?.Dispose();
            _disposed = true;
        }
    }
}
