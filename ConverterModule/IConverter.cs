#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : ConverterModule
// File   : IConverter.cs
// Description: Defines the contract for the converter that bridges JSON messages and business logic.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – compliance with C# Coding Guidelines V2.1
//--------------------------------------------------------------------
#endregion

using Infrastructure;

namespace ConverterModule
{
    /// <summary>
    /// Defines the contract for the converter that bridges incoming JSON messages to business
    /// logic commands and outgoing replies back to JSON.
    /// </summary>
    /// <reqkeys>
    /// <reqkey> REQUIREMENT_KEY </reqkey>
    /// </reqkeys>
    public interface IConverter : ILifeCycle
    {
    }
}
