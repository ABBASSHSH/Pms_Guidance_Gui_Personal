#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : Pms_GuidanceGUI.Tests
// File   : ConnectionManagerTests.cs
// Description: Unit tests for ConnectionManager. IWebViewWrapper and ILogger
//              are mocked with Moq. All tests follow the Given/When/Then convention.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – compliance with C# Coding Guidelines V2.1
//--------------------------------------------------------------------
#endregion

using System;
using ConnectionModule;
using Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Pms_GuidanceGUI.Tests.Unit
{
    /// <summary>
    /// Unit tests for <see cref="ConnectionManager"/>.
    /// All external dependencies are mocked with Moq.
    /// All tests follow the Given/When/Then naming convention.
    /// </summary>
    [TestClass]
    public class ConnectionManagerTests
    {
        #region Private Members

        private Mock<IWebViewWrapper> m_mockWebView       = null!;
        private Mock<ILogger>         m_mockLogger        = null!;
        private ConnectionManager     m_connectionManager = null!;

        #endregion

        /// <summary>
        /// Initializes the test context before each test method runs.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            m_mockWebView = new Mock<IWebViewWrapper>();
            m_mockLogger  = new Mock<ILogger>();

            m_connectionManager = new ConnectionManager(m_mockWebView.Object, m_mockLogger.Object);
            m_connectionManager.Open();
        }

        /// <summary>
        /// Cleans up resources after each test method runs.
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            m_connectionManager.Close();
        }

        // ── Constructor guards ────────────────────────────────────────────────────

        /// <summary>
    /// Given ConnectionManager When NullWebViewConstructed Then ThrowsArgumentNullException
        /// </summary>
        [TestMethod]
        public void Given_ConnectionManager_When_NullWebViewConstructed_Then_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new ConnectionManager(null!, new Mock<ILogger>().Object));
        }

        /// <summary>
    /// Given ConnectionManager When NullLoggerConstructed Then ThrowsArgumentNullException
        /// </summary>
        [TestMethod]
        public void Given_ConnectionManager_When_NullLoggerConstructed_Then_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new ConnectionManager(new Mock<IWebViewWrapper>().Object, null!));
        }

        // ── Inbound: valid message routing ────────────────────────────────────────

        /// <summary>
    /// Given ConnectionManager When ValidJsonMessageReceived Then MessageReceivedEventIsRaised
        /// </summary>
        [TestMethod]
        public void Given_ConnectionManager_When_ValidJsonMessageReceived_Then_MessageReceivedEventIsRaised()
        {
            MessageReceivedEventArgs? capturedArgs = null;
            m_connectionManager.MessageReceived += (s, e) => capturedArgs = e;

            RaiseIncoming(BuildValidJson("LogMessage"));

            Assert.IsNotNull(capturedArgs);
        }

        /// <summary>
    /// Given ConnectionManager When ValidJsonMessageReceived Then EventArgsHasCorrectAction
        /// </summary>
        [TestMethod]
        public void Given_ConnectionManager_When_ValidJsonMessageReceived_Then_EventArgsHasCorrectAction()
        {
            string? capturedAction = null;
            m_connectionManager.MessageReceived += (s, e) => capturedAction = e.Action;

            RaiseIncoming(BuildValidJson("LogMessage"));

            Assert.AreEqual("LogMessage", capturedAction);
        }

        /// <summary>
    /// Given ConnectionManager When ValidJsonWithPayloadMessageReceived Then EventArgsPayloadContainsExpectedData
        /// </summary>
        [TestMethod]
        public void Given_ConnectionManager_When_ValidJsonWithPayloadMessageReceived_Then_EventArgsPayloadContainsExpectedData()
        {
            string? capturedPayload = null;
            m_connectionManager.MessageReceived += (s, e) => capturedPayload = e.Payload;

            RaiseIncoming(BuildJsonWithPayload("InstallSoftware", "\"Key\":\"Value\""));

            // The payload is re-serialized from the JsonElement; it must contain the original field.
            Assert.IsNotNull(capturedPayload);
            StringAssert.Contains(capturedPayload, "Value");
        }

        /// <summary>
    /// Given ConnectionManager When ValidJsonMessageReceived Then LogDebugIsCalled
        /// </summary>
        [TestMethod]
        public void Given_ConnectionManager_When_ValidJsonMessageReceived_Then_LogDebugIsCalled()
        {
            RaiseIncoming(BuildValidJson("CloseApp"));

            m_mockLogger.Verify(x => x.LogDebug(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        /// <summary>
    /// Given ConnectionManager When TwoSuccessiveValidMessagesMessagesReceived Then BothEventsAreRaised
        /// </summary>
        [TestMethod]
        public void Given_ConnectionManager_When_TwoSuccessiveValidMessagesMessagesReceived_Then_BothEventsAreRaised()
        {
            int count = 0;
            m_connectionManager.MessageReceived += (s, e) => count++;

            RaiseIncoming(BuildValidJson("CloseApp"));
            RaiseIncoming(BuildValidJson("InstallSoftware"));

            Assert.AreEqual(2, count);
        }

        /// <summary>
    /// Given ConnectionManager When NoSubscribersValidJsonReceived Then NoExceptionThrown
        /// </summary>
        [TestMethod]
        public void Given_ConnectionManager_When_NoSubscribersValidJsonReceived_Then_NoExceptionThrown()
        {
            // MessageReceived has no subscribers — the null-conditional invoke must not throw.
            bool threw = false;
            try
            {
                RaiseIncoming(BuildValidJson("CloseApp"));
            }
            catch
            {
                threw = true;
            }

            Assert.IsFalse(threw);
        }

        // ── Inbound: malformed / edge-case input ──────────────────────────────────

        /// <summary>
    /// Given ConnectionManager When MalformedJsonMessageReceived Then LogErrorIsCalled
        /// </summary>
        [TestMethod]
        public void Given_ConnectionManager_When_MalformedJsonMessageReceived_Then_LogErrorIsCalled()
        {
            RaiseIncoming("NOT_VALID_JSON");

            m_mockLogger.Verify(x => x.LogError(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        /// <summary>
    /// Given ConnectionManager When MalformedJsonMessageReceived Then MessageReceivedEventIsNotRaised
        /// </summary>
        [TestMethod]
        public void Given_ConnectionManager_When_MalformedJsonMessageReceived_Then_MessageReceivedEventIsNotRaised()
        {
            bool eventFired = false;
            m_connectionManager.MessageReceived += (s, e) => eventFired = true;

            RaiseIncoming("NOT_VALID_JSON");

            Assert.IsFalse(eventFired);
        }

        /// <summary>
    /// Given ConnectionManager When EmptyStringMessageReceived Then LogWarnIsCalled
        /// </summary>
        [TestMethod]
        public void Given_ConnectionManager_When_EmptyStringMessageReceived_Then_LogWarnIsCalled()
        {
            RaiseIncoming(string.Empty);

            m_mockLogger.Verify(x => x.LogWarn(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        /// <summary>
    /// Given ConnectionManager When WhitespaceStringMessageReceived Then LogWarnIsCalled
        /// </summary>
        [TestMethod]
        public void Given_ConnectionManager_When_WhitespaceStringMessageReceived_Then_LogWarnIsCalled()
        {
            RaiseIncoming("   ");

            m_mockLogger.Verify(x => x.LogWarn(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        /// <summary>
    /// Given ConnectionManager When EmptyStringMessageReceived Then MessageReceivedEventIsNotRaised
        /// </summary>
        [TestMethod]
        public void Given_ConnectionManager_When_EmptyStringMessageReceived_Then_MessageReceivedEventIsNotRaised()
        {
            bool eventFired = false;
            m_connectionManager.MessageReceived += (s, e) => eventFired = true;

            RaiseIncoming(string.Empty);

            Assert.IsFalse(eventFired);
        }

        /// <summary>
    /// Given ConnectionManager When JsonMissingCallContextMessageReceived Then LogWarnIsCalled
        /// </summary>
        [TestMethod]
        public void Given_ConnectionManager_When_JsonMissingCallContextMessageReceived_Then_LogWarnIsCalled()
        {
            RaiseIncoming("{\"Payload\":{}}");

            m_mockLogger.Verify(x => x.LogWarn(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        /// <summary>
    /// Given ConnectionManager When JsonMissingCallContextMessageReceived Then MessageReceivedEventIsNotRaised
        /// </summary>
        [TestMethod]
        public void Given_ConnectionManager_When_JsonMissingCallContextMessageReceived_Then_MessageReceivedEventIsNotRaised()
        {
            bool eventFired = false;
            m_connectionManager.MessageReceived += (s, e) => eventFired = true;

            RaiseIncoming("{\"Payload\":{}}");

            Assert.IsFalse(eventFired);
        }

        /// <summary>
    /// Given ConnectionManager When JsonWithNullActionMessageReceived Then LogWarnIsCalled
        /// </summary>
        [TestMethod]
        public void Given_ConnectionManager_When_JsonWithNullActionMessageReceived_Then_LogWarnIsCalled()
        {
            RaiseIncoming("{\"CallContext\":{\"Action\":null},\"Payload\":{}}");

            m_mockLogger.Verify(x => x.LogWarn(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        /// <summary>
    /// Given ConnectionManager When JsonWithNullActionMessageReceived Then MessageReceivedEventIsNotRaised
        /// </summary>
        [TestMethod]
        public void Given_ConnectionManager_When_JsonWithNullActionMessageReceived_Then_MessageReceivedEventIsNotRaised()
        {
            bool eventFired = false;
            m_connectionManager.MessageReceived += (s, e) => eventFired = true;

            RaiseIncoming("{\"CallContext\":{\"Action\":null},\"Payload\":{}}");

            Assert.IsFalse(eventFired);
        }

        /// <summary>
    /// Given ConnectionManager When JsonWithEmptyStringActionMessageReceived Then MessageReceivedEventIsNotRaised
        /// </summary>
        [TestMethod]
        public void Given_ConnectionManager_When_JsonWithEmptyStringActionMessageReceived_Then_MessageReceivedEventIsNotRaised()
        {
            bool eventFired = false;
            m_connectionManager.MessageReceived += (s, e) => eventFired = true;

            RaiseIncoming("{\"CallContext\":{\"Action\":\"\"},\"Payload\":{}}");

            Assert.IsFalse(eventFired);
        }

        /// <summary>
    /// Given ConnectionManager When JsonWithEmptyStringActionMessageReceived Then LogWarnIsCalled
        /// </summary>
        [TestMethod]
        public void Given_ConnectionManager_When_JsonWithEmptyStringActionMessageReceived_Then_LogWarnIsCalled()
        {
            RaiseIncoming("{\"CallContext\":{\"Action\":\"\"},\"Payload\":{}}");

            m_mockLogger.Verify(x => x.LogWarn(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        /// <summary>
    /// Given ConnectionManager When SubscriberThrowsMessageReceived Then LogErrorIsCalled
        /// </summary>
        [TestMethod]
        public void Given_ConnectionManager_When_SubscriberThrowsMessageReceived_Then_LogErrorIsCalled()
        {
            m_connectionManager.MessageReceived += (s, e) => throw new InvalidOperationException("subscriber error");

            RaiseIncoming(BuildValidJson("CloseApp"));

            m_mockLogger.Verify(x => x.LogError(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        /// <summary>
    /// Given ConnectionManager When SubscriberThrowsMessageReceived Then ExceptionDoesNotPropagate
        /// </summary>
        [TestMethod]
        public void Given_ConnectionManager_When_SubscriberThrowsMessageReceived_Then_ExceptionDoesNotPropagate()
        {
            m_connectionManager.MessageReceived += (s, e) => throw new InvalidOperationException("subscriber error");

            bool threw = false;
            try { RaiseIncoming(BuildValidJson("CloseApp")); }
            catch { threw = true; }

            Assert.IsFalse(threw);
        }

        // ── Lifecycle ─────────────────────────────────────────────────────────────

        /// <summary>
    /// Given ConnectionManager When ClosedManagerWebViewFiresEvent Then MessageReceivedIsNotRaised
        /// </summary>
        [TestMethod]
        public void Given_ConnectionManager_When_ClosedManagerWebViewFiresEvent_Then_MessageReceivedIsNotRaised()
        {
            m_connectionManager.Close();

            bool eventFired = false;
            m_connectionManager.MessageReceived += (s, e) => eventFired = true;

            RaiseIncoming(BuildValidJson("LogMessage"));

            Assert.IsFalse(eventFired);
        }

        /// <summary>
    /// Given ConnectionManager When ClosedThenReopenedManagerWebViewFiresEvent Then MessageReceivedIsRaised
        /// </summary>
        [TestMethod]
        public void Given_ConnectionManager_When_ClosedThenReopenedManagerWebViewFiresEvent_Then_MessageReceivedIsRaised()
        {
            m_connectionManager.Close();
            m_connectionManager.Open();

            MessageReceivedEventArgs? capturedArgs = null;
            m_connectionManager.MessageReceived += (s, e) => capturedArgs = e;

            RaiseIncoming(BuildValidJson("CloseApp"));

            Assert.IsNotNull(capturedArgs);
        }

        // ── Outbound: SendMessage(OutboundMessage) ───────────────────────────────

        /// <summary>
    /// Given ConnectionManager When ValidOutboundMessageSendMessageCalled Then WebViewSendMessageIsCalledOnce
        /// </summary>
        [TestMethod]
        public void Given_ConnectionManager_When_ValidOutboundMessageSendMessageCalled_Then_WebViewSendMessageIsCalledOnce()
        {
            m_connectionManager.SendMessage(new TestOutboundMessage { Status = "Success" });

            m_mockWebView.Verify(x => x.SendMessage(It.IsAny<string>()), Times.Once());
        }

        /// <summary>
    /// Given ConnectionManager When ValidOutboundMessageSendMessageCalled Then SerializedJsonContainsAction
        /// </summary>
        [TestMethod]
        public void Given_ConnectionManager_When_ValidOutboundMessageSendMessageCalled_Then_SerializedJsonContainsAction()
        {
            m_connectionManager.SendMessage(new TestOutboundMessage { Status = "Success" });

            m_mockWebView.Verify(
                x => x.SendMessage(It.Is<string>(s => s.Contains("TestAction"))),
                Times.Once());
        }

        /// <summary>
    /// Given ConnectionManager When ValidOutboundMessageSendMessageCalled Then SerializedJsonContainsPayloadField
        /// </summary>
        [TestMethod]
        public void Given_ConnectionManager_When_ValidOutboundMessageSendMessageCalled_Then_SerializedJsonContainsPayloadField()
        {
            m_connectionManager.SendMessage(new TestOutboundMessage { Status = "Success" });

            m_mockWebView.Verify(
                x => x.SendMessage(It.Is<string>(s => s.Contains("Success"))),
                Times.Once());
        }

        /// <summary>
    /// Given ConnectionManager When NullMessageSendMessageCalled Then WebViewSendMessageIsNotCalled
        /// </summary>
        [TestMethod]
        public void Given_ConnectionManager_When_NullMessageSendMessageCalled_Then_WebViewSendMessageIsNotCalled()
        {
            m_connectionManager.SendMessage(null!);

            m_mockWebView.Verify(x => x.SendMessage(It.IsAny<string>()), Times.Never());
        }

        /// <summary>
    /// Given ConnectionManager When NullMessageSendMessageCalled Then LogWarnIsCalled
        /// </summary>
        [TestMethod]
        public void Given_ConnectionManager_When_NullMessageSendMessageCalled_Then_LogWarnIsCalled()
        {
            m_connectionManager.SendMessage(null!);

            m_mockLogger.Verify(x => x.LogWarn(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        // ── Outbound: serialization failure in SendMessage ─────────────────────────

        /// <summary>
    /// Given ConnectionManager When UnserializableMessageSendMessageCalled Then LogErrorIsCalled
        /// </summary>
        [TestMethod]
        public void Given_ConnectionManager_When_UnserializableMessageSendMessageCalled_Then_LogErrorIsCalled()
        {
            m_connectionManager.SendMessage(new BadSerializationMessage());

            m_mockLogger.Verify(
                x => x.LogError(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                Times.Once());
        }

        /// <summary>
    /// Given ConnectionManager When UnserializableMessageSendMessageCalled Then WebViewSendMessageIsNotCalled
        /// </summary>
        [TestMethod]
        public void Given_ConnectionManager_When_UnserializableMessageSendMessageCalled_Then_WebViewSendMessageIsNotCalled()
        {
            m_connectionManager.SendMessage(new BadSerializationMessage());

            m_mockWebView.Verify(x => x.SendMessage(It.IsAny<string>()), Times.Never());
        }

        // ── Helpers ───────────────────────────────────────────────────────────────

        private void RaiseIncoming(string json)
        {
            m_mockWebView.Raise(x => x.OnMessageReceived += null, this, json);
        }

        private static string BuildValidJson(string action)
            => $"{{\"CallContext\":{{\"Action\":\"{action}\"}},\"Payload\":{{}}}}";

        private static string BuildJsonWithPayload(string action, string payloadFields)
            => $"{{\"CallContext\":{{\"Action\":\"{action}\"}},\"Payload\":{{{payloadFields}}}}}";

        /// <summary>
        /// Minimal concrete <see cref="OutboundMessage"/> stub used only within these unit tests.
        /// </summary>
        private sealed class TestOutboundMessage : OutboundMessage
        {
            public override string Action => "TestAction";
            public string Status { get; set; } = string.Empty;
        }

        /// <summary>
        /// Stub whose <c>UnsupportedProperty</c> has a type (<see cref="Func{TResult}"/>) that
        /// <see cref="System.Text.Json.JsonSerializer"/> cannot serialize, triggering
        /// <see cref="NotSupportedException"/> and exercising the serialization-error catch
        /// branch in <see cref="ConnectionManager.SendMessage"/>.
        /// </summary>
        private sealed class BadSerializationMessage : OutboundMessage
        {
            public override string Action => "BadAction";
            public Func<int>? UnsupportedProperty { get; set; } = () => 0;
        }
    }
}


