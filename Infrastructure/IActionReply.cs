#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : Infrastructure
// File   : IActionReply.cs
// Description: Defines the contract for action reply event notification.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – compliance with C# Coding Guidelines V2.1
//--------------------------------------------------------------------
#endregion

using System;

namespace Infrastructure
{
    /// <summary>
    /// Defines the contract for action reply event notification.
    /// </summary>
    /// <reqkeys>
    /// <reqkey> REQUIREMENT_KEY </reqkey>
    /// </reqkeys>
    public interface IActionReply
    {
        /// <summary>
        /// Occurs when a command has been handled.
        /// </summary>
        event EventHandler<EventArgs>? OnCommandHandled;
    }
}
