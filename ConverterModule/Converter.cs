#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : ConverterModule
// File   : Converter.cs
// Description: Bridges incoming JSON messages to business logic commands and outgoing replies to JSON.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – compliance with C# Coding Guidelines V2.1
//--------------------------------------------------------------------
#endregion

using System;
using BusinessLogicModule.EventArgs;
using ConnectionModule;
using ConverterModule.JsonWriter;
using Infrastructure;


namespace ConverterModule
{
    /// <summary>
    /// Bridges incoming JSON messages to business logic commands and outgoing replies to JSON.
    /// </summary>
    /// <reqkeys>
    /// <reqkey> REQUIREMENT_KEY </reqkey>
    /// </reqkeys>
    public class Converter : IConverter
    {
        #region Public Members

        /// <summary>
        /// Initializes a new instance of the <see cref="Converter"/> class.
        /// </summary>
        /// <param name="theBusinessLogicModule">The business logic module to forward commands to.</param>
        /// <param name="theConnectionManager">The connection manager for sending and receiving messages.</param>
        /// <param name="logger">Logger for converter routing events.</param>
        public Converter(IBusinessLogicModule theBusinessLogicModule, IConnectionManager theConnectionManager, ILogger logger)
        {
            m_jsonActionHandler   = new JsonActionHandlerManager(logger ?? throw new ArgumentNullException(nameof(logger)));
            m_jsonWriterManager   = new JsonWriterManager(logger);
            m_businessLogicModule = theBusinessLogicModule;
            m_connectionManager   = theConnectionManager;
            m_logger              = logger;

            // ── Register inbound action handlers ──────────────────────────────────
            AddJsonActionHandler(new LogJsonActionHandler(logger));
            AddJsonActionHandler(new UIAppStartedJsonActionHandler(logger));
            AddJsonActionHandler(new VerifyInstallationPrerequisitesJsonActionHandler(logger));
            AddJsonActionHandler(new InstallSoftwareJsonActionHandler(logger));
            AddJsonActionHandler(new CloseAppJsonActionHandler(logger));

            // ── Register outbound reply writers ───────────────────────────────────
            m_jsonWriterManager.AddJsonWriter(new ShowVerifyInstallationPrerequisitesJsonWriter(
                typeof(VerifyInstallationPrerequisitesStatusEventArgs)));
            m_jsonWriterManager.AddJsonWriter(new ShowSystemLanguageJsonWriter(typeof(ShowSystemLanguageEventArgs)));
        }

        /// <inheritdoc/>
        public void Open()
        {
            if (m_isOpen) { return; }
            m_connectionManager.MessageReceived += OnMessageReceived;
            m_businessLogicModule.ActionReplyEvent.OnCommandHandled += ActionReplyEvent_OnCommandHandled;
            m_isOpen = true;
        }

        /// <inheritdoc/>
        public void Close()
        {
            if (!m_isOpen) { return; }
            m_connectionManager.MessageReceived -= OnMessageReceived;
            m_businessLogicModule.ActionReplyEvent.OnCommandHandled -= ActionReplyEvent_OnCommandHandled;
            m_isOpen = false;
        }

        #endregion

        #region Private Members

        private readonly JsonWriterManager m_jsonWriterManager;
        private readonly IBusinessLogicModule m_businessLogicModule;
        private readonly IConnectionManager m_connectionManager;
        private readonly JsonActionHandlerManager m_jsonActionHandler;
        private readonly ILogger m_logger;
        private bool m_isOpen;

        private void ActionReplyEvent_OnCommandHandled(object? sender, EventArgs e)
        {
            var outbound = m_jsonWriterManager.HandleJsonReply(e);

            if (outbound == null)
            {
                m_logger.LogWarn($"No JSON writer registered for reply event type: {e.GetType().Name}");
                return;
            }

            m_connectionManager.SendMessage(outbound);
        }

        private void AddJsonActionHandler(IJsonActionHandler jsonActionHandler)
        {
            m_jsonActionHandler.AddJsonActionHandler(jsonActionHandler);
        }

        private void OnMessageReceived(object? sender, MessageReceivedEventArgs aMessageReceivedEventArgs)
        {
            if (aMessageReceivedEventArgs == null)
            {
                return;
            }

            var aCommand = m_jsonActionHandler.HandleJsonAction(aMessageReceivedEventArgs.Action, aMessageReceivedEventArgs.Payload);

            if (aCommand == null)
            {
                m_logger.LogWarn($"No handler registered for action: {aMessageReceivedEventArgs.Action}");
                return;
            }

            m_logger.LogDebug($"Action routed: {aMessageReceivedEventArgs.Action}");

            m_businessLogicModule.HandleCommand(aCommand);
        }

        #endregion
    }
}
