namespace WinDeck.Core.Models;

/// <summary>
/// Logging configuration settings
/// </summary>
public class LoggingConfiguration
{
    /// <summary>
    /// Whether debug logging is enabled (default: true in Debug, false in Release)
    /// </summary>
    public bool IsDebugEnabled { get; set; } =
#if DEBUG
        true;
#else
        false;
#endif

    /// <summary>
    /// Minimum log level to write to console
    /// </summary>
    public string MinimumLevel { get; set; } = "Information";

    /// <summary>
    /// Whether to log to file
    /// </summary>
    public bool LogToFile { get; set; } = true;

    /// <summary>
    /// Log file path (relative to application directory)
    /// </summary>
    public string LogFilePath { get; set; } = "logs/windeck.log";

    /// <summary>
    /// Maximum log file size in MB before rotation
    /// </summary>
    public int MaxLogFileSizeMB { get; set; } = 10;

    /// <summary>
    /// Number of log files to keep during rotation
    /// </summary>
    public int MaxLogFiles { get; set; } = 5;

    /// <summary>
    /// Whether to include timestamps in log messages
    /// </summary>
    public bool IncludeTimestamp { get; set; } = true;

    /// <summary>
    /// Whether to include scope information in log messages
    /// </summary>
    public bool IncludeScopes { get; set; } = true;
}
