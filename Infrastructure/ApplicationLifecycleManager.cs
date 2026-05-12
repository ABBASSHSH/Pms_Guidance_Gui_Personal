#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : Infrastructure
// File   : ApplicationLifecycleManager.cs
// Description: Orchestrates the ordered startup and shutdown of all registered
//              ILifeCycle components. Components are opened in registration order
//              on startup and closed in reverse registration order on shutdown,
//              ensuring every event subscription made during Open() is undone
//              by the matching Close() call.
// Notes:
// Modification History : Code Review, 06-May-2026, Introduced ApplicationLifecycleManager
//                        to provide a controlled startup and shutdown strategy with
//                        proper event subscription and unsubscription handling.
//                        Code Review, 07-May-2026, Added duplicate registration guard
//                        and improved startup rollback diagnostics.
//--------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;

namespace Infrastructure
{
    /// <summary>
    /// Orchestrates the ordered startup and shutdown of all registered
    /// <see cref="ILifeCycle"/> components.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Components are opened in the order they were registered and closed in the
    /// reverse order. This guarantees that every event subscription established
    /// during <see cref="Open"/> is correctly unwound by the corresponding
    /// <see cref="Close"/> call.
    /// </para>
    /// <para>
    /// All components must be registered via <see cref="Register"/> before
    /// <see cref="Open"/> is called. Calling <see cref="Open"/> or
    /// <see cref="Close"/> more than once is a no-op.
    /// </para>
    /// </remarks>
    /// <reqkeys>
    /// <reqkey> REQUIREMENT_KEY </reqkey>
    /// </reqkeys>
    public sealed class ApplicationLifecycleManager : IApplicationLifecycleManager
    {
        #region Public Members

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="component"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when <see cref="Open"/> has already been called.
        /// </exception>
        public void Register(ILifeCycle component)
        {
            if (component == null)
            {
                throw new ArgumentNullException(nameof(component));
            }

            if (m_isOpen)
            {
                throw new InvalidOperationException(
                    "Components cannot be registered after Open() has been called.");
            }

            if (!m_registeredComponents.Add(component))
            {
                throw new InvalidOperationException("The same component instance cannot be registered twice.");
            }

            m_components.Add(component);
        }

        /// <inheritdoc/>
        public void SubscribeToCloseApplicationRequests(
            ICloseApplicationRequestSource requestSource,
            Action shutdownCallback)
        {
            if (requestSource == null)
            {
                throw new ArgumentNullException(nameof(requestSource));
            }

            if (shutdownCallback == null)
            {
                throw new ArgumentNullException(nameof(shutdownCallback));
            }

            if (m_closeRequestSource != null)
            {
                throw new InvalidOperationException(
                    "A close-application request source is already subscribed.");
            }

            m_closeRequestSource = requestSource;
            m_shutdownCallback = shutdownCallback;
            m_closeRequestSource.CloseApplicationRequested += OnCloseApplicationRequested;
        }

        /// <summary>
        /// Opens all registered components in registration order.
        /// If any component throws during <see cref="ILifeCycle.Open"/>, all previously
        /// opened components are closed in reverse order before the exception is
        /// re-thrown, leaving the system in a consistent closed state.
        /// Subsequent calls are ignored when already open.
        /// </summary>
        public void Open()
        {
            if (m_isOpen)
            {
                return;
            }

            int openedCount = 0;
            try
            {
                foreach (ILifeCycle component in m_components)
                {
                    component.Open();
                    openedCount++;
                }
                m_isOpen = true;
            }
            catch (Exception startupException)
            {
                // Roll back all successfully opened components in reverse order.
                // If rollback fails, aggregate startup and rollback failures so diagnostics are not lost.
                var rollbackExceptions = new List<Exception>();
                for (int i = openedCount - 1; i >= 0; i--)
                {
                    try
                    {
                        m_components[i].Close();
                    }
                    catch (Exception rollbackException)
                    {
                        rollbackExceptions.Add(rollbackException);
                    }
                }

                if (rollbackExceptions.Count > 0)
                {
                    rollbackExceptions.Insert(0, startupException);
                    throw new AggregateException(
                        "Startup failed and one or more rollback operations also failed.",
                        rollbackExceptions);
                }

                ExceptionDispatchInfo.Capture(startupException).Throw();
                throw;
            }
        }

        /// <summary>
        /// Closes all registered components in reverse registration order.
        /// Each component is closed independently so that a failure in one component
        /// does not prevent the remaining components from being closed.
        /// If any components throw, an <see cref="AggregateException"/> containing all
        /// failures is thrown after all components have been processed.
        /// Subsequent calls are ignored when already closed.
        /// </summary>
        /// <exception cref="AggregateException">
        /// Thrown when one or more components throw during <see cref="ILifeCycle.Close"/>.
        /// </exception>
        public void Close()
        {
            if (!m_isOpen)
            {
                return;
            }

            // Mark closed before the loop so that a reentrant or concurrent second
            // call is a no-op even if this call throws.
            m_isOpen = false;

            // Unsubscribe the close-request listener before tearing down components so
            // a late-arriving event during shutdown does not trigger a second close.
            if (m_closeRequestSource != null)
            {
                m_closeRequestSource.CloseApplicationRequested -= OnCloseApplicationRequested;
                m_closeRequestSource = null;
            }

            var closeExceptions = new List<Exception>();
            for (int i = m_components.Count - 1; i >= 0; i--)
            {
                try
                {
                    m_components[i].Close();
                }
                catch (Exception ex)
                {
                    closeExceptions.Add(ex);
                }
            }

            if (closeExceptions.Count > 0)
            {
                throw new AggregateException(
                    "One or more components failed to close cleanly.", closeExceptions);
            }
        }

        #endregion

        #region Private Members

        private readonly List<ILifeCycle> m_components = new List<ILifeCycle>();
        private readonly HashSet<ILifeCycle> m_registeredComponents =
            new HashSet<ILifeCycle>(ReferenceEqualityComparer.Instance);
        private bool m_isOpen;
        private ICloseApplicationRequestSource? m_closeRequestSource;
        private Action? m_shutdownCallback;

        private void OnCloseApplicationRequested(object? sender, EventArgs e)
        {
            // Use try/finally so the shutdown callback is always invoked even if
            // one or more components throw during Close().
            try
            {
                Close();
            }
            finally
            {
                m_shutdownCallback?.Invoke();
            }
        }

        #endregion
    }
}
