#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : ConverterModule
// File   : LogMessage.cs
// Description: Represents a log message received from the web app as a JSON payload.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – compliance with C# Coding Guidelines V2.1
//--------------------------------------------------------------------
#endregion

using System;

namespace ConverterModule.JsonMessage
{
    /// <summary>
    /// Represents a log message received from the web app as a JSON payload.
    /// </summary>
    internal class LogMessage
    {
        /// <summary>
        /// Gets or sets the timestamp of the log entry.
        /// </summary>
        /// <value>The UTC timestamp of the log entry.</value>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the log message text.
        /// </summary>
        /// <value>The log message string.</value>
        public string? Message { get; set; }
    }
}
