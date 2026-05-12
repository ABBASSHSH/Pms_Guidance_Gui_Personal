#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : Infrastructure
// File   : IWebViewWrapper.cs
// Description: Defines the functions supporting the WebView2 control.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – compliance with C# Coding Guidelines V2.1
//              Code Review, 07-May-2026, Extended IWebViewWrapper with ILifeCycle so that
//              WebViewWrapper participates in the managed startup/shutdown lifecycle,
//              enabling clean unsubscription of CoreWebView2.WebMessageReceived on close.
//              Code Review, 07-May-2026, Changed InitializeAsync to Task-based contract
//              so startup can be awaited deterministically.
//--------------------------------------------------------------------
#endregion

using System;
using System.Threading.Tasks;

namespace Infrastructure
{
    /// <summary>
    /// Defines the functions supporting the WebView2 control.
    /// Extends <see cref="ILifeCycle"/> so that the underlying WebView2
    /// event subscription is released cleanly during managed shutdown.
    /// </summary>
    /// <reqkeys>
    /// <reqkey> REQUIREMENT_KEY </reqkey>
    /// </reqkeys>
    public interface IWebViewWrapper : ILifeCycle
    {
        /// <summary>
        /// Initializes the WebView2 control and navigates to the specified URI.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if WebView2 initialization fails.</exception>
        Task InitializeAsync();

        /// <summary>
        /// Sends a message to the embedded web app.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <exception cref="InvalidOperationException">Thrown if the WebView2 control is not initialized.</exception>
        void SendMessage(string message);

        /// <summary>
        /// Occurs when a message is received from the web app.
        /// </summary>
        event EventHandler<string> OnMessageReceived;

    }
}
