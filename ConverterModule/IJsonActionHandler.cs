#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : ConverterModule
// File   : IJsonActionHandler.cs
// Description: Defines the contract for handling a specific JSON action.
// Notes:
// Modification History : Abbas Bahrainwala, 29-Apr-2026, Code review – compliance with C# Coding Guidelines V2.1
//--------------------------------------------------------------------
#endregion

using Infrastructure;

namespace ConverterModule
{
    /// <summary>
    /// Defines the contract for handling a specific JSON action.
    /// </summary>
    internal interface IJsonActionHandler
    {
        /// <summary>
        /// Gets the action name this handler is responsible for.
        /// </summary>
        /// <value>The action name string.</value>
        string ActionName { get; }

        /// <summary>
        /// Handles the action and returns the corresponding command.
        /// </summary>
        /// <param name="message">The JSON message payload.</param>
        /// <returns>The command produced from the action, or <see langword="null"/> if it cannot be handled.</returns>
        ICommand? HandleAction(string message);
    }
}
