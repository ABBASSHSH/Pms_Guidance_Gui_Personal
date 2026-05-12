#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : Pms_GuidanceGUI.Tests
// File   : BackendFlowComponentTests.cs
// Description: Component tests for the complete backend message flow.
//              Real implementations are used throughout the stack:
//                IWebViewWrapper (mock)
//                  ↓↑
//                ConnectionManager  (real)
//                  ↓↑
//                Converter          (real)
//                  ↓↑
//                BusinessLogicModuleSetup  (real)
//                  ↓↑
//                Command handlers   (real)
//              ILogger is mocked to avoid file I/O.
//              All tests follow the Given/When/Then naming convention.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – compliance with C# Coding Guidelines V2.1
//--------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using BusinessLogicModule;
using BusinessLogicModule.EventArgs;
using ConfigurationModule;
using ConnectionModule;
using ConverterModule;
using Infrastructure;
using LoggingModule;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Pms_GuidanceGUI.Tests.Component
{
    /// <summary>
    /// Component tests for the complete backend message flow.
    ///
    /// Flow under test:
    ///   Mock&lt;IWebViewWrapper&gt; → ConnectionManager → Converter → BusinessLogicModuleSetup
    ///                                                                    ↓
    ///                        Mock&lt;IWebViewWrapper&gt; ← ConnectionManager ← Converter ← ActionReplyEvent
    ///
    /// Only <see cref="IWebViewWrapper"/> and <see cref="ILogger"/> are mocked — all
    /// other classes are real implementations wired together exactly as in production.
    /// </summary>
    [TestClass]
    public class BackendFlowComponentTests
    {
        #region Private Members

        private Mock<IWebViewWrapper>    m_mockWebView        = null!;
        private Mock<ILogger>            m_mockLogger         = null!;
        private List<string>             m_sentMessages       = null!;
        private ConnectionManager        m_connectionManager = null!;
        private BusinessLogicModuleSetup m_blm               = null!;
        private Converter                m_converter         = null!;
        private Mock<ISystemLanguageProvider> m_mockSystemLanguageProvider = null!;
        private Mock<IConfigurationProvider> m_mockConfigurationProvider = null!;

        #endregion

        /// <summary>
        /// Initializes the test context before each test method runs.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            m_mockWebView        = new Mock<IWebViewWrapper>();
            m_mockLogger         = new Mock<ILogger>();
            m_mockSystemLanguageProvider = new Mock<ISystemLanguageProvider>();
            m_mockConfigurationProvider  = new Mock<IConfigurationProvider>();
            m_mockConfigurationProvider.Setup(x => x.GetVerificationCommand()).Returns("exit 1");
            m_mockConfigurationProvider.Setup(x => x.GetInstallationCommand()).Returns("exit 1");
            m_sentMessages = new List<string>();

            m_mockWebView
                .Setup(x => x.SendMessage(It.IsAny<string>()))
                .Callback<string>(msg => m_sentMessages.Add(msg));

            m_connectionManager = new ConnectionManager(m_mockWebView.Object, m_mockLogger.Object);
            m_blm               = new BusinessLogicModuleSetup(
                m_mockLogger.Object,
                m_mockSystemLanguageProvider.Object,
                m_mockConfigurationProvider.Object);
            m_converter         = new Converter(m_blm, m_connectionManager, m_mockLogger.Object);

            m_connectionManager.Open();
            m_converter.Open();
        }

        /// <summary>
        /// Cleans up resources after each test method runs.
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            m_converter.Close();
            m_connectionManager.Close();
        }

        // ─────────────────────────────────────────────────────────────────────────
        // LogMessage — full round-trip
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>
    /// Given LogMessagePath LogMessage When IncomingMessageReceived Then LoggerLogIsCalled
        /// </summary>
        [TestMethod]
        public void Given_LogMessagePath_LogMessage_When_IncomingMessageReceived_Then_LoggerLogIsCalled()
        {
            SimulateIncoming("LogMessage", BuildLogPayload("logger check", DateTime.UtcNow));

            m_mockLogger.Verify(x => x.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.AtLeastOnce());
        }

        /// <summary>
    /// Given LogMessagePath LogMessageWithSpecificText When IncomingMessageReceived Then LoggerReceivesMessageText
        /// </summary>
        [TestMethod]
        public void Given_LogMessagePath_LogMessageWithSpecificText_When_IncomingMessageReceived_Then_LoggerReceivesMessageText()
        {
            const string expectedText = "specific log text 99";
            SimulateIncoming("LogMessage", BuildLogPayload(expectedText, DateTime.UtcNow));

            m_mockLogger.Verify(
                x => x.LogInfo(It.Is<string>(s => s.Contains(expectedText)), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                Times.Once());
        }

        // ─────────────────────────────────────────────────────────────────────────
        // VerifyInstallationPrerequisite — full round-trip
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>
    /// Given VerifyPrerequisitePath VerifyPrerequisiteMessage When IncomingMessageReceived Then ReplyIsSent
        /// </summary>
        [TestMethod]
        public void Given_VerifyPrerequisitePath_VerifyPrerequisiteMessage_When_IncomingMessageReceived_Then_ReplyIsSent()
        {
            SimulateIncoming("VerifyInstallationPrerequisite", "{}");

            Assert.AreEqual(1, m_sentMessages.Count);
        }

        /// <summary>
        /// Given VerifyPrerequisitePath VerifyPrerequisiteMessage When IncomingMessageReceived Then ReplyActionIsShowInstallationPrerequisite
        /// </summary>
        [TestMethod]
        public void Given_VerifyPrerequisitePath_VerifyPrerequisiteMessage_When_IncomingMessageReceived_Then_ReplyActionIsShowInstallationPrerequisite()
        {
            SimulateIncoming("VerifyInstallationPrerequisite", "{}");

            Assert.AreEqual("ShowInstallationPrerequisite", ParseAction(m_sentMessages[0]));
        }

        /// <summary>
    /// Given VerifyPrerequisitePath VerifyPrerequisiteMessage When IncomingMessageReceived Then StatusIsOk
        /// </summary>
        [TestMethod]
        public void Given_VerifyPrerequisitePath_VerifyPrerequisiteMessage_When_IncomingMessageReceived_Then_StatusIsOk()
        {
            SimulateIncoming("VerifyInstallationPrerequisite", "{}");

            Assert.AreEqual("OK", ParsePayloadString(m_sentMessages[0], "Status"));
        }

        /// <summary>
    /// Given VerifyPrerequisitePath VerificationCommandFailing When IncomingMessageReceived Then StatusIsNotOk
        /// </summary>
        [TestMethod]
    public void Given_VerifyPrerequisitePath_VerificationCommandFailing_When_IncomingMessageReceived_Then_StatusIsNotOk()
        {
            m_converter.Close();
            m_connectionManager.Close();

            m_mockConfigurationProvider.Setup(x => x.GetVerificationCommand()).Returns("exit 0");
            m_mockConfigurationProvider.Setup(x => x.GetInstallationCommand()).Returns("exit 1");
            m_sentMessages.Clear();

            m_blm = new BusinessLogicModuleSetup(
                m_mockLogger.Object,
                m_mockSystemLanguageProvider.Object,
                m_mockConfigurationProvider.Object);
            m_connectionManager = new ConnectionManager(m_mockWebView.Object, m_mockLogger.Object);
            m_converter = new Converter(m_blm, m_connectionManager, m_mockLogger.Object);

            m_connectionManager.Open();
            m_converter.Open();

            SimulateIncoming("VerifyInstallationPrerequisite", "{}");

            Assert.AreEqual(1, m_sentMessages.Count);
            Assert.AreEqual("Not Ok", ParsePayloadString(m_sentMessages[0], "Status"));
        }

        /// <summary>
    /// Given VerifyPrerequisitePath VerifyPrerequisiteMessage When IncomingMessageReceived Then OnCommandHandledFiredOnce
        /// </summary>
        [TestMethod]
        public void Given_VerifyPrerequisitePath_VerifyPrerequisiteMessage_When_IncomingMessageReceived_Then_OnCommandHandledFiredOnce()
        {
            int eventCount = 0;
            m_blm.ActionReplyEvent.OnCommandHandled += (s, e) => eventCount++;

            SimulateIncoming("VerifyInstallationPrerequisite", "{}");

            Assert.AreEqual(1, eventCount);
        }

        /// <summary>
    /// Given VerifyPrerequisitePath VerifyPrerequisiteMessage When IncomingMessageReceived Then EventArgsIsCorrectType
        /// </summary>
        [TestMethod]
        public void Given_VerifyPrerequisitePath_VerifyPrerequisiteMessage_When_IncomingMessageReceived_Then_EventArgsIsCorrectType()
        {
            System.EventArgs? capturedArgs = null;
            m_blm.ActionReplyEvent.OnCommandHandled += (s, e) => capturedArgs = e;

            SimulateIncoming("VerifyInstallationPrerequisite", "{}");

            Assert.IsInstanceOfType(capturedArgs, typeof(VerifyInstallationPrerequisitesStatusEventArgs));
        }

        /// <summary>
    /// Given VerifyPrerequisitePath VerifyPrerequisiteMessage When IncomingMessageReceived Then LogInfoCalledAtLeastOnce
        /// </summary>
        [TestMethod]
        public void Given_VerifyPrerequisitePath_VerifyPrerequisiteMessage_When_IncomingMessageReceived_Then_LogInfoCalledAtLeastOnce()
        {
            SimulateIncoming("VerifyInstallationPrerequisite", "{}");

            m_mockLogger.Verify(x => x.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.AtLeastOnce());
        }

        // ─────────────────────────────────────────────────────────────────────────
        // InstallSoftware — full round-trip
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Given InstallSoftwarePath InstallSoftwareMessage When IncomingMessageReceived Then NoReplyIsSent
        /// </summary>
        [TestMethod]
        public void Given_InstallSoftwarePath_InstallSoftwareMessage_When_IncomingMessageReceived_Then_NoReplyIsSent()
        {
            SimulateIncoming("InstallSoftware", "{}");

            Assert.AreEqual(0, m_sentMessages.Count);
        }

        /// <summary>
    /// Given InstallSoftwarePath InstallSoftwareMessage When IncomingMessageReceived Then NoInstallReplyActionIsSent
        /// </summary>
        [TestMethod]
        public void Given_InstallSoftwarePath_InstallSoftwareMessage_When_IncomingMessageReceived_Then_NoInstallReplyActionIsSent()
        {
            SimulateIncoming("InstallSoftware", "{}");

            Assert.AreEqual(0, m_sentMessages.Count);
        }

        /// <summary>
    /// Given InstallSoftwarePath InstallSoftwareMessage When IncomingMessageReceived Then NoInstallPayloadIsSent
        /// </summary>
        [TestMethod]
        public void Given_InstallSoftwarePath_InstallSoftwareMessage_When_IncomingMessageReceived_Then_NoInstallPayloadIsSent()
        {
            SimulateIncoming("InstallSoftware", "{}");

            Assert.AreEqual(0, m_sentMessages.Count);
        }

        /// <summary>
    /// Given InstallSoftwarePath InstallationCommandFailing When IncomingMessageReceived Then NoInstallPayloadIsSent
        /// </summary>
        [TestMethod]
    public void Given_InstallSoftwarePath_InstallationCommandFailing_When_IncomingMessageReceived_Then_NoInstallPayloadIsSent()
        {
            m_converter.Close();
            m_connectionManager.Close();

            m_mockConfigurationProvider.Setup(x => x.GetVerificationCommand()).Returns("exit 1");
            m_mockConfigurationProvider.Setup(x => x.GetInstallationCommand()).Returns("exit 0");
            m_sentMessages.Clear();

            m_blm = new BusinessLogicModuleSetup(
                m_mockLogger.Object,
                m_mockSystemLanguageProvider.Object,
                m_mockConfigurationProvider.Object);
            m_connectionManager = new ConnectionManager(m_mockWebView.Object, m_mockLogger.Object);
            m_converter = new Converter(m_blm, m_connectionManager, m_mockLogger.Object);

            m_connectionManager.Open();
            m_converter.Open();

            SimulateIncoming("InstallSoftware", "{}");

            Assert.AreEqual(0, m_sentMessages.Count);
        }

        /// <summary>
    /// Given InstallSoftwarePath InstallSoftwareMessage When IncomingMessageReceived Then OnCommandHandledFiredOnce
        /// </summary>
        [TestMethod]
        public void Given_InstallSoftwarePath_InstallSoftwareMessage_When_IncomingMessageReceived_Then_OnCommandHandledFiredOnce()
        {
            int eventCount = 0;
            m_blm.ActionReplyEvent.OnCommandHandled += (s, e) => eventCount++;

            SimulateIncoming("InstallSoftware", "{}");

            Assert.AreEqual(1, eventCount);
        }

        /// <summary>
    /// Given InstallSoftwarePath InstallSoftwareMessage When IncomingMessageReceived Then EventArgsIsCorrectType
        /// </summary>
        [TestMethod]
        public void Given_InstallSoftwarePath_InstallSoftwareMessage_When_IncomingMessageReceived_Then_EventArgsIsCorrectType()
        {
            System.EventArgs? capturedArgs = null;
            m_blm.ActionReplyEvent.OnCommandHandled += (s, e) => capturedArgs = e;

            SimulateIncoming("InstallSoftware", "{}");

            Assert.IsInstanceOfType(capturedArgs, typeof(InstallSoftwareStatusEventArgs));
        }

        /// <summary>
    /// Given InstallSoftwarePath InstallSoftwareMessage When IncomingMessageReceived Then LogInfoCalledAtLeastOnce
        /// </summary>
        [TestMethod]
        public void Given_InstallSoftwarePath_InstallSoftwareMessage_When_IncomingMessageReceived_Then_LogInfoCalledAtLeastOnce()
        {
            SimulateIncoming("InstallSoftware", "{}");

            m_mockLogger.Verify(x => x.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.AtLeastOnce());
        }

        // ─────────────────────────────────────────────────────────────────────────
        // CloseApp — full round-trip
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>
    /// Given CloseAppPath CloseAppMessage When IncomingMessageReceived Then OnCommandHandledFiredOnce
        /// </summary>
        [TestMethod]
        public void Given_CloseAppPath_CloseAppMessage_When_IncomingMessageReceived_Then_OnCommandHandledFiredOnce()
        {
            int eventCount = 0;
            m_blm.ActionReplyEvent.OnCommandHandled += (s, e) => eventCount++;

            SimulateIncoming("CloseApp", "{}");

            Assert.AreEqual(1, eventCount);
        }

        /// <summary>
    /// Given CloseAppPath CloseAppMessage When IncomingMessageReceived Then EventArgsIsCorrectType
        /// </summary>
        [TestMethod]
        public void Given_CloseAppPath_CloseAppMessage_When_IncomingMessageReceived_Then_EventArgsIsCorrectType()
        {
            System.EventArgs? capturedArgs = null;
            m_blm.ActionReplyEvent.OnCommandHandled += (s, e) => capturedArgs = e;

            SimulateIncoming("CloseApp", "{}");

            Assert.IsInstanceOfType(capturedArgs, typeof(CloseAppStatusEventArgs));
        }

        /// <summary>
    /// Given CloseAppPath CloseAppMessage When IncomingMessageReceived Then LogInfoCalledAtLeastOnce
        /// </summary>
        [TestMethod]
        public void Given_CloseAppPath_CloseAppMessage_When_IncomingMessageReceived_Then_LogInfoCalledAtLeastOnce()
        {
            SimulateIncoming("CloseApp", "{}");

            m_mockLogger.Verify(x => x.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.AtLeastOnce());
        }

        // ─────────────────────────────────────────────────────────────────────────
        // Multi-action sequence — one incoming triggers exactly one outgoing reply
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>
    /// Given MultiActionPath AllFourActions When EachReceivedInSequence Then OneReplySent
        /// </summary>
        [TestMethod]
        public void Given_MultiActionPath_AllFourActions_When_EachReceivedInSequence_Then_OneReplySent()
        {
            SimulateIncoming("LogMessage",                    BuildLogPayload("x", DateTime.UtcNow));
            SimulateIncoming("VerifyInstallationPrerequisite", "{}");
            SimulateIncoming("InstallSoftware",               "{}");
            SimulateIncoming("CloseApp",                      "{}");

            Assert.AreEqual(1, m_sentMessages.Count);
        }

        /// <summary>
    /// Given MultiActionPath AllFourActions When EachReceivedInSequence Then OnlyVerifyReplyIsSent
        /// </summary>
        [TestMethod]
        public void Given_MultiActionPath_AllFourActions_When_EachReceivedInSequence_Then_OnlyVerifyReplyIsSent()
        {
            SimulateIncoming("LogMessage",                    BuildLogPayload("x", DateTime.UtcNow));
            SimulateIncoming("VerifyInstallationPrerequisite", "{}");
            SimulateIncoming("InstallSoftware",               "{}");
            SimulateIncoming("CloseApp",                      "{}");

            Assert.AreEqual("ShowInstallationPrerequisite", ParseAction(m_sentMessages[0]));
            Assert.AreEqual(1, m_sentMessages.Count);
        }

        /// <summary>
    /// Given MultiActionPath AllFourActions When EachReceivedInSequence Then ThreeCommandHandledEventsFired
        /// </summary>
        [TestMethod]
        public void Given_MultiActionPath_AllFourActions_When_EachReceivedInSequence_Then_ThreeCommandHandledEventsFired()
        {
            int eventCount = 0;
            m_blm.ActionReplyEvent.OnCommandHandled += (s, e) => eventCount++;

            SimulateIncoming("LogMessage",                    BuildLogPayload("x", DateTime.UtcNow));
            SimulateIncoming("VerifyInstallationPrerequisite", "{}");
            SimulateIncoming("InstallSoftware",               "{}");
            SimulateIncoming("CloseApp",                      "{}");

            Assert.AreEqual(3, eventCount);
        }

        // ─────────────────────────────────────────────────────────────────────────
        // Backend + Frontend logger distinction (two-logger overload)
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>
    /// Given DualLoggerPath SeparateBackendAndFrontendLoggers When LogMessageReceived Then FrontendLoggerIsUsedForLogEntry
        /// </summary>
        [TestMethod]
        public void Given_DualLoggerPath_SeparateBackendAndFrontendLoggers_When_LogMessageReceived_Then_FrontendLoggerIsUsedForLogEntry()
        {
            string tempFolder = Path.Combine(
                System.IO.Path.GetTempPath(), "PmsCompTest_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempFolder);
            string logFile = Path.Combine(tempFolder, "logs", "app.log");

            try
            {
                var logger = AppLoggerSetup.Create(tempFolder);
                var mockConfigurationProvider = new Mock<IConfigurationProvider>();
                mockConfigurationProvider.Setup(x => x.GetVerificationCommand()).Returns("exit 1");
                mockConfigurationProvider.Setup(x => x.GetInstallationCommand()).Returns("exit 1");
                var blm       = new BusinessLogicModuleSetup(
                    logger,
                    new SystemLanguageProvider(logger),
                    mockConfigurationProvider.Object);
                var converter = new Converter(blm, m_connectionManager, m_mockLogger.Object);
                converter.Open();

                SimulateIncoming("LogMessage", BuildLogPayload("frontend log test", DateTime.UtcNow));

                Assert.IsTrue(File.Exists(logFile));
                string content = File.ReadAllText(logFile);
                StringAssert.Contains(content, "frontend log test");

                converter.Close();
            }
            finally
            {
                Directory.Delete(tempFolder, recursive: true);
            }
        }

        /// <summary>
    /// Given DualLoggerPath SeparateBackendAndFrontendLoggers When InstallSoftwareReceived Then BackendLoggerIsUsed
        /// </summary>
        [TestMethod]
        public void Given_DualLoggerPath_SeparateBackendAndFrontendLoggers_When_InstallSoftwareReceived_Then_BackendLoggerIsUsed()
        {
            string tempFolder = Path.Combine(
                System.IO.Path.GetTempPath(), "PmsCompTest_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempFolder);
            string logFile = Path.Combine(tempFolder, "logs", "app.log");

            try
            {
                var logger = AppLoggerSetup.Create(tempFolder);
                var mockConfigurationProvider = new Mock<IConfigurationProvider>();
                mockConfigurationProvider.Setup(x => x.GetVerificationCommand()).Returns("exit 1");
                mockConfigurationProvider.Setup(x => x.GetInstallationCommand()).Returns("exit 1");
                var blm       = new BusinessLogicModuleSetup(
                    logger,
                    new SystemLanguageProvider(logger),
                    mockConfigurationProvider.Object);
                var converter = new Converter(blm, m_connectionManager, m_mockLogger.Object);
                converter.Open();

                SimulateIncoming("InstallSoftware", "{}");

                Assert.IsTrue(File.Exists(logFile));
                string content = File.ReadAllText(logFile);
                Assert.IsTrue(content.Length > 0, "Log file must not be empty.");

                converter.Close();
            }
            finally
            {
                Directory.Delete(tempFolder, recursive: true);
            }
        }

        // ─────────────────────────────────────────────────────────────────────────
        // Error / edge cases
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>
    /// Given ErrorHandlingPath MalformedRawJson When IncomingMessageReceived Then NoReplyAndNoExceptionThrown
        /// </summary>
        [TestMethod]
        public void Given_ErrorHandlingPath_MalformedRawJson_When_IncomingMessageReceived_Then_NoReplyAndNoExceptionThrown()
        {
            bool threw = false;
            try { SimulateRaw("THIS IS NOT JSON"); } catch { threw = true; }

            Assert.IsFalse(threw);
            Assert.AreEqual(0, m_sentMessages.Count);
        }

        /// <summary>
    /// Given ErrorHandlingPath MalformedRawJson When IncomingMessageReceived Then LogErrorIsCalled
        /// </summary>
        [TestMethod]
        public void Given_ErrorHandlingPath_MalformedRawJson_When_IncomingMessageReceived_Then_LogErrorIsCalled()
        {
            SimulateRaw("NOT JSON");

            m_mockLogger.Verify(x => x.LogError(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        /// <summary>
    /// Given ErrorHandlingPath UnknownAction When IncomingMessageReceived Then NoReplySent
        /// </summary>
        [TestMethod]
        public void Given_ErrorHandlingPath_UnknownAction_When_IncomingMessageReceived_Then_NoReplySent()
        {
            SimulateIncoming("SomeUnknownAction", "{}");

            Assert.AreEqual(0, m_sentMessages.Count);
        }

        /// <summary>
    /// Given ErrorHandlingPath UnknownAction When IncomingMessageReceived Then LogWarnIsCalledAtLeastOnce
        /// </summary>
        [TestMethod]
        public void Given_ErrorHandlingPath_UnknownAction_When_IncomingMessageReceived_Then_LogWarnIsCalledAtLeastOnce()
        {
            SimulateIncoming("SomeUnknownAction", "{}");

            m_mockLogger.Verify(x => x.LogWarn(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.AtLeastOnce());
        }

        /// <summary>
    /// Given ErrorHandlingPath EmptyActionName When IncomingMessageReceived Then NoReplySent
        /// </summary>
        [TestMethod]
        public void Given_ErrorHandlingPath_EmptyActionName_When_IncomingMessageReceived_Then_NoReplySent()
        {
            SimulateIncoming(string.Empty, "{}");

            Assert.AreEqual(0, m_sentMessages.Count);
        }

        /// <summary>
    /// Given ErrorHandlingPath MalformedLogMessagePayload When IncomingMessageReceived Then NoReplySent
        /// </summary>
        [TestMethod]
        public void Given_ErrorHandlingPath_MalformedLogMessagePayload_When_IncomingMessageReceived_Then_NoReplySent()
        {
            // JSON is valid at the wire level but LogMessage payload is invalid JSON
            SimulateIncoming("LogMessage", "\"not_an_object\"");

            Assert.AreEqual(0, m_sentMessages.Count);
        }

        /// <summary>
    /// Given ErrorHandlingPath JsonMissingCallContext When IncomingMessageReceived Then NoReplyAndNoException
        /// </summary>
        [TestMethod]
        public void Given_ErrorHandlingPath_JsonMissingCallContext_When_IncomingMessageReceived_Then_NoReplyAndNoException()
        {
            bool threw = false;
            try { SimulateRaw("{\"Payload\":{}}"); } catch { threw = true; }

            Assert.IsFalse(threw);
            Assert.AreEqual(0, m_sentMessages.Count);
        }

        // ─────────────────────────────────────────────────────────────────────────
        // Lifecycle: Close/Reopen
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>
    /// Given LifecyclePath ConverterClosed When MessageReceived Then NoReplySent
        /// </summary>
        [TestMethod]
        public void Given_LifecyclePath_ConverterClosed_When_MessageReceived_Then_NoReplySent()
        {
            m_converter.Close();

            SimulateIncoming("CloseApp", "{}");

            Assert.AreEqual(0, m_sentMessages.Count);
        }

        /// <summary>
    /// Given LifecyclePath ConnectionManagerClosed When MessageReceived Then NoReplySent
        /// </summary>
        [TestMethod]
        public void Given_LifecyclePath_ConnectionManagerClosed_When_MessageReceived_Then_NoReplySent()
        {
            m_connectionManager.Close();

            SimulateIncoming("CloseApp", "{}");

            Assert.AreEqual(0, m_sentMessages.Count);
        }

        /// <summary>
    /// Given LifecyclePath ConverterClosedAndReopened When MessageReceived Then ReplyIsSent
        /// </summary>
        [TestMethod]
        public void Given_LifecyclePath_ConverterClosedAndReopened_When_MessageReceived_Then_ReplyIsSent()
        {
            m_converter.Close();
            m_converter.Open();

            SimulateIncoming("VerifyInstallationPrerequisite", "{}");

            Assert.AreEqual(1, m_sentMessages.Count);
        }

        /// <summary>
    /// Given LifecyclePath ConnectionManagerClosedAndReopened When MessageReceived Then ReplyIsSent
        /// </summary>
        [TestMethod]
        public void Given_LifecyclePath_ConnectionManagerClosedAndReopened_When_MessageReceived_Then_ReplyIsSent()
        {
            m_connectionManager.Close();
            m_connectionManager.Open();

            SimulateIncoming("VerifyInstallationPrerequisite", "{}");

            Assert.AreEqual(1, m_sentMessages.Count);
        }

        // ─────────────────────────────────────────────────────────────────────────
        // Data integrity — message text survives the full stack to the log file
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>
    /// Given DataIntegrityPath ValidLogMessage When IncomingMessageReceived Then MessageTextPreservedInLogFile
        /// </summary>
        [TestMethod]
        public void Given_DataIntegrityPath_ValidLogMessage_When_IncomingMessageReceived_Then_MessageTextPreservedInLogFile()
        {
            string tempFolder = System.IO.Path.Combine(
                System.IO.Path.GetTempPath(), "PmsLogTest_" + Guid.NewGuid().ToString("N"));
            System.IO.Directory.CreateDirectory(tempFolder);
            string expectedLogFile = System.IO.Path.Combine(tempFolder, "logs", "app.log");

            try
            {
                var logger          = AppLoggerSetup.Create(tempFolder);
                var mockConfigurationProvider = new Mock<IConfigurationProvider>();
                mockConfigurationProvider.Setup(x => x.GetVerificationCommand()).Returns("exit 1");
                mockConfigurationProvider.Setup(x => x.GetInstallationCommand()).Returns("exit 1");
                var blm             = new BusinessLogicModuleSetup(
                    logger,
                    new SystemLanguageProvider(logger),
                    mockConfigurationProvider.Object);
                var converter       = new Converter(blm, m_connectionManager, m_mockLogger.Object);
                converter.Open();

                const string expectedMessage = "Data integrity check 12345";
                SimulateIncoming("LogMessage", BuildLogPayload(expectedMessage, DateTime.UtcNow));

                Assert.IsTrue(System.IO.File.Exists(expectedLogFile));
                StringAssert.Contains(System.IO.File.ReadAllText(expectedLogFile), expectedMessage);

                converter.Close();
            }
            finally
            {
                System.IO.Directory.Delete(tempFolder, recursive: true);
            }
        }

        /// <summary>
    /// Given DataIntegrityPath ValidLogMessage When IncomingMessageReceived Then TimestampPreservedInLogFile
        /// </summary>
        [TestMethod]
        public void Given_DataIntegrityPath_ValidLogMessage_When_IncomingMessageReceived_Then_TimestampPreservedInLogFile()
        {
            string tempFolder = System.IO.Path.Combine(
                System.IO.Path.GetTempPath(), "PmsLogTest_" + Guid.NewGuid().ToString("N"));
            System.IO.Directory.CreateDirectory(tempFolder);

            try
            {
                var logger          = AppLoggerSetup.Create(tempFolder);
                var mockConfigurationProvider = new Mock<IConfigurationProvider>();
                mockConfigurationProvider.Setup(x => x.GetVerificationCommand()).Returns("exit 1");
                mockConfigurationProvider.Setup(x => x.GetInstallationCommand()).Returns("exit 1");
                var blm             = new BusinessLogicModuleSetup(
                    logger,
                    new SystemLanguageProvider(logger),
                    mockConfigurationProvider.Object);
                var converter       = new Converter(blm, m_connectionManager, m_mockLogger.Object);
                converter.Open();

                SimulateIncoming("LogMessage", BuildLogPayload("ts test", DateTime.UtcNow));

                string content = System.IO.File.ReadAllText(
                    System.IO.Path.Combine(tempFolder, "logs", "app.log"));
                StringAssert.Contains(content, "ts test");

                converter.Close();
            }
            finally
            {
                System.IO.Directory.Delete(tempFolder, recursive: true);
            }
        }

        // ─────────────────────────────────────────────────────────────────────────
        // Concurrency — multiple parallel messages processed without corruption
        // ─────────────────────────────────────────────────────────────────────────

        // ─────────────────────────────────────────────────────────────────────────
        // Helpers
        // ─────────────────────────────────────────────────────────────────────────

        private void SimulateIncoming(string action, string payload)
        {
            string raw = $"{{\"CallContext\":{{\"Action\":\"{action}\"}},\"Payload\":{payload}}}";
            m_mockWebView.Raise(x => x.OnMessageReceived += null, this, raw);
        }

        private void SimulateRaw(string raw)
        {
            m_mockWebView.Raise(x => x.OnMessageReceived += null, this, raw);
        }

        private static string BuildLogPayload(string message, DateTime timestamp)
            => $"{{\"Message\":\"{message}\",\"Timestamp\":\"{timestamp:O}\"}}";

        private static string ParseAction(string json)
        {
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("Action").GetString()!;
        }

        private static string? ParsePayloadString(string json, string property)
        {
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty(property).GetString();
        }

        private static bool ParsePayloadBool(string json, string property)
        {
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty(property).GetBoolean();
        }
    }
}


