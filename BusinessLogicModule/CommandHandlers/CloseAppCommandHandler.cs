#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : BusinessLogicModule
// File   : CloseAppCommandHandler.cs
// Description: Handles CloseAppCommand by initiating a graceful application shutdown.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – compliance with C# Coding Guidelines V2.1
//--------------------------------------------------------------------
#endregion

using System;
using BusinessLogicModule.Commands;
using BusinessLogicModule.EventArgs;
using Infrastructure;

namespace BusinessLogicModule
{
    /// <summary>
    /// Handles <see cref="CloseAppCommand"/> by initiating a graceful shutdown and
    /// reporting the result via <see cref="IActionReplyPrivate"/>.
    /// </summary>
    /// <reqkeys>
    /// <reqkey> REQUIREMENT_KEY </reqkey>
    /// </reqkeys>
    internal class CloseAppCommandHandler : AbstractCommandHandler
    {
        #region Public Members

        /// <inheritdoc/>
        public override Type CommandType => typeof(CloseAppCommand);

        #endregion

        #region Protected Members

        /// <inheritdoc/>
        protected override void ExecuteCommand(ICommand theCommand)
        {
            m_logger.LogInfo("Close application requested. Initiating shutdown.");

            // Fire acknowledgement before shutdown so the front end receives the reply
            m_actionReplyPrivate.InvokeEvent(new CloseAppStatusEventArgs(isClosing: true));

            m_raiseCloseApplicationRequested();
        }

        #endregion

        #region Private Members

        internal CloseAppCommandHandler(
            IActionReplyPrivate replyPrivate,
            ILogger logger,
            Action raiseCloseApplicationRequested)
            : base(replyPrivate, logger)
        {
            if (raiseCloseApplicationRequested == null) { throw new ArgumentNullException(nameof(raiseCloseApplicationRequested)); }

            m_raiseCloseApplicationRequested = raiseCloseApplicationRequested;
        }

        private readonly Action m_raiseCloseApplicationRequested;

        #endregion
    }
}
