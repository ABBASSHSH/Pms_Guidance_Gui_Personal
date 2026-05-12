#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : ConnectionModule
// File   : CallContext.cs
// Description: Represents the call context portion of a raw JSON message.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – compliance with C# Coding Guidelines V2.1
//--------------------------------------------------------------------
#endregion

using System.Text.Json.Serialization;

namespace ConnectionModule.JsonMessage
{
    /// <summary>
    /// Represents the call context portion of a raw JSON message.
    /// </summary>
    internal class CallContext
    {
        /// <summary>
        /// Gets or sets the action name for this message.
        /// </summary>
        /// <value>The action name string.</value>
        [JsonPropertyName("Action")]
        public string? Action { get; set; }
    }
}
