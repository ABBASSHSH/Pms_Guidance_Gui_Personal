#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : ConnectionModule
// File   : RawMessage.cs
// Description: Represents a raw JSON message exchanged with the web app.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – compliance with C# Coding Guidelines V2.1
//--------------------------------------------------------------------
#endregion

using System.Text.Json;
using System.Text.Json.Serialization;

namespace ConnectionModule.JsonMessage
{
    /// <summary>
    /// Represents a raw JSON message exchanged with the web app.
    /// </summary>
    internal class RawMessage
    {
        /// <summary>
        /// Gets or sets the call context, which typically identifies the message type or action.
        /// </summary>
        /// <value>The call context object.</value>
        [JsonPropertyName("CallContext")]
        public JsonMessage.CallContext? CallContext { get; set; }

        /// <summary>
        /// Gets or sets the payload, which contains the message data.
        /// </summary>
        /// <value>The raw JSON payload element.</value>
        [JsonPropertyName("Payload")]
        public JsonElement Payload { get; set; }
    }
}
