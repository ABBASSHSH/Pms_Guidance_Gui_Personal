#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : Infrastructure
// File   : ICommandHandler.cs
// Description: Defines the contract for command handlers.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – compliance with C# Coding Guidelines V2.1
//--------------------------------------------------------------------
#endregion

using System;

namespace Infrastructure
{
    /// <summary>
    /// Defines the contract for command handlers.
    /// </summary>
    /// <reqkeys>
    /// <reqkey> REQUIREMENT_KEY </reqkey>
    /// </reqkeys>
    public interface ICommandHandler
    {
        /// <summary>
        /// Gets the type of command this handler is responsible for.
        /// </summary>
        /// <value>The command type.</value>
        Type CommandType { get; }

        /// <summary>
        /// Handles the specified command.
        /// </summary>
        /// <param name="command">The command to handle.</param>
        void HandleCommand(ICommand command);
    }
}
