#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : LoggingModule
// File   : LogEntryFormatter.cs
// Description: Single source of truth for log entry formatting.
//              Produces: [yyyy-MM-dd HH:mm:ss.fff] [SOURCE] message
//              SourceLogger delegates here so the format is defined exactly once.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – compliance with C# Coding Guidelines V2.1
//--------------------------------------------------------------------
#endregion

using System.IO;
using System.Text;
using System;

namespace LoggingModule
{
    /// <summary>
    /// Formats a log entry into its final string representation.
    /// All loggers in the module delegate to this class so the format
    /// is defined in exactly one place.
    /// </summary>
    /// <remarks>
    /// Output format: <c>[yyyy-MM-dd HH:mm:ss.fff] [LEVEL] [FileName.Method:line] message</c>
    /// <para>
    /// When an exception is supplied, additional lines are appended:
    /// <code>
    ///          Exception : ExceptionType: message
    ///          StackTrace: ...
    ///          InnerException: ExceptionType: message   (if present)
    /// </code>
    /// </para>
    /// </remarks>
    internal static class LogEntryFormatter
    {
        /// <summary>
        /// Formats a log entry string ready for writing to the log file.
        /// </summary>
        /// <param name="level">The severity label written between the timestamp and the caller tag (e.g. <c>INFO</c>).</param>
        /// <param name="message">The log message text.</param>
        /// <param name="ex">Optional exception to append after the message.</param>
        /// <returns>A formatted, newline-terminated log entry string.</returns>
        internal static string FormatLogMessage(string level, string message, Exception? ex,
            string callerMember = "", string callerFile = "", int callerLine = 0)
        {
            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var sb = new StringBuilder();

            string callerTag = string.IsNullOrEmpty(callerMember)
                ? string.Empty
                : $" [{Path.GetFileNameWithoutExtension(callerFile)}.{callerMember}:{callerLine}]";

            sb.AppendLine($"[{timestamp}] [{level}]{callerTag} {message}");

            if (ex != null)
            {
                sb.AppendLine($"         Exception : {ex.GetType().Name}: {ex.Message}");
                if (ex.StackTrace != null)
                {
                    sb.AppendLine($"         StackTrace: {ex.StackTrace}");
                }

                if (ex.InnerException != null)
                {
                    sb.AppendLine($"         InnerException: {ex.InnerException.GetType().Name}: {ex.InnerException.Message}");
                }
            }

            return sb.ToString();
        }

    }
}
