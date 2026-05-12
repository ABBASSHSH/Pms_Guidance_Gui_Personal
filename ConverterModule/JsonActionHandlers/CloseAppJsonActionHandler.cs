#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : ConverterModule
// File   : CloseAppJsonActionHandler.cs
// Description: Handles the CloseApp JSON action received from the front end.
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
    /// Handles the <c>CloseApp</c> JSON action received from the front end.
    /// No payload fields are expected — the action name alone triggers the shutdown command.
    /// </summary>
    /// <reqkeys>
    /// <reqkey> REQUIREMENT_KEY </reqkey>
    /// </reqkeys>
    internal class CloseAppJsonActionHandler : AbstractJsonActionHandler
    {
        #region Public Members

        /// <inheritdoc/>
        public override string ActionName => "CloseApp";

        #endregion

        #region Protected Members

        /// <inheritdoc/>
        protected override ICommand BuildCommand(string message)
        {
            var payload = DeserializeMessage<CloseAppMessage>(message);
            return new CloseAppCommand();
        }

        #endregion

        #region Private Members

        internal CloseAppJsonActionHandler(ILogger logger)
            : base(logger)
        {
        }

        #endregion
    }
}
