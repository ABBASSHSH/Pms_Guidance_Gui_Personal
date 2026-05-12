#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : ConverterModule
// File   : IJsonWriter.cs
// Description: Defines the contract for creating JSON reply messages from event data.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – compliance with C# Coding Guidelines V2.1
//--------------------------------------------------------------------
#endregion

using System;
using Infrastructure;

namespace ConverterModule
{
    /// <summary>
    /// Defines the contract for creating JSON reply messages from event data.
    /// </summary>
    internal interface IJsonWriter
    {
        /// <summary>
        /// Creates an outbound message from the given event data.
        /// </summary>
        /// <param name="theDataEvent">The event data to serialize.</param>
        /// <returns>An <see cref="OutboundMessage"/> representing the event.</returns>
        OutboundMessage CreateJsonMessage(EventArgs theDataEvent);

        /// <summary>
        /// Determines whether this writer can handle the specified event data.
        /// </summary>
        /// <param name="theDataEvent">The event data to check.</param>
        /// <returns><c>true</c> if this writer can handle the event; otherwise, <c>false</c>.</returns>
        bool CanWrite(EventArgs theDataEvent);
    }
}
