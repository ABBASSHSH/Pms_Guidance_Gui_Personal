#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : Infrastructure
// File   : IBusinessLogicModule.cs
// Description: Defines the contract for the business logic module.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – compliance with C# Coding Guidelines V2.1
//--------------------------------------------------------------------
#endregion

namespace Infrastructure
{
    /// <summary>
    /// Defines the contract for the business logic module.
    /// </summary>
    /// <reqkeys>
    /// <reqkey> REQUIREMENT_KEY </reqkey>
    /// </reqkeys>
    public interface IBusinessLogicModule : ILifeCycle
    {
        /// <summary>
        /// Handles the specified command.
        /// </summary>
        /// <param name="command">The command to handle.</param>
        void HandleCommand(ICommand command);

        /// <summary>
        /// Gets the action reply event source.
        /// </summary>
        /// <value>The action reply event source.</value>
        IActionReply ActionReplyEvent { get; }
    }
}
