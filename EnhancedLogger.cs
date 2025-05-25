using System;
using System.Collections.Generic;
using System.Text;
using MediaBrowser.Model.Logging;

namespace EmbyIcons;

public class EnhancedLogger : ILogger
{
    private readonly ILogger      _baseLogger;
    private readonly List<string> _logEntries;

    /// <summary>
    ///     Initializes a new instance of the <see cref="EnhancedLogger" /> class.
    /// </summary>
    /// <param name="baseLogger">The base logger to wrap and extend.</param>
    public EnhancedLogger(ILogger baseLogger)
    {
        _baseLogger = baseLogger ?? throw new ArgumentNullException(nameof(baseLogger));
        _logEntries = new List<string>();
    }

    /// <summary>
    ///     Infoes the specified message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="paramList">The param list.</param>
    public void Info(string message, params object[] paramList)
    {
        Info(message, false, paramList);
    }

    /// <summary>
    ///     Errors the specified message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="paramList">The param list.</param>
    public void Error(string message, params object[] paramList)
    {
        Error(message, false, paramList);
    }

    /// <summary>
    ///     Warns the specified message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="paramList">The param list.</param>
    public void Warn(string message, params object[] paramList)
    {
        Warn(message, false, paramList);
    }

    /// <summary>
    ///     Debugs the specified message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="paramList">The param list.</param>
    public void Debug(string message, params object[] paramList)
    {
        Debug(message, false, paramList);
    }

    /// <summary>
    ///     Fatals the specified message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="paramList">The param list.</param>
    public void Fatal(string message, params object[] paramList)
    {
        Fatal(message, false, paramList);
    }

    /// <summary>
    ///     Fatals the exception.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    /// <param name="paramList">The param list.</param>
    public void FatalException(string message, Exception exception, params object[] paramList)
    {
        FatalException(message, exception, false, paramList);
    }

    /// <summary>
    ///     Logs the exception.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    /// <param name="paramList">The param list.</param>
    public void ErrorException(string message, Exception exception, params object[] paramList)
    {
        ErrorException(message, exception, false, paramList);
    }

    /// <summary>
    ///     Logs the multiline.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="severity">The severity.</param>
    /// <param name="additionalContent">Content of the additional.</param>
    public void LogMultiline(string message, LogSeverity severity, StringBuilder additionalContent)
    {
        LogMultiline(message, severity, additionalContent, false);
    }

    /// <summary>
    ///     Logs the specified severity.
    /// </summary>
    /// <param name="severity">The severity.</param>
    /// <param name="message">The message.</param>
    /// <param name="paramList">The parameter list.</param>
    public void Log(LogSeverity severity, string message, params object[] paramList)
    {
        Log(severity, message, false, paramList);
    }

    // Implementation of obsolete methods from the interface
    [Obsolete("Do not use. This member will be removed in future versions.", true)]
    public void Log(LogSeverity severity, ReadOnlyMemory<char> message)
    {
        _baseLogger.Log(severity, message);
    }

    [Obsolete("Do not use. This member will be removed in future versions.", true)]
    public void Error(ReadOnlyMemory<char> message)
    {
        _baseLogger.Error(message);
    }

    [Obsolete("Do not use. This member will be removed in future versions.", true)]
    public void Warn(ReadOnlyMemory<char> message)
    {
        _baseLogger.Warn(message);
    }

    [Obsolete("Do not use. This member will be removed in future versions.", true)]
    public void Info(ReadOnlyMemory<char> message)
    {
        _baseLogger.Info(message);
    }

    [Obsolete("Do not use. This member will be removed in future versions.", true)]
    public void Debug(ReadOnlyMemory<char> message)
    {
        _baseLogger.Debug(message);
    }

    /// <summary>
    ///     Gets all log entries that have been collected.
    /// </summary>
    /// <returns>A list of log entries as strings.</returns>
    public IReadOnlyList<string> GetAllLogs()
    {
        return _logEntries.AsReadOnly();
    }

    /// <summary>
    ///     Gets all log entries as a single string with each log on its own line.
    /// </summary>
    /// <returns>A string containing all logs separated by newlines.</returns>
    public string GetAllLogsAsString()
    {
        return string.Join(Environment.NewLine, _logEntries);
    }

    /// <summary>
    ///     Clears all stored log entries.
    /// </summary>
    public void ClearLogs()
    {
        _logEntries.Clear();
    }

    /// <summary>
    ///     Infoes the specified message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="storeInList">if set to <c>true</c> store in the logs list.</param>
    /// <param name="paramList">The param list.</param>
    public void Info(string message, bool storeInList = false, params object[] paramList)
    {
        _baseLogger.Info(message, paramList);
        if (storeInList)
        {
            var formattedMessage = FormatMessage("INFO", message, paramList);
            _logEntries.Add(formattedMessage);
        }
    }

