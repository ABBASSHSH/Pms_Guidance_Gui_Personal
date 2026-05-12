#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : ConverterModule
// File   : AbstractJsonActionHandler.cs
// Description: Provides the common infrastructure shared by all JSON action handlers.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Extracted common code from concrete JSON action handlers
//--------------------------------------------------------------------
#endregion

using System;
using System.Text.Json;
using Infrastructure;

namespace ConverterModule
{
    /// <summary>
    /// Base class for all JSON action handlers. Holds the shared <see cref="ILogger"/>
    /// dependency, wraps <see cref="HandleAction"/> in a consistent try/catch block,
    /// provides the <see cref="DeserializeMessage{T}"/> helper, and delegates
    /// handler-specific logic to the abstract <see cref="BuildCommand"/> method.
    /// </summary>
    internal abstract class AbstractJsonActionHandler : IJsonActionHandler
    {
        #region Public Members

        /// <inheritdoc/>
        public abstract string ActionName { get; }

        /// <inheritdoc/>
        public ICommand? HandleAction(string message)
        {
            try
            {
                return BuildCommand(message);
            }
            catch (JsonException jex)
            {
                m_logger.LogError(string.Format("[{0}] JSON deserialization failed: {1}", ActionName, jex.Message), jex);
                return null;
            }
            catch (InvalidOperationException ioex)
            {
                m_logger.LogError(string.Format("[{0}] JSON payload validation failed: {1}", ActionName, ioex.Message), ioex);
                return null;
            }
        }

        #endregion

        #region Protected Members

        /// <summary>The logger available to subclasses.</summary>
        protected readonly ILogger m_logger;

        /// <summary>
        /// Initializes the common handler state.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="logger"/> is <c>null</c>.
        /// </exception>
        protected AbstractJsonActionHandler(ILogger logger)
        {
            m_logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Deserializes <paramref name="message"/> to <typeparamref name="T"/> and
        /// throws <see cref="InvalidOperationException"/> when the result is <c>null</c>.
        /// </summary>
        /// <typeparam name="T">The target message type.</typeparam>
        /// <param name="message">The raw JSON string to deserialize.</param>
        /// <returns>The deserialized, non-null message object.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when <see cref="JsonSerializer.Deserialize{TValue}(string)"/> returns <c>null</c>.
        /// </exception>
        protected static T DeserializeMessage<T>(string message) where T : class
        {
            var result = JsonSerializer.Deserialize<T>(message);

            if (result == null)
            {
                throw new InvalidOperationException(
                    string.Format("Deserialized {0} is null.", typeof(T).Name));
            }

            return result;
        }

        /// <summary>
        /// Builds the command from the JSON <paramref name="message"/>. Called by
        /// <see cref="HandleAction"/> after the outer try block is entered.
        /// May throw <see cref="JsonException"/> — it will be caught and logged by the base class.
        /// </summary>
        /// <param name="message">The raw JSON string to process.</param>
        /// <returns>The command produced from the message.</returns>
        protected abstract ICommand BuildCommand(string message);

        #endregion
    }
}
