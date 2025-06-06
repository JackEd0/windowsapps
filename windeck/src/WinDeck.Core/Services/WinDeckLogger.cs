using Microsoft.Extensions.Logging;
using WinDeck.Core.Interfaces;
using WinDeck.Core.Models;
using System.Text;

namespace WinDeck.Core.Services;

/// <summary>
/// WinDeck logging service with configurable debug/production modes
/// </summary>
public class WinDeckLogger : IWinDeckLogger, IDisposable
{
    private readonly ILogger _logger;
    private readonly LoggingConfiguration _config;
    private readonly string _scope;
    private bool _disposed = false;

    public bool IsDebugEnabled => _config.IsDebugEnabled;

    public WinDeckLogger(ILogger logger, LoggingConfiguration config, string scope = "WinDeck")
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _scope = scope;
    }

    public void SetDebugEnabled(bool enabled)
    {
        _config.IsDebugEnabled = enabled;
        LogInfo($"Debug logging {(enabled ? "enabled" : "disabled")}");
    }

    public void LogDebug(string message, params object[] args)
    {
        if (!_config.IsDebugEnabled) return;

        try
        {
            var formattedMessage = FormatMessage(message, args);
            _logger.LogDebug("[{Scope}] {Message}", _scope, formattedMessage);
        }
        catch (Exception ex)
        {
            // Prevent logging errors from crashing the application
            _logger.LogError(ex, "Error formatting debug log message");
        }
    }

    public void LogInfo(string message, params object[] args)
    {
        try
        {
            var formattedMessage = FormatMessage(message, args);
            _logger.LogInformation("[{Scope}] {Message}", _scope, formattedMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error formatting info log message");
        }
    }

    public void LogWarning(string message, params object[] args)
    {
        try
        {
            var formattedMessage = FormatMessage(message, args);
            _logger.LogWarning("[{Scope}] {Message}", _scope, formattedMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error formatting warning log message");
        }
    }

    public void LogError(string message, Exception? exception = null, params object[] args)
    {
        try
        {
            var formattedMessage = FormatMessage(message, args);
            if (exception != null)
            {
                _logger.LogError(exception, "[{Scope}] {Message}", _scope, formattedMessage);
            }
            else
            {
                _logger.LogError("[{Scope}] {Message}", _scope, formattedMessage);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error formatting error log message");
        }
    }

    public void LogCritical(string message, Exception? exception = null, params object[] args)
    {
        try
        {
            var formattedMessage = FormatMessage(message, args);
            if (exception != null)
            {
                _logger.LogCritical(exception, "[{Scope}] {Message}", _scope, formattedMessage);
            }
            else
            {
                _logger.LogCritical("[{Scope}] {Message}", _scope, formattedMessage);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error formatting critical log message");
        }
    }

    public IWinDeckLogger CreateScoped(string scope)
    {
        var scopedName = string.IsNullOrWhiteSpace(_scope) ? scope : $"{_scope}.{scope}";
        return new WinDeckLogger(_logger, _config, scopedName);
    }

    private static string FormatMessage(string message, params object[] args)
    {
        if (args == null || args.Length == 0)
            return message;

        try
        {
            return string.Format(message, args);
        }
        catch (FormatException)
        {
            // If formatting fails, return the original message with args appended
            var sb = new StringBuilder(message);
            sb.Append(" [Args: ");
            for (int i = 0; i < args.Length; i++)
            {
                if (i > 0) sb.Append(", ");
                sb.Append(args[i]?.ToString() ?? "null");
            }
            sb.Append("]");
            return sb.ToString();
        }
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
            // Clean up resources if needed
            _disposed = true;
        }
    }
}
