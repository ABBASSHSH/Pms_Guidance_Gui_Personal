#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : Infrastructure
// File   : OutboundMessage.cs
// Description: Abstract base class for all outbound JSON messages sent to the Angular frontend.
//              Each concrete subclass owns its Action identifier and its payload fields,
//              so the entire message can be serialized in one step without runtime JSON manipulation.
// Notes:
// Modification History : Abbas Bahrainwala, 05-May-2026, Initial creation
//--------------------------------------------------------------------
#endregion

namespace Infrastructure
{
    /// <summary>
    /// Abstract base class for all outbound JSON messages sent to the Angular frontend.
    /// </summary>
    /// <remarks>
    /// Subclasses declare their <see cref="Action"/> as a fixed string and add their
    /// payload properties alongside it. <see cref="ConnectionModule.ConnectionManager"/> serializes
    /// each message using its concrete runtime type so that all properties — both
    /// <see cref="Action"/> and the subclass payload fields — appear in a single flat
    /// JSON object, e.g.
    /// <code>{ "Action": "ShowSystemLanguage", "Language": "en-US" }</code>
    /// </remarks>
    /// <reqkeys>
    /// <reqkey> REQUIREMENT_KEY </reqkey>
    /// </reqkeys>
    public abstract class OutboundMessage
    {
        /// <summary>
        /// Gets the action identifier that the Angular frontend uses to route this message
        /// to the correct handler.
        /// </summary>
        public abstract string Action { get; }
    }
}
