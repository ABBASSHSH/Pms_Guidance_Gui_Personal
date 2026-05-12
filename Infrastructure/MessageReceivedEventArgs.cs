#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : Infrastructure
// File   : MessageReceivedEventArgs.cs
// Description: Event arguments for messages received from the web app.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – compliance with C# Coding Guidelines V2.1
//--------------------------------------------------------------------
#endregion

using System;

namespace Infrastructure
{
    /// <summary>
    /// Event arguments for messages received from the web app.
    /// </summary>
    /// <reqkeys>
    /// <reqkey> REQUIREMENT_KEY </reqkey>
    /// </reqkeys>
    public class MessageReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the action name associated with the received message.
        /// </summary>
        /// <value>The action name string.</value>
        public string Action { get; }

        /// <summary>
        /// Gets the payload of the received message.
        /// </summary>
        /// <value>The payload string.</value>
        public string Payload { get; } = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageReceivedEventArgs"/> class.
        /// </summary>
        /// <param name="action">The action name.</param>
        /// <param name="payload">The message payload.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="action"/> or <paramref name="payload"/> is null.</exception>
        public MessageReceivedEventArgs(string action, string payload)
        {
            Action = action ?? throw new ArgumentNullException(nameof(action));
            Payload = payload ?? throw new ArgumentNullException(nameof(payload));
        }
    }
}
