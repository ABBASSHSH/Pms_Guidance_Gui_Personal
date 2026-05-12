#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : Infrastructure
// File   : ICloseApplicationRequestSource.cs
// Description: Defines a source for close-application requests.
// Notes:
// Modification History : Code Review, 08-May-2026, Introduced close-request
//                        event contract used by lifecycle orchestration.
//--------------------------------------------------------------------
#endregion

using System;

namespace Infrastructure
{
    /// <summary>
    /// Defines a source for close-application requests.
    /// </summary>
    /// <reqkeys>
    /// <reqkey> REQUIREMENT_KEY </reqkey>
    /// </reqkeys>
    public interface ICloseApplicationRequestSource
    {
        /// <summary>
        /// Occurs when a graceful application shutdown is requested.
        /// </summary>
        event EventHandler<EventArgs>? CloseApplicationRequested;
    }
}