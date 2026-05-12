#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : BusinessLogicModule
// File   : ActionReplyHandler.cs
// Description: Handles action reply events raised after command execution.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – compliance with C# Coding Guidelines V2.1
//--------------------------------------------------------------------
#endregion

using Infrastructure;

namespace BusinessLogicModule
{
    /// <summary>
    /// Handles action reply events raised after command execution.
    /// </summary>
    /// <reqkeys>
    /// <reqkey> REQUIREMENT_KEY </reqkey>
    /// </reqkeys>
    internal class ActionReplyHandler : IActionReply, IActionReplyPrivate
    {
        #region Public Members

        /// <inheritdoc/>
        public event System.EventHandler<System.EventArgs>? OnCommandHandled;

        #endregion

        #region Private Members

        private void Invoke(object sender, System.EventArgs eventArgs)
        {
            OnCommandHandled?.Invoke(sender, eventArgs);
        }

        void IActionReplyPrivate.InvokeEvent(System.EventArgs theEventArgs)
        {
            Invoke(this, theEventArgs);
        }

        #endregion
    }
}
