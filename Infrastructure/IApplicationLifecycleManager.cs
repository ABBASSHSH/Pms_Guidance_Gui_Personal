#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : Infrastructure
// File   : IApplicationLifecycleManager.cs
// Description: Defines the contract for orchestrating the ordered startup and
//              shutdown of all registered ILifeCycle components.
// Notes:
// Modification History : Code Review, 06-May-2026, Introduced IApplicationLifecycleManager
//                        to provide a controlled startup and shutdown strategy.
//--------------------------------------------------------------------
#endregion

namespace Infrastructure
{
    /// <summary>
    /// Defines the contract for orchestrating the ordered startup and shutdown of
    /// all registered <see cref="ILifeCycle"/> components.
    /// Components are opened in registration order and closed in reverse order.
    /// </summary>
    /// <reqkeys>
    /// <reqkey> REQUIREMENT_KEY </reqkey>
    /// </reqkeys>
    public interface IApplicationLifecycleManager : ILifeCycle
    {
        /// <summary>
        /// Registers an <see cref="ILifeCycle"/> component for managed startup and shutdown.
        /// Components must be registered before <see cref="ILifeCycle.Open"/> is called.
        /// </summary>
        /// <param name="component">The component to register. Must not be <c>null</c>.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="component"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// Thrown when <see cref="ILifeCycle.Open"/> has already been called.
        /// </exception>
        void Register(ILifeCycle component);

        /// <summary>
        /// Subscribes to an external close-application request source.
        /// When a close request is raised, the lifecycle manager closes all
        /// registered components and then executes the provided shutdown callback.
        /// </summary>
        /// <param name="requestSource">The source that raises close requests.</param>
        /// <param name="shutdownCallback">
        /// Callback invoked after lifecycle close to finalize process shutdown.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="requestSource"/> or
        /// <paramref name="shutdownCallback"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// Thrown when a request source has already been subscribed.
        /// </exception>
        void SubscribeToCloseApplicationRequests(
            ICloseApplicationRequestSource requestSource,
            System.Action shutdownCallback);
    }
}
