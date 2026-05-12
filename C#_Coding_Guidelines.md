# Coding Guidelines

**Name:** Coding Guidelines
**Version:** V2.1

**Maintained By:** SHS TE DC IND AT ACF SER
**Release Date:** 09/09/2025

## Author

**Name/Dept:** Linto Mathew (SHS TE DC IND AT ACF SER)

## REVISION HISTORY

| SR. NO. | DATE OF REVISION | VERSION | SECTION NUMBER | DESCRIPTION OF CHANGE | CHANGE MADE BY |
|---------|------------------|---------|----------------|----------------------|----------------|
| 1 | 12/08/2014 | 0.1 | | Initial Draft | Tressy Francis |
| 2 | 14/08/2014 | 0.2 | | Review comments implemented and sent for verification | Tressy Francis |
| 3 | 01/09/2014 | 1.0 | | Verified and released | Tressy Francis |
| 4 | 26/08/2025 | 2.0 | | Updated with clean code guideline | Charvi Dosi |
| 5 | 09/08/2025 | 2.1 | | Added guidelines for Copilot generated codes and updated general guidelines, Added C# coding guideline | Linto Mathew |

## TABLE OF CONTENTS

1. [INTRODUCTION](#1-introduction)
   - [1.1 Purpose](#11-purpose)
   - [1.2 Scope](#12-scope)
   - [1.3 Definitions, Acronyms and Abbreviations](#13-definitions-acronyms-and-abbreviations)
     - [1.3.1 Definitions](#131-definitions)
     - [1.3.2 Acronyms and Abbreviations](#132-acronyms-and-abbreviations)
   - [1.4 References](#14-references)

2. [GUIDELINES FOR C#](#2-guidelines-for-c)
   - [2.1 Naming Conventions](#21-naming-conventions)
   - [2.2 Programming Recommendations](#22-programming-recommendations)
     - [2.2.1 Classes](#221-classes)
     - [2.2.2 Methods](#222-methods)
     - [2.2.3 Class Members/Properties/Variables](#223-class-memberspropertiesvariables)
     - [2.2.4 Coding Style](#224-coding-style)
     - [2.2.5 Interoperability](#225-interoperability)
     - [2.2.6 COM Interoperability](#226-com-interoperability)
     - [2.2.7 Error Raising and Handling Guidelines](#227-error-raising-and-handling-guidelines)
     - [2.2.8 Use finally block to cleanup](#228-use-finally-block-to-cleanup)
     - [2.2.9 Do not implement a Finalizer](#229-do-not-implement-a-finalizer)
     - [2.2.10 Use the using directive for disposable classes](#2210-use-the-using-directive-for-disposable-classes)
     - [2.2.11 Use Managed equivalents where available](#2211-use-managed-equivalents-where-available)
     - [2.2.12 Do not initialize variables unnecessarily](#2212-do-not-initialize-variables-unnecessarily)
     - [2.2.13 Class code Complexity](#2213-class-code-complexity)
     - [2.2.14 Create Shortcut Names](#2214-create-shortcut-names)
     - [2.2.15 Use Generics](#2215-use-generics)
     - [2.2.16 XML Parser](#2216-xml-parser)
     - [2.2.17 Remoting Channel](#2217-remoting-channel)
   - [2.3 Dispose the objects](#23-dispose-the-objects)
   - [2.4 File Organization](#24-file-organization)
     - [2.4.1 General](#241-general)
     - [2.4.2 File Header](#242-file-header)
     - [2.4.3 Use # region to group members](#243-use--region-to-group-members)
     - [2.4.4 XML Commenting](#244-xml-commenting)
     - [2.4.5 Requirement key tracing](#245-requirement-key-tracing)
     - [2.4.6 Hazard Code](#246-hazard-code)
   - [2.5 Directory layout](#25-directory-layout)

3. [SECURE CODING GUIDELINES](#3-secure-coding-guidelines)

4. [GUIDELINES FOR AI TOOLS](#4-guidelines-for-ai-tools)

5. [GUIDELINE CHECKING TOOLS](#5-guideline-checking-tools)

6. [APPENDIX](#6-appendix)
   - [6.1 File Header](#61-file-header)
   - [6.2 Function Header](#62-function-header)
   - [6.3 Hazard Code](#63-hazard-code)
   - [6.4 AI Generated code header](#64-ai-generated-code-header)

---

## 1. INTRODUCTION

### 1.1 Purpose

The purpose of this document is to define a common style guide for C# programming. It should improve code quality and assist:

- the developers to write programs that are readable and maintainable by different programmers.
- the reviewer to review the code based on the guidelines captured in this document.

New or modified guidelines or recommendations are applicable for newly written source code.

Some of the guidelines and recommendations may be checked automatically; for some it is impossible to control compliance in this way. In these cases the control should be part of the code review or an inspection.

### 1.2 Scope

This document provides the coding guidelines for C#. It is recommended that each and every developer follow these guidelines. It does not apply to any auto-generated code.

The guidelines and recommendations here should be used for every new source code written in C# within the sources for the PMS project. These rules do not serve as a basis for code:

- which is created by tools (GUI-builder, NLS, etc.)
- which is purchased from external suppliers as commercially available code.
- which is reused from former projects (legacy code) with their own programming guidelines.

### 1.3 Definitions, Acronyms and Abbreviations

#### 1.3.1 Definitions

| Term | Definition |
|------|------------|
| **Class** | A class is a user-defined data type which consists of data elements and functions which operate on the data. |
| **Component** | Limited functional entity which is implemented as hardware, software, or mechanical parts. |
| **Declaration** | A declaration introduces one or more identifiers into a program. Before an identifier can be used it must be declared; its type must be specified to inform the compiler what kind of entity the identifier refers to. |
| **Definition** | A definition defines an entity for an identifier to refer to. That entity is an appropriate amount of memory, allocated by the compiler, to be used by the identifier. |
| **Enumeration Type** | An enumeration type is an explicitly declared set of symbolic integral constants, declared as an enum. |
| **Header** | A comment written before a special part of source code. Types include: file header, method header, function header, class header. |
| **Identifier** | A name referring to a variable, const, function, type, class, or macro. |
| **Scope** | The scope of an identifier refers to the context in which it is visible (can be used). Scopes include: local, function, file, and class. |
| **Pascal Casing** | The first letter of each word in an identifier is capitalized. Example: FontColor. |
| **Camel Casing** | Except the first letter of the first word, all first letters of subsequent words are capitalized. Example: pageNumber. |
| **DOM Parser (Document Object Model)** | A tree-based parser that reads the entire content of an XML document and creates an in-memory object representation. |

#### 1.3.2 Acronyms and Abbreviations

| Acronym | Definition |
|---------|------------|
| **API** | Application Programmer's Interface |
| **CSA** | Common Software Architecture |
| **GUI** | Graphical User Interface |
| **NLS** | Native Language Support |
| **PMS** | Prüfung, Montage Service; Manufacturing, Installation, Service |

### 1.4 References

[1] `\\ww005.siemens.net\1_AX\03_Projects\30_PLM\76_SupportSoftware_FOR\Projekt\Checklists\Codereview_Checklist_from_PLM_Process_details_SW_Development_V1.1`

---

## 2. GUIDELINES FOR C#

### 2.1 Naming Conventions

Pascal and Camel naming conventions will be followed for C#. The following table shows the naming convention for different identifiers.

| Identifier | Style | Example/Note | Description/Recommendations |
|------------|-------|--------------|----------------------------|
| **Class** | Pascal | TableView | Use a noun or noun phrase to name a class. Do not prefix any letter with the class name. Keep the class name and file name the same. Do not use underscore (_). |
| **Enum Type** | Pascal | FileMode | Use a noun or noun phrase. |
| **Enum Values** | Pascal | Create, Append | Use verbs or verb phrases to name methods. |
| **EventHandler** | Pascal | ButtonClickHandler | Use an EventHandler suffix on event handler names. Consider naming events with a verb. |
| **Attribute** | Pascal | ObsoleteAttribute | Always add the suffix Attribute to custom attribute classes. |
| **Const Field** | Pascal | MaxValue | - |
| **Interface** | Pascal | IView | Name interfaces with nouns, noun phrases, or adjectives that describe behavior. |
| **Method** | Pascal | GetBrowser | Use verbs or verb phrases to name methods. |
| **Namespace** | Pascal | Siemens.Automation.CommonService | Prefix namespace names with an organisation name to avoid conflicts. |
| **Parameter** | Camel | pmtPage | - |
| **Property** | Pascal | Height | Use a noun or noun phrase to name properties. |
| **Static Variable** | Pascal | s_Count | Use static properties instead of public static fields wherever possible. |
| **Local Variable** | Camel | m_GenericBrowser | Do not use instance fields that are public or protected. |
| **EventArgs** | Pascal | MouseEventArgs | Should be suffixed with EventArgs. |
| **Exception** | Pascal | DomainModelException | Should be suffixed with Exception. |
| **Permission** | Pascal | UserPermission | Should be suffixed with Permission. |
| **Event** | Pascal | BeforeClose | Use a verb for naming events. |

### 2.2 Programming Recommendations

#### 2.2.1 Classes

- Only define a destructor if really necessary. The garbage collector can handle classes without destructors more efficiently.
- If a destructor is defined, implement the IDisposable interface and follow the recommended implementation pattern. Refer to the Dispose Objects section.
- Use interfaces to decouple classes for better testability.

#### 2.2.2 Methods

- If a clean-up section is required in a method that must always be executed, use a try-finally construct with normal code in the try block and cleanup code in the finally block.
- Restrict visibility as much as possible (private, protected, internal, protected internal, public).

#### 2.2.3 Class Members/Properties/Variables

**Properties vs. Methods**

- Use properties to represent data.
- Use methods to represent an action (e.g., conversion).
- Use a method when the get accessor would have an observable side effect (e.g., performance impact, implicit changes to other state).
- Use a method when the order of execution is important.

#### 2.2.4 Coding Style

- For hazard code, use the `checked` keyword to detect overflow situations.
- If .NET code is consumed from unmanaged code, mark as many classes as possible with the `[ComVisible(false)]` attribute to avoid unnecessarily large imports in unmanaged code.
- To check whether two objects are equal, override `Object.Equals(object)`. Do not overload the `==` operator, as it is intended for checking object identity.
- Use `StringBuilder` or `String.Format` for constructing strings, as this is more efficient than concatenating individual string objects:

```csharp
StringBuilder aBuilder = new StringBuilder("Message:");
aBuilder.Append("appending some result");
aBuilder.Append("another string");
aBuilder.ToString();

Trace.Debug(this, String.Format("{0}File could not be found", filename_in));
```

- As C# does not support default arguments, provide additional overloaded methods that take fewer arguments and internally delegate to the original method.

#### 2.2.5 Interoperability

- CLS-compliant (CLS = Common Language Specification) interfaces allow cross-language development within a single software system.
- Interface assemblies should be CLS-compliant. Some language compilers can check whether a program element is CLS-compliant.

**Example:**

```csharp
using System;
[assembly:CLSCompliantAttribute(true)]
public class CompliantClass
{
    // ...
}
```

- The following FCL (Framework Class Library) types are CLS-compliant:
  - System.Byte, System.Int16, System.Int32, System.Int64
  - System.Single, System.Double, System.Boolean
  - System.Char, System.Decimal, System.String, System.Object

For further reading, refer to MSDN Library, Cross-Language Interoperability.

#### 2.2.6 COM Interoperability

- Use blittable data types or arrays of blittable data types in COM visible interface methods.
- Blittable types have a common representation across the interop boundary. Non-blittable types are converted during marshaling, causing additional overhead.
- The following data types are blittable: System.Byte, System.SByte, System.Int16, System.UInt16, System.Int32, System.UInt32, System.Int64, System.UInt64, System.IntPtr, System.UIntPtr.
- Use only the BSTR string type in COM exported interfaces.
- All data types used in COM visible interface methods must be COM visible as well.
- Use only simple data types or interfaces in COM visible interface methods.
- Specify the `GuidAttribute` for all COM visible types to provide a fixed GUID.

**Example:**

```csharp
[ComVisible(true), GuidAttribute("BDD89F3C-3D24-4a1a-B6CF-91FD048A17D3")]
public interface IPresentationStateModule
{
    // ...
}
```

- Specify the `MarshalAsAttribute` to provide the data type visible in COM, even when default marshaling is used.

**Example:**

```csharp
void DoSomeThing([In, MarshalAs(UnmanagedType.Int32)]Int32 value_in);
```

- Specify the `ProgIdAttribute` for COM visible classes to improve creation, readability, and maintainability.

**Example:**

```csharp
[ProgId("InteropSample.MyClass")]
public class MyClass
{
    public MyClass()
    {
        // ...
    }
}
```

- Catch all exceptions within a COM method. Use HRESULT to provide error information.
- Exceptions must never leave a COM method. Do not use the `PreserveSigAttribute`, as it suppresses the HRESULT signature transformation during COM interop calls.

#### 2.2.7 Error Raising and Handling Guidelines

- Use standard exceptions.
- All code paths that result in an exception should provide a method to check for success without throwing. For example, call `File.Exists` to avoid a `FileNotFoundException`.
- Use a localized description string in every exception.
- Do not expose privileged information (such as local file system paths) in exception messages.
- Do not use exceptions for normal or expected errors, or for normal flow of control.
- Do not derive all new exceptions directly from `System.Exception`. Inherit from `System.Exception` only when creating new exceptions in System namespaces. Inherit from `Application.Exception` when creating new exceptions in other namespaces.
- Throw exceptions instead of returning an error code or HRESULT.
- Throw the most specific exception possible.
- Use inner exceptions (chained exceptions). Do not catch and re-throw exceptions unless adding additional information or changing the exception type.

#### 2.2.8 Use finally block to cleanup

The finally block is always executed regardless of whether an exception is thrown. Use the finally block for all cleanup tasks. Do not use the catch block for cleanup.

#### 2.2.9 Do not implement a Finalizer

Do not implement a finalizer or destructor unless it is necessary to clean up unmanaged resources. In that case, create a separate dedicated class for handling a single unmanaged resource and implement a destructor with the dispose pattern. Derive this class from a system-provided base class such as SafeHandle where appropriate.

**Example:**

```csharp
// Only if necessary
~MyClass() { ... }
```

#### 2.2.10 Use the using directive for disposable classes

Wrap the instantiation of IDisposable objects with a `using` statement to ensure that `Dispose` is automatically called.

**Example:**

```csharp
using (SqlConnection cn = new SqlConnection(connectionString))
{
    // ...
}
```

#### 2.2.11 Use Managed equivalents where available

Do not use unmanaged code when the same functionality can be achieved using managed code.

#### 2.2.12 Do not initialize variables unnecessarily

The Common Language Runtime initializes all fields to their default values before calling the constructor. Unnecessary initialization increases time and space complexity.

#### 2.2.13 Class code Complexity

It is recommended to keep the code complexity of any class below 15.

#### 2.2.14 Create Shortcut Names

Use shortcut aliases for long namespace names.

```csharp
using CS = Siemens.Automation.CommonService;
```

#### 2.2.15 Use Generics

Use generics wherever possible to avoid casting overhead, which degrades performance.

#### 2.2.16 XML Parser

The framework provides an XML parser that supports both DOM and STAX parsers. The choice depends on the characteristics of the application being developed.

##### 2.2.16.1 Use DOM Parser

Use the DOM parser when:

- The application needs to access widely separated parts of the document at the same time.
- The application may use an internal data structure nearly as complex as the document itself.
- The application needs to modify the document repeatedly.
- The application must store the document for a significant amount of time across multiple method calls.

##### 2.2.16.2 Use STAX Parser

Use the STAX parser when:

- The document is too large for available memory.
- The document needs to be processed in small contiguous chunks.
- Processing can begin before the entire document is available.

#### 2.2.17 Remoting Channel

Different channel services are available for remoting applications. Refer to the project architecture documentation for the recommended channel configuration.

### 2.3 Dispose the objects

```csharp
// Design pattern for a base class.
public class Base : IDisposable
{
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Free managed state (managed objects).
        }
        // Free unmanaged resources.
        // Set large fields to null.
    }

    ~Base()
    {
        Dispose(false);
    }
}

// Design pattern for a derived class.
public class Derived : Base
{
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Release managed resources.
        }
        // Release unmanaged resources.
        // Set large fields to null.
        base.Dispose(disposing);
    }
    // The derived class does not have a Finalize method
    // or a Dispose method with parameters because it inherits
    // them from the base class.
}
```

**Note:** If another class is derived from this class, it should only override `Dispose(bool)`. It should not implement `IDisposable` itself, nor provide a destructor.

### 2.4 File Organization

#### 2.4.1 General

- File names and class names must be the same.
- There must not be more than 500 lines of code per file.
- There must not be more than 50 lines of code in any method (private or public).

#### 2.4.2 File Header

Use the following file header for all C# source files:

```csharp
#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : <module>
// File   : FileName.cs
// Description:
// Notes:
// Modification History : <name>, <Date> <Reason for change>
//--------------------------------------------------------------------
#endregion
```

#### 2.4.3 Use #region to group members

Group all non-public members in a region. Use separate regions to split private, protected, and internal members.

For test classes, grouping by basic tests and extended tests is expected.

#### 2.4.4 XML Commenting

All public and protected types, methods, fields, events, delegates, and similar members must be documented using XML tags. These tags enable IntelliSense to provide useful details and allow automatic documentation generation.

All XML comments must start with three forward slashes (`///`).

| Tag | Description | Mandatory? | Location |
|-----|-------------|------------|----------|
| `<summary>` | Short description | Yes | Type or member |
| `<remarks>` | Supplemental information | No | Type or member |
| `<param>` | Describes method parameters | Yes | Method |
| `<returns>` | Describes the return value | Yes | Method |
| `<exception>` | Lists the exceptions | No | Method, event, or property |
| `<value>` | Describes a property | Yes | Property |
| `<example>` | Provides examples | No | Type or member |
| `<reqkeys>` | Lists all requirement keys (not a predefined tag) | Yes | Type |
| `<reqkey>` | Contains a single requirement key (not a predefined tag) | Yes | Type |

#### 2.4.5 Requirement key tracing

The class header must list all requirement keys that are partially or fully implemented by the class.

**Example:**

```csharp
/// <summary> Summary information of the class </summary>
/// <reqkeys>
/// <reqkey> REQUIREMENT_KEY_ONE </reqkey>
/// <reqkey> REQUIREMENT_KEY_TWO </reqkey>
/// </reqkeys>
```

This is not applicable for test classes.

#### 2.4.6 Hazard Code

Hazard code is identified by placing special begin and end tags. The hazard key name is part of this tag. The format is as follows:

```csharp
// {:IMPLEMENT:hm_xx_xxxxxxxxxxxxxx(PMS VERSION_xxxxxx): :}

// Hazard code

//{:ENDIMPL::}
```

### 2.5 Directory layout

Create a separate folder for every namespace. Use the namespace name as the folder name. Source files may be logically clustered in different folders within the same namespace, with folder names starting with an underscore (_).

**Example:**

Files related to the `Siemens.Automation.CommonService` namespace must be stored under the `Siemens\Automation\CommonService` folder.

---

## 3. SECURE CODING GUIDELINES

Applying the Secure Coding Guideline of SOP418 is mandatory. The guideline has the SAP-ID `[11275139_418_COD_2 ASD E00 01]`.

---

## 4. GUIDELINES FOR AI TOOLS

Code generated by AI tools such as Copilot must comply with the company's coding guidelines. These guidelines are available at `[SHS_GH_Copilot_Usage_Guide_-_Extended.pptx]`.

For additional information on usage, refer to `[Rollout]`.

---

## 5. GUIDELINE CHECKING TOOLS

Every rule that is a guideline can be checked by a tool. If it is not possible to check a guideline automatically, it must become a recommendation only. Compliance with recommendations must be verified during code review or inspection.

---

## 6. APPENDIX

### 6.1 File Header

```csharp
#region Copyright
//--------------------------------------------------------------------
//
// Copyright © Siemens Healthineers AG, All Rights Reserved, Confidential
//
//--------------------------------------------------------------------
// Project: PMS Service Software
// Module : <module>
// File   : FileName.cs
// Description:
// Notes:
// Modification History : <name>, <Date> <Reason for change>
//--------------------------------------------------------------------
#endregion
```

### 6.2 Function Header

```csharp
//--------------------------------------------------------------------
//
// Name:        MethodName()
//
// Author:      FirstName, LastName
// Created:     mmm - yyyy
//
// Parameters:
//   type ParamName
//
// ReturnValue:
//   type
//
// Description:
//   Description of the method.
//
// Modified:    DD.MM.YY [modifier] [Modification]
//
//--------------------------------------------------------------------
```

### 6.3 Hazard Code

Hazard code is identified by placing special begin and end tags. The hazard key (hm_xx_xxxxxxxxxxxxxxx) name is part of this tag. The format is as follows:

```csharp
// {:IMPLEMENT:hm_xx_xxxxxxxxxxxxxx(PMS VERSION_xxxxxx): :}

// Hazard code

//{:ENDIMPL::}
```

### 6.4 AI Generated code header

An AI generated code header is required only for code that was purely created by AI and has not been manually modified or reviewed.

#### C#, TypeScript, JavaScript, Java, Kotlin, Go

```csharp
// Created by AI – begin
// ---- some code -----
// Created by AI – end
```

#### PowerShell, Python, Terraform, YAML

```python
# Created by AI – begin
# ---- some code -----
# Created by AI – end
```

#### HTML, Markdown, XML

```html
<!-- Created by AI – begin -->
<!-- ---- some code ----- -->
<!-- Created by AI – end -->
```
