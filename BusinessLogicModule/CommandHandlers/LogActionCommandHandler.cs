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

namespace BusinessLogicModule
{
    /// <summary>
    /// Handles <see cref="LogCommand"/> by reading the log level prefix from the
    /// Angular-formatted message (<c>[INFO]</c>, <c>[DEBUG]</c>, <c>[WARN]</c>,
    /// <c>[ERROR]</c>) and routing to the appropriate <see cref="ILogger"/> method.
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
            var message = ((LogCommand)theCommand).Message;

            if (string.IsNullOrWhiteSpace(message))
            {
                m_logger.LogWarn("Received empty log message from the frontend.");
                return;
            }

            if (message.StartsWith("[DEBUG]", StringComparison.OrdinalIgnoreCase))
                m_logger.LogDebug(message);
            else if (message.StartsWith("[WARN]", StringComparison.OrdinalIgnoreCase))
                m_logger.LogWarn(message);
            else if (message.StartsWith("[ERROR]", StringComparison.OrdinalIgnoreCase))
                m_logger.LogError(message);
            else
                m_logger.LogInfo(message);
        }

        #endregion

        #region Internal Members

        internal LogActionCommandHandler(IActionReplyPrivate replyPrivate, ILogger logger)
            : base(replyPrivate, logger)
        {
        }

        #endregion
    }
}
