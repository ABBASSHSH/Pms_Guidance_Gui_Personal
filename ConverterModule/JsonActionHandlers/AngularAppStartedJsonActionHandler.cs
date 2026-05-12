#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : ConverterModule
// File   : UIAppStartedJsonActionHandler.cs
// Description: Handles the UIAppStarted JSON action.
// Notes:
//--------------------------------------------------------------------
#endregion

using BusinessLogicModule.Commands;
using ConverterModule.JsonMessage;
using Infrastructure;

namespace ConverterModule
{
    /// <summary>
    /// Handles the <c>UIAppStarted</c> JSON action received from the front end.
    /// No payload fields are expected — the action name alone triggers the command.
    /// </summary>
    /// <reqkeys>
    /// <reqkey> REQUIREMENT_KEY </reqkey>
    /// </reqkeys>
    internal class UIAppStartedJsonActionHandler : AbstractJsonActionHandler
    {
        #region Public Members

        /// <inheritdoc/>
        public override string ActionName => "UIAppStarted";

        #endregion

        #region Protected Members

        /// <inheritdoc/>
        protected override ICommand BuildCommand(string message)
        {
            var payload = DeserializeMessage<UIAppStartedMessage>(message);
            return new UIAppStartedCommand();
        }

        #endregion

        #region Private Members

        internal UIAppStartedJsonActionHandler(ILogger logger)
            : base(logger)
        {
        }

        #endregion
    }
}
