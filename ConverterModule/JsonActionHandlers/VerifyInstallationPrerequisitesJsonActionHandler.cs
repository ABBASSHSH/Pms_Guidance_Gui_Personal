#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : ConverterModule
// File   : VerifyInstallationPrerequisitesJsonActionHandler.cs
// Description: Handles the VerifyInstallationPrerequisites JSON action.
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
    /// Handles the <c>VerifyInstallationPrerequisite</c> JSON action received from the front end.
    /// No payload fields are expected — the action name alone triggers the command.
    /// </summary>
    /// <reqkeys>
    /// <reqkey> REQUIREMENT_KEY </reqkey>
    /// </reqkeys>
    internal class VerifyInstallationPrerequisitesJsonActionHandler : AbstractJsonActionHandler
    {
        #region Public Members

        /// <inheritdoc/>
        public override string ActionName => "VerifyInstallationPrerequisite";

        #endregion

        #region Protected Members

        /// <inheritdoc/>
        protected override ICommand BuildCommand(string message)
        {
            var payload = DeserializeMessage<VerifyInstallationPrerequisiteMessage>(message);
            return new VerifyInstallationPrerequisitesCommand();
        }

        #endregion

        #region Private Members

        internal VerifyInstallationPrerequisitesJsonActionHandler(ILogger logger)
            : base(logger)
        {
        }

        #endregion
    }
}
