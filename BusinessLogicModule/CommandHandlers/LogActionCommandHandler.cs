#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : BusinessLogicModule
// File   : LogActionCommandHandler.cs
// Description: Handles LogCommand by parsing the log level from the Angular
//              message prefix ([INFO]/[DEBUG]/[WARN]/[ERROR]) and routing to
//              the corresponding ILogger method.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – added file header, null guards, and XML documentation
//                        Abbas Bahrainwala, 08-May-2026, Unified logger: uses ILogger.LogFrontend instead of IFrontendLogger
//                        Abbas Bahrainwala, 11-May-2026, Removed LogFrontend; handler now parses level and routes to LogInfo/Debug/Warn/Error
//--------------------------------------------------------------------
#endregion

using BusinessLogicModule.Commands;
using Infrastructure;
using System;
using System.Collections.Generic;

namespace BusinessLogicModule
{
    /// <summary>
    /// Handles <see cref="LogCommand"/> by reading the log level prefix from the
    /// Angular-formatted message (<c>[INFO]</c>, <c>[DEBUG]</c>, <c>[WARN]</c>,
    /// <c>[ERROR]</c>), stripping it, and routing the remaining body to the
    /// appropriate <see cref="ILogger"/> method.
    /// Unrecognised or missing prefixes default to <see cref="ILogger.LogInfo"/>.
    /// </summary>
    /// <reqkeys>
    /// <reqkey> REQUIREMENT_KEY </reqkey>
    /// </reqkeys>
    internal class LogActionCommandHandler : AbstractCommandHandler
    {
        #region Public Members

        /// <inheritdoc/>
        public override Type CommandType => typeof(LogCommand);

        #endregion

        #region Protected Members

        /// <inheritdoc/>
        protected override void ExecuteCommand(ICommand theCommand)
        {
            var incomingLog = ((LogCommand)theCommand).Message;

            if (string.IsNullOrWhiteSpace(incomingLog))
            {
                m_logger.LogWarn("Received empty log message from the frontend.");
                return;
            }

            var (level, body) = ParseLevel(incomingLog);

            switch (level)
            {
                case LogLevel.Debug: m_logger.LogDebug(body); break;
                case LogLevel.Warn:  m_logger.LogWarn(body);  break;
                case LogLevel.Error: m_logger.LogError(body); break;
                default:             m_logger.LogInfo(body);  break;
            }
        }

        #endregion

        #region Internal Members

        internal LogActionCommandHandler(IActionReplyPrivate replyPrivate, ILogger logger)
            : base(replyPrivate, logger)
        {
        }

        #endregion

        #region Private Members

        private enum LogLevel { Info, Debug, Warn, Error }

        private static readonly IReadOnlyList<(string Prefix, LogLevel Level)> s_prefixes =
        [
            ("[INFO]",  LogLevel.Info),
            ("[DEBUG]", LogLevel.Debug),
            ("[WARN]",  LogLevel.Warn),
            ("[ERROR]", LogLevel.Error),
        ];

        /// <summary>
        /// Checks whether <paramref name="incomingLog"/> starts with a known level prefix.
        /// Returns the matched <see cref="LogLevel"/> and the message body with the
        /// prefix stripped and leading whitespace trimmed.
        /// Falls back to <see cref="LogLevel.Info"/> with the full original string
        /// when no prefix is found.
        /// </summary>
        private static (LogLevel Level, string Body) ParseLevel(string incomingLog)
        {
            foreach (var (prefix, level) in s_prefixes)
            {
                if (incomingLog.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    return (level, incomingLog[prefix.Length..].TrimStart());
            }

            return (LogLevel.Info, incomingLog);
        }

        #endregion
    }
}
