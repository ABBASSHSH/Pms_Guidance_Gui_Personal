#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : ConverterModule
// File   : JsonWriterManager.cs
// Description: Dispatches outbound reply events to the appropriate JSON writer.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – compliance with C# Coding Guidelines V2.1; added XML documentation
//--------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using Infrastructure;

namespace ConverterModule
{
    /// <summary>
    /// Dispatches outbound reply events to the appropriate registered JSON writer.
    /// </summary>
    /// <reqkeys>
    /// <reqkey> REQUIREMENT_KEY </reqkey>
    /// </reqkeys>
    internal class JsonWriterManager
    {
        #region Private Members

        private readonly List<IJsonWriter> m_jsonWritersList = new List<IJsonWriter>();
        private readonly ILogger m_logger;

        /// <summary>
        /// Initialises a new <see cref="JsonWriterManager"/>.
        /// </summary>
        /// <param name="logger">Logger used to report registration warnings.</param>
        internal JsonWriterManager(ILogger logger)
        {
            m_logger = logger;
        }

        /// <summary>
        /// Registers a new <see cref="IJsonWriter"/>.
        /// Duplicate registrations and null writers are silently ignored with a warning log.
        /// </summary>
        /// <param name="jsonWriter">The writer to register.</param>
        internal void AddJsonWriter(IJsonWriter jsonWriter)
        {
            if (jsonWriter == null)
            {
                m_logger.LogWarn("AddJsonWriter: writer is null. Skipping.");
                return;
            }

            if (m_jsonWritersList.Contains(jsonWriter))
            {
                m_logger.LogWarn($"AddJsonWriter: writer '{jsonWriter.GetType().Name}' is already registered. Skipping.");
                return;
            }

            m_jsonWritersList.Add(jsonWriter);
        }

        /// <summary>
        /// Unregisters the specified <see cref="IJsonWriter"/>.
        /// Null writers or writers not previously registered are silently ignored with a warning log.
        /// </summary>
        /// <param name="jsonWriter">The writer to unregister.</param>
        internal void RemoveJsonWriter(IJsonWriter jsonWriter)
        {
            if (jsonWriter == null)
            {
                m_logger.LogWarn("RemoveJsonWriter: writer is null. Skipping.");
                return;
            }

            if (!m_jsonWritersList.Contains(jsonWriter))
            {
                m_logger.LogWarn($"RemoveJsonWriter: writer '{jsonWriter.GetType().Name}' is not registered. Skipping.");
                return;
            }

            m_jsonWritersList.Remove(jsonWriter);
        }

        /// <summary>
        /// Dispatches the event to the first writer that can handle it.
        /// </summary>
        /// <param name="theDataEvent">The reply event to dispatch.</param>
        /// <returns>
        /// The <see cref="OutboundMessage"/> produced by the matching writer,
        /// or <c>null</c> if no writer is registered for the event type.
        /// </returns>
        internal OutboundMessage? HandleJsonReply(EventArgs theDataEvent)
        {
            foreach (var writer in m_jsonWritersList)
            {
                if (writer.CanWrite(theDataEvent))
                {
                    return writer.CreateJsonMessage(theDataEvent);
                }
            }

            return null;
        }

        #endregion
    }
}
