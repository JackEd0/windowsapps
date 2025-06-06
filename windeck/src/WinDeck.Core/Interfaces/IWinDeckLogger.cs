using Microsoft.Extensions.Logging;

namespace WinDeck.Core.Interfaces;

/// <summary>
/// Interface for WinDeck logging service with configurable debug/production modes
/// </summary>
public interface IWinDeckLogger
{
    /// <summary>
    /// Gets whether debug logging is enabled
    /// </summary>
    bool IsDebugEnabled { get; }

    /// <summary>
    /// Enable or disable debug logging
    /// </summary>
    /// <param name="enabled">True to enable debug logging, false to disable</param>
    void SetDebugEnabled(bool enabled);

    /// <summary>
    /// Log debug information (only when debug is enabled)
    /// </summary>
    /// <param name="message">Debug message</param>
    /// <param name="args">Message arguments</param>
    void LogDebug(string message, params object[] args);

    /// <summary>
    /// Log informational message
    /// </summary>
    /// <param name="message">Information message</param>
    /// <param name="args">Message arguments</param>
    void LogInfo(string message, params object[] args);

    /// <summary>
    /// Log warning message
    /// </summary>
    /// <param name="message">Warning message</param>
    /// <param name="args">Message arguments</param>
    void LogWarning(string message, params object[] args);

    /// <summary>
    /// Log error message
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="exception">Optional exception</param>
    /// <param name="args">Message arguments</param>
    void LogError(string message, Exception? exception = null, params object[] args);

    /// <summary>
    /// Log critical error message
    /// </summary>
    /// <param name="message">Critical error message</param>
    /// <param name="exception">Optional exception</param>
    /// <param name="args">Message arguments</param>
    void LogCritical(string message, Exception? exception = null, params object[] args);

    /// <summary>
    /// Create a scoped logger for a specific component
    /// </summary>
    /// <param name="scope">Scope name (e.g., component or service name)</param>
    /// <returns>Scoped logger instance</returns>
    IWinDeckLogger CreateScoped(string scope);
}
