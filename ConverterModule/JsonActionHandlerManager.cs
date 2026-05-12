#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : ConverterModule
// File   : JsonActionHandlerManager.cs
// Description: Dispatches incoming JSON action messages to the appropriate handler.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – compliance with C# Coding Guidelines V2.1; fixed header file name; added XML documentation
//--------------------------------------------------------------------
#endregion

using System.Collections.Generic;
using System.Linq;
using Infrastructure;

namespace ConverterModule
{
    /// <summary>
    /// Dispatches incoming JSON action messages to the appropriate registered handler.
    /// </summary>
    /// <reqkeys>
    /// <reqkey> REQUIREMENT_KEY </reqkey>
    /// </reqkeys>
    internal class JsonActionHandlerManager
    {
        #region Private Members

        private readonly List<IJsonActionHandler> m_jsonActionHandlersList = new List<IJsonActionHandler>();
        private readonly ILogger m_logger;

        /// <summary>
        /// Initialises a new <see cref="JsonActionHandlerManager"/>.
        /// </summary>
        /// <param name="logger">Logger used to report registration warnings and routing events.</param>
        internal JsonActionHandlerManager(ILogger logger)
        {
            m_logger = logger;
        }

        /// <summary>
        /// Registers a new <see cref="IJsonActionHandler"/> for the action name it advertises.
        /// Duplicate registrations and null handlers are silently ignored with a warning log.
        /// </summary>
        /// <param name="jsonActionHandler">The handler to register.</param>
        internal void AddJsonActionHandler(IJsonActionHandler jsonActionHandler)
        {
            if (jsonActionHandler == null)
            {
                m_logger.LogWarn("AddJsonActionHandler: handler is null. Skipping.");
                return;
            }

            if (m_jsonActionHandlersList.Contains(jsonActionHandler))
            {
                m_logger.LogWarn($"AddJsonActionHandler: handler '{jsonActionHandler.GetType().Name}' is already registered. Skipping.");
                return;
            }

            m_jsonActionHandlersList.Add(jsonActionHandler);
        }

        /// <summary>
        /// Unregisters the specified <see cref="IJsonActionHandler"/>.
        /// Null handlers or handlers not previously registered are silently ignored with a warning log.
        /// </summary>
        /// <param name="jsonActionHandler">The handler to unregister.</param>
        internal void RemoveJsonActionHandler(IJsonActionHandler jsonActionHandler)
        {
            if (jsonActionHandler == null)
            {
                m_logger.LogWarn("RemoveJsonActionHandler: handler is null. Skipping.");
                return;
            }

            if (!m_jsonActionHandlersList.Contains(jsonActionHandler))
            {
                m_logger.LogWarn($"RemoveJsonActionHandler: handler '{jsonActionHandler.GetType().Name}' is not registered. Skipping.");
                return;
            }

            m_jsonActionHandlersList.Remove(jsonActionHandler);
        }

        /// <summary>
        /// Dispatches <paramref name="payload"/> to the first handler whose
        /// <c>ActionName</c> matches <paramref name="actionName"/>.
        /// </summary>
        /// <param name="actionName">The action name extracted from the incoming message.</param>
        /// <param name="payload">The JSON payload string to pass to the handler.</param>
        /// <returns>
        /// The <see cref="ICommand"/> produced by the matching handler,
        /// or <c>null</c> if no handler is registered for <paramref name="actionName"/>.
        /// </returns>
        internal ICommand? HandleJsonAction(string actionName, string payload)
        {
            var handler = m_jsonActionHandlersList.FirstOrDefault(registeredHandler => registeredHandler.ActionName == actionName);
            if (handler == null)
            {
                return null;
            }

            return handler.HandleAction(payload);
        }

        #endregion
    }
}
