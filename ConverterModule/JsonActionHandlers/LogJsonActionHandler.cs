#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : ConverterModule
// File   : LogJsonActionHandler.cs
// Description: Handles the LogMessage JSON action by deserializing and creating a LogCommand.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – replaced Console.WriteLine with ILogger; added logger constructor
//--------------------------------------------------------------------
#endregion

using BusinessLogicModule.Commands;
using ConverterModule.JsonMessage;
using Infrastructure;

namespace ConverterModule
{
    /// <summary>
    /// Handles the LogMessage JSON action by deserializing and creating a <see cref="LogCommand"/>.
    /// </summary>
    /// <reqkeys>
    /// <reqkey> REQUIREMENT_KEY </reqkey>
    /// </reqkeys>
    internal class LogJsonActionHandler : AbstractJsonActionHandler
    {
        #region Public Members

        /// <inheritdoc/>
        public override string ActionName => "LogMessage";

        #endregion

        #region Protected Members

        /// <inheritdoc/>
        protected override ICommand BuildCommand(string message)
        {
            var log = DeserializeMessage<LogMessage>(message);
            m_logger.LogDebug(string.Format("[{0}] Message: {1}, Timestamp: {2:O}", ActionName, log.Message, log.Timestamp));
            return new LogCommand(log.Message ?? string.Empty, log.Timestamp);
        }

        #endregion

        #region Private Members

        internal LogJsonActionHandler(ILogger logger)
            : base(logger)
        {
        }

        #endregion
    }
}
