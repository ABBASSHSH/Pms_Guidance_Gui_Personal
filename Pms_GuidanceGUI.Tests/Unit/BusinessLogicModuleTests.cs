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
    /// Given BusinessLogicModule When NullBackendLoggerTwoParamConstructorCalled Then ThrowsArgumentNullException
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_When_NullBackendLoggerTwoParamConstructorCalled_Then_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new BusinessLogicModuleSetup(
                    null!,
                    new Mock<ISystemLanguageProvider>().Object,
                    new Mock<IConfigurationProvider>().Object));
        }

        /// <summary>
    /// Given BusinessLogicModule When NullSystemLanguageProviderConstructorCalled Then ThrowsArgumentNullException
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_When_NullSystemLanguageProviderConstructorCalled_Then_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new BusinessLogicModuleSetup(
                    m_mockLogger.Object,
                    null!,
                    m_mockConfigurationProvider.Object));
        }

        /// <summary>
    /// Given BusinessLogicModule When NullConfigurationProviderConstructorCalled Then ThrowsArgumentNullException
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_When_NullConfigurationProviderConstructorCalled_Then_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new BusinessLogicModuleSetup(
                    m_mockLogger.Object,
                    m_mockSystemLanguageProvider.Object,
                    null!));
        }

        /// <summary>
    /// Given BusinessLogicModule When ValidLoggersTwoParamConstructorCalled Then ActionReplyEventIsNotNull
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_When_ValidLoggersTwoParamConstructorCalled_Then_ActionReplyEventIsNotNull()
        {
            Assert.IsNotNull(m_sut.ActionReplyEvent);
        }

        /// <summary>
    /// Given BusinessLogicModule When ValidLoggersTwoParamConstructorCalled Then ActionReplyEventImplementsIActionReply
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_When_ValidLoggersTwoParamConstructorCalled_Then_ActionReplyEventImplementsIActionReply()
        {
            Assert.IsInstanceOfType(m_sut.ActionReplyEvent, typeof(IActionReply));
        }

        // ─────────────────────────────────────────────────────────────────────────
        // HandleCommand — null guard
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>
    /// Given BusinessLogicModule When NullCommandHandleCommandCalled Then ThrowsArgumentNullException
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_When_NullCommandHandleCommandCalled_Then_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => m_sut.HandleCommand(null!));
        }

        /// <summary>
    /// Given BusinessLogicModule When OpenCalledTwice Then NoExceptionThrown
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_When_OpenCalledTwice_Then_NoExceptionThrown()
        {
            m_sut.Open();
            m_sut.Open();
        }

        /// <summary>
    /// Given BusinessLogicModule When CloseCalledBeforeOpenThenAfterOpenThenAgain Then NoExceptionThrown
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_When_CloseCalledBeforeOpenThenAfterOpenThenAgain_Then_NoExceptionThrown()
        {
            m_sut.Close();
            m_sut.Open();
            m_sut.Close();
            m_sut.Close();
        }

        /// <summary>
    /// Given BusinessLogicModule When ClosedHandleCommandCalled Then CommandIsIgnoredAndLogWarnIsCalled
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_When_ClosedHandleCommandCalled_Then_CommandIsIgnoredAndLogWarnIsCalled()
        {
            bool eventFired = false;
            m_sut.ActionReplyEvent.OnCommandHandled += (_, _) => eventFired = true;
            m_sut.Close();

            m_sut.HandleCommand(new CloseAppCommand());

            Assert.IsFalse(eventFired);
            m_mockLogger.Verify(
                x => x.LogWarn(It.Is<string>(msg => msg.Contains("closed")), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                Times.Once());
        }

        // ─────────────────────────────────────────────────────────────────────────
        // HandleCommand — unregistered command type
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>
    /// Given BusinessLogicModule When UnregisteredCommandHandleCommandCalled Then NoExceptionIsThrown
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_When_UnregisteredCommandHandleCommandCalled_Then_NoExceptionIsThrown()
        {
            // Should not throw; unrecognised commands are silently logged.
            m_sut.HandleCommand(new UnregisteredCommand());
        }

        /// <summary>
    /// Given BusinessLogicModule When UnregisteredCommandHandleCommandCalled Then BackendLoggerLogWarnIsCalled
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_When_UnregisteredCommandHandleCommandCalled_Then_BackendLoggerLogWarnIsCalled()
        {
            m_sut.HandleCommand(new UnregisteredCommand());

            m_mockLogger.Verify(
                x => x.LogWarn(It.Is<string>(msg => msg.Contains(nameof(UnregisteredCommand))), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                Times.Once());
        }

        /// <summary>
    /// Given BusinessLogicModule When UnregisteredCommandHandleCommandCalled Then OnCommandHandledIsNotRaised
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_When_UnregisteredCommandHandleCommandCalled_Then_OnCommandHandledIsNotRaised()
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
    /// Given BusinessLogicModule When InfoPrefixedLogCommandHandleCommandCalled Then LogInfoIsCalled
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_When_InfoPrefixedLogCommandHandleCommandCalled_Then_LogInfoIsCalled()
        {
            m_sut.HandleCommand(new LogCommand("[INFO] [App] app started", DateTime.UtcNow));

            m_mockLogger.Verify(x => x.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        /// <summary>
    /// Given BusinessLogicModule When DebugPrefixedLogCommandHandleCommandCalled Then LogDebugIsCalled
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_When_DebugPrefixedLogCommandHandleCommandCalled_Then_LogDebugIsCalled()
        {
            m_sut.HandleCommand(new LogCommand("[DEBUG] [App] detail", DateTime.UtcNow));

            m_mockLogger.Verify(x => x.LogDebug(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        /// <summary>
    /// Given BusinessLogicModule When WarnPrefixedLogCommandHandleCommandCalled Then LogWarnIsCalled
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_When_WarnPrefixedLogCommandHandleCommandCalled_Then_LogWarnIsCalled()
        {
            m_sut.HandleCommand(new LogCommand("[WARN] [App] something unusual", DateTime.UtcNow));

            m_mockLogger.Verify(x => x.LogWarn(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        /// <summary>
    /// Given BusinessLogicModule When ErrorPrefixedLogCommandHandleCommandCalled Then LogErrorIsCalled
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_When_ErrorPrefixedLogCommandHandleCommandCalled_Then_LogErrorIsCalled()
        {
            m_sut.HandleCommand(new LogCommand("[ERROR] [App] failure occurred", DateTime.UtcNow));

            m_mockLogger.Verify(x => x.LogError(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        /// <summary>
    /// Given BusinessLogicModule When UnprefixedLogCommandHandleCommandCalled Then LogInfoIsCalled
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_When_UnprefixedLogCommandHandleCommandCalled_Then_LogInfoIsCalled()
        {
            m_sut.HandleCommand(new LogCommand("no level prefix here", DateTime.UtcNow));

            m_mockLogger.Verify(x => x.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        /// <summary>
    /// Given BusinessLogicModule When LogCommandHandleCommandCalled Then LoggedMessageContainsCommandMessage
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_When_LogCommandHandleCommandCalled_Then_LoggedMessageContainsCommandMessage()
        {
            const string expectedMessage = "[INFO] [App] unique-log-content";

            m_sut.HandleCommand(new LogCommand(expectedMessage, DateTime.UtcNow));

            m_mockLogger.Verify(
                x => x.LogInfo(It.Is<string>(msg => msg.Contains("unique-log-content")), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                Times.Once());
        }

        /// <summary>
    /// Given BusinessLogicModule When EmptyLogCommandHandleCommandCalled Then LogWarnIsCalled
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_When_EmptyLogCommandHandleCommandCalled_Then_LogWarnIsCalled()
        {
            m_sut.HandleCommand(new LogCommand(string.Empty, DateTime.UtcNow));

            m_mockLogger.Verify(x => x.LogWarn(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        // ─────────────────────────────────────────────────────────────────────────
        // VerifyInstallationPrerequisitesCommandHandler
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>
    /// Given BusinessLogicModule When ValidVerifyPrerequisitesCommandHandleCommandCalled Then BackendLoggerLogInfoIsCalled
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_When_ValidVerifyPrerequisitesCommandHandleCommandCalled_Then_BackendLoggerLogInfoIsCalled()
        {
            m_sut.HandleCommand(new VerifyInstallationPrerequisitesCommand());

            m_mockLogger.Verify(x => x.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.AtLeastOnce());
        }

        /// <summary>
    /// Given BusinessLogicModule When ValidVerifyPrerequisitesCommandHandleCommandCalled Then OnCommandHandledIsRaised
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_When_ValidVerifyPrerequisitesCommandHandleCommandCalled_Then_OnCommandHandledIsRaised()
        {
            bool eventFired = false;
            m_sut.ActionReplyEvent.OnCommandHandled += (_, _) => eventFired = true;

            m_sut.HandleCommand(new VerifyInstallationPrerequisitesCommand());

            Assert.IsTrue(eventFired);
        }

        /// <summary>
    /// Given BusinessLogicModule When ValidVerifyPrerequisitesCommandHandleCommandCalled Then EventArgsIsVerifyPrerequisitesStatusEventArgs
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_When_ValidVerifyPrerequisitesCommandHandleCommandCalled_Then_EventArgsIsVerifyPrerequisitesStatusEventArgs()
        {
            System.EventArgs? capturedArgs = null;
            m_sut.ActionReplyEvent.OnCommandHandled += (_, e) => capturedArgs = e;

            m_sut.HandleCommand(new VerifyInstallationPrerequisitesCommand());

            Assert.IsInstanceOfType(capturedArgs, typeof(VerifyInstallationPrerequisitesStatusEventArgs));
        }

        /// <summary>
    /// Given BusinessLogicModule When ValidVerifyPrerequisitesCommandHandleCommandCalled Then PrerequisitesMetIsTrue
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_When_ValidVerifyPrerequisitesCommandHandleCommandCalled_Then_PrerequisitesMetIsTrue()
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
    /// Given BusinessLogicModule When ValidInstallSoftwareCommandHandleCommandCalled Then BackendLoggerLogInfoIsCalled
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_When_ValidInstallSoftwareCommandHandleCommandCalled_Then_BackendLoggerLogInfoIsCalled()
        {
            m_sut.HandleCommand(new InstallSoftwareCommand());

            m_mockLogger.Verify(x => x.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.AtLeastOnce());
        }

        /// <summary>
    /// Given BusinessLogicModule When ValidInstallSoftwareCommandHandleCommandCalled Then OnCommandHandledIsRaised
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_When_ValidInstallSoftwareCommandHandleCommandCalled_Then_OnCommandHandledIsRaised()
        {
            bool eventFired = false;
            m_sut.ActionReplyEvent.OnCommandHandled += (_, _) => eventFired = true;

            m_sut.HandleCommand(new InstallSoftwareCommand());

            Assert.IsTrue(eventFired);
        }

        /// <summary>
    /// Given BusinessLogicModule When ValidInstallSoftwareCommandHandleCommandCalled Then EventArgsIsInstallSoftwareStatusEventArgs
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_When_ValidInstallSoftwareCommandHandleCommandCalled_Then_EventArgsIsInstallSoftwareStatusEventArgs()
        {
            System.EventArgs? capturedArgs = null;
            m_sut.ActionReplyEvent.OnCommandHandled += (_, e) => capturedArgs = e;

            m_sut.HandleCommand(new InstallSoftwareCommand());

            Assert.IsInstanceOfType(capturedArgs, typeof(InstallSoftwareStatusEventArgs));
        }

        /// <summary>
    /// Given BusinessLogicModule When ValidInstallSoftwareCommandHandleCommandCalled Then IsInstalledIsTrue
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_When_ValidInstallSoftwareCommandHandleCommandCalled_Then_IsInstalledIsTrue()
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
    /// Given BusinessLogicModule When ValidCloseAppCommandHandleCommandCalled Then BackendLoggerLogInfoIsCalled
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_When_ValidCloseAppCommandHandleCommandCalled_Then_BackendLoggerLogInfoIsCalled()
        {
            m_sut.HandleCommand(new CloseAppCommand());

            m_mockLogger.Verify(x => x.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        /// <summary>
    /// Given BusinessLogicModule When ValidCloseAppCommandHandleCommandCalled Then OnCommandHandledIsRaised
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_When_ValidCloseAppCommandHandleCommandCalled_Then_OnCommandHandledIsRaised()
        {
            bool eventFired = false;
            m_sut.ActionReplyEvent.OnCommandHandled += (_, _) => eventFired = true;

            m_sut.HandleCommand(new CloseAppCommand());

            Assert.IsTrue(eventFired);
        }

        /// <summary>
    /// Given BusinessLogicModule When ValidCloseAppCommandHandleCommandCalled Then EventArgsIsCloseAppStatusEventArgs
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_When_ValidCloseAppCommandHandleCommandCalled_Then_EventArgsIsCloseAppStatusEventArgs()
        {
            System.EventArgs? capturedArgs = null;
            m_sut.ActionReplyEvent.OnCommandHandled += (_, e) => capturedArgs = e;

            m_sut.HandleCommand(new CloseAppCommand());

            Assert.IsInstanceOfType(capturedArgs, typeof(CloseAppStatusEventArgs));
        }

        /// <summary>
    /// Given BusinessLogicModule When ValidCloseAppCommandHandleCommandCalled Then IsClosingIsTrue
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_When_ValidCloseAppCommandHandleCommandCalled_Then_IsClosingIsTrue()
        {
            CloseAppStatusEventArgs? capturedArgs = null;
            m_sut.ActionReplyEvent.OnCommandHandled += (_, e) =>
                capturedArgs = e as CloseAppStatusEventArgs;

            m_sut.HandleCommand(new CloseAppCommand());

            Assert.IsNotNull(capturedArgs);
            Assert.IsTrue(capturedArgs.IsClosing);
        }

        /// <summary>
    /// Given BusinessLogicModule When ValidCloseAppCommandHandleCommandCalled Then CloseApplicationRequestedIsRaised
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_When_ValidCloseAppCommandHandleCommandCalled_Then_CloseApplicationRequestedIsRaised()
        {
            bool closeRequested = false;
            m_sut.CloseApplicationRequested += (_, _) => closeRequested = true;

            m_sut.HandleCommand(new CloseAppCommand());

            Assert.IsTrue(closeRequested);
        }

        // ─────────────────────────────────────────────────────────────────────────
        // ActionReplyHandler — event propagation
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>
    /// Given BusinessLogicModule When NoSubscribersCommandIsHandled Then NoExceptionIsThrown
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_When_NoSubscribersCommandIsHandled_Then_NoExceptionIsThrown()
        {
            // ActionReplyEvent has no subscribers — must not throw.
            m_sut.HandleCommand(new LogCommand("no subscriber", DateTime.UtcNow));
        }

        /// <summary>
    /// Given BusinessLogicModule When SubscriberCommandIsHandled Then SenderIsNotNull
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_When_SubscriberCommandIsHandled_Then_SenderIsNotNull()
        {
            object? capturedSender = null;
            m_sut.ActionReplyEvent.OnCommandHandled += (sender, _) => capturedSender = sender;

            m_sut.HandleCommand(new CloseAppCommand());

            Assert.IsNotNull(capturedSender);
        }

        /// <summary>
    /// Given BusinessLogicModule When MultipleCommandsHandledEachCommandIsDispatched Then CorrectEventArgsTypeRaisedPerCommand
        /// </summary>
        [TestMethod]
        public void Given_BusinessLogicModule_When_MultipleCommandsHandledEachCommandIsDispatched_Then_CorrectEventArgsTypeRaisedPerCommand()
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
        // Private helpers
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>Command with no handler registered in <see cref="BusinessLogicModuleSetup"/>.</summary>
        private sealed class UnregisteredCommand : ICommand { }
    }
}

