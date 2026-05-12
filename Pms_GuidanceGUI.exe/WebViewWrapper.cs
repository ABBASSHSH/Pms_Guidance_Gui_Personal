#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : Pms_GuidanceGUI
// File   : WebViewWrapper.cs
// Description: Encapsulates the initialization and messaging logic for a WebView2 control
//              hosting an Angular web app.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – compliance with C# Coding Guidelines V2.1
//              Code Review, 07-May-2026, Implemented ILifeCycle so that the CoreWebView2.WebMessageReceived
//              subscription is released during managed shutdown via Close().
//              Code Review, 07-May-2026, Converted InitializeAsync to Task-based implementation
//              and added synchronization guards to prevent late event subscription after shutdown.
//--------------------------------------------------------------------
#endregion

using System;
using System.Threading.Tasks;
using Infrastructure;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

namespace Pms_GuidanceGUI
{
    /// <summary>
    /// Encapsulates the initialization and messaging logic for a WebView2 control
    /// hosting an Angular web app.
    /// </summary>
    /// <reqkeys>
    /// <reqkey> REQUIREMENT_KEY </reqkey>
    /// </reqkeys>
    public class WebViewWrapper : IWebViewWrapper
    {
        #region Public Members

        /// <summary>
        /// Initializes a new instance of the <see cref="WebViewWrapper"/> class.
        /// </summary>
        public WebViewWrapper()
        {
            m_webView = new WebView2();
        }

        /// <summary>
        /// Occurs when a raw string message is received from the Angular web app via WebView2.
        /// Only messages originating from the trusted virtual host
        /// <c>https://pmsGuidanceFrontendApp</c> are forwarded.
        /// </summary>
        event EventHandler<string> IWebViewWrapper.OnMessageReceived
        {
            add { m_onMessageReceived += value; }
            remove { m_onMessageReceived -= value; }
        }

        /// <summary>
        /// Gets the underlying WebView2 control.
        /// </summary>
        public WebView2 WebViewControl => m_webView;

        /// <summary>
        /// Open is a no-op for <see cref="WebViewWrapper"/>.
        /// The WebView2 control is initialized asynchronously via
        /// <see cref="IWebViewWrapper.InitializeAsync"/> after the WPF visual tree is ready.
        /// </summary>
        void ILifeCycle.Open() { }

        /// <summary>
        /// Unsubscribes <c>CoreWebView2.WebMessageReceived</c> if WebView2 has been initialized.
        /// This releases the handler subscribed during <see cref="IWebViewWrapper.InitializeAsync"/>
        /// so that no messages are processed after the application begins shutting down.
        /// </summary>
        void ILifeCycle.Close()
        {
            lock (m_syncRoot)
            {
                m_isClosed = true;

                if (m_webView.CoreWebView2 != null && m_isMessageHandlerAttached)
                {
                    m_webView.CoreWebView2.WebMessageReceived -= WebMessageReceived;
                    m_isMessageHandlerAttached = false;
                }

                // Release delegates to avoid holding references after shutdown.
                m_onMessageReceived = null;
            }
        }

        /// <summary>
        /// Asynchronously initializes the WebView2 environment, maps the Angular build output
        /// to the virtual host <c>https://pmsGuidanceFrontendApp</c>, and navigates to
        /// <c>index.html</c>. Also subscribes to the <c>WebMessageReceived</c> event so that
        /// messages from the Angular app are forwarded via <see cref="IWebViewWrapper.OnMessageReceived"/>.
        /// </summary>
        /// <remarks>
        /// A remote debugging port (9222) is opened via <see cref="CoreWebView2EnvironmentOptions"/>
        /// to support Chrome DevTools inspection during development.
        /// </remarks>
        async Task IWebViewWrapper.InitializeAsync()
        {
            var options = new CoreWebView2EnvironmentOptions("--remote-debugging-port=9222");
            var env = await CoreWebView2Environment.CreateAsync(options: options);
            await m_webView.EnsureCoreWebView2Async(env);

            lock (m_syncRoot)
            {
                if (m_isClosed)
                {
                    return;
                }
            }

            string absoluteFolderPath = System.IO.Path.GetFullPath(
                System.IO.Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    @"..\..\..\..\..\Upgrade\dist\upgrade\browser"));
            m_webView.CoreWebView2.SetVirtualHostNameToFolderMapping(
                hostName: "pmsGuidanceFrontendApp",
                folderPath: absoluteFolderPath,
                accessKind: CoreWebView2HostResourceAccessKind.Allow);

            m_webView.CoreWebView2.Navigate("https://pmsGuidanceFrontendApp/index.html");

            lock (m_syncRoot)
            {
                if (m_isClosed)
                {
                    return;
                }

                if (!m_isMessageHandlerAttached)
                {
                    m_webView.CoreWebView2.WebMessageReceived += WebMessageReceived;
                    m_isMessageHandlerAttached = true;
                }
            }
        }

        /// <summary>
        /// Posts <paramref name="message"/> to the Angular web app as a JSON string via
        /// <see cref="CoreWebView2.PostWebMessageAsJson"/>.
        /// </summary>
        /// <param name="message">The JSON string to send. Must not be null or empty.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="message"/> is null or empty.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the WebView2 control has not yet been initialized (call
        /// <see cref="IWebViewWrapper.InitializeAsync"/> first), or when the underlying
        /// <c>PostWebMessageAsJson</c> call fails.
        /// </exception>
        void IWebViewWrapper.SendMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentException("Message must not be null or empty.", nameof(message));
            }

            if (m_webView.CoreWebView2 == null)
            {
                throw new InvalidOperationException("WebView2 is not initialized. Call InitializeAsync first.");
            }

            try
            {
                m_webView.CoreWebView2.PostWebMessageAsJson(message);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to send message to the Angular app.", ex);
            }
        }

        #endregion

        #region Private Members

        private readonly WebView2 m_webView;
        private readonly object m_syncRoot = new object();
        private bool m_isClosed;
        private bool m_isMessageHandlerAttached;
        private event EventHandler<string>? m_onMessageReceived;

        /// <summary>
        /// Handles the <see cref="CoreWebView2.WebMessageReceived"/> event.
        /// Validates that the message originates from <c>https://pmsGuidanceFrontendApp</c>
        /// to prevent processing messages from untrusted origins, then raises
        /// <see cref="m_onMessageReceived"/> with the raw message string.
        /// </summary>
        /// <param name="sender">The CoreWebView2 that raised the event.</param>
        /// <param name="e">Event arguments containing the source origin and message payload.</param>
        private void WebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            if (!Uri.TryCreate(e.Source, UriKind.Absolute, out Uri? sourceUri) ||
                !string.Equals(sourceUri.Scheme, "https", StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(sourceUri.Host, "pmsGuidanceFrontendApp", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            string message = e.TryGetWebMessageAsString();
            m_onMessageReceived?.Invoke(this, message);
        }

        #endregion
    }
}
