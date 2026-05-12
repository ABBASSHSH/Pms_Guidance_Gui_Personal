#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : ConnectionModule
// File   : ConnectionManager.cs
// Description: Manages the communication channel between the web app and the application.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – compliance with C# Coding Guidelines V2.1
//--------------------------------------------------------------------
#endregion

using System;
using System.Text.Json;
using ConnectionModule.JsonMessage;
using Infrastructure;

namespace ConnectionModule
{
    /// <summary>
    /// Manages the communication channel between the web app and the application.
    /// </summary>
    /// <reqkeys>
    /// <reqkey> REQUIREMENT_KEY </reqkey>
    /// </reqkeys>
    public class ConnectionManager : IConnectionManager
    {
        #region Public Members

        /// <inheritdoc/>
        public event EventHandler<MessageReceivedEventArgs>? MessageReceived;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionManager"/> class.
        /// </summary>
        /// <param name="theWebView">The web view wrapper used for communication.</param>
        /// <param name="logger">Logger for connection-layer events.</param>
        /// <exception cref="ArgumentNullException">Thrown when either argument is null.</exception>
        public ConnectionManager(IWebViewWrapper theWebView, ILogger logger)
        {
            m_webView = theWebView ?? throw new ArgumentNullException(nameof(theWebView));
            m_logger  = logger     ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Opens the connection and subscribes to web view messages.
        /// Subsequent calls are ignored when already open.
        /// </summary>
        public void Open()
        {
            if (m_isOpen) { return; }
            m_webView.OnMessageReceived += OnMessageReceived;
            m_isOpen = true;
        }

        /// <summary>
        /// Closes the connection and unsubscribes from web view messages.
        /// Subsequent calls are ignored when already closed.
        /// </summary>
        public void Close()
        {
            if (!m_isOpen) { return; }
            m_webView.OnMessageReceived -= OnMessageReceived;
            m_isOpen = false;
        }

        /// <inheritdoc/>
        public void SendMessage(OutboundMessage message)
        {
            if (message == null)
            {
                m_logger.LogWarn("SendMessage called with a null message. Message not sent.");
                return;
            }

            try
            {
                // Serialize using the runtime concrete type so all payload properties are
                // included alongside Action in the flat JSON object the Angular frontend
                // expects, e.g. { "Action": "ShowSystemLanguage", "Language": "en-US" }.
                m_webView.SendMessage(JsonSerializer.Serialize(message, message.GetType()));
            }
            catch (Exception ex) when (ex is JsonException or NotSupportedException)
            {
                m_logger.LogError($"Failed to serialize message for action '{message.Action}'.", ex);
            }
        }

        #endregion

        #region Private Members

        private readonly IWebViewWrapper m_webView;
        private readonly ILogger m_logger;
        private bool m_isOpen;

        private void OnMessageReceived(object? sender, string e)
        {
            if (string.IsNullOrWhiteSpace(e))
            {
                m_logger.LogWarn("Received an empty or null message. Message ignored.");
                return;
            }

            try
            {
                RawMessage? rawMessage = JsonSerializer.Deserialize<RawMessage>(e);

                if (rawMessage != null && rawMessage.CallContext != null)
                {
                    HandleCallContext(rawMessage.CallContext, rawMessage.Payload);
                }
                else
                {
                    m_logger.LogWarn("Received message could not be parsed — missing CallContext.");
                }
            }
            catch (JsonException ex)
            {
                m_logger.LogError("JSON deserialization error.", ex);
            }
            catch (Exception ex)
            {
                m_logger.LogError("Unexpected error while processing incoming message.", ex);
            }
        }

        private void HandleCallContext(CallContext callContext, object payload)
        {
            if (callContext == null || string.IsNullOrEmpty(callContext.Action))
            {
                m_logger.LogWarn("Received message has no valid Action. Message ignored.");
                return;
            }

            m_logger.LogDebug($"Message received: action={callContext.Action}");

            string message;
            try
            {
                message = JsonSerializer.Serialize(payload);
            }
            catch (Exception ex) when (ex is JsonException or NotSupportedException)
            {
                m_logger.LogError($"Failed to serialize payload for action '{callContext.Action}'.", ex);
                return;
            }

            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(callContext.Action, message));
        }

        #endregion
    }
}