#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : Pms_GuidanceGUI.Tests
// File   : JsonActionHandlerManagerTests.cs
// Description: Unit tests for JsonActionHandlerManager. All external dependencies
//              are mocked with Moq. All tests follow the Given/When/Then convention.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – compliance with C# Coding Guidelines V2.1
//--------------------------------------------------------------------
#endregion

using BusinessLogicModule.Commands;
using ConverterModule;
using Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Pms_GuidanceGUI.Tests.Unit
{
    /// <summary>
    /// Unit tests for <see cref="JsonActionHandlerManager"/>.
    /// All external dependencies are mocked with Moq.
    /// All tests follow the Given/When/Then naming convention.
    /// </summary>
    [TestClass]
    public class JsonActionHandlerManagerTests
    {
        #region Private Members

        private Mock<ILogger>             m_mockLogger  = null!;
        private JsonActionHandlerManager  m_manager     = null!;

        #endregion

        /// <summary>
        /// Initializes the test context before each test method runs.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            m_mockLogger = new Mock<ILogger>();
            m_manager    = new JsonActionHandlerManager(m_mockLogger.Object);
        }

        // ── AddJsonActionHandler: null guard ──────────────────────────────────────

        /// <summary>
    /// Given JsonActionHandlerManager When NullHandlerAddJsonActionHandlerCalled Then LogWarnIsCalled
        /// </summary>
        [TestMethod]
        public void Given_JsonActionHandlerManager_When_NullHandlerAddJsonActionHandlerCalled_Then_LogWarnIsCalled()
        {
            m_manager.AddJsonActionHandler(null!);

            m_mockLogger.Verify(x => x.LogWarn(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        /// <summary>
    /// Given JsonActionHandlerManager When NullHandlerAddJsonActionHandlerCalled Then HandlerIsNotAdded
        /// </summary>
        [TestMethod]
        public void Given_JsonActionHandlerManager_When_NullHandlerAddJsonActionHandlerCalled_Then_HandlerIsNotAdded()
        {
            m_manager.AddJsonActionHandler(null!);

            // Verify nothing is registered — HandleJsonAction should return null for any action.
            var result = m_manager.HandleJsonAction("AnyAction", "{}");
            Assert.IsNull(result);
        }

        // ── AddJsonActionHandler: duplicate guard ─────────────────────────────────

        /// <summary>
    /// Given JsonActionHandlerManager When AlreadyRegisteredHandlerAddJsonActionHandlerCalledAgain Then LogWarnIsCalled
        /// </summary>
        [TestMethod]
        public void Given_JsonActionHandlerManager_When_AlreadyRegisteredHandlerAddJsonActionHandlerCalledAgain_Then_LogWarnIsCalled()
        {
            var mockHandler = CreateHandler("TestAction");

            m_manager.AddJsonActionHandler(mockHandler.Object);
            m_manager.AddJsonActionHandler(mockHandler.Object);

            m_mockLogger.Verify(x => x.LogWarn(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        /// <summary>
    /// Given JsonActionHandlerManager When AlreadyRegisteredHandlerAddJsonActionHandlerCalledAgain Then HandlerIsNotDuplicated
        /// </summary>
        [TestMethod]
        public void Given_JsonActionHandlerManager_When_AlreadyRegisteredHandlerAddJsonActionHandlerCalledAgain_Then_HandlerIsNotDuplicated()
        {
            var mockHandler = CreateHandler("DupAction");
            mockHandler.Setup(h => h.HandleAction(It.IsAny<string>())).Returns((ICommand?)null);

            m_manager.AddJsonActionHandler(mockHandler.Object);
            m_manager.AddJsonActionHandler(mockHandler.Object);

            m_manager.HandleJsonAction("DupAction", "{}");

            // HandleAction must be called exactly once — no duplicate dispatch.
            mockHandler.Verify(h => h.HandleAction(It.IsAny<string>()), Times.Once());
        }

        // ── AddJsonActionHandler: happy path ──────────────────────────────────────

        /// <summary>
    /// Given JsonActionHandlerManager When ValidNewHandlerAddJsonActionHandlerCalled Then NoLogWarnIsCalled
        /// </summary>
        [TestMethod]
        public void Given_JsonActionHandlerManager_When_ValidNewHandlerAddJsonActionHandlerCalled_Then_NoLogWarnIsCalled()
        {
            var mockHandler = CreateHandler("SomeAction");

            m_manager.AddJsonActionHandler(mockHandler.Object);

            m_mockLogger.Verify(x => x.LogWarn(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
        }

        /// <summary>
    /// Given JsonActionHandlerManager When ValidNewHandlerAddJsonActionHandlerCalled Then HandlerIsDispatched
        /// </summary>
        [TestMethod]
        public void Given_JsonActionHandlerManager_When_ValidNewHandlerAddJsonActionHandlerCalled_Then_HandlerIsDispatched()
        {
            var mockHandler = CreateHandler("SomeAction");
            mockHandler.Setup(h => h.HandleAction(It.IsAny<string>())).Returns((ICommand?)null);

            m_manager.AddJsonActionHandler(mockHandler.Object);
            m_manager.HandleJsonAction("SomeAction", "{}");

            mockHandler.Verify(h => h.HandleAction(It.IsAny<string>()), Times.Once());
        }

        // ── RemoveJsonActionHandler: null guard ───────────────────────────────────

        /// <summary>
    /// Given JsonActionHandlerManager When NullHandlerRemoveJsonActionHandlerCalled Then LogWarnIsCalled
        /// </summary>
        [TestMethod]
        public void Given_JsonActionHandlerManager_When_NullHandlerRemoveJsonActionHandlerCalled_Then_LogWarnIsCalled()
        {
            m_manager.RemoveJsonActionHandler(null!);

            m_mockLogger.Verify(x => x.LogWarn(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        // ── RemoveJsonActionHandler: not-registered guard ─────────────────────────

        /// <summary>
    /// Given JsonActionHandlerManager When UnregisteredHandlerRemoveJsonActionHandlerCalled Then LogWarnIsCalled
        /// </summary>
        [TestMethod]
        public void Given_JsonActionHandlerManager_When_UnregisteredHandlerRemoveJsonActionHandlerCalled_Then_LogWarnIsCalled()
        {
            var mockHandler = CreateHandler("GhostAction");

            m_manager.RemoveJsonActionHandler(mockHandler.Object);

            m_mockLogger.Verify(x => x.LogWarn(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        // ── RemoveJsonActionHandler: happy path ───────────────────────────────────

        /// <summary>
    /// Given JsonActionHandlerManager When RegisteredHandlerRemoveJsonActionHandlerCalled Then NoLogWarnIsCalled
        /// </summary>
        [TestMethod]
        public void Given_JsonActionHandlerManager_When_RegisteredHandlerRemoveJsonActionHandlerCalled_Then_NoLogWarnIsCalled()
        {
            var mockHandler = CreateHandler("RemoveMe");

            m_manager.AddJsonActionHandler(mockHandler.Object);
            m_manager.RemoveJsonActionHandler(mockHandler.Object);

            m_mockLogger.Verify(x => x.LogWarn(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
        }

        /// <summary>
    /// Given JsonActionHandlerManager When RegisteredHandlerRemovedAndActionReceived Then HandlerIsNotDispatched
        /// </summary>
        [TestMethod]
        public void Given_JsonActionHandlerManager_When_RegisteredHandlerRemovedAndActionReceived_Then_HandlerIsNotDispatched()
        {
            var mockHandler = CreateHandler("RemoveMe");
            mockHandler.Setup(h => h.HandleAction(It.IsAny<string>())).Returns((ICommand?)null);

            m_manager.AddJsonActionHandler(mockHandler.Object);
            m_manager.RemoveJsonActionHandler(mockHandler.Object);

            m_manager.HandleJsonAction("RemoveMe", "{}");

            mockHandler.Verify(h => h.HandleAction(It.IsAny<string>()), Times.Never());
        }

        // ── Helpers ───────────────────────────────────────────────────────────────

        private static Mock<IJsonActionHandler> CreateHandler(string actionName)
        {
            var mock = new Mock<IJsonActionHandler>();
            mock.Setup(h => h.ActionName).Returns(actionName);
            return mock;
        }
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // Concrete internal JSON action handler tests
    // Exercises UIAppStartedJsonActionHandler directly and the
    // AbstractJsonActionHandler.DeserializeMessage null-guard branch.
    // ─────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Direct unit tests for <see cref="UIAppStartedJsonActionHandler"/> and
    /// the null-result guard in <see cref="AbstractJsonActionHandler.DeserializeMessage{T}"/>.
    /// </summary>
    [TestClass]
    public class ConcreteJsonActionHandlerTests
    {
        private Mock<ILogger> m_mockLogger = null!;

        /// <summary>
        /// Initializes the test context before each test method runs.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            m_mockLogger = new Mock<ILogger>();
        }

        /// <summary>
    /// Given ConcreteJsonActionHandler When UIAppStartedHandlerHandleActionCalledWithValidJson Then ReturnsUIAppStartedCommand
        /// </summary>
        [TestMethod]
        public void Given_ConcreteJsonActionHandler_When_UIAppStartedHandlerHandleActionCalledWithValidJson_Then_ReturnsUIAppStartedCommand()
        {
            var handler = new UIAppStartedJsonActionHandler(m_mockLogger.Object);
            var result  = handler.HandleAction("{}");
            Assert.IsInstanceOfType(result, typeof(UIAppStartedCommand));
        }

        /// <summary>
    /// Given ConcreteJsonActionHandler When UIAppStartedHandlerHandleActionCalledWithMalformedJson Then ReturnsNull
        /// </summary>
        [TestMethod]
        public void Given_ConcreteJsonActionHandler_When_UIAppStartedHandlerHandleActionCalledWithMalformedJson_Then_ReturnsNull()
        {
            var handler = new UIAppStartedJsonActionHandler(m_mockLogger.Object);
            var result  = handler.HandleAction("NOT_JSON");
            Assert.IsNull(result);
        }

        /// <summary>
    /// Given ConcreteJsonActionHandler When UIAppStartedHandlerHandleActionCalledWithMalformedJson Then LogErrorIsCalled
        /// </summary>
        [TestMethod]
        public void Given_ConcreteJsonActionHandler_When_UIAppStartedHandlerHandleActionCalledWithMalformedJson_Then_LogErrorIsCalled()
        {
            var handler = new UIAppStartedJsonActionHandler(m_mockLogger.Object);
            handler.HandleAction("NOT_JSON");
            m_mockLogger.Verify(
                x => x.LogError(It.IsAny<string>(), It.IsAny<System.Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                Times.Once());
        }

        /// <summary>
    /// Given ConcreteJsonActionHandler When UIAppStartedHandlerActionNameAccessed Then ReturnsUIAppStarted
        /// </summary>
        [TestMethod]
        public void Given_ConcreteJsonActionHandler_When_UIAppStartedHandlerActionNameAccessed_Then_ReturnsUIAppStarted()
        {
            var handler = new UIAppStartedJsonActionHandler(m_mockLogger.Object);
            Assert.AreEqual("UIAppStarted", handler.ActionName);
        }

        /// <summary>
        /// Given UIAppStartedHandler When HandleActionCalledWithNullJson Then ReturnsNull
        /// Exercises AbstractJsonActionHandler.DeserializeMessage null-guard (lines 83-85):
        /// deserializing the JSON literal "null" returns null, which the guard rejects.
        /// </summary>
        [TestMethod]
        public void Given_ConcreteJsonActionHandler_When_UIAppStartedHandlerHandleActionCalledWithNullJson_Then_ReturnsNull()
        {
            var handler = new UIAppStartedJsonActionHandler(m_mockLogger.Object);
            var result = handler.HandleAction("null");

            Assert.IsNull(result);
            m_mockLogger.Verify(
                x => x.LogError(It.IsAny<string>(), It.IsAny<System.Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
                Times.Once());
        }
    }
}

