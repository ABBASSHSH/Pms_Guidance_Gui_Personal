#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : ConverterModule
// File   : ShowVerifyInstallationPrerequisitesJsonWriter.cs
// Description: Creates the outbound JSON reply for installation prerequisites check results.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – compliance with C# Coding Guidelines V2.1
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
    /// <see cref="VerifyInstallationPrerequisitesStatusEventArgs"/>.
    /// </summary>
    /// <reqkeys>
    /// <reqkey> REQUIREMENT_KEY </reqkey>
    /// </reqkeys>
    internal class ShowVerifyInstallationPrerequisitesJsonWriter : IJsonWriter
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
            var aStatusEvent = theDataEvent as VerifyInstallationPrerequisitesStatusEventArgs;
            if (aStatusEvent == null)
            {
                throw new ArgumentException(
                    "Invalid event data — expected VerifyInstallationPrerequisitesStatusEventArgs.");
            }

            return new ShowVerifyInstallationPrerequisitesMessage
            {
                Status = aStatusEvent.PrerequisitesMet ? "OK" : "Not Ok"
            };
        }

        #endregion

        #region Private Members

        private readonly Type m_eventType;

        internal ShowVerifyInstallationPrerequisitesJsonWriter(Type theEventType)
        {
            m_eventType = theEventType;
        }

        #endregion
    }
}
