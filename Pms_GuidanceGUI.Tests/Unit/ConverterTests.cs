#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : Pms_GuidanceGUI.Tests
// File   : ConverterTests.cs
// Description: Unit tests for the Converter module. All external dependencies
//              are mocked with Moq. All tests follow the Given/When/Then convention.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – compliance with C# Coding Guidelines V2.1
//--------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using BusinessLogicModule.Commands;
using BusinessLogicModule.EventArgs;
using ConnectionModule;
using ConverterModule;
using Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Pms_GuidanceGUI.Tests.Unit
{
    /// <summary>
    /// Unit tests for <see cref="Converter"/>.
    /// All external dependencies are mocked with Moq.
    /// All tests follow the Given/When/Then naming convention.
    /// </summary>
    [TestClass]
    public class ConverterTests
    {
        #region Private Members

        private Mock<IBusinessLogicModule> m_mockBlm              = null!;
        private Mock<IActionReply>         m_mockActionReply      = null!;
        private Mock<IConnectionManager>   m_mockConnectionManager = null!;
        private Mock<ILogger>              m_mockLogger           = null!;
        private List<ICommand>             m_receivedCommands     = null!;
        private List<OutboundMessage>      m_sentMessages         = null!;
        private Converter                  m_converter            = null!;

        #endregion

        /// <summary>
        /// Initializes the test context before each test method runs.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            m_mockBlm              = new Mock<IBusinessLogicModule>();
            m_mockActionReply      = new Mock<IActionReply>();
            m_mockConnectionManager = new Mock<IConnectionManager>();
            m_mockLogger           = new Mock<ILogger>();
            m_receivedCommands     = new List<ICommand>();
            m_sentMessages         = new List<OutboundMessage>();

            m_mockBlm.Setup(x => x.ActionReplyEvent).Returns(m_mockActionReply.Object);
            m_mockBlm.Setup(x => x.HandleCommand(It.IsAny<ICommand>()))
                     .Callback<ICommand>(cmd => m_receivedCommands.Add(cmd));

            m_mockConnectionManager
                .Setup(x => x.SendMessage(It.IsAny<OutboundMessage>()))
                .Callback<OutboundMessage>(msg => m_sentMessages.Add(msg));

            m_converter = new Converter(
                m_mockBlm.Object, m_mockConnectionManager.Object, m_mockLogger.Object);
            m_converter.Open();
        }

        /// <summary>
        /// Cleans up resources after each test method runs.
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            m_converter.Close();
        }

        // ── Constructor guards ────────────────────────────────────────────────────

        /// <summary>
    /// Given Converter NullBlm When OpenCalled Then ThrowsNullReferenceException
        /// </summary>
        [TestMethod]
        public void Given_Converter_NullBlm_When_OpenCalled_Then_ThrowsNullReferenceException()
        {
            // The constructor no longer accesses ActionReplyEvent; Open() dereferences it, which throws NRE.
            var converter = new Converter(null!, m_mockConnectionManager.Object, m_mockLogger.Object);

            Assert.ThrowsException<NullReferenceException>(() => converter.Open());
        }

        /// <summary>
    /// Given Converter NullConnectionManager When OpenCalled Then ThrowsNullReferenceException
        /// </summary>
        [TestMethod]
        public void Given_Converter_NullConnectionManager_When_OpenCalled_Then_ThrowsNullReferenceException()
        {
            // The constructor succeeds (no dereference of ConnectionManager there),
            // but Open() dereferences it to subscribe to MessageReceived.
            var converter = new Converter(m_mockBlm.Object, null!, m_mockLogger.Object);

            Assert.ThrowsException<NullReferenceException>(() => converter.Open());
        }

        /// <summary>
    /// Given Converter NullLogger When Constructed Then ThrowsArgumentNullException
        /// </summary>
        [TestMethod]
        public void Given_Converter_NullLogger_When_Constructed_Then_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new Converter(m_mockBlm.Object, m_mockConnectionManager.Object, null!));
        }

        // ── Inbound: LogMessage ───────────────────────────────────────────────────

        /// <summary>
    /// Given Converter ValidLogMessagePayload When MessageReceived Then LogCommandForwardedToBlm
        /// </summary>
        [TestMethod]
        public void Given_Converter_ValidLogMessagePayload_When_MessageReceived_Then_LogCommandForwardedToBlm()
        {
            SimulateIncoming("LogMessage", BuildLogPayload("Hello", DateTime.UtcNow));

            Assert.AreEqual(1, m_receivedCommands.Count);
            Assert.IsInstanceOfType(m_receivedCommands[0], typeof(LogCommand));
        }

        /// <summary>
    /// Given Converter ValidLogMessagePayload When MessageReceived Then CommandHasCorrectMessage
        /// </summary>
        [TestMethod]
        public void Given_Converter_ValidLogMessagePayload_When_MessageReceived_Then_CommandHasCorrectMessage()
        {
            SimulateIncoming("LogMessage", BuildLogPayload("Expected text", DateTime.UtcNow));

            Assert.AreEqual("Expected text", ((LogCommand)m_receivedCommands[0]).Message);
        }

        /// <summary>
    /// Given Converter ValidLogMessagePayload When MessageReceived Then CommandHasCorrectTimestamp
        /// </summary>
        [TestMethod]
        public void Given_Converter_ValidLogMessagePayload_When_MessageReceived_Then_CommandHasCorrectTimestamp()
        {
            var expected = new DateTime(2026, 4, 1, 12, 0, 0, DateTimeKind.Utc);
            SimulateIncoming("LogMessage", BuildLogPayload("msg", expected));

            Assert.AreEqual(expected, ((LogCommand)m_receivedCommands[0]).Timestamp);
        }

        /// <summary>
    /// Given Converter LogMessageWithNullMessageField When MessageReceived Then CommandUsesEmptyString
        /// </summary>
        [TestMethod]
        public void Given_Converter_LogMessageWithNullMessageField_When_MessageReceived_Then_CommandUsesEmptyString()
        {
            // LogJsonActionHandler falls back to string.Empty when the Message field is null.
            SimulateIncoming("LogMessage", "{\"Message\":null,\"Timestamp\":\"2026-01-01T00:00:00Z\"}");

            Assert.AreEqual(1, m_receivedCommands.Count);
            Assert.AreEqual(string.Empty, ((LogCommand)m_receivedCommands[0]).Message);
        }

        // ── Inbound: other actions ────────────────────────────────────────────────

        /// <summary>
    /// Given Converter VerifyPrerequisiteAction When MessageReceived Then VerifyCommandForwardedToBlm
        /// </summary>
        [TestMethod]
        public void Given_Converter_VerifyPrerequisiteAction_When_MessageReceived_Then_VerifyCommandForwardedToBlm()
        {
            SimulateIncoming("VerifyInstallationPrerequisite", "{}");

            Assert.AreEqual(1, m_receivedCommands.Count);
            Assert.IsInstanceOfType(m_receivedCommands[0], typeof(VerifyInstallationPrerequisitesCommand));
        }

        /// <summary>
    /// Given Converter InstallSoftwareAction When MessageReceived Then InstallCommandForwardedToBlm
        /// </summary>
        [TestMethod]
        public void Given_Converter_InstallSoftwareAction_When_MessageReceived_Then_InstallCommandForwardedToBlm()
        {
            SimulateIncoming("InstallSoftware", "{}");

            Assert.AreEqual(1, m_receivedCommands.Count);
            Assert.IsInstanceOfType(m_receivedCommands[0], typeof(InstallSoftwareCommand));
        }

        /// <summary>
    /// Given Converter CloseAppAction When MessageReceived Then CloseCommandForwardedToBlm
        /// </summary>
        [TestMethod]
        public void Given_Converter_CloseAppAction_When_MessageReceived_Then_CloseCommandForwardedToBlm()
        {
            SimulateIncoming("CloseApp", "{}");

            Assert.AreEqual(1, m_receivedCommands.Count);
            Assert.IsInstanceOfType(m_receivedCommands[0], typeof(CloseAppCommand));
        }

        // ── Inbound: routing quality ──────────────────────────────────────────────

        /// <summary>
    /// Given Converter ValidAction When MessageReceived Then LogDebugIsCalled
        /// </summary>
        [TestMethod]
        public void Given_Converter_ValidAction_When_MessageReceived_Then_LogDebugIsCalled()
        {
            SimulateIncoming("CloseApp", "{}");

            m_mockLogger.Verify(x => x.LogDebug(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        /// <summary>
    /// Given Converter TwoSuccessiveMessages When MessageReceived Then BothCommandsForwardedToBlm
        /// </summary>
        [TestMethod]
        public void Given_Converter_TwoSuccessiveMessages_When_MessageReceived_Then_BothCommandsForwardedToBlm()
        {
            SimulateIncoming("CloseApp", "{}");
            SimulateIncoming("InstallSoftware", "{}");

            Assert.AreEqual(2, m_receivedCommands.Count);
        }

        // ── Inbound: edge cases ───────────────────────────────────────────────────

        /// <summary>
    /// Given Converter UnknownAction When MessageReceived Then NoCommandForwardedAndLogWarnIsCalled
        /// </summary>
        [TestMethod]
        public void Given_Converter_UnknownAction_When_MessageReceived_Then_NoCommandForwardedAndLogWarnIsCalled()
        {
            SimulateIncoming("UnknownAction", "{}");

            Assert.AreEqual(0, m_receivedCommands.Count);
            m_mockLogger.Verify(x => x.LogWarn(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        /// <summary>
    /// Given Converter EmptyActionName When MessageReceived Then NoCommandForwardedAndLogWarnIsCalled
        /// </summary>
        [TestMethod]
        public void Given_Converter_EmptyActionName_When_MessageReceived_Then_NoCommandForwardedAndLogWarnIsCalled()
        {
            SimulateIncoming(string.Empty, "{}");

            Assert.AreEqual(0, m_receivedCommands.Count);
            m_mockLogger.Verify(x => x.LogWarn(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        /// <summary>
    /// Given Converter MalformedJsonPayload When MessageReceived Then NoCommandForwardedAndNoExceptionThrown
        /// </summary>
        [TestMethod]
        public void Given_Converter_MalformedJsonPayload_When_MessageReceived_Then_NoCommandForwardedAndNoExceptionThrown()
        {
            bool threw = false;
            try
            {
                SimulateIncoming("LogMessage", "NOT_JSON");
            }
            catch
            {
                threw = true;
            }

            Assert.IsFalse(threw);
            Assert.AreEqual(0, m_receivedCommands.Count);
        }

        /// <summary>
    /// Given Converter MalformedJsonPayload ForCloseApp When MessageReceived Then NoCommandForwardedAndNoExceptionThrown
        /// </summary>
        [TestMethod]
        public void Given_Converter_MalformedJsonPayload_ForCloseApp_When_MessageReceived_Then_NoCommandForwardedAndNoExceptionThrown()
        {
            bool threw = false;
            try
            {
                SimulateIncoming("CloseApp", "NOT_JSON");
            }
            catch
            {
                threw = true;
            }

            Assert.IsFalse(threw);
            Assert.AreEqual(0, m_receivedCommands.Count);
        }

        /// <summary>
    /// Given Converter MalformedJsonPayload ForInstallSoftware When MessageReceived Then NoCommandForwardedAndNoExceptionThrown
        /// </summary>
        [TestMethod]
        public void Given_Converter_MalformedJsonPayload_ForInstallSoftware_When_MessageReceived_Then_NoCommandForwardedAndNoExceptionThrown()
        {
            bool threw = false;
            try
            {
                SimulateIncoming("InstallSoftware", "NOT_JSON");
            }
            catch
            {
                threw = true;
            }

            Assert.IsFalse(threw);
            Assert.AreEqual(0, m_receivedCommands.Count);
        }

        /// <summary>
    /// Given Converter MalformedJsonPayload ForVerifyPrerequisite When MessageReceived Then NoCommandForwardedAndNoExceptionThrown
        /// </summary>
        [TestMethod]
        public void Given_Converter_MalformedJsonPayload_ForVerifyPrerequisite_When_MessageReceived_Then_NoCommandForwardedAndNoExceptionThrown()
        {
            bool threw = false;
            try
            {
                SimulateIncoming("VerifyInstallationPrerequisite", "NOT_JSON");
            }
            catch
            {
                threw = true;
            }

            Assert.IsFalse(threw);
            Assert.AreEqual(0, m_receivedCommands.Count);
        }

        /// <summary>
    /// Given Converter NullEventArgs When MessageReceived Then NoExceptionThrown
        /// </summary>
        [TestMethod]
        public void Given_Converter_NullEventArgs_When_MessageReceived_Then_NoExceptionThrown()
        {
            bool threw = false;
            try
            {
                m_mockConnectionManager.Raise(
                    x => x.MessageReceived += null, this, (MessageReceivedEventArgs)null!);
            }
            catch
            {
                threw = true;
            }

            Assert.IsFalse(threw);
        }

        // ── Outbound: SendMessage is called ──────────────────────────────────────

        /// <summary>
    /// Given Converter VerifyPrerequisitesStatusEventArgs When ActionReplyFired Then SendMessageIsCalledOnce
        /// </summary>
        [TestMethod]
        public void Given_Converter_VerifyPrerequisitesStatusEventArgs_When_ActionReplyFired_Then_SendMessageIsCalledOnce()
        {
            m_mockActionReply.Raise(
                x => x.OnCommandHandled += null, this,
                new VerifyInstallationPrerequisitesStatusEventArgs(true));

            m_mockConnectionManager.Verify(x => x.SendMessage(It.IsAny<OutboundMessage>()), Times.Once());
        }

        /// <summary>
        /// Given Converter InstallSoftwareStatusEventArgs When ActionReplyFired Then SendMessageIsNotCalled
        /// </summary>
        [TestMethod]
        public void Given_Converter_InstallSoftwareStatusEventArgs_When_ActionReplyFired_Then_SendMessageIsNotCalled()
        {
            m_mockActionReply.Raise(
                x => x.OnCommandHandled += null, this,
                new InstallSoftwareStatusEventArgs(true));

            m_mockConnectionManager.Verify(x => x.SendMessage(It.IsAny<OutboundMessage>()), Times.Never());
        }

        // ── Outbound: reply action names ──────────────────────────────────────────

        /// <summary>
        /// Given VerifyPrerequisitesStatusEventArgs When ActionReplyFired Then ReplyActionIsShowInstallationPrerequisite
        /// </summary>
        [TestMethod]

        public void Given_Converter_VerifyPrerequisitesStatusEventArgs_When_ActionReplyFired_Then_ReplyActionIsShowInstallationPrerequisite()
        {
            m_mockActionReply.Raise(
                x => x.OnCommandHandled += null, this,
                new VerifyInstallationPrerequisitesStatusEventArgs(true));

            Assert.AreEqual("ShowInstallationPrerequisite", m_sentMessages[0].Action);
        }

        /// <summary>
        /// Given Converter InstallSoftwareStatusEventArgs When ActionReplyFired Then NoInstallReplyActionIsSent
        /// </summary>
        [TestMethod]
        public void Given_Converter_InstallSoftwareStatusEventArgs_When_ActionReplyFired_Then_NoInstallReplyActionIsSent()
        {
            m_mockActionReply.Raise(
                x => x.OnCommandHandled += null, this,
                new InstallSoftwareStatusEventArgs(true));

            Assert.AreEqual(0, m_sentMessages.Count);
        }

        // ── Outbound: reply payload content ──────────────────────────────────────

        /// <summary>
    /// Given Converter VerifyPrerequisitesMetEventArgs When ActionReplyFired Then ReplyStatusIsOk
        /// </summary>
        [TestMethod]
        public void Given_Converter_VerifyPrerequisitesMetEventArgs_When_ActionReplyFired_Then_ReplyStatusIsOk()
        {
            m_mockActionReply.Raise(
                x => x.OnCommandHandled += null, this,
                new VerifyInstallationPrerequisitesStatusEventArgs(true));

            Assert.AreEqual("OK", GetMessageProperty(m_sentMessages[0], "Status"));
        }

        /// <summary>
    /// Given Converter VerifyPrerequisitesNotMetEventArgs When ActionReplyFired Then ReplyStatusIsNotOk
        /// </summary>
        [TestMethod]
        public void Given_Converter_VerifyPrerequisitesNotMetEventArgs_When_ActionReplyFired_Then_ReplyStatusIsNotOk()
        {
            m_mockActionReply.Raise(
                x => x.OnCommandHandled += null, this,
                new VerifyInstallationPrerequisitesStatusEventArgs(false));

            Assert.AreEqual("Not Ok", GetMessageProperty(m_sentMessages[0], "Status"));
        }

        /// <summary>
    /// Given Converter InstallSoftwareSuccessEventArgs When ActionReplyFired Then NoInstallPayloadIsSent
        /// </summary>
        [TestMethod]
        public void Given_Converter_InstallSoftwareSuccessEventArgs_When_ActionReplyFired_Then_NoInstallPayloadIsSent()
        {
            m_mockActionReply.Raise(
                x => x.OnCommandHandled += null, this,
                new InstallSoftwareStatusEventArgs(true));

            Assert.AreEqual(0, m_sentMessages.Count);
        }

        /// <summary>
    /// Given Converter InstallSoftwareFailureEventArgs When ActionReplyFired Then NoInstallPayloadIsSent
        /// </summary>
        [TestMethod]
        public void Given_Converter_InstallSoftwareFailureEventArgs_When_ActionReplyFired_Then_NoInstallPayloadIsSent()
        {
            m_mockActionReply.Raise(
                x => x.OnCommandHandled += null, this,
                new InstallSoftwareStatusEventArgs(false));

            Assert.AreEqual(0, m_sentMessages.Count);
        }

        // ── Outbound: unregistered event type ────────────────────────────────────

        /// <summary>
    /// Given Converter UnregisteredEventType When ActionReplyFired Then LogWarnIsCalled
        /// </summary>
        [TestMethod]
        public void Given_Converter_UnregisteredEventType_When_ActionReplyFired_Then_LogWarnIsCalled()
        {
            // EventArgs.Empty has no matching writer registered in Converter.
            m_mockActionReply.Raise(
                x => x.OnCommandHandled += null, this, EventArgs.Empty);

            m_mockLogger.Verify(x => x.LogWarn(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        /// <summary>
    /// Given Converter UnregisteredEventType When ActionReplyFired Then SendMessageIsNotCalled
        /// </summary>
        [TestMethod]
        public void Given_Converter_UnregisteredEventType_When_ActionReplyFired_Then_SendMessageIsNotCalled()
        {
            m_mockActionReply.Raise(
                x => x.OnCommandHandled += null, this, EventArgs.Empty);

            m_mockConnectionManager.Verify(x => x.SendMessage(It.IsAny<OutboundMessage>()), Times.Never());
        }

        // ── Lifecycle ─────────────────────────────────────────────────────────────

        /// <summary>
    /// Given Converter ClosedConverter When MessageReceived Then NoCommandForwardedToBlm
        /// </summary>
        [TestMethod]
        public void Given_Converter_ClosedConverter_When_MessageReceived_Then_NoCommandForwardedToBlm()
        {
            m_converter.Close();

            SimulateIncoming("LogMessage", BuildLogPayload("ignored", DateTime.UtcNow));

            Assert.AreEqual(0, m_receivedCommands.Count);
        }

        /// <summary>
    /// Given Converter ClosedThenReopenedConverter When MessageReceived Then CommandForwardedToBlm
        /// </summary>
        [TestMethod]
        public void Given_Converter_ClosedThenReopenedConverter_When_MessageReceived_Then_CommandForwardedToBlm()
        {
            m_converter.Close();
            m_converter.Open();

            SimulateIncoming("CloseApp", "{}");

            Assert.AreEqual(1, m_receivedCommands.Count);
            Assert.IsInstanceOfType(m_receivedCommands[0], typeof(CloseAppCommand));
        }

        // ── Inbound: UIAppStarted action ─────────────────────────────────────

        /// <summary>
    /// Given Converter UIAppStartedAction When MessageReceived Then UIAppStartedCommandForwardedToBlm
        /// </summary>
        [TestMethod]
        public void Given_Converter_UIAppStartedAction_When_MessageReceived_Then_UIAppStartedCommandForwardedToBlm()
        {
            SimulateIncoming("UIAppStarted", "{}");

            Assert.AreEqual(1, m_receivedCommands.Count);
            Assert.IsInstanceOfType(m_receivedCommands[0], typeof(UIAppStartedCommand));
        }

        // ── Outbound: ShowSystemLanguage reply ────────────────────────────────────

        /// <summary>
    /// Given Converter ShowSystemLanguageEventArgs When ActionReplyFired Then SendMessageIsCalledOnce
        /// </summary>
        [TestMethod]
        public void Given_Converter_ShowSystemLanguageEventArgs_When_ActionReplyFired_Then_SendMessageIsCalledOnce()
        {
            m_mockActionReply.Raise(
                x => x.OnCommandHandled += null, this,
                new ShowSystemLanguageEventArgs("en-US"));

            m_mockConnectionManager.Verify(x => x.SendMessage(It.IsAny<OutboundMessage>()), Times.Once());
        }

        /// <summary>
    /// Given Converter ShowSystemLanguageEventArgs When ActionReplyFired Then ReplyActionIsShowSystemLanguage
        /// </summary>
        [TestMethod]
        public void Given_Converter_ShowSystemLanguageEventArgs_When_ActionReplyFired_Then_ReplyActionIsShowSystemLanguage()
        {
            m_mockActionReply.Raise(
                x => x.OnCommandHandled += null, this,
                new ShowSystemLanguageEventArgs("en-US"));

            Assert.AreEqual("ShowSystemLanguage", m_sentMessages[0].Action);
        }

        /// <summary>
    /// Given Converter ShowSystemLanguageEventArgs When ActionReplyFired Then ReplyLanguageMatchesEvent
        /// </summary>
        [TestMethod]
        public void Given_Converter_ShowSystemLanguageEventArgs_When_ActionReplyFired_Then_ReplyLanguageMatchesEvent()
        {
            m_mockActionReply.Raise(
                x => x.OnCommandHandled += null, this,
                new ShowSystemLanguageEventArgs("de-DE"));

            Assert.AreEqual("de-DE", GetMessageProperty(m_sentMessages[0], "Language"));
        }

        // ── Helpers ───────────────────────────────────────────────────────────────

        private void SimulateIncoming(string action, string payload)
        {
            m_mockConnectionManager.Raise(
                x => x.MessageReceived += null,
                this,
                new MessageReceivedEventArgs(action, payload));
        }

        private static string BuildLogPayload(string message, DateTime timestamp)
            => $"{{\"Message\":\"{message}\",\"Timestamp\":\"{timestamp:O}\"}}";

        /// <summary>
        /// Reads a property directly from the <see cref="OutboundMessage"/> via reflection.
        /// This is required because the concrete message types are internal to ConverterModule.
        /// </summary>
        private static object? GetMessageProperty(OutboundMessage reply, string propertyName)
            => reply.GetType().GetProperty(propertyName)?.GetValue(reply);
    }
}

