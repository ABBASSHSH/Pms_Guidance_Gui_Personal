#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : BusinessLogicModule
// File   : LogCommand.cs
// Description: Command carrying log message data to be processed by the business logic module.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – compliance with C# Coding Guidelines V2.1
//--------------------------------------------------------------------
#endregion

using System;
using Infrastructure;

namespace BusinessLogicModule.Commands
{
    /// <summary>
    /// Command carrying log message data to be processed by the business logic module.
    /// </summary>
    /// <reqkeys>
    /// <reqkey> REQUIREMENT_KEY </reqkey>
    /// </reqkeys>
    public class LogCommand : ICommand
    {
        #region Public Members

        /// <summary>
        /// Gets or sets the log message text.
        /// </summary>
        /// <value>The log message string.</value>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the log entry.
        /// </summary>
        /// <value>The UTC timestamp of the log entry.</value>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogCommand"/> class.
        /// </summary>
        /// <param name="message">The log message text.</param>
        /// <param name="timestamp">The timestamp of the log entry.</param>
        public LogCommand(string message, DateTime timestamp)
        {
            Message = message;
            Timestamp = timestamp;
        }

        #endregion
    }
}
