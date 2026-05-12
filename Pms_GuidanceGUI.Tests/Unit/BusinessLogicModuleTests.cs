#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : Pms_GuidanceGUI.Tests
// File   : BusinessLogicModuleTests.cs
// Description: Unit tests for BusinessLogicModuleSetup, all command handlers, and
//              ActionReplyHandler event propagation. Verifies constructor guards,
//              command dispatch, logger interactions, and event notification.
//              External dependencies (ILogger) are mocked with Moq.
//              All tests follow the Given/When/Then naming convention.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – compliance with C# Coding Guidelines V2.1
//--------------------------------------------------------------------
#endregion

using System;
using BusinessLogicModule;
using BusinessLogicModule.Commands;
using BusinessLogicModule.EventArgs;
using Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Pms_GuidanceGUI.Tests.Unit
{
    /// <summary>
    /// Unit tests for <see cref="BusinessLogicModuleSetup"/> and all command handlers.
    /// Covers constructor guards, command dispatch, logger interactions, and event notification.
    /// All tests follow the Given/When/Then naming convention.
    /// Moq is used for all external dependencies.
    /// </summary>
    [TestClass]
    public class BusinessLogicModuleTests
    {
        #region Private Members

        private Mock<ILogger>                  m_mockLogger                 = null!;
        private Mock<ISystemLanguageProvider>   m_mockSystemLanguageProvider = null!;
        private Mock<IConfigurationProvider>    m_mockConfigurationProvider  = null!;
        private BusinessLogicModuleSetup m_sut                = null!;

        #endregion

        // ─────────────────────────────────────────────────────────────────────────
        // Test lifecycle
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Initializes the test context before each test method runs.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            m_mockLogger                 = new Mock<ILogger>();
            m_mockSystemLanguageProvider = new Mock<ISystemLanguageProvider>();
            m_mockConfigurationProvider  = new Mock<IConfigurationProvider>();
            m_mockConfigurationProvider.Setup(x => x.GetVerificationCommand()).Returns("exit 1");
            m_mockConfigurationProvider.Setup(x => x.GetInstallationCommand()).Returns("exit 1");
            m_sut = new BusinessLogicModuleSetup(
                m_mockLogger.Object,
                m_mockSystemLanguageProvider.Object,
                m_mockConfigurationProvider.Object);
        }

        // ─────────────────────────────────────────────────────────────────────────
        // Constructor — two-parameter overload
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>
    /// Given BusinessLogicModule NullBackendLogger When TwoParamConstructorCalled Then ThrowsArgumentNullException
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_NullBackendLogger_When_TwoParamConstructorCalled_Then_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new BusinessLogicModuleSetup(
                    null!,
                    new Mock<ISystemLanguageProvider>().Object,
                    new Mock<IConfigurationProvider>().Object));
        }

        /// <summary>
    /// Given BusinessLogicModule ValidLoggers When TwoParamConstructorCalled Then ActionReplyEventIsNotNull
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_ValidLoggers_When_TwoParamConstructorCalled_Then_ActionReplyEventIsNotNull()
        {
            Assert.IsNotNull(m_sut.ActionReplyEvent);
        }

        /// <summary>
    /// Given BusinessLogicModule ValidLoggers When TwoParamConstructorCalled Then ActionReplyEventImplementsIActionReply
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_ValidLoggers_When_TwoParamConstructorCalled_Then_ActionReplyEventImplementsIActionReply()
        {
            Assert.IsInstanceOfType(m_sut.ActionReplyEvent, typeof(IActionReply));
        }

        // ─────────────────────────────────────────────────────────────────────────
        // HandleCommand — null guard
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>
    /// Given BusinessLogicModule NullCommand When HandleCommandCalled Then ThrowsArgumentNullException
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_NullCommand_When_HandleCommandCalled_Then_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => m_sut.HandleCommand(null!));
        }

        // ─────────────────────────────────────────────────────────────────────────
        // HandleCommand — unregistered command type
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>
    /// Given BusinessLogicModule UnregisteredCommand When HandleCommandCalled Then NoExceptionIsThrown
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_UnregisteredCommand_When_HandleCommandCalled_Then_NoExceptionIsThrown()
        {
            // Should not throw; unrecognised commands are silently logged.
            m_sut.HandleCommand(new UnregisteredCommand());
        }

        /// <summary>
    /// Given BusinessLogicModule UnregisteredCommand When HandleCommandCalled Then BackendLoggerLogWarnIsCalled
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_UnregisteredCommand_When_HandleCommandCalled_Then_BackendLoggerLogWarnIsCalled()
        {
            m_sut.HandleCommand(new UnregisteredCommand());

            m_mockLogger.Verify(
                x => x.LogWarn(It.Is<string>(msg => msg.Contains(nameof(UnregisteredCommand))), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                Times.Once());
        }

        /// <summary>
    /// Given BusinessLogicModule UnregisteredCommand When HandleCommandCalled Then OnCommandHandledIsNotRaised
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_UnregisteredCommand_When_HandleCommandCalled_Then_OnCommandHandledIsNotRaised()
        {
            bool eventFired = false;
            m_sut.ActionReplyEvent.OnCommandHandled += (_, _) => eventFired = true;

            m_sut.HandleCommand(new UnregisteredCommand());

            Assert.IsFalse(eventFired);
        }

        // ─────────────────────────────────────────────────────────────────────────
        // LogActionCommandHandler — via LogCommand
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>
    /// Given BusinessLogicModule InfoPrefixedLogCommand When HandleCommandCalled Then LogInfoIsCalled
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_InfoPrefixedLogCommand_When_HandleCommandCalled_Then_LogInfoIsCalled()
        {
            m_sut.HandleCommand(new LogCommand("[INFO] [App] app started", DateTime.UtcNow));

            m_mockLogger.Verify(x => x.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        /// <summary>
    /// Given BusinessLogicModule DebugPrefixedLogCommand When HandleCommandCalled Then LogDebugIsCalled
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_DebugPrefixedLogCommand_When_HandleCommandCalled_Then_LogDebugIsCalled()
        {
            m_sut.HandleCommand(new LogCommand("[DEBUG] [App] detail", DateTime.UtcNow));

            m_mockLogger.Verify(x => x.LogDebug(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        /// <summary>
    /// Given BusinessLogicModule WarnPrefixedLogCommand When HandleCommandCalled Then LogWarnIsCalled
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_WarnPrefixedLogCommand_When_HandleCommandCalled_Then_LogWarnIsCalled()
        {
            m_sut.HandleCommand(new LogCommand("[WARN] [App] something unusual", DateTime.UtcNow));

            m_mockLogger.Verify(x => x.LogWarn(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        /// <summary>
    /// Given BusinessLogicModule ErrorPrefixedLogCommand When HandleCommandCalled Then LogErrorIsCalled
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_ErrorPrefixedLogCommand_When_HandleCommandCalled_Then_LogErrorIsCalled()
        {
            m_sut.HandleCommand(new LogCommand("[ERROR] [App] failure occurred", DateTime.UtcNow));

            m_mockLogger.Verify(x => x.LogError(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        /// <summary>
    /// Given BusinessLogicModule UnprefixedLogCommand When HandleCommandCalled Then LogInfoIsCalled
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_UnprefixedLogCommand_When_HandleCommandCalled_Then_LogInfoIsCalled()
        {
            m_sut.HandleCommand(new LogCommand("no level prefix here", DateTime.UtcNow));

            m_mockLogger.Verify(x => x.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        /// <summary>
    /// Given BusinessLogicModule LogCommand When HandleCommandCalled Then LoggedMessageContainsCommandMessage
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_LogCommand_When_HandleCommandCalled_Then_LoggedMessageContainsCommandMessage()
        {
            const string expectedMessage = "[INFO] [App] unique-log-content";

            m_sut.HandleCommand(new LogCommand(expectedMessage, DateTime.UtcNow));

            m_mockLogger.Verify(
                x => x.LogInfo(It.Is<string>(msg => msg.Contains("unique-log-content")), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                Times.Once());
        }

        /// <summary>
    /// Given BusinessLogicModule EmptyLogCommand When HandleCommandCalled Then LogWarnIsCalled
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_EmptyLogCommand_When_HandleCommandCalled_Then_LogWarnIsCalled()
        {
            m_sut.HandleCommand(new LogCommand(string.Empty, DateTime.UtcNow));

            m_mockLogger.Verify(x => x.LogWarn(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        // ─────────────────────────────────────────────────────────────────────────
        // VerifyInstallationPrerequisitesCommandHandler
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>
    /// Given BusinessLogicModule ValidVerifyPrerequisitesCommand When HandleCommandCalled Then BackendLoggerLogInfoIsCalled
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_ValidVerifyPrerequisitesCommand_When_HandleCommandCalled_Then_BackendLoggerLogInfoIsCalled()
        {
            m_sut.HandleCommand(new VerifyInstallationPrerequisitesCommand());

            m_mockLogger.Verify(x => x.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.AtLeastOnce());
        }

        /// <summary>
    /// Given BusinessLogicModule ValidVerifyPrerequisitesCommand When HandleCommandCalled Then OnCommandHandledIsRaised
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_ValidVerifyPrerequisitesCommand_When_HandleCommandCalled_Then_OnCommandHandledIsRaised()
        {
            bool eventFired = false;
            m_sut.ActionReplyEvent.OnCommandHandled += (_, _) => eventFired = true;

            m_sut.HandleCommand(new VerifyInstallationPrerequisitesCommand());

            Assert.IsTrue(eventFired);
        }

        /// <summary>
    /// Given BusinessLogicModule ValidVerifyPrerequisitesCommand When HandleCommandCalled Then EventArgsIsVerifyPrerequisitesStatusEventArgs
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_ValidVerifyPrerequisitesCommand_When_HandleCommandCalled_Then_EventArgsIsVerifyPrerequisitesStatusEventArgs()
        {
            System.EventArgs? capturedArgs = null;
            m_sut.ActionReplyEvent.OnCommandHandled += (_, e) => capturedArgs = e;

            m_sut.HandleCommand(new VerifyInstallationPrerequisitesCommand());

            Assert.IsInstanceOfType(capturedArgs, typeof(VerifyInstallationPrerequisitesStatusEventArgs));
        }

        /// <summary>
    /// Given BusinessLogicModule ValidVerifyPrerequisitesCommand When HandleCommandCalled Then PrerequisitesMetIsTrue
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_ValidVerifyPrerequisitesCommand_When_HandleCommandCalled_Then_PrerequisitesMetIsTrue()
        {
            VerifyInstallationPrerequisitesStatusEventArgs? capturedArgs = null;
            m_sut.ActionReplyEvent.OnCommandHandled += (_, e) =>
                capturedArgs = e as VerifyInstallationPrerequisitesStatusEventArgs;

            m_sut.HandleCommand(new VerifyInstallationPrerequisitesCommand());

            Assert.IsNotNull(capturedArgs);
            Assert.IsTrue(capturedArgs.PrerequisitesMet);
        }

        // ─────────────────────────────────────────────────────────────────────────
        // InstallSoftwareCommandHandler
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>
    /// Given BusinessLogicModule ValidInstallSoftwareCommand When HandleCommandCalled Then BackendLoggerLogInfoIsCalled
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_ValidInstallSoftwareCommand_When_HandleCommandCalled_Then_BackendLoggerLogInfoIsCalled()
        {
            m_sut.HandleCommand(new InstallSoftwareCommand());

            m_mockLogger.Verify(x => x.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.AtLeastOnce());
        }

        /// <summary>
    /// Given BusinessLogicModule ValidInstallSoftwareCommand When HandleCommandCalled Then OnCommandHandledIsRaised
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_ValidInstallSoftwareCommand_When_HandleCommandCalled_Then_OnCommandHandledIsRaised()
        {
            bool eventFired = false;
            m_sut.ActionReplyEvent.OnCommandHandled += (_, _) => eventFired = true;

            m_sut.HandleCommand(new InstallSoftwareCommand());

            Assert.IsTrue(eventFired);
        }

        /// <summary>
    /// Given BusinessLogicModule ValidInstallSoftwareCommand When HandleCommandCalled Then EventArgsIsInstallSoftwareStatusEventArgs
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_ValidInstallSoftwareCommand_When_HandleCommandCalled_Then_EventArgsIsInstallSoftwareStatusEventArgs()
        {
            System.EventArgs? capturedArgs = null;
            m_sut.ActionReplyEvent.OnCommandHandled += (_, e) => capturedArgs = e;

            m_sut.HandleCommand(new InstallSoftwareCommand());

            Assert.IsInstanceOfType(capturedArgs, typeof(InstallSoftwareStatusEventArgs));
        }

        /// <summary>
    /// Given BusinessLogicModule ValidInstallSoftwareCommand When HandleCommandCalled Then IsInstalledIsTrue
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_ValidInstallSoftwareCommand_When_HandleCommandCalled_Then_IsInstalledIsTrue()
        {
            InstallSoftwareStatusEventArgs? capturedArgs = null;
            m_sut.ActionReplyEvent.OnCommandHandled += (_, e) =>
                capturedArgs = e as InstallSoftwareStatusEventArgs;

            m_sut.HandleCommand(new InstallSoftwareCommand());

            Assert.IsNotNull(capturedArgs);
            Assert.IsTrue(capturedArgs.IsInstalled);
        }

        // ─────────────────────────────────────────────────────────────────────────
        // CloseAppCommandHandler
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>
    /// Given BusinessLogicModule ValidCloseAppCommand When HandleCommandCalled Then BackendLoggerLogInfoIsCalled
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_ValidCloseAppCommand_When_HandleCommandCalled_Then_BackendLoggerLogInfoIsCalled()
        {
            m_sut.HandleCommand(new CloseAppCommand());

            m_mockLogger.Verify(x => x.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        /// <summary>
    /// Given BusinessLogicModule ValidCloseAppCommand When HandleCommandCalled Then OnCommandHandledIsRaised
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_ValidCloseAppCommand_When_HandleCommandCalled_Then_OnCommandHandledIsRaised()
        {
            bool eventFired = false;
            m_sut.ActionReplyEvent.OnCommandHandled += (_, _) => eventFired = true;

            m_sut.HandleCommand(new CloseAppCommand());

            Assert.IsTrue(eventFired);
        }

        /// <summary>
    /// Given BusinessLogicModule ValidCloseAppCommand When HandleCommandCalled Then EventArgsIsCloseAppStatusEventArgs
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_ValidCloseAppCommand_When_HandleCommandCalled_Then_EventArgsIsCloseAppStatusEventArgs()
        {
            System.EventArgs? capturedArgs = null;
            m_sut.ActionReplyEvent.OnCommandHandled += (_, e) => capturedArgs = e;

            m_sut.HandleCommand(new CloseAppCommand());

            Assert.IsInstanceOfType(capturedArgs, typeof(CloseAppStatusEventArgs));
        }

        /// <summary>
    /// Given BusinessLogicModule ValidCloseAppCommand When HandleCommandCalled Then IsClosingIsTrue
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_ValidCloseAppCommand_When_HandleCommandCalled_Then_IsClosingIsTrue()
        {
            CloseAppStatusEventArgs? capturedArgs = null;
            m_sut.ActionReplyEvent.OnCommandHandled += (_, e) =>
                capturedArgs = e as CloseAppStatusEventArgs;

            m_sut.HandleCommand(new CloseAppCommand());

            Assert.IsNotNull(capturedArgs);
            Assert.IsTrue(capturedArgs.IsClosing);
        }

        // ─────────────────────────────────────────────────────────────────────────
        // ActionReplyHandler — event propagation
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>
    /// Given BusinessLogicModule NoSubscribers When CommandIsHandled Then NoExceptionIsThrown
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_NoSubscribers_When_CommandIsHandled_Then_NoExceptionIsThrown()
        {
            // ActionReplyEvent has no subscribers — must not throw.
            m_sut.HandleCommand(new LogCommand("no subscriber", DateTime.UtcNow));
        }

        /// <summary>
    /// Given BusinessLogicModule Subscriber When CommandIsHandled Then SenderIsNotNull
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_Subscriber_When_CommandIsHandled_Then_SenderIsNotNull()
        {
            object? capturedSender = null;
            m_sut.ActionReplyEvent.OnCommandHandled += (sender, _) => capturedSender = sender;

            m_sut.HandleCommand(new CloseAppCommand());

            Assert.IsNotNull(capturedSender);
        }

        /// <summary>
    /// Given BusinessLogicModule MultipleCommandsHandled When EachCommandIsDispatched Then CorrectEventArgsTypeRaisedPerCommand
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_MultipleCommandsHandled_When_EachCommandIsDispatched_Then_CorrectEventArgsTypeRaisedPerCommand()
        {
            Type? closeType = null;

            m_sut.ActionReplyEvent.OnCommandHandled += (_, e) =>
            {
                if (e is CloseAppStatusEventArgs) closeType = e.GetType();
            };

            m_sut.HandleCommand(new LogCommand("msg", DateTime.UtcNow));
            m_sut.HandleCommand(new CloseAppCommand());

            Assert.AreEqual(typeof(CloseAppStatusEventArgs), closeType);
        }

        // ─────────────────────────────────────────────────────────────────────────
        // InstallSoftwareCommandHandler — exception path via IInstallationService
        // (tested directly using internal types visible via InternalsVisibleTo)
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>
    /// Given BusinessLogicModule InstallationServiceThrows When HandleCommandCalled Then IsInstalledIsFalse
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_InstallationServiceThrows_When_HandleCommandCalled_Then_IsInstalledIsFalse()
        {
            var mockConfigurationProvider = new Mock<IConfigurationProvider>();
            mockConfigurationProvider
                .Setup(x => x.GetInstallationCommand())
                .Throws(new InvalidOperationException("install failure"));

            var replyHandler = new ActionReplyHandler();
            var handler      = new InstallSoftwareCommandHandler(
                (IActionReplyPrivate)replyHandler,
                m_mockLogger.Object,
                mockConfigurationProvider.Object);

            InstallSoftwareStatusEventArgs? args = null;
            replyHandler.OnCommandHandled += (_, e) => args = e as InstallSoftwareStatusEventArgs;

            handler.HandleCommand(new InstallSoftwareCommand());

            Assert.IsNotNull(args);
            Assert.IsFalse(args.IsInstalled);
        }

        /// <summary>
    /// Given BusinessLogicModule InstallationServiceThrows When HandleCommandCalled Then LogErrorIsCalledWithException
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_InstallationServiceThrows_When_HandleCommandCalled_Then_LogErrorIsCalledWithException()
        {
            var mockConfigurationProvider = new Mock<IConfigurationProvider>();
            mockConfigurationProvider
                .Setup(x => x.GetInstallationCommand())
                .Throws(new InvalidOperationException("install failure"));

            var replyHandler = new ActionReplyHandler();
            var handler      = new InstallSoftwareCommandHandler(
                (IActionReplyPrivate)replyHandler,
                m_mockLogger.Object,
                mockConfigurationProvider.Object);

            handler.HandleCommand(new InstallSoftwareCommand());

            m_mockLogger.Verify(
                x => x.LogError(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                Times.Once());
        }

        /// <summary>
    /// Given BusinessLogicModule InstallationServiceThrows When HandleCommandCalled Then OnCommandHandledIsStillRaised
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_InstallationServiceThrows_When_HandleCommandCalled_Then_OnCommandHandledIsStillRaised()
        {
            var mockConfigurationProvider = new Mock<IConfigurationProvider>();
            mockConfigurationProvider
                .Setup(x => x.GetInstallationCommand())
                .Throws(new InvalidOperationException("install failure"));

            var replyHandler = new ActionReplyHandler();
            var handler      = new InstallSoftwareCommandHandler(
                (IActionReplyPrivate)replyHandler,
                m_mockLogger.Object,
                mockConfigurationProvider.Object);

            bool fired = false;
            replyHandler.OnCommandHandled += (_, _) => fired = true;

            handler.HandleCommand(new InstallSoftwareCommand());

            Assert.IsTrue(fired);
        }

        // ─────────────────────────────────────────────────────────────────────────
        // VerifyInstallationPrerequisitesCommandHandler — exception path via IPrerequisiteChecker
        // (tested directly using internal types visible via InternalsVisibleTo)
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>
    /// Given BusinessLogicModule PrerequisiteCheckerThrows When HandleCommandCalled Then PrerequisitesMetIsFalse
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_PrerequisiteCheckerThrows_When_HandleCommandCalled_Then_PrerequisitesMetIsFalse()
        {
            var mockConfigurationProvider = new Mock<IConfigurationProvider>();
            mockConfigurationProvider
                .Setup(x => x.GetVerificationCommand())
                .Throws(new InvalidOperationException("check failure"));

            var replyHandler = new ActionReplyHandler();
            var handler      = new VerifyInstallationPrerequisitesCommandHandler(
                (IActionReplyPrivate)replyHandler,
                m_mockLogger.Object,
                mockConfigurationProvider.Object);

            VerifyInstallationPrerequisitesStatusEventArgs? args = null;
            replyHandler.OnCommandHandled += (_, e) =>
                args = e as VerifyInstallationPrerequisitesStatusEventArgs;

            handler.HandleCommand(new VerifyInstallationPrerequisitesCommand());

            Assert.IsNotNull(args);
            Assert.IsFalse(args.PrerequisitesMet);
        }

        /// <summary>
    /// Given BusinessLogicModule PrerequisiteCheckerThrows When HandleCommandCalled Then LogErrorIsCalledWithException
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_PrerequisiteCheckerThrows_When_HandleCommandCalled_Then_LogErrorIsCalledWithException()
        {
            var mockConfigurationProvider = new Mock<IConfigurationProvider>();
            mockConfigurationProvider
                .Setup(x => x.GetVerificationCommand())
                .Throws(new InvalidOperationException("check failure"));

            var replyHandler = new ActionReplyHandler();
            var handler      = new VerifyInstallationPrerequisitesCommandHandler(
                (IActionReplyPrivate)replyHandler,
                m_mockLogger.Object,
                mockConfigurationProvider.Object);

            handler.HandleCommand(new VerifyInstallationPrerequisitesCommand());

            m_mockLogger.Verify(
                x => x.LogError(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                Times.Once());
        }

        /// <summary>
    /// Given BusinessLogicModule PrerequisiteCheckerThrows When HandleCommandCalled Then OnCommandHandledIsStillRaised
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_PrerequisiteCheckerThrows_When_HandleCommandCalled_Then_OnCommandHandledIsStillRaised()
        {
            var mockConfigurationProvider = new Mock<IConfigurationProvider>();
            mockConfigurationProvider
                .Setup(x => x.GetVerificationCommand())
                .Throws(new InvalidOperationException("check failure"));

            var replyHandler = new ActionReplyHandler();
            var handler      = new VerifyInstallationPrerequisitesCommandHandler(
                (IActionReplyPrivate)replyHandler,
                m_mockLogger.Object,
                mockConfigurationProvider.Object);

            bool fired = false;
            replyHandler.OnCommandHandled += (_, _) => fired = true;

            handler.HandleCommand(new VerifyInstallationPrerequisitesCommand());

            Assert.IsTrue(fired);
        }

        // ─────────────────────────────────────────────────────────────────────────
        // UIAppStartedCommandHandler — via UIAppStartedCommand
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>
    /// Given BusinessLogicModule ValidUIAppStartedCommand When HandleCommandCalled Then OnCommandHandledIsRaised
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_ValidUIAppStartedCommand_When_HandleCommandCalled_Then_OnCommandHandledIsRaised()
        {
            m_mockSystemLanguageProvider.Setup(p => p.FetchSystemLanguage()).Returns("en-US");
            bool eventFired = false;
            m_sut.ActionReplyEvent.OnCommandHandled += (_, _) => eventFired = true;

            m_sut.HandleCommand(new UIAppStartedCommand());

            Assert.IsTrue(eventFired);
        }

        /// <summary>
    /// Given BusinessLogicModule ValidUIAppStartedCommand When HandleCommandCalled Then EventArgsIsShowSystemLanguageEventArgs
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_ValidUIAppStartedCommand_When_HandleCommandCalled_Then_EventArgsIsShowSystemLanguageEventArgs()
        {
            m_mockSystemLanguageProvider.Setup(p => p.FetchSystemLanguage()).Returns("en-US");
            System.EventArgs? capturedArgs = null;
            m_sut.ActionReplyEvent.OnCommandHandled += (_, e) => capturedArgs = e;

            m_sut.HandleCommand(new UIAppStartedCommand());

            Assert.IsInstanceOfType(capturedArgs, typeof(ShowSystemLanguageEventArgs));
        }

        /// <summary>
    /// Given BusinessLogicModule ValidUIAppStartedCommand When HandleCommandCalled Then EventArgsLanguageMatchesProviderResult
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_ValidUIAppStartedCommand_When_HandleCommandCalled_Then_EventArgsLanguageMatchesProviderResult()
        {
            const string expectedLanguage = "de-DE";
            m_mockSystemLanguageProvider.Setup(p => p.FetchSystemLanguage()).Returns(expectedLanguage);
            ShowSystemLanguageEventArgs? capturedArgs = null;
            m_sut.ActionReplyEvent.OnCommandHandled += (_, e) =>
                capturedArgs = e as ShowSystemLanguageEventArgs;

            m_sut.HandleCommand(new UIAppStartedCommand());

            Assert.IsNotNull(capturedArgs);
            Assert.AreEqual(expectedLanguage, capturedArgs.Language);
        }

        /// <summary>
    /// Given BusinessLogicModule ValidUIAppStartedCommand When HandleCommandCalled Then BackendLoggerLogInfoIsCalled
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_ValidUIAppStartedCommand_When_HandleCommandCalled_Then_BackendLoggerLogInfoIsCalled()
        {
            m_mockSystemLanguageProvider.Setup(p => p.FetchSystemLanguage()).Returns("en-US");

            m_sut.HandleCommand(new UIAppStartedCommand());

            m_mockLogger.Verify(
                x => x.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                Times.AtLeastOnce());
        }


        // ─────────────────────────────────────────────────────────────────────────
        // ShowSystemLanguageEventArgs
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>
    /// Given BusinessLogicModule ValidLanguage When ShowSystemLanguageEventArgsConstructed Then LanguagePropertyIsSet
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_ValidLanguage_When_ShowSystemLanguageEventArgsConstructed_Then_LanguagePropertyIsSet()
        {
            var args = new ShowSystemLanguageEventArgs("fr-FR");

            Assert.AreEqual("fr-FR", args.Language);
        }

        /// <summary>
    /// Given BusinessLogicModule NullLanguage When ShowSystemLanguageEventArgsConstructed Then ThrowsArgumentNullException
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_NullLanguage_When_ShowSystemLanguageEventArgsConstructed_Then_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new ShowSystemLanguageEventArgs(null!));
        }

        // ─────────────────────────────────────────────────────────────────────────
        // UIAppStartedCommandHandler — constructor guards (via InternalsVisibleTo)
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>
    /// Given BusinessLogicModule NullSystemLanguageProvider When UIAppStartedCommandHandlerConstructed Then ThrowsArgumentNullException
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_NullSystemLanguageProvider_When_UIAppStartedCommandHandlerConstructed_Then_ThrowsArgumentNullException()
        {
            var replyHandler = new ActionReplyHandler();
            Assert.ThrowsException<ArgumentNullException>(
                () => new UIAppStartedCommandHandler(
                    (IActionReplyPrivate)replyHandler,
                    m_mockLogger.Object,
                    null!));
        }

        // ─────────────────────────────────────────────────────────────────────────
        // AbstractCommandHandler — null guard in HandleCommand (direct handler call)
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>
    /// Given BusinessLogicModule NullCommand When HandleCommandCalledOnConcreteHandler Then ThrowsArgumentNullException
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_NullCommand_When_HandleCommandCalledOnConcreteHandler_Then_ThrowsArgumentNullException()
        {
            var replyHandler = new ActionReplyHandler();
            var handler      = new CloseAppCommandHandler(
                (IActionReplyPrivate)replyHandler,
                m_mockLogger.Object,
                () => { });

            Assert.ThrowsException<ArgumentNullException>(() => handler.HandleCommand(null!));
        }

        // ─────────────────────────────────────────────────────────────────────────
        // CommandType properties on each concrete handler
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>
    /// Given BusinessLogicModule CloseAppCommandHandler When CommandTypeAccessed Then ReturnsCloseAppCommandType
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_CloseAppCommandHandler_When_CommandTypeAccessed_Then_ReturnsCloseAppCommandType()
        {
            var replyHandler = new ActionReplyHandler();
            var handler = new CloseAppCommandHandler(
                (IActionReplyPrivate)replyHandler, m_mockLogger.Object, () => { });

            Assert.AreEqual(typeof(CloseAppCommand), handler.CommandType);
        }

        /// <summary>
    /// Given BusinessLogicModule InstallSoftwareCommandHandler When CommandTypeAccessed Then ReturnsInstallSoftwareCommandType
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_InstallSoftwareCommandHandler_When_CommandTypeAccessed_Then_ReturnsInstallSoftwareCommandType()
        {
            var replyHandler = new ActionReplyHandler();
            var handler = new InstallSoftwareCommandHandler(
                (IActionReplyPrivate)replyHandler,
                m_mockLogger.Object,
                new Mock<IConfigurationProvider>().Object);

            Assert.AreEqual(typeof(InstallSoftwareCommand), handler.CommandType);
        }

        /// <summary>
    /// Given BusinessLogicModule LogActionCommandHandler When CommandTypeAccessed Then ReturnsLogCommandType
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_LogActionCommandHandler_When_CommandTypeAccessed_Then_ReturnsLogCommandType()
        {
            var replyHandler = new ActionReplyHandler();
            var handler = new LogActionCommandHandler(
                (IActionReplyPrivate)replyHandler,
                m_mockLogger.Object);

            Assert.AreEqual(typeof(LogCommand), handler.CommandType);
        }

        /// <summary>
    /// Given BusinessLogicModule VerifyInstallationPrerequisitesCommandHandler When CommandTypeAccessed Then ReturnsVerifyCommandType
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_VerifyInstallationPrerequisitesCommandHandler_When_CommandTypeAccessed_Then_ReturnsVerifyCommandType()
        {
            var replyHandler = new ActionReplyHandler();
            var handler = new VerifyInstallationPrerequisitesCommandHandler(
                (IActionReplyPrivate)replyHandler,
                m_mockLogger.Object,
                new Mock<IConfigurationProvider>().Object);

            Assert.AreEqual(typeof(VerifyInstallationPrerequisitesCommand), handler.CommandType);
        }

        /// <summary>
    /// Given BusinessLogicModule UIAppStartedCommandHandler When CommandTypeAccessed Then ReturnsUIAppStartedCommandType
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_UIAppStartedCommandHandler_When_CommandTypeAccessed_Then_ReturnsUIAppStartedCommandType()
        {
            var replyHandler = new ActionReplyHandler();
            var handler = new UIAppStartedCommandHandler(
                (IActionReplyPrivate)replyHandler,
                m_mockLogger.Object,
                m_mockSystemLanguageProvider.Object);

            Assert.AreEqual(typeof(UIAppStartedCommand), handler.CommandType);
        }

        // ─────────────────────────────────────────────────────────────────────────
        // Private helpers
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>Command with no handler registered in <see cref="BusinessLogicModuleSetup"/>.</summary>
        private sealed class UnregisteredCommand : ICommand { }
    }
}
