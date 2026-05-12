#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : Pms_GuidanceGUI.Tests
// File   : ApplicationLifecycleManagerTests.cs
// Description: Unit tests for ApplicationLifecycleManager. All dependencies are
//              mocked with Moq. Tests verify the ordered-open, reverse-ordered-close,
//              idempotency, and guard-clause behaviour of the lifecycle manager.
//              All tests follow the Given/When/Then naming convention.
// Notes:
// Modification History : Code Review, 06-May-2026, Added unit tests for ApplicationLifecycleManager,
//                        covering startup order, shutdown order, idempotency, and guard clauses.
//                        Code Review, 07-May-2026, Added tests for duplicate registration guard
//                        and aggregated startup-plus-rollback failure handling.
//--------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Pms_GuidanceGUI.Tests.Unit
{
    /// <summary>
    /// Unit tests for <see cref="ApplicationLifecycleManager"/>.
    /// All <see cref="ILifeCycle"/> components are mocked with Moq.
    /// All tests follow the Given/When/Then naming convention.
    /// </summary>
    [TestClass]
    public class ApplicationLifecycleManagerTests
    {
        // ── Register: guard clauses ───────────────────────────────────────────────

        /// <summary>
    /// Given ApplicationLifecycleManager NullComponent When RegisterCalled Then ThrowsArgumentNullException
        /// </summary>
        [TestMethod]
        public void Given_ApplicationLifecycleManager_NullComponent_When_RegisterCalled_Then_ThrowsArgumentNullException()
        {
            var manager = new ApplicationLifecycleManager();

            Assert.ThrowsException<ArgumentNullException>(
                () => manager.Register(null!));
        }

        /// <summary>
    /// Given ApplicationLifecycleManager OpenAlreadyCalled When RegisterCalled Then ThrowsInvalidOperationException
        /// </summary>
        [TestMethod]
        public void Given_ApplicationLifecycleManager_OpenAlreadyCalled_When_RegisterCalled_Then_ThrowsInvalidOperationException()
        {
            var manager   = new ApplicationLifecycleManager();
            var component = new Mock<ILifeCycle>();
            manager.Register(component.Object);
            manager.Open();

            Assert.ThrowsException<InvalidOperationException>(
                () => manager.Register(new Mock<ILifeCycle>().Object));
        }

        /// <summary>
    /// Given ApplicationLifecycleManager SameComponentRegisteredTwice When RegisterCalled Then ThrowsInvalidOperationException
        /// </summary>
        [TestMethod]
        public void Given_ApplicationLifecycleManager_SameComponentRegisteredTwice_When_RegisterCalled_Then_ThrowsInvalidOperationException()
        {
            var manager   = new ApplicationLifecycleManager();
            var component = new Mock<ILifeCycle>();
            manager.Register(component.Object);

            Assert.ThrowsException<InvalidOperationException>(
                () => manager.Register(component.Object));
        }

        // ── Open: ordering ────────────────────────────────────────────────────────

        /// <summary>
    /// Given ApplicationLifecycleManager ThreeComponents When OpenCalled Then ComponentsAreOpenedInRegistrationOrder
        /// </summary>
        [TestMethod]
        public void Given_ApplicationLifecycleManager_ThreeComponents_When_OpenCalled_Then_ComponentsAreOpenedInRegistrationOrder()
        {
            var manager         = new ApplicationLifecycleManager();
            var callOrder       = new List<int>();
            var firstComponent  = CreateOrderedComponent(callOrder, 1);
            var secondComponent = CreateOrderedComponent(callOrder, 2);
            var thirdComponent  = CreateOrderedComponent(callOrder, 3);

            manager.Register(firstComponent.Object);
            manager.Register(secondComponent.Object);
            manager.Register(thirdComponent.Object);

            manager.Open();

            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, callOrder);
        }

        /// <summary>
    /// Given ApplicationLifecycleManager EmptyManager When OpenCalled Then NoExceptionThrown
        /// </summary>
        [TestMethod]
        public void Given_ApplicationLifecycleManager_EmptyManager_When_OpenCalled_Then_NoExceptionThrown()
        {
            var manager = new ApplicationLifecycleManager();

            // Must not throw
            manager.Open();
        }

        // ── Open: idempotency ────────────────────────────────────────────────────

        /// <summary>
    /// Given ApplicationLifecycleManager AlreadyOpenedManager When OpenCalledAgain Then ComponentsAreNotOpenedTwice
        /// </summary>
        [TestMethod]
        public void Given_ApplicationLifecycleManager_AlreadyOpenedManager_When_OpenCalledAgain_Then_ComponentsAreNotOpenedTwice()
        {
            var manager   = new ApplicationLifecycleManager();
            var component = new Mock<ILifeCycle>();
            manager.Register(component.Object);

            manager.Open();
            manager.Open();

            component.Verify(c => c.Open(), Times.Once());
        }

        // ── Close: reverse ordering ───────────────────────────────────────────────

        /// <summary>
    /// Given ApplicationLifecycleManager ThreeOpenedComponents When CloseCalled Then ComponentsAreClosedInReverseRegistrationOrder
        /// </summary>
        [TestMethod]
        public void Given_ApplicationLifecycleManager_ThreeOpenedComponents_When_CloseCalled_Then_ComponentsAreClosedInReverseRegistrationOrder()
        {
            var manager         = new ApplicationLifecycleManager();
            var callOrder       = new List<int>();
            var firstComponent  = CreateOrderedCloseComponent(callOrder, 1);
            var secondComponent = CreateOrderedCloseComponent(callOrder, 2);
            var thirdComponent  = CreateOrderedCloseComponent(callOrder, 3);

            manager.Register(firstComponent.Object);
            manager.Register(secondComponent.Object);
            manager.Register(thirdComponent.Object);
            manager.Open();

            manager.Close();

            CollectionAssert.AreEqual(new[] { 3, 2, 1 }, callOrder);
        }

        /// <summary>
    /// Given ApplicationLifecycleManager EmptyOpenedManager When CloseCalled Then NoExceptionThrown
        /// </summary>
        [TestMethod]
        public void Given_ApplicationLifecycleManager_EmptyOpenedManager_When_CloseCalled_Then_NoExceptionThrown()
        {
            var manager = new ApplicationLifecycleManager();
            manager.Open();

            // Must not throw
            manager.Close();
        }

        // ── Close: idempotency ────────────────────────────────────────────────────

        /// <summary>
    /// Given ApplicationLifecycleManager AlreadyClosedManager When CloseCalledAgain Then ComponentsAreNotClosedTwice
        /// </summary>
        [TestMethod]
        public void Given_ApplicationLifecycleManager_AlreadyClosedManager_When_CloseCalledAgain_Then_ComponentsAreNotClosedTwice()
        {
            var manager   = new ApplicationLifecycleManager();
            var component = new Mock<ILifeCycle>();
            manager.Register(component.Object);
            manager.Open();

            manager.Close();
            manager.Close();

            component.Verify(c => c.Close(), Times.Once());
        }

        /// <summary>
    /// Given ApplicationLifecycleManager NeverOpenedManager When CloseCalled Then ComponentsAreNeverClosed
        /// </summary>
        [TestMethod]
        public void Given_ApplicationLifecycleManager_NeverOpenedManager_When_CloseCalled_Then_ComponentsAreNeverClosed()
        {
            var manager   = new ApplicationLifecycleManager();
            var component = new Mock<ILifeCycle>();
            manager.Register(component.Object);

            manager.Close();

            component.Verify(c => c.Close(), Times.Never());
        }

        // ── Full lifecycle: open then close ───────────────────────────────────────

        /// <summary>
    /// Given ApplicationLifecycleManager TwoComponents When OpenThenCloseCalled Then EachComponentOpenedOnceAndClosedOnce
        /// </summary>
        [TestMethod]
        public void Given_ApplicationLifecycleManager_TwoComponents_When_OpenThenCloseCalled_Then_EachComponentOpenedOnceAndClosedOnce()
        {
            var manager          = new ApplicationLifecycleManager();
            var firstComponent   = new Mock<ILifeCycle>();
            var secondComponent  = new Mock<ILifeCycle>();

            manager.Register(firstComponent.Object);
            manager.Register(secondComponent.Object);

            manager.Open();
            manager.Close();

            firstComponent.Verify(c => c.Open(),  Times.Once());
            firstComponent.Verify(c => c.Close(), Times.Once());
            secondComponent.Verify(c => c.Open(),  Times.Once());
            secondComponent.Verify(c => c.Close(), Times.Once());
        }

        /// <summary>
    /// Given ApplicationLifecycleManager ClosedManager When OpenCalledAgain Then ComponentsCanBeReopenedAndClosed
        /// </summary>
        [TestMethod]
        public void Given_ApplicationLifecycleManager_ClosedManager_When_OpenCalledAgain_Then_ComponentsCanBeReopenedAndClosed()
        {
            var manager   = new ApplicationLifecycleManager();
            var component = new Mock<ILifeCycle>();
            manager.Register(component.Object);

            manager.Open();
            manager.Close();
            manager.Open();
            manager.Close();

            component.Verify(c => c.Open(),  Times.Exactly(2));
            component.Verify(c => c.Close(), Times.Exactly(2));
        }

        // ── Open: failure rollback ────────────────────────────────────────────────

        /// <summary>
    /// Given ApplicationLifecycleManager SecondComponentFailsOpen When OpenCalled Then FirstComponentIsRolledBackByClose
        /// </summary>
        [TestMethod]
        public void Given_ApplicationLifecycleManager_SecondComponentFailsOpen_When_OpenCalled_Then_FirstComponentIsRolledBackByClose()
        {
            var manager          = new ApplicationLifecycleManager();
            var firstComponent   = new Mock<ILifeCycle>();
            var failingComponent = new Mock<ILifeCycle>();
            failingComponent.Setup(c => c.Open()).Throws(new InvalidOperationException("open failed"));

            manager.Register(firstComponent.Object);
            manager.Register(failingComponent.Object);

            try { manager.Open(); } catch { /* expected */ }

            firstComponent.Verify(c => c.Close(), Times.Once());
        }

        /// <summary>
    /// Given ApplicationLifecycleManager ComponentFailsOpen When OpenCalled Then OriginalExceptionIsPropagated
        /// </summary>
        [TestMethod]
        public void Given_ApplicationLifecycleManager_ComponentFailsOpen_When_OpenCalled_Then_OriginalExceptionIsPropagated()
        {
            var manager          = new ApplicationLifecycleManager();
            var expected         = new InvalidOperationException("open failed");
            var failingComponent = new Mock<ILifeCycle>();
            failingComponent.Setup(c => c.Open()).Throws(expected);

            manager.Register(failingComponent.Object);

            var actual = Assert.ThrowsException<InvalidOperationException>(() => manager.Open());
            Assert.AreSame(expected, actual);
        }

        /// <summary>
    /// Given ApplicationLifecycleManager ComponentFailsOpen When OpenCalled Then ManagerIsNotMarkedOpen
        /// </summary>
        [TestMethod]
        public void Given_ApplicationLifecycleManager_ComponentFailsOpen_When_OpenCalled_Then_ManagerIsNotMarkedOpen()
        {
            var manager          = new ApplicationLifecycleManager();
            var successComponent = new Mock<ILifeCycle>();
            var failingComponent = new Mock<ILifeCycle>();
            failingComponent.Setup(c => c.Open()).Throws(new InvalidOperationException("open failed"));

            manager.Register(successComponent.Object);
            manager.Register(failingComponent.Object);
            try { manager.Open(); } catch { /* expected */ }

            // Manager is not open — explicit Close() must be a no-op (no extra Close calls beyond rollback).
            manager.Close();
            successComponent.Verify(c => c.Close(), Times.Once()); // rollback only; not called again by explicit Close
        }

        /// <summary>
    /// Given ApplicationLifecycleManager StartupAndRollbackBothFail When OpenCalled Then AggregateExceptionContainsBothFailures
        /// </summary>
        [TestMethod]
        public void Given_ApplicationLifecycleManager_StartupAndRollbackBothFail_When_OpenCalled_Then_AggregateExceptionContainsBothFailures()
        {
            var manager          = new ApplicationLifecycleManager();
            var firstComponent   = new Mock<ILifeCycle>();
            var failingComponent = new Mock<ILifeCycle>();

            firstComponent.Setup(c => c.Close()).Throws(new InvalidOperationException("rollback failed"));
            failingComponent.Setup(c => c.Open()).Throws(new InvalidOperationException("startup failed"));

            manager.Register(firstComponent.Object);
            manager.Register(failingComponent.Object);

            var ex = Assert.ThrowsException<AggregateException>(() => manager.Open());
            Assert.AreEqual(2, ex.InnerExceptions.Count);
            StringAssert.Contains(ex.InnerExceptions[0].Message, "startup failed");
            StringAssert.Contains(ex.InnerExceptions[1].Message, "rollback failed");
        }

        // ── Close: fault isolation ─────────────────────────────────────────────────

        /// <summary>
    /// Given ApplicationLifecycleManager SecondComponentFailsClose When CloseCalled Then FirstComponentIsStillClosed
        /// </summary>
        [TestMethod]
        public void Given_ApplicationLifecycleManager_SecondComponentFailsClose_When_CloseCalled_Then_FirstComponentIsStillClosed()
        {
            // Registered first → closed second (reverse order).
            // Registered second → closed first (reverse order, throws).
            var manager         = new ApplicationLifecycleManager();
            var firstComponent  = new Mock<ILifeCycle>();
            var failingComponent = new Mock<ILifeCycle>();
            failingComponent.Setup(c => c.Close()).Throws(new InvalidOperationException("close failed"));

            manager.Register(firstComponent.Object);
            manager.Register(failingComponent.Object);
            manager.Open();

            try { manager.Close(); } catch { /* expected */ }

            firstComponent.Verify(c => c.Close(), Times.Once());
        }

        /// <summary>
    /// Given ApplicationLifecycleManager ComponentFailsClose When CloseCalled Then AggregateExceptionIsThrown
        /// </summary>
        [TestMethod]
        public void Given_ApplicationLifecycleManager_ComponentFailsClose_When_CloseCalled_Then_AggregateExceptionIsThrown()
        {
            var manager          = new ApplicationLifecycleManager();
            var failingComponent = new Mock<ILifeCycle>();
            failingComponent.Setup(c => c.Close()).Throws(new InvalidOperationException("close failed"));

            manager.Register(failingComponent.Object);
            manager.Open();

            Assert.ThrowsException<AggregateException>(() => manager.Close());
        }

        /// <summary>
    /// Given ApplicationLifecycleManager TwoComponentsFailClose When CloseCalled Then AggregateExceptionContainsBothErrors
        /// </summary>
        [TestMethod]
        public void Given_ApplicationLifecycleManager_TwoComponentsFailClose_When_CloseCalled_Then_AggregateExceptionContainsBothErrors()
        {
            var manager               = new ApplicationLifecycleManager();
            var firstFailingComponent = new Mock<ILifeCycle>();
            var secondFailingComponent = new Mock<ILifeCycle>();
            firstFailingComponent.Setup(c => c.Close()).Throws(new InvalidOperationException("error 1"));
            secondFailingComponent.Setup(c => c.Close()).Throws(new InvalidOperationException("error 2"));

            manager.Register(firstFailingComponent.Object);
            manager.Register(secondFailingComponent.Object);
            manager.Open();

            var ex = Assert.ThrowsException<AggregateException>(() => manager.Close());
            Assert.AreEqual(2, ex.InnerExceptions.Count);
        }

        /// <summary>
    /// Given ApplicationLifecycleManager ComponentFailsClose When CloseCalledAgain Then SecondCallIsNoOp
        /// </summary>
        [TestMethod]
        public void Given_ApplicationLifecycleManager_ComponentFailsClose_When_CloseCalledAgain_Then_SecondCallIsNoOp()
        {
            var manager          = new ApplicationLifecycleManager();
            var failingComponent = new Mock<ILifeCycle>();
            failingComponent.Setup(c => c.Close()).Throws(new InvalidOperationException("close failed"));

            manager.Register(failingComponent.Object);
            manager.Open();

            try { manager.Close(); } catch { /* expected */ }

            // Second call must be a no-op — manager is already marked closed even after the throw.
            manager.Close();
            failingComponent.Verify(c => c.Close(), Times.Once());
        }

        // ── Helpers ───────────────────────────────────────────────────────────────

        private static Mock<ILifeCycle> CreateOrderedComponent(List<int> callOrder, int id)
        {
            var mock = new Mock<ILifeCycle>();
            mock.Setup(c => c.Open()).Callback(() => callOrder.Add(id));
            return mock;
        }

        private static Mock<ILifeCycle> CreateOrderedCloseComponent(List<int> callOrder, int id)
        {
            var mock = new Mock<ILifeCycle>();
            mock.Setup(c => c.Close()).Callback(() => callOrder.Add(id));
            return mock;
        }
    }
}
