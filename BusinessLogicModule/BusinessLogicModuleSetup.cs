#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : BusinessLogicModule
// File   : BusinessLogicModuleSetup.cs
// Description: Sets up and manages the business logic module, registering command handlers.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – compliance with C# Coding Guidelines V2.1
//--------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using BusinessLogicModule.Commands;
using Infrastructure;

namespace BusinessLogicModule
{
    /// <summary>
    /// Sets up and manages the business logic module, registering command handlers.
    /// </summary>
    /// <reqkeys>
    /// <reqkey> REQUIREMENT_KEY </reqkey>
    /// </reqkeys>
    public class BusinessLogicModuleSetup : IBusinessLogicModule, ICloseApplicationRequestSource
    {
        #region Public Members

        /// <inheritdoc/>
        public IActionReply ActionReplyEvent { get; }

        /// <inheritdoc/>
        public event EventHandler<System.EventArgs>? CloseApplicationRequested;

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessLogicModuleSetup"/> class
        /// and registers all known command handlers.
        /// </summary>
        /// <param name="logger">Logger used by all command handlers.</param>
        /// <param name="systemLanguageProvider">
        /// Used by <see cref="UIAppStartedCommandHandler"/> to fetch the system UI language.
        /// </param>
        /// <param name="configurationProvider">
        /// Used by handlers that execute configured commands from
        /// <c>pms_guidance_configuration.json</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when any constructor dependency is null.
        /// </exception>
        public BusinessLogicModuleSetup(
            ILogger                 logger,
            ISystemLanguageProvider systemLanguageProvider,
            IConfigurationProvider  configurationProvider)
        {
            if (logger                 == null) { throw new ArgumentNullException(nameof(logger)); }
            if (systemLanguageProvider == null) { throw new ArgumentNullException(nameof(systemLanguageProvider)); }
            if (configurationProvider  == null) { throw new ArgumentNullException(nameof(configurationProvider)); }

            m_logger  = logger;
            ActionReplyEvent = new ActionReplyHandler();
            IActionReplyPrivate actionReply = (IActionReplyPrivate)ActionReplyEvent;

            AddCommandHandler(typeof(LogCommand),
                new LogActionCommandHandler(actionReply, logger));

            AddCommandHandler(typeof(UIAppStartedCommand),
                new UIAppStartedCommandHandler(actionReply, logger, systemLanguageProvider));

            AddCommandHandler(typeof(VerifyInstallationPrerequisitesCommand),
                new VerifyInstallationPrerequisitesCommandHandler(actionReply, logger, configurationProvider));

            AddCommandHandler(typeof(InstallSoftwareCommand),
                new InstallSoftwareCommandHandler(actionReply, logger, configurationProvider));

            AddCommandHandler(typeof(CloseAppCommand),
                new CloseAppCommandHandler(actionReply, logger, RaiseCloseApplicationRequested));
        }

        /// <inheritdoc/>
        public void HandleCommand(ICommand theCommand)
        {
            if (theCommand == null)
            {
                throw new ArgumentNullException(nameof(theCommand));
            }

            if (!m_isOpen)
            {
                m_logger.LogWarn($"Ignoring command '{theCommand.GetType().Name}' because business logic module is closed.");
                return;
            }

            if (m_commandHandlersList.ContainsKey(theCommand.GetType()))
            {
                m_commandHandlersList[theCommand.GetType()].HandleCommand(theCommand);
            }
            else
            {
                m_logger.LogWarn($"No handler registered for command type: {theCommand.GetType().Name}");
            }
        }

        /// <inheritdoc/>
        public void Open()
        {
            if (m_isOpen)
            {
                return;
            }

            m_isOpen = true;
        }

        /// <inheritdoc/>
        public void Close()
        {
            if (!m_isOpen)
            {
                return;
            }

            m_isOpen = false;
        }

        #endregion

        #region Private Members

        private readonly ILogger m_logger;
        private readonly Dictionary<Type, ICommandHandler> m_commandHandlersList = new Dictionary<Type, ICommandHandler>();
        private bool m_isOpen = true;

        private void RaiseCloseApplicationRequested()
        {
            CloseApplicationRequested?.Invoke(this, System.EventArgs.Empty);
        }

        private void AddCommandHandler(Type theCommand, ICommandHandler commandHandler)
        {
            if (theCommand == null)
            {
                throw new ArgumentNullException(nameof(theCommand));
            }

            if (commandHandler == null)
            {
                throw new ArgumentNullException(nameof(commandHandler));
            }

            if (m_commandHandlersList.ContainsKey(theCommand))
            {
                throw new InvalidOperationException(
                    $"A handler for command type '{theCommand.Name}' is already registered.");
            }

            m_commandHandlersList.Add(theCommand, commandHandler);
        }

        #endregion
    }
}