    /// <summary>
    ///     Errors the specified message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="storeInList">if set to <c>true</c> store in the logs list.</param>
    /// <param name="paramList">The param list.</param>
    public void Error(string message, bool storeInList = false, params object[] paramList)
    {
        _baseLogger.Error(message, paramList);
        if (storeInList)
        {
            var formattedMessage = FormatMessage("ERROR", message, paramList);
            _logEntries.Add(formattedMessage);
        }
    }

    /// <summary>
    ///     Warns the specified message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="storeInList">if set to <c>true</c> store in the logs list.</param>
    /// <param name="paramList">The param list.</param>
    public void Warn(string message, bool storeInList = false, params object[] paramList)
    {
        _baseLogger.Warn(message, paramList);
        if (storeInList)
        {
            var formattedMessage = FormatMessage("WARN", message, paramList);
            _logEntries.Add(formattedMessage);
        }
    }

    /// <summary>
    ///     Debugs the specified message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="storeInList">if set to <c>true</c> store in the logs list.</param>
    /// <param name="paramList">The param list.</param>
    public void Debug(string message, bool storeInList = false, params object[] paramList)
    {
        _baseLogger.Debug(message, paramList);
        if (storeInList)
        {
            var formattedMessage = FormatMessage("DEBUG", message, paramList);
            _logEntries.Add(formattedMessage);
        }
    }

    /// <summary>
    ///     Fatals the specified message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="storeInList">if set to <c>true</c> store in the logs list.</param>
    /// <param name="paramList">The param list.</param>
    public void Fatal(string message, bool storeInList = false, params object[] paramList)
    {
        _baseLogger.Fatal(message, paramList);
        if (storeInList)
        {
            var formattedMessage = FormatMessage("FATAL", message, paramList);
            _logEntries.Add(formattedMessage);
        }
    }

    /// <summary>
    ///     Fatals the exception.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    /// <param name="storeInList">if set to <c>true</c> store in the logs list.</param>
    /// <param name="paramList">The param list.</param>
    public void FatalException(string message, Exception exception, bool storeInList = false, params object[] paramList)
    {
        _baseLogger.FatalException(message, exception, paramList);
        if (storeInList)
        {
            var formattedMessage = FormatExceptionMessage("FATAL", message, exception, paramList);
            _logEntries.Add(formattedMessage);
        }
    }

    /// <summary>
    ///     Logs the exception.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    /// <param name="storeInList">if set to <c>true</c> store in the logs list.</param>
    /// <param name="paramList">The param list.</param>
    public void ErrorException(string message, Exception exception, bool storeInList = false, params object[] paramList)
    {
        _baseLogger.ErrorException(message, exception, paramList);
        if (storeInList)
        {
            var formattedMessage = FormatExceptionMessage("ERROR", message, exception, paramList);
            _logEntries.Add(formattedMessage);
        }
    }

    /// <summary>
    ///     Logs the multiline.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="severity">The severity.</param>
    /// <param name="additionalContent">Content of the additional.</param>
    /// <param name="storeInList">if set to <c>true</c> store in the logs list.</param>
    public void LogMultiline(string message,
                             LogSeverity severity,
                             StringBuilder additionalContent,
                             bool storeInList = false)
    {
        _baseLogger.LogMultiline(message, severity, additionalContent);
        if (storeInList)
        {
            var formattedMessage = $"[{severity}] {message}\n{additionalContent}";
            _logEntries.Add(formattedMessage);
        }
    }

    /// <summary>
    ///     Logs the specified severity.
    /// </summary>
    /// <param name="severity">The severity.</param>
    /// <param name="message">The message.</param>
    /// <param name="storeInList">if set to <c>true</c> store in the logs list.</param>
    /// <param name="paramList">The parameter list.</param>
    public void Log(LogSeverity severity, string message, bool storeInList = false, params object[] paramList)
    {
        _baseLogger.Log(severity, message, paramList);
        if (storeInList)
        {
            var formattedMessage = FormatMessage(severity.ToString(), message, paramList);
            _logEntries.Add(formattedMessage);
        }
    }

    /// <summary>
    ///     Formats a log message with parameters.
    /// </summary>
    /// <param name="level">The log level.</param>
    /// <param name="message">The message template.</param>
    /// <param name="paramList">The parameters.</param>
    /// <returns>A formatted log message.</returns>
    private string FormatMessage(string level, string message, params object[] paramList)
    {
        var formattedMessage = message;
        try
        {
            if (paramList != null && paramList.Length > 0) formattedMessage = string.Format(message, paramList);
        }
        catch (FormatException)
        {
            // In case of format errors, just use the original message
        }

        return $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [{level}] {formattedMessage}";
    }

    /// <summary>
    ///     Formats an exception log message.
    /// </summary>
    /// <param name="level">The log level.</param>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    /// <param name="paramList">The parameters.</param>
    /// <returns>A formatted exception log message.</returns>
    private string FormatExceptionMessage(string level, string message, Exception exception, params object[] paramList)
    {
        var baseMessage = FormatMessage(level, message, paramList);
        return
            $"{baseMessage}\nException: {exception.GetType().Name}: {exception.Message}\nStackTrace: {exception.StackTrace}";
    }
}