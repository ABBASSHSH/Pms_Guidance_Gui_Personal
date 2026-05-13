#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : Pms_GuidanceGUI
// File   : MainWindow.xaml.cs
// Description: Main application window; initialises all modules and hosts the WebView2 control.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – compliance with C# Coding Guidelines V2.1
//              Code Review, 06-May-2026, Introduced ApplicationLifecycleManager to provide a
//              controlled startup and shutdown strategy. Window.Closing drives clean shutdown;
//              event subscriptions and unsubscriptions are fully handled by ILifeCycle.Open/Close.
//              Code Review, 07-May-2026, Registered WebViewWrapper in lifecycle manager so that
//              CoreWebView2.WebMessageReceived is unsubscribed on shutdown; added startup failure
//              handling and AggregateException handling in MainWindow_Closing; Closing handler
//              self-unsubscribes; removed redundant intermediate cast variable.
//              Code Review, 07-May-2026, Updated window loaded path to await Task-based WebView
//              initialization for deterministic startup exception handling.
//--------------------------------------------------------------------
#endregion

using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using BusinessLogicModule;
using ConfigurationModule;
using ConnectionModule;
using ConverterModule;
using Infrastructure;
using LoggingModule;
using Pms_GuidanceGUI;

namespace WebAppWrapper
{
    /// <summary>
    /// Main application window. Wires together all backend modules
    /// (<see cref="BusinessLogicModuleSetup"/>, <see cref="ConnectionManager"/>,
    /// <see cref="Converter"/>) and hosts the WebView2 control that renders the Angular
    /// frontend. The window is the composition root for the entire application.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Startup and shutdown are fully delegated to an <see cref="IApplicationLifecycleManager"/>.
    /// All event subscriptions are established in <c>Open()</c> and released in <c>Close()</c>
    /// for every registered <see cref="ILifeCycle"/> component.
    /// </para>
    /// <para>
    /// Registration order determines the startup and shutdown sequence:
    /// <list type="number">
    ///   <item><see cref="WebViewWrapper"/> — opens first (no-op), closed last (releases CoreWebView2.WebMessageReceived).</item>
    ///   <item><see cref="BusinessLogicModuleSetup"/> — participates in the same managed lifecycle and remains available while dependent modules close.</item>
    ///   <item><see cref="ConnectionManager"/> — subscribes to WebView.OnMessageReceived on open; unsubscribes on close.</item>
    ///   <item><see cref="Converter"/> — subscribes to ConnectionManager and BusinessLogicModule events on open; unsubscribes first on close.</item>
    /// </list>
    /// </para>
    /// <para>
    /// The <see cref="Window.Closing"/> event triggers <see cref="ILifeCycle.Close"/> so that
    /// the application shuts down in a controlled, predictable order regardless of how it is
    /// terminated.
    /// </para>
    /// </remarks>
    /// <reqkeys>
    /// <reqkey> REQUIREMENT_KEY </reqkey>
    /// </reqkeys>
    public partial class MainWindow : Window
    {
        #region Public Members

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class,
        /// setting up the WebView2 control and all application modules.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            var logger =
                AppLoggerSetup.Create(AppDomain.CurrentDomain.BaseDirectory);
            m_logger = logger;
            m_logger.LogInfo("Application starting.");

            m_webViewWrapper = new WebViewWrapper();

            IConfigurationProvider configurationProvider = new GuidanceConfigurationProvider(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pms_guidance_configuration.json"),
                logger);
            ILifeCycle configurationLifeCycle = (ILifeCycle)configurationProvider;

            IBusinessLogicModule businessLogicModule = new BusinessLogicModuleSetup(
                logger,
                configurationProvider);
            ICloseApplicationRequestSource closeApplicationRequestSource =
                (ICloseApplicationRequestSource)businessLogicModule;

            IConnectionManager connectionManager = new ConnectionManager(m_webViewWrapper, logger);
            IConverter         converter         = new Converter(businessLogicModule, connectionManager, logger);

            // ── Register components in startup order ──────────────────────────────
            // WebViewWrapper is registered first so it is closed last, ensuring the
            // underlying WebView2 channel remains available while ConnectionManager and
            // Converter complete their unsubscriptions.
            m_lifecycleManager = new ApplicationLifecycleManager();
            m_lifecycleManager.Register(m_webViewWrapper);
            m_lifecycleManager.Register(configurationLifeCycle);
            m_lifecycleManager.Register(businessLogicModule);
            m_lifecycleManager.Register(connectionManager);
            m_lifecycleManager.Register(converter);
            m_lifecycleManager.SubscribeToCloseApplicationRequests(
                closeApplicationRequestSource,
                ShutdownApplication);

            try
            {
                m_lifecycleManager.Open();
            }
            catch (Exception ex)
            {
                m_logger.LogError("Fatal: one or more components failed to start. Shutting down.", ex);
                Dispatcher.InvokeAsync(() => Application.Current.Shutdown(1));
                return;
            }

            Content  = ((WebViewWrapper)m_webViewWrapper).WebViewControl;
            Loaded  += MainWindow_Loaded;
            Closing += MainWindow_Closing;
        }

        #endregion

        #region Private Members

        private readonly ILogger               m_logger;
        private readonly IWebViewWrapper              m_webViewWrapper;
        private readonly IApplicationLifecycleManager m_lifecycleManager;

        private void ShutdownApplication()
        {
            Dispatcher.InvokeAsync(() => Application.Current.Shutdown());
        }

        /// <summary>
        /// Handles the <see cref="Window.Loaded"/> event. Calls
        /// <see cref="IWebViewWrapper.InitializeAsync"/> after the window is fully rendered
        /// so that the WPF visual tree is ready before WebView2 is embedded.
        /// The handler unsubscribes itself because the event must fire only once.
        /// Any initialization failure is logged via <see cref="IBackendLogger.LogError"/> rather
        /// than surfaced as an unhandled exception.
        /// </summary>
        /// <param name="sender">The <see cref="MainWindow"/> that raised the event.</param>
        /// <param name="e">Routed event arguments (unused).</param>
        private async void MainWindow_Loaded(object? sender, RoutedEventArgs e)
        {
            Loaded -= MainWindow_Loaded;

            try
            {
                await m_webViewWrapper.InitializeAsync();
            }
            catch (Exception ex)
            {
                m_logger.LogError("Failed to initialize WebView2.", ex);
            }
        }

        /// <summary>
        /// Handles the <see cref="Window.Closing"/> event. Delegates shutdown to the
        /// <see cref="IApplicationLifecycleManager"/> so that all components are closed in
        /// reverse registration order and every event subscription is released cleanly.
        /// If any component fails to close, the error is logged and the window close
        /// proceeds regardless.
        /// </summary>
        /// <param name="sender">The <see cref="MainWindow"/> that raised the event.</param>
        /// <param name="e">Cancel event arguments (shutdown is not cancelled on close failure).</param>
        private void MainWindow_Closing(object? sender, CancelEventArgs e)
        {
            Closing -= MainWindow_Closing;
            m_logger.LogInfo("Application shutting down.");

            try
            {
                m_lifecycleManager.Close();
            }
            catch (AggregateException ex)
            {
                m_logger.LogError("One or more components failed to close cleanly.", ex);
            }
        }

        #endregion
    }
}