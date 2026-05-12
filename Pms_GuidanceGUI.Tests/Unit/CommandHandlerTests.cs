#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : Pms_GuidanceGUI.Tests
// File   : CommandHandlerTests.cs
// Description: Unit tests for concrete command handlers and related event args.
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
    [TestClass]
    public class CommandHandlerTests
    {
        #region Private Members

        private Mock<ILogger> m_mockLogger = null!;
        private Mock<ISystemLanguageProvider> m_mockSystemLanguageProvider = null!;
        private Mock<IConfigurationProvider> m_mockConfigurationProvider = null!;
        private BusinessLogicModuleSetup m_sut = null!;

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            m_mockLogger = new Mock<ILogger>();
            m_mockSystemLanguageProvider = new Mock<ISystemLanguageProvider>();
            m_mockConfigurationProvider = new Mock<IConfigurationProvider>();
            m_mockConfigurationProvider.Setup(x => x.GetVerificationCommand()).Returns("exit 1");
            m_mockConfigurationProvider.Setup(x => x.GetInstallationCommand()).Returns("exit 1");
            m_sut = new BusinessLogicModuleSetup(
                m_mockLogger.Object,
                m_mockSystemLanguageProvider.Object,
                m_mockConfigurationProvider.Object);
        }

        // InstallSoftwareCommandHandler direct tests

        [TestMethod]
        public void Given_BusinessLogicModule_When_InstallationServiceHandleCommandCalled_Then_IsInstalledIsFalse()
        {
            var mockConfigurationProvider = new Mock<IConfigurationProvider>();
            mockConfigurationProvider
                .Setup(x => x.GetInstallationCommand())
                .Throws(new InvalidOperationException("install failure"));

            var replyHandler = new ActionReplyHandler();
            var handler = new InstallSoftwareCommandHandler(
                (IActionReplyPrivate)replyHandler,
                m_mockLogger.Object,
                mockConfigurationProvider.Object);

            InstallSoftwareStatusEventArgs? args = null;
            replyHandler.OnCommandHandled += (_, e) => args = e as InstallSoftwareStatusEventArgs;

            handler.HandleCommand(new InstallSoftwareCommand());

            Assert.IsNotNull(args);
            Assert.IsFalse(args.IsInstalled);
        }

        [TestMethod]
        public void Given_BusinessLogicModule_When_InstallationCommandIsWhitespaceHandleCommandCalled_Then_IsInstalledIsFalse()
        {
            var mockConfigurationProvider = new Mock<IConfigurationProvider>();
            mockConfigurationProvider
                .Setup(x => x.GetInstallationCommand())
                .Returns("   ");

            var replyHandler = new ActionReplyHandler();
            var handler = new InstallSoftwareCommandHandler(
                (IActionReplyPrivate)replyHandler,
                m_mockLogger.Object,
                mockConfigurationProvider.Object);

            InstallSoftwareStatusEventArgs? args = null;
            replyHandler.OnCommandHandled += (_, e) => args = e as InstallSoftwareStatusEventArgs;

            handler.HandleCommand(new InstallSoftwareCommand());

            Assert.IsNotNull(args);
            Assert.IsFalse(args.IsInstalled);
            m_mockLogger.Verify(
                x => x.LogWarn(It.Is<string>(s => s.Contains("missing") || s.Contains("empty")), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                Times.Once());
        }

        [TestMethod]
        public void Given_BusinessLogicModule_When_InstallationCommandReturnsExitZeroHandleCommandCalled_Then_IsInstalledIsFalse()
        {
            var mockConfigurationProvider = new Mock<IConfigurationProvider>();
            mockConfigurationProvider
                .Setup(x => x.GetInstallationCommand())
                .Returns("exit 0");

            var replyHandler = new ActionReplyHandler();
            var handler = new InstallSoftwareCommandHandler(
                (IActionReplyPrivate)replyHandler,
                m_mockLogger.Object,
                mockConfigurationProvider.Object);

            InstallSoftwareStatusEventArgs? args = null;
            replyHandler.OnCommandHandled += (_, e) => args = e as InstallSoftwareStatusEventArgs;

            handler.HandleCommand(new InstallSoftwareCommand());

            Assert.IsNotNull(args);
            Assert.IsFalse(args.IsInstalled);
        }

        [TestMethod]
        public void Given_BusinessLogicModule_When_InstallationServiceHandleCommandCalled_Then_LogErrorIsCalledWithException()
        {
            var mockConfigurationProvider = new Mock<IConfigurationProvider>();
            mockConfigurationProvider
                .Setup(x => x.GetInstallationCommand())
                .Throws(new InvalidOperationException("install failure"));

            var replyHandler = new ActionReplyHandler();
            var handler = new InstallSoftwareCommandHandler(
                (IActionReplyPrivate)replyHandler,
                m_mockLogger.Object,
                mockConfigurationProvider.Object);

            handler.HandleCommand(new InstallSoftwareCommand());

            m_mockLogger.Verify(
                x => x.LogError(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                Times.Once());
        }

        [TestMethod]
        public void Given_BusinessLogicModule_When_InstallationServiceHandleCommandCalled_Then_OnCommandHandledIsStillRaised()
        {
            var mockConfigurationProvider = new Mock<IConfigurationProvider>();
            mockConfigurationProvider
                .Setup(x => x.GetInstallationCommand())
                .Throws(new InvalidOperationException("install failure"));

            var replyHandler = new ActionReplyHandler();
            var handler = new InstallSoftwareCommandHandler(
                (IActionReplyPrivate)replyHandler,
                m_mockLogger.Object,
                mockConfigurationProvider.Object);

            bool fired = false;
            replyHandler.OnCommandHandled += (_, _) => fired = true;

            handler.HandleCommand(new InstallSoftwareCommand());

            Assert.IsTrue(fired);
        }

        // VerifyInstallationPrerequisitesCommandHandler direct tests

        [TestMethod]
        public void Given_BusinessLogicModule_When_PrerequisiteCheckerThrowsHandleCommandCalled_Then_PrerequisitesMetIsFalse()
        {
            var mockConfigurationProvider = new Mock<IConfigurationProvider>();
            mockConfigurationProvider
                .Setup(x => x.GetVerificationCommand())
                .Throws(new InvalidOperationException("check failure"));

            var replyHandler = new ActionReplyHandler();
            var handler = new VerifyInstallationPrerequisitesCommandHandler(
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

        [TestMethod]
        public void Given_BusinessLogicModule_When_VerificationCommandIsWhitespaceHandleCommandCalled_Then_PrerequisitesMetIsFalse()
        {
            var mockConfigurationProvider = new Mock<IConfigurationProvider>();
            mockConfigurationProvider
                .Setup(x => x.GetVerificationCommand())
                .Returns("  ");

            var replyHandler = new ActionReplyHandler();
            var handler = new VerifyInstallationPrerequisitesCommandHandler(
                (IActionReplyPrivate)replyHandler,
                m_mockLogger.Object,
                mockConfigurationProvider.Object);

            VerifyInstallationPrerequisitesStatusEventArgs? args = null;
            replyHandler.OnCommandHandled += (_, e) => args = e as VerifyInstallationPrerequisitesStatusEventArgs;

            handler.HandleCommand(new VerifyInstallationPrerequisitesCommand());

            Assert.IsNotNull(args);
            Assert.IsFalse(args.PrerequisitesMet);
            m_mockLogger.Verify(
                x => x.LogWarn(It.Is<string>(s => s.Contains("missing") || s.Contains("empty")), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                Times.Once());
        }

        [TestMethod]
        public void Given_BusinessLogicModule_When_VerificationCommandReturnsExitZeroHandleCommandCalled_Then_PrerequisitesMetIsFalse()
        {
            var mockConfigurationProvider = new Mock<IConfigurationProvider>();
            mockConfigurationProvider
                .Setup(x => x.GetVerificationCommand())
                .Returns("exit 0");

            var replyHandler = new ActionReplyHandler();
            var handler = new VerifyInstallationPrerequisitesCommandHandler(
                (IActionReplyPrivate)replyHandler,
                m_mockLogger.Object,
                mockConfigurationProvider.Object);

            VerifyInstallationPrerequisitesStatusEventArgs? args = null;
            replyHandler.OnCommandHandled += (_, e) => args = e as VerifyInstallationPrerequisitesStatusEventArgs;

            handler.HandleCommand(new VerifyInstallationPrerequisitesCommand());

            Assert.IsNotNull(args);
            Assert.IsFalse(args.PrerequisitesMet);
        }

        [TestMethod]
        public void Given_BusinessLogicModule_When_PrerequisiteCheckerThrowsHandleCommandCalled_Then_LogErrorIsCalledWithException()
        {
            var mockConfigurationProvider = new Mock<IConfigurationProvider>();
            mockConfigurationProvider
                .Setup(x => x.GetVerificationCommand())
                .Throws(new InvalidOperationException("check failure"));

            var replyHandler = new ActionReplyHandler();
            var handler = new VerifyInstallationPrerequisitesCommandHandler(
                (IActionReplyPrivate)replyHandler,
                m_mockLogger.Object,
                mockConfigurationProvider.Object);

            handler.HandleCommand(new VerifyInstallationPrerequisitesCommand());

            m_mockLogger.Verify(
                x => x.LogError(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                Times.Once());
        }

        [TestMethod]
        public void Given_BusinessLogicModule_When_PrerequisiteCheckerThrowsHandleCommandCalled_Then_OnCommandHandledIsStillRaised()
        {
            var mockConfigurationProvider = new Mock<IConfigurationProvider>();
            mockConfigurationProvider
                .Setup(x => x.GetVerificationCommand())
                .Throws(new InvalidOperationException("check failure"));

            var replyHandler = new ActionReplyHandler();
            var handler = new VerifyInstallationPrerequisitesCommandHandler(
                (IActionReplyPrivate)replyHandler,
                m_mockLogger.Object,
                mockConfigurationProvider.Object);

            bool fired = false;
            replyHandler.OnCommandHandled += (_, _) => fired = true;

            handler.HandleCommand(new VerifyInstallationPrerequisitesCommand());

            Assert.IsTrue(fired);
        }

        // UIAppStartedCommandHandler and related event args

        [TestMethod]
        public void Given_BusinessLogicModule_When_ValidUIAppStartedCommandHandleCommandCalled_Then_OnCommandHandledIsRaised()
        {
            m_mockSystemLanguageProvider.Setup(p => p.FetchSystemLanguage()).Returns("en-US");
            bool eventFired = false;
            m_sut.ActionReplyEvent.OnCommandHandled += (_, _) => eventFired = true;

            m_sut.HandleCommand(new UIAppStartedCommand());

            Assert.IsTrue(eventFired);
        }

        [TestMethod]
        public void Given_BusinessLogicModule_When_ValidUIAppStartedCommandHandleCommandCalled_Then_EventArgsIsShowSystemLanguageEventArgs()
        {
            m_mockSystemLanguageProvider.Setup(p => p.FetchSystemLanguage()).Returns("en-US");
            System.EventArgs? capturedArgs = null;
            m_sut.ActionReplyEvent.OnCommandHandled += (_, e) => capturedArgs = e;

            m_sut.HandleCommand(new UIAppStartedCommand());

            Assert.IsInstanceOfType(capturedArgs, typeof(ShowSystemLanguageEventArgs));
        }

        [TestMethod]
        public void Given_BusinessLogicModule_When_ValidUIAppStartedCommandHandleCommandCalled_Then_EventArgsLanguageMatchesProviderResult()
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

        [TestMethod]
        public void Given_BusinessLogicModule_When_ValidUIAppStartedCommandHandleCommandCalled_Then_BackendLoggerLogInfoIsCalled()
        {
            m_mockSystemLanguageProvider.Setup(p => p.FetchSystemLanguage()).Returns("en-US");

            m_sut.HandleCommand(new UIAppStartedCommand());

            m_mockLogger.Verify(
                x => x.LogInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                Times.AtLeastOnce());
        }

        [TestMethod]
        public void Given_BusinessLogicModule_When_ValidLanguageShowSystemLanguageEventArgsConstructed_Then_LanguagePropertyIsSet()
        {
            var args = new ShowSystemLanguageEventArgs("fr-FR");

            Assert.AreEqual("fr-FR", args.Language);
        }

        [TestMethod]
        public void Given_BusinessLogicModule_When_NullLanguageShowSystemLanguageEventArgsConstructed_Then_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new ShowSystemLanguageEventArgs(null!));
        }

        // Constructor guards and CommandType checks on concrete handlers

        [TestMethod]
        public void Given_BusinessLogicModule_When_NullSystemLanguageProviderUIAppStartedCommandHandlerConstructed_Then_ThrowsArgumentNullException()
        {
            var replyHandler = new ActionReplyHandler();
            Assert.ThrowsException<ArgumentNullException>(
                () => new UIAppStartedCommandHandler(
                    (IActionReplyPrivate)replyHandler,
                    m_mockLogger.Object,
                    null!));
        }

        [TestMethod]
        public void Given_BusinessLogicModule_When_NullRaiseCloseApplicationRequestedCloseAppCommandHandlerConstructed_Then_ThrowsArgumentNullException()
        {
            var replyHandler = new ActionReplyHandler();

            Assert.ThrowsException<ArgumentNullException>(
                () => new CloseAppCommandHandler(
                    (IActionReplyPrivate)replyHandler,
                    m_mockLogger.Object,
                    null!));
        }

        [TestMethod]
        public void Given_BusinessLogicModule_When_NullConfigurationInstallSoftwareCommandHandlerConstructed_Then_ThrowsArgumentNullException()
        {
            var replyHandler = new ActionReplyHandler();

            Assert.ThrowsException<ArgumentNullException>(
                () => new InstallSoftwareCommandHandler(
                    (IActionReplyPrivate)replyHandler,
                    m_mockLogger.Object,
                    null!));
        }

        [TestMethod]
        public void Given_BusinessLogicModule_When_NullConfigurationVerifyInstallationPrerequisitesCommandHandlerConstructed_Then_ThrowsArgumentNullException()
        {
            var replyHandler = new ActionReplyHandler();

            Assert.ThrowsException<ArgumentNullException>(
                () => new VerifyInstallationPrerequisitesCommandHandler(
                    (IActionReplyPrivate)replyHandler,
                    m_mockLogger.Object,
                    null!));
        }

        [TestMethod]
        public void Given_BusinessLogicModule_When_NullCommandHandleCommandCalledOnConcreteHandler_Then_ThrowsArgumentNullException()
        {
            var replyHandler = new ActionReplyHandler();
            var handler = new CloseAppCommandHandler(
                (IActionReplyPrivate)replyHandler,
                m_mockLogger.Object,
                () => { });

            Assert.ThrowsException<ArgumentNullException>(() => handler.HandleCommand(null!));
        }

        [TestMethod]
        public void Given_BusinessLogicModule_When_CloseAppCommandHandlerCommandTypeAccessed_Then_ReturnsCloseAppCommandType()
        {
            var replyHandler = new ActionReplyHandler();
            var handler = new CloseAppCommandHandler(
                (IActionReplyPrivate)replyHandler, m_mockLogger.Object, () => { });

            Assert.AreEqual(typeof(CloseAppCommand), handler.CommandType);
        }

        [TestMethod]
        public void Given_BusinessLogicModule_When_InstallSoftwareCommandHandlerCommandTypeAccessed_Then_ReturnsInstallSoftwareCommandType()
        {
            var replyHandler = new ActionReplyHandler();
            var handler = new InstallSoftwareCommandHandler(
                (IActionReplyPrivate)replyHandler,
                m_mockLogger.Object,
                new Mock<IConfigurationProvider>().Object);

            Assert.AreEqual(typeof(InstallSoftwareCommand), handler.CommandType);
        }

        [TestMethod]
        public void Given_BusinessLogicModule_When_LogActionCommandHandlerCommandTypeAccessed_Then_ReturnsLogCommandType()
        {
            var replyHandler = new ActionReplyHandler();
            var handler = new LogActionCommandHandler(
                (IActionReplyPrivate)replyHandler,
                m_mockLogger.Object);

            Assert.AreEqual(typeof(LogCommand), handler.CommandType);
        }

        [TestMethod]
        public void Given_BusinessLogicModule_When_VerifyInstallationPrerequisitesCommandHandlerCommandTypeAccessed_Then_ReturnsVerifyCommandType()
        {
            var replyHandler = new ActionReplyHandler();
            var handler = new VerifyInstallationPrerequisitesCommandHandler(
                (IActionReplyPrivate)replyHandler,
                m_mockLogger.Object,
                new Mock<IConfigurationProvider>().Object);

            Assert.AreEqual(typeof(VerifyInstallationPrerequisitesCommand), handler.CommandType);
        }

        [TestMethod]
        public void Given_BusinessLogicModule_When_UIAppStartedCommandHandlerCommandTypeAccessed_Then_ReturnsUIAppStartedCommandType()
        {
            var replyHandler = new ActionReplyHandler();
            var handler = new UIAppStartedCommandHandler(
                (IActionReplyPrivate)replyHandler,
                m_mockLogger.Object,
                m_mockSystemLanguageProvider.Object);

            Assert.AreEqual(typeof(UIAppStartedCommand), handler.CommandType);
        }
    }
}
