#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : Pms_GuidanceGUI.Tests
// File   : JsonWriterManagerTests.cs
// Description: Unit tests for JsonWriterManager. All external dependencies
//              are mocked with Moq. All tests follow the Given/When/Then convention.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – compliance with C# Coding Guidelines V2.1
//--------------------------------------------------------------------
#endregion

using System;
using BusinessLogicModule.EventArgs;
using ConverterModule;
using ConverterModule.JsonWriter;
using Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Pms_GuidanceGUI.Tests.Unit
{
    /// <summary>
    /// Unit tests for <see cref="JsonWriterManager"/>.
    /// All external dependencies are mocked with Moq.
    /// All tests follow the Given/When/Then naming convention.
    /// </summary>
    [TestClass]
    public class JsonWriterManagerTests
    {
        #region Private Members

        private Mock<ILogger>    m_mockLogger = null!;
        private JsonWriterManager m_manager   = null!;

        #endregion

        /// <summary>
        /// Initializes the test context before each test method runs.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            m_mockLogger = new Mock<ILogger>();
            m_manager    = new JsonWriterManager(m_mockLogger.Object);
        }

        // ── AddJsonWriter: null guard ─────────────────────────────────────────────

        /// <summary>
    /// Given JsonWriterManager When NullWriterAddJsonWriterCalled Then LogWarnIsCalled
        /// </summary>
        [TestMethod]
        public void Given_JsonWriterManager_When_NullWriterAddJsonWriterCalled_Then_LogWarnIsCalled()
        {
            m_manager.AddJsonWriter(null!);

            m_mockLogger.Verify(x => x.LogWarn(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        /// <summary>
    /// Given JsonWriterManager When NullWriterAddJsonWriterCalled Then WriterIsNotAdded
        /// </summary>
        [TestMethod]
        public void Given_JsonWriterManager_When_NullWriterAddJsonWriterCalled_Then_WriterIsNotAdded()
        {
            m_manager.AddJsonWriter(null!);

            // Nothing registered — HandleJsonReply should return null for any event.
            var result = m_manager.HandleJsonReply(EventArgs.Empty);
            Assert.IsNull(result);
        }

        // ── AddJsonWriter: duplicate guard ────────────────────────────────────────

        /// <summary>
    /// Given JsonWriterManager When AlreadyRegisteredWriterAddJsonWriterCalledAgain Then LogWarnIsCalled
        /// </summary>
        [TestMethod]
        public void Given_JsonWriterManager_When_AlreadyRegisteredWriterAddJsonWriterCalledAgain_Then_LogWarnIsCalled()
        {
            var mockWriter = CreateWriter(canWrite: true);

            m_manager.AddJsonWriter(mockWriter.Object);
            m_manager.AddJsonWriter(mockWriter.Object);

            m_mockLogger.Verify(x => x.LogWarn(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        /// <summary>
    /// Given JsonWriterManager When AlreadyRegisteredWriterAddJsonWriterCalledAgain Then WriterIsNotDuplicated
        /// </summary>
        [TestMethod]
        public void Given_JsonWriterManager_When_AlreadyRegisteredWriterAddJsonWriterCalledAgain_Then_WriterIsNotDuplicated()
        {
            var mockWriter = CreateWriter(canWrite: true);

            m_manager.AddJsonWriter(mockWriter.Object);
            m_manager.AddJsonWriter(mockWriter.Object);

            m_manager.HandleJsonReply(EventArgs.Empty);

            // CreateJsonMessage must be called exactly once — no duplicate dispatch.
            mockWriter.Verify(w => w.CreateJsonMessage(It.IsAny<EventArgs>()), Times.Once());
        }

        // ── AddJsonWriter: happy path ─────────────────────────────────────────────

        /// <summary>
    /// Given JsonWriterManager When ValidNewWriterAddJsonWriterCalled Then NoLogWarnIsCalled
        /// </summary>
        [TestMethod]
        public void Given_JsonWriterManager_When_ValidNewWriterAddJsonWriterCalled_Then_NoLogWarnIsCalled()
        {
            m_manager.AddJsonWriter(CreateWriter(canWrite: false).Object);

            m_mockLogger.Verify(x => x.LogWarn(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
        }

        /// <summary>
    /// Given JsonWriterManager When ValidNewWriterAddJsonWriterCalled Then WriterIsDispatched
        /// </summary>
        [TestMethod]
        public void Given_JsonWriterManager_When_ValidNewWriterAddJsonWriterCalled_Then_WriterIsDispatched()
        {
            var mockWriter = CreateWriter(canWrite: true);

            m_manager.AddJsonWriter(mockWriter.Object);
            m_manager.HandleJsonReply(EventArgs.Empty);

            mockWriter.Verify(w => w.CreateJsonMessage(It.IsAny<EventArgs>()), Times.Once());
        }

        // ── RemoveJsonWriter: null guard ──────────────────────────────────────────

        /// <summary>
    /// Given JsonWriterManager When NullWriterRemoveJsonWriterCalled Then LogWarnIsCalled
        /// </summary>
        [TestMethod]
        public void Given_JsonWriterManager_When_NullWriterRemoveJsonWriterCalled_Then_LogWarnIsCalled()
        {
            m_manager.RemoveJsonWriter(null!);

            m_mockLogger.Verify(x => x.LogWarn(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        // ── RemoveJsonWriter: not-registered guard ────────────────────────────────

        /// <summary>
    /// Given JsonWriterManager When UnregisteredWriterRemoveJsonWriterCalled Then LogWarnIsCalled
        /// </summary>
        [TestMethod]
        public void Given_JsonWriterManager_When_UnregisteredWriterRemoveJsonWriterCalled_Then_LogWarnIsCalled()
        {
            var mockWriter = CreateWriter(canWrite: true);

            m_manager.RemoveJsonWriter(mockWriter.Object);

            m_mockLogger.Verify(x => x.LogWarn(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        // ── RemoveJsonWriter: happy path ──────────────────────────────────────────

        /// <summary>
    /// Given JsonWriterManager When RegisteredWriterRemoveJsonWriterCalled Then NoLogWarnIsCalled
        /// </summary>
        [TestMethod]
        public void Given_JsonWriterManager_When_RegisteredWriterRemoveJsonWriterCalled_Then_NoLogWarnIsCalled()
        {
            var mockWriter = CreateWriter(canWrite: true);

            m_manager.AddJsonWriter(mockWriter.Object);
            m_manager.RemoveJsonWriter(mockWriter.Object);

            m_mockLogger.Verify(x => x.LogWarn(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
        }

        /// <summary>
    /// Given JsonWriterManager When RegisteredWriterRemovedAndEventReceived Then WriterIsNotDispatched
        /// </summary>
        [TestMethod]
        public void Given_JsonWriterManager_When_RegisteredWriterRemovedAndEventReceived_Then_WriterIsNotDispatched()
        {
            var mockWriter = CreateWriter(canWrite: true);

            m_manager.AddJsonWriter(mockWriter.Object);
            m_manager.RemoveJsonWriter(mockWriter.Object);

            m_manager.HandleJsonReply(EventArgs.Empty);

            mockWriter.Verify(w => w.CreateJsonMessage(It.IsAny<EventArgs>()), Times.Never());
        }

        // ── HandleJsonReply: no match ─────────────────────────────────────────────

        /// <summary>
    /// Given JsonWriterManager When NoRegisteredWritersHandleJsonReplyCalled Then ReturnsNull
        /// </summary>
        [TestMethod]
        public void Given_JsonWriterManager_When_NoRegisteredWritersHandleJsonReplyCalled_Then_ReturnsNull()
        {
            var result = m_manager.HandleJsonReply(EventArgs.Empty);

            Assert.IsNull(result);
        }

        /// <summary>
    /// Given JsonWriterManager When WriterThatCannotWriteHandleJsonReplyCalled Then ReturnsNull
        /// </summary>
        [TestMethod]
        public void Given_JsonWriterManager_When_WriterThatCannotWriteHandleJsonReplyCalled_Then_ReturnsNull()
        {
            m_manager.AddJsonWriter(CreateWriter(canWrite: false).Object);

            var result = m_manager.HandleJsonReply(EventArgs.Empty);

            Assert.IsNull(result);
        }

        // ── HandleJsonReply: match ────────────────────────────────────────────────

        /// <summary>
    /// Given JsonWriterManager When MatchingWriterHandleJsonReplyCalled Then ReturnsOutboundMessage
        /// </summary>
        [TestMethod]
        public void Given_JsonWriterManager_When_MatchingWriterHandleJsonReplyCalled_Then_ReturnsOutboundMessage()
        {
            var expected   = new StubOutboundMessage();
            var mockWriter = CreateWriter(canWrite: true, reply: expected);

            m_manager.AddJsonWriter(mockWriter.Object);
            var result = m_manager.HandleJsonReply(EventArgs.Empty);

            Assert.IsNotNull(result);
            Assert.AreEqual("TestAction", result.Action);
        }

        /// <summary>
    /// Given JsonWriterManager When MultipleWritersFirstCanWrite Then OnlyFirstWriterIsDispatched
        /// </summary>
        [TestMethod]
        public void Given_JsonWriterManager_When_MultipleWritersFirstCanWrite_Then_OnlyFirstWriterIsDispatched()
        {
            var first  = CreateWriter(canWrite: true);
            var second = CreateWriter(canWrite: true);

            m_manager.AddJsonWriter(first.Object);
            m_manager.AddJsonWriter(second.Object);

            m_manager.HandleJsonReply(EventArgs.Empty);

            first.Verify(w => w.CreateJsonMessage(It.IsAny<EventArgs>()), Times.Once());
            second.Verify(w => w.CreateJsonMessage(It.IsAny<EventArgs>()), Times.Never());
        }

        /// <summary>
    /// Given JsonWriterManager When MultipleWritersOnlySecondCanWrite Then SecondWriterIsDispatched
        /// </summary>
        [TestMethod]
        public void Given_JsonWriterManager_When_MultipleWritersOnlySecondCanWrite_Then_SecondWriterIsDispatched()
        {
            var first  = CreateWriter(canWrite: false);
            var second = CreateWriter(canWrite: true);

            m_manager.AddJsonWriter(first.Object);
            m_manager.AddJsonWriter(second.Object);

            m_manager.HandleJsonReply(EventArgs.Empty);

            first.Verify(w => w.CreateJsonMessage(It.IsAny<EventArgs>()), Times.Never());
            second.Verify(w => w.CreateJsonMessage(It.IsAny<EventArgs>()), Times.Once());
        }

        // ── Helpers ───────────────────────────────────────────────────────────────

        /// <summary>Minimal concrete <see cref="OutboundMessage"/> for use in tests.</summary>
        private sealed class StubOutboundMessage : OutboundMessage
        {
            public override string Action => "TestAction";
        }

        private static Mock<IJsonWriter> CreateWriter(bool canWrite, OutboundMessage? reply = null)
        {
            var mock = new Mock<IJsonWriter>();
            mock.Setup(w => w.CanWrite(It.IsAny<EventArgs>())).Returns(canWrite);
            mock.Setup(w => w.CreateJsonMessage(It.IsAny<EventArgs>()))
                .Returns(reply ?? new StubOutboundMessage());
            return mock;
        }
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // Concrete internal JSON writer tests
    // Exercises CanWrite / CreateJsonMessage on the real writer implementations.
    // ─────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Direct unit tests for the concrete internal <see cref="IJsonWriter"/> implementations.
    /// Tests the correct-type happy path and the wrong-type <see cref="ArgumentException"/> path.
    /// </summary>
    [TestClass]
    public class ConcreteJsonWriterTests
    {
        // ── ShowVerifyInstallationPrerequisitesJsonWriter ──────────────────────────

        /// <summary>
    /// Given ConcreteJsonWriter When ShowVerifyPrerequisitesJsonWriterCanWriteCalledWithCorrectType Then ReturnsTrue
        /// </summary>
        [TestMethod]
        public void Given_ConcreteJsonWriter_When_ShowVerifyPrerequisitesJsonWriterCanWriteCalledWithCorrectType_Then_ReturnsTrue()
        {
            var writer = new ShowVerifyInstallationPrerequisitesJsonWriter(typeof(VerifyInstallationPrerequisitesStatusEventArgs));
            Assert.IsTrue(writer.CanWrite(new VerifyInstallationPrerequisitesStatusEventArgs(true)));
        }

        /// <summary>
    /// Given ConcreteJsonWriter When ShowVerifyPrerequisitesJsonWriterCanWriteCalledWithWrongType Then ReturnsFalse
        /// </summary>
        [TestMethod]
        public void Given_ConcreteJsonWriter_When_ShowVerifyPrerequisitesJsonWriterCanWriteCalledWithWrongType_Then_ReturnsFalse()
        {
            var writer = new ShowVerifyInstallationPrerequisitesJsonWriter(typeof(VerifyInstallationPrerequisitesStatusEventArgs));
            Assert.IsFalse(writer.CanWrite(EventArgs.Empty));
        }

        /// <summary>
    /// Given ConcreteJsonWriter When ShowVerifyPrerequisitesJsonWriterCreateJsonMessageCalledWithCorrectType Then StatusIsSet
        /// </summary>
        [TestMethod]
        public void Given_ConcreteJsonWriter_When_ShowVerifyPrerequisitesJsonWriterCreateJsonMessageCalledWithCorrectType_Then_StatusIsSet()
        {
            var writer = new ShowVerifyInstallationPrerequisitesJsonWriter(typeof(VerifyInstallationPrerequisitesStatusEventArgs));
            var result = writer.CreateJsonMessage(new VerifyInstallationPrerequisitesStatusEventArgs(false));
            Assert.AreEqual("Not Ok", result.GetType().GetProperty("Status")?.GetValue(result));
        }

        /// <summary>
    /// Given ConcreteJsonWriter When ShowVerifyPrerequisitesJsonWriterCreateJsonMessageCalledWithWrongType Then ThrowsArgumentException
        /// </summary>
        [TestMethod]
        public void Given_ConcreteJsonWriter_When_ShowVerifyPrerequisitesJsonWriterCreateJsonMessageCalledWithWrongType_Then_ThrowsArgumentException()
        {
            var writer = new ShowVerifyInstallationPrerequisitesJsonWriter(typeof(VerifyInstallationPrerequisitesStatusEventArgs));
            Assert.ThrowsException<ArgumentException>(() => writer.CreateJsonMessage(EventArgs.Empty));
        }

        // ── ShowSystemLanguageJsonWriter ──────────────────────────────────────────

        /// <summary>
    /// Given ConcreteJsonWriter When ShowSystemLanguageJsonWriterCanWriteCalledWithCorrectType Then ReturnsTrue
        /// </summary>
        [TestMethod]
        public void Given_ConcreteJsonWriter_When_ShowSystemLanguageJsonWriterCanWriteCalledWithCorrectType_Then_ReturnsTrue()
        {
            var writer = new ShowSystemLanguageJsonWriter(typeof(ShowSystemLanguageEventArgs));
            Assert.IsTrue(writer.CanWrite(new ShowSystemLanguageEventArgs("en-US")));
        }

        /// <summary>
    /// Given ConcreteJsonWriter When ShowSystemLanguageJsonWriterCanWriteCalledWithWrongType Then ReturnsFalse
        /// </summary>
        [TestMethod]
        public void Given_ConcreteJsonWriter_When_ShowSystemLanguageJsonWriterCanWriteCalledWithWrongType_Then_ReturnsFalse()
        {
            var writer = new ShowSystemLanguageJsonWriter(typeof(ShowSystemLanguageEventArgs));
            Assert.IsFalse(writer.CanWrite(EventArgs.Empty));
        }

        /// <summary>
    /// Given ConcreteJsonWriter When ShowSystemLanguageJsonWriterCreateJsonMessageCalledWithCorrectType Then LanguageIsSet
        /// </summary>
        [TestMethod]
        public void Given_ConcreteJsonWriter_When_ShowSystemLanguageJsonWriterCreateJsonMessageCalledWithCorrectType_Then_LanguageIsSet()
        {
            var writer = new ShowSystemLanguageJsonWriter(typeof(ShowSystemLanguageEventArgs));
            var result = writer.CreateJsonMessage(new ShowSystemLanguageEventArgs("fr-FR"));
            Assert.AreEqual("fr-FR", result.GetType().GetProperty("Language")?.GetValue(result));
        }

        /// <summary>
    /// Given ConcreteJsonWriter When ShowSystemLanguageJsonWriterCreateJsonMessageCalledWithWrongType Then ThrowsArgumentException
        /// </summary>
        [TestMethod]
        public void Given_ConcreteJsonWriter_When_ShowSystemLanguageJsonWriterCreateJsonMessageCalledWithWrongType_Then_ThrowsArgumentException()
        {
            var writer = new ShowSystemLanguageJsonWriter(typeof(ShowSystemLanguageEventArgs));
            Assert.ThrowsException<ArgumentException>(() => writer.CreateJsonMessage(EventArgs.Empty));
        }
    }
}

