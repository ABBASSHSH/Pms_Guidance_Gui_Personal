#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : Infrastructure
// File   : ILifeCycle.cs
// Description: Defines the lifecycle contract for components with open/close semantics.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – compliance with C# Coding Guidelines V2.1
//--------------------------------------------------------------------
#endregion

namespace Infrastructure
{
    /// <summary>
    /// Defines the lifecycle contract for components with open/close semantics.
    /// </summary>
    /// <reqkeys>
    /// <reqkey> REQUIREMENT_KEY </reqkey>
    /// </reqkeys>
    public interface ILifeCycle
    {
        /// <summary>
        /// Opens and activates the component.
        /// </summary>
        void Open();

        /// <summary>
        /// Closes and deactivates the component.
        /// </summary>
        void Close();
    }
}
