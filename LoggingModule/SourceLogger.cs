#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : LoggingModule
// File   : SourceLogger.cs
// Description: Single ILogger implementation used for every log channel.
//              The source tag (e.g. "BACKEND") is supplied at construction time;
//              all formatting is delegated to LogEntryFormatter and all file I/O
//              is delegated to FileLogWriter.
//              Instances are created exclusively via AppLoggerSetup.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – compliance with C# Coding Guidelines V2.1
//                        Abbas Bahrainwala, 08-May-2026, Implements ILogger (unified interface)
//                        Abbas Bahrainwala, 11-May-2026, Removed LogFrontend
//--------------------------------------------------------------------
#endregion

using Infrastructure;

namespace LoggingModule
{
    /// <summary>
    /// <see cref="ILogger"/> implementation. Formatting is delegated to
    /// <see cref="LogEntryFormatter"/> and all file I/O to <see cref="FileLogWriter"/>.
    /// Instances are created exclusively via <see cref="AppLoggerSetup"/>.
    /// </summary>
    /// <remarks>
    /// This class is <c>internal</c>. Consumers obtain instances via
    /// <see cref="AppLoggerSetup.Create"/>, which returns the instance as
    /// <see cref="ILogger"/> so that callers remain decoupled from the concrete
    /// implementation.
    /// </remarks>
    internal sealed class SourceLogger : ILogger
    {
        #region Internal Members

        /// <summary>
        /// Initialises a new <see cref="SourceLogger"/>.
        /// </summary>
        /// <param name="writer">Shared log write sink.</param>
        internal SourceLogger(ILogWriter writer)
        {
            m_writer = writer ?? throw new ArgumentNullException(nameof(writer));
        }

        #endregion

        #region Public Members

        /// <inheritdoc/>
        public void LogInfo(string message, string callerMember = "", string callerFile = "", int callerLine = 0)
            => m_writer.Write(LogEntryFormatter.FormatLogMessage(message, null, callerMember, callerFile, callerLine));

        /// <inheritdoc/>
        public void LogDebug(string message, string callerMember = "", string callerFile = "", int callerLine = 0)
            => m_writer.Write(LogEntryFormatter.FormatLogMessage(message, null, callerMember, callerFile, callerLine));

        /// <inheritdoc/>
        public void LogWarn(string message, string callerMember = "", string callerFile = "", int callerLine = 0)
            => m_writer.Write(LogEntryFormatter.FormatLogMessage(message, null, callerMember, callerFile, callerLine));

        /// <inheritdoc/>
        public void LogError(string message, Exception? ex = null, string callerMember = "", string callerFile = "", int callerLine = 0)
            => m_writer.Write(LogEntryFormatter.FormatLogMessage(message, ex, callerMember, callerFile, callerLine));

        #endregion

        #region Private Members

        private readonly ILogWriter m_writer;

        #endregion
    }
}
