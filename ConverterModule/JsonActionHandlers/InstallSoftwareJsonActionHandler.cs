#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : ConverterModule
// File   : InstallSoftwareJsonActionHandler.cs
// Description: Handles the InstallSoftware JSON action received from the front end.
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
    /// Handles the <c>InstallSoftware</c> JSON action received from the front end.
    /// No payload fields are expected — the action name alone triggers the command.
    /// </summary>
    /// <reqkeys>
    /// <reqkey> REQUIREMENT_KEY </reqkey>
    /// </reqkeys>
    internal class InstallSoftwareJsonActionHandler : AbstractJsonActionHandler
    {
        #region Public Members

        /// <inheritdoc/>
        public override string ActionName => "InstallSoftware";

        #endregion

        #region Protected Members

        /// <inheritdoc/>
        protected override ICommand BuildCommand(string message)
        {
            var payload = DeserializeMessage<InstallSoftwareMessage>(message);
            return new InstallSoftwareCommand();
        }

        #endregion

        #region Private Members

        internal InstallSoftwareJsonActionHandler(ILogger logger)
            : base(logger)
        {
        }

        #endregion
    }
}
