#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : ConverterModule
// File   : ShowSystemLanguageJsonWriter.cs
// Description: Creates the outbound JSON reply for the system language detected event.
// Notes:
// Modification History : Abbas Bahrainwala, 05-May-2026, Initial creation
//--------------------------------------------------------------------
#endregion

using System;
using BusinessLogicModule.EventArgs;
using ConverterModule.JsonMessage;
using Infrastructure;

namespace ConverterModule.JsonWriter
{
    /// <summary>
    /// Creates the outbound JSON reply message for
    /// <see cref="ShowSystemLanguageEventArgs"/>.
    /// </summary>
    /// <reqkeys>
    /// <reqkey> REQUIREMENT_KEY </reqkey>
    /// </reqkeys>
    internal class ShowSystemLanguageJsonWriter : IJsonWriter
    {
        #region Public Members

        /// <inheritdoc/>
        public bool CanWrite(EventArgs theDataEvent)
        {
            return theDataEvent.GetType() == m_eventType;
        }

        /// <inheritdoc/>
        public OutboundMessage CreateJsonMessage(EventArgs theDataEvent)
        {
            var aLanguageEvent = theDataEvent as ShowSystemLanguageEventArgs;
            if (aLanguageEvent == null)
            {
                throw new ArgumentException(
                    "Invalid event data — expected ShowSystemLanguageEventArgs.");
            }

            return new ShowSystemLanguageMessage
            {
                Language = aLanguageEvent.Language
            };
        }

        #endregion

        #region Private Members

        private readonly Type m_eventType;

        internal ShowSystemLanguageJsonWriter(Type theEventType)
        {
            m_eventType = theEventType;
        }

        #endregion
    }
}
