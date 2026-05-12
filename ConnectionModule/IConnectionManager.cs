#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : ConnectionModule
// File   : IConnectionManager.cs
// Description: Defines the contract for managing the communication channel with the web app.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – compliance with C# Coding Guidelines V2.1
//--------------------------------------------------------------------
#endregion

using System;
using Infrastructure;

namespace ConnectionModule
{
    /// <summary>
    /// Defines the contract for managing the communication channel with the web app.
    /// </summary>
    /// <reqkeys>
    /// <reqkey> REQUIREMENT_KEY </reqkey>
    /// </reqkeys>
    public interface IConnectionManager : ILifeCycle
    {
        /// <summary>
        /// Serializes <paramref name="message"/> and sends it to the Angular frontend.
        /// The message is serialized using its concrete runtime type so that all
        /// payload properties alongside <see cref="OutboundMessage.Action"/> are included.
        /// </summary>
        /// <param name="message">The outbound message to send.</param>
        void SendMessage(OutboundMessage message);

        /// <summary>
        /// Occurs when a message is received from the web app.
        /// </summary>
        event EventHandler<MessageReceivedEventArgs> MessageReceived;
    }
}
