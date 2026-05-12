# HybridWebApps � Complete Solution Guide

## What Is This Application?

This is a **WPF desktop application** (.NET 8, C# 12) that embeds a web app (Angular) inside a native Windows window using Microsoft's **WebView2** control (a real Chromium browser inside WPF).

The two sides � the Angular web app and the C# desktop code � communicate by sending **JSON messages** back and forth. Think of it like two people passing notes written in a specific format.

---

## Project Structure Overview

```
Solution
?
??? Pms_GuidanceGUI          ? The app you run (WPF window + browser host)
??? ConnectionModule         ? The message bridge (raw JSON ? typed events)
??? ConverterModule          ? The translator (JSON ? typed C# commands)
??? BusinessLogicModule      ? The brain (does the real work)
??? Infrastructure           ? The shared language (interfaces & data types only)
```

> **Key Concept � Layered Architecture:**  
> Each project has a specific job and only talks to others through agreed-upon contracts called *interfaces*. No project reaches into another's internals. This makes the code easier to maintain, test, and extend.

---

## The Dependency Map

```
Pms_GuidanceGUI
    ?
    ???? ConnectionModule ??? Infrastructure
    ???? ConverterModule  ??? Infrastructure
    ???? BusinessLogicModule ??? Infrastructure
    ???? Infrastructure
```

All four outer projects depend on **Infrastructure**, but not on each other directly.

---

## Project 1 � Infrastructure (The Shared Language)

> **No logic lives here.** This is purely contracts (interfaces) and shared data shapes (classes) that every other project agrees to use.

### Interfaces

| Interface | File | What It Promises |
|---|---|---|
| `IWebViewWrapper` | `IWebViewWrapper.cs` | Exposes the browser: `InitializeAsync()`, `SendMessage()`, `OnMessageReceived` event |
| `IConnectionManager` | *(in ConnectionModule)* | `SendMessage()`, `MessageReceived` event |
| `IBusinessLogicModule` | `IBusinessLogicModule.cs` | `HandleCommand(ICommand)`, `ActionReplyEvent` property |
| `ICommand` | `ICommand.cs` | Marker only � "this object is a command" |
| `ICommandHandler` | `ICommandHandler.cs` | `CommandType` property + `HandleCommand(ICommand)` |
| `IActionReply` | `IActionReply.cs` | Exposes `OnCommandHandled` event (subscribe-only view) |
| `ILifeCycle` | `ILifeCycle.cs` | `Open()` and `Close()` � start and stop listening |

### Data Classes

| Class | File | What It Holds |
|---|---|---|
| `MessageReceivedEventArgs` | `MessageReceivedEventArgs.cs` | `Action` (string) + `Payload` (string) � travels from Angular inward |
| `JsonReplyMessage` | `JsonReplyMessage.cs` | `Action` (string) + `Message` (object) � travels from C# back to Angular |

> **Key Concept � Interface:**  
> An interface is a *contract* with no code in it. It says "whoever implements me *must* provide these methods/properties/events." It lets modules depend on the *shape* of a thing, not on a specific class. This means you can swap the real implementation for a fake one in tests.

---

## Project 2 � Pms_GuidanceGUI (The Shell / Entry Point)

This is the WPF application itself. It owns the window and is responsible for creating and wiring up all the other modules.

### Files

| File | Role |
|---|---|
| `App.xaml.cs` | Application entry point |
| `MainWindow.xaml.cs` | Creates all modules and wires them together |
| `WebViewWrapper.cs` | Wraps the WebView2 browser control |

### MainWindow Startup Sequence

```
MainWindow constructor
?
??? 1. new WebViewWrapper()              ? creates the Chromium browser control
?
??? 2. new BusinessLogicModuleSetup()    ? creates the business logic layer
?
??? 3. new ConnectionManager(webView)    ? creates the bridge (needs the browser)
?
??? 4. new Converter(bizLogic, connMgr)  ? creates the translator (needs both)
?
??? 5. connectionManager.Open()          ? start listening to browser messages
?
??? 6. converter.Open()                  ? start processing those messages
?
??? 7. Content = webView.GetWebViewControl()  ? make the browser fill the window
?
??? 8. Loaded event ? webView.InitializeAsync()  ? navigate to the Angular app
```

### WebViewWrapper.cs � The Browser Host

```
WebView2 (Chromium browser inside WPF)
    ?
    ?  fires CoreWebView2.WebMessageReceived
    ?
WebViewWrapper.WebMessageReceived()
    ?  extracts the raw JSON string
    ?
raises _onMessageReceived event
    ?  passes the string to whoever subscribed
    ?
ConnectionManager (subscribed via OnMessageReceived)
```

**Outbound (C# ? Angular):**
```csharp
myWebView.CoreWebView2.PostWebMessageAsJson(jsonString);
```

The browser is initialized with a virtual host name mapping:
```
https://pmsGuidanceFrontendApp/index.html
    maps to
..\Upgrade\dist\upgrade\browser\   (the built Angular app files on disk)
```

> **Key Concept � Event:**  
> An event is a notification mechanism. Code fires an event ("something happened"), and any code that subscribed to that event gets called automatically. It decouples the sender (who doesn't need to know who's listening) from the receiver (who doesn't need to know who fired it).

---

## Project 3 � ConnectionModule (The Message Bridge)

Sits directly above `WebViewWrapper`. Its job is to turn raw JSON strings from the browser into structured typed events, and to serialize typed objects back into JSON for the browser.

### Files

| File | Role |
|---|---|
| `ConnectionManager.cs` | Core bridge class |
| `IConnectionManager.cs` | Its public contract |
| `JsonMessage/RawMessage.cs` | Shape of every incoming message |
| `JsonMessage/CallContext.cs` | The "action name" part of a message |

### The JSON Message Format

Every message between Angular and C# follows this shape:

```json
{
  "CallContext": {
    "Action": "LogMessage"
  },
  "Payload": {
    "Message": "Hello world",
    "timestamp": "2024-01-01T12:00:00"
  }
}
```

Mapped to C# classes:

```
RawMessage
??? CallContext
?     ??? Action : string   ? tells us what to do ("LogMessage")
??? Payload : JsonElement   ? the data for that action (kept as raw JSON)
```

### Inbound Flow (Angular ? C#)

```
Raw JSON string arrives from browser
    ?
    ?
ConnectionManager.OnMessageReceived(string e)
    ?
    ??? JsonSerializer.Deserialize<RawMessage>(e)
    ?
    ??? raises MessageReceived event
          with MessageReceivedEventArgs(
                  action:  rawMessage.CallContext.Action,   // "LogMessage"
                  payload: JsonSerializer.Serialize(rawMessage.Payload)
               )
```

### Outbound Flow (C# ? Angular)

```csharp
connectionManager.SendMessage("ShowLogMessage", payloadObject)
    ?
    ??? wraps into RawMessage { CallContext = { Action = "ShowLogMessage" }, Payload = ... }
    ??? serializes to JSON string
    ??? calls webViewWrapper.SendMessage(jsonString)
```

> **Key Concept � Deserialization:**  
> JSON is just text. *Deserializing* means reading that text and creating a real C# object from it. *Serializing* is the reverse � turning a C# object into a JSON string.

---

## Project 4 � ConverterModule (The Translator)

This is the glue between the raw message world (`ConnectionModule`) and the typed command world (`BusinessLogicModule`). It translates in both directions.

### Files

| File | Role |
|---|---|
| `Converter.cs` | Orchestrates everything, implements `ILifeCycle` |
| `JsonActionHandler.cs` | Registry of inbound action handlers |
| `IJsonActionHandler.cs` | Contract for "can handle one named action from Angular" |
| `IJsonActionHandlerGeneric.cs` | Generic version of above, typed to a message class |
| `IJsonWriter.cs` | Contract for "can convert a business event to a JSON reply" |
| `LogJsonActionHandler.cs` | Handles the `"LogMessage"` action |
| `VerifyInstallationPrerequisitesJsonActionHandler.cs` | Handles `"VerifyInstallationPrerequisites"` (stub) |
| `JsonWriter/ShowLogStatusJsonWriter.cs` | Converts `LogStatusEvent` ? JSON reply |
| `JsonMessage/LogMessage.cs` | Shape of the payload for `"LogMessage"` |
| `JsonMessage/ShowLogMessage.cs` | Shape of the reply for `"ShowLogMessage"` |

### The Two Interfaces Explained

#### `IJsonActionHandler` � Inbound

```csharp
internal interface IJsonActionHandler
{
    string ActionName { get; }                        // e.g. "LogMessage"
    Infrastructure.ICommand HandleAction(string message); // payload JSON ? ICommand
}
```

Each implementation handles exactly **one named action** coming from Angular and returns a typed `ICommand` for `BusinessLogicModule` to execute.

#### `IJsonWriter` � Outbound

```csharp
internal interface IJsonWriter
{
    bool CanWrite(EventArgs theDataEvent);             // do I handle this event type?
    JsonReplyMessage CreateJsonMessage(EventArgs theDataEvent); // event ? JSON reply
}
```

Each implementation converts exactly **one type of business event** into a `JsonReplyMessage` to send back to Angular.

### JsonActionHandler.cs � The Handler Registry

```
JsonActionHandler
??? JsonActionHandlersList : List<IJsonActionHandler>
      ??? LogJsonActionHandler          (ActionName = "LogMessage")
      ??? VerifyInstallationPrerequisitesJsonActionHandler (ActionName = "VerifyInstallationPrerequisites")
```

When `HandleJsonAction("LogMessage", payload)` is called:
1. Scans the list for a handler whose `ActionName == "LogMessage"`
2. Calls `handler.HandleAction(payload)`
3. Returns the resulting `ICommand`
4. Returns `null` if no matching handler is found

> **Key Concept � Strategy Pattern:**  
> Instead of one big `if/else` block checking the action name, each handler is its own class registered in a list. Adding a new action = adding a new class + registering it. Nothing else changes.

### Converter.cs � The Orchestrator

`Converter` is the only class in this module that is visible to `MainWindow`. It:

- Holds the `JsonActionHandler` registry
- Holds the list of `IJsonWriter` outbound writers
- Subscribes to `ConnectionManager.MessageReceived` (inbound)
- Subscribes to `BusinessLogicModule.ActionReplyEvent.OnCommandHandled` (outbound)

#### Inbound path through Converter:

```
ConnectionManager raises MessageReceived("LogMessage", payloadJson)
    ?
    ?
Converter.OnMessageReceived()
    ?
    ??? myJsonActionHandler.HandleJsonAction("LogMessage", payloadJson)
    ?     ??? finds LogJsonActionHandler
    ?           ??? deserializes payload ? LogCommand(message, timestamp)
    ?
    ??? myBusinessLogicModule.HandleCommand(LogCommand)
```

#### Outbound path through Converter:

```
BusinessLogicModule raises OnCommandHandled(LogStatusEvent)
    ?
    ?
Converter.ActionReplyEvent_OnCommandHandled()
    ?
    ??? loops through myJsonWriters
    ?     ??? ShowLogStatusJsonWriter.CanWrite(LogStatusEvent) ? true
    ?           ??? CreateJsonMessage(LogStatusEvent)
    ?                 ? JsonReplyMessage { Action="ShowLogMessage", Message={Status="Success"} }
    ?
    ??? myConnectionManager.SendMessage(jsonReplyMessage)
```

### Concrete Handler: LogJsonActionHandler

```
Inbound payload JSON:
{ "Message": "hello", "timestamp": "2024-01-01T12:00:00" }
    ?
    ?
JsonSerializer.Deserialize<LogMessage>(payload)
    ?
    ?
new LogCommand(log.Message, log.timestamp)
    ?
    ?
returned to Converter ? forwarded to BusinessLogicModule
```

### Concrete Writer: ShowLogStatusJsonWriter

```
LogStatusEvent(IsLogged: true) arrives
    ?
    ??? CanWrite() checks: event type == LogStatusEvent? ? yes
    ?
    ??? CreateJsonMessage()
          ? JsonReplyMessage {
              Action  = "ShowLogMessage",
              Message = ShowLogMessage { Status = "Success" }
            }
```

---

## Project 5 � BusinessLogicModule (The Brain)

Contains the actual application work. Currently implements one action: writing a log file to disk.

### Files

| File | Role |
|---|---|
| `BusinessLogicModuleSetup.cs` | Implements `IBusinessLogicModule`; owns the command handler registry |
| `LogActionCommandHandler.cs` | Handles `LogCommand` � writes a `.txt` file |
| `ActionReplyHandler.cs` | Implements both `IActionReply` and `IActionReplyPrivate` |
| `IActionReplyPrivate.cs` | Internal-only interface to *fire* the reply event |
| `Commands/LogCommand.cs` | Data for a log request: `Message` + `Timestamp` |
| `EventArgs/LogStatusEvent.cs` | Result of a log action: `IsLogged` (bool) |

### BusinessLogicModuleSetup.cs � The Command Dispatcher

```
BusinessLogicModuleSetup
?
??? CommandHandlersList : Dictionary<Type, ICommandHandler>
?     ??? typeof(LogCommand) ? LogActionCommandHandler
?
??? ActionReplyEvent : ActionReplyHandler  (exposed as IActionReply)
```

`HandleCommand(ICommand theCommand)`:
```
receives LogCommand
    ?
    ??? looks up typeof(LogCommand) in dictionary
    ??? finds LogActionCommandHandler
    ??? calls LogActionCommandHandler.HandleCommand(logCommand)
```

> **Key Concept � Command Pattern:**  
> A command is an object that represents a request ("log this message"). The dispatcher doesn't know what a `LogCommand` does � it just finds the right handler. This makes it trivial to add new commands: create a new `ICommand` class, create a new `ICommandHandler` class, register the pair. The dispatcher code never changes.

### LogActionCommandHandler.cs � The Worker

```csharp
// What it does when LogCommand arrives:
string logFileName = $"Log_{DateTime.Now:yyyyMMdd_HHmmss_fff}.txt";
string logEntry    = $"Time: {timestamp}\nMessage: {message}";
File.WriteAllText(logFileName, logEntry);

// Then notifies the rest of the app it succeeded:
myActionReplyPrivate.InvokeEvent(new LogStatusEvent(true));
```

### The IActionReply / IActionReplyPrivate Split

`ActionReplyHandler` implements **two interfaces at once**:

```
ActionReplyHandler
?
??? as IActionReply (public � given to Converter)
?     ??? can only SUBSCRIBE to OnCommandHandled
?
??? as IActionReplyPrivate (internal � given to LogActionCommandHandler)
      ??? can FIRE InvokeEvent()
```

```
Outside world (Converter)         Inside world (LogActionCommandHandler)
        ?                                       ?
        ? subscribes to OnCommandHandled        ? calls InvokeEvent(LogStatusEvent)
        ?                                       ?
              ActionReplyHandler.OnCommandHandled fires
```

> **Key Concept � Encapsulation:**  
> By exposing two different interfaces to two different audiences, the code ensures that only internal handlers can fire the event, while external modules can only listen. This prevents accidental misuse.

---

## Complete End-to-End Flow

### Scenario: User clicks "Log" in the Angular app

```
???????????????????????????????????????????????????????????
?  ANGULAR APP                                            ?
?  User clicks Log button                                 ?
?  Sends:                                                 ?
?  {                                                      ?
?    "CallContext": { "Action": "LogMessage" },           ?
?    "Payload": {                                         ?
?      "Message": "Hello",                               ?
?      "timestamp": "2024-01-01T12:00:00"                ?
?    }                                                    ?
?  }                                                      ?
???????????????????????????????????????????????????????????
                       ? browser posts message
                       ?
???????????????????????????????????????????????????????????
?  WebViewWrapper                                         ?
?  WebMessageReceived fires                               ?
?  ? raises OnMessageReceived(rawJsonString)              ?
???????????????????????????????????????????????????????????
                       ?
                       ?
???????????????????????????????????????????????????????????
?  ConnectionManager                                      ?
?  Deserializes ? RawMessage                              ?
?  ? raises MessageReceived(                              ?
?        action:  "LogMessage",                           ?
?        payload: "{\"Message\":\"Hello\",...}"           ?
?    )                                                    ?
???????????????????????????????????????????????????????????
                       ?
                       ?
???????????????????????????????????????????????????????????
?  Converter                                              ?
?  OnMessageReceived() called                             ?
?  ? JsonActionHandler finds LogJsonActionHandler         ?
?  ? deserializes payload ? LogCommand("Hello", ...)      ?
?  ? calls BusinessLogicModule.HandleCommand(LogCommand)  ?
???????????????????????????????????????????????????????????
                       ?
                       ?
???????????????????????????????????????????????????????????
?  BusinessLogicModuleSetup                               ?
?  Finds LogActionCommandHandler for LogCommand           ?
?  ? writes Log_20240101_120000_000.txt to disk           ?
?  ? fires OnCommandHandled(LogStatusEvent(true))         ?
???????????????????????????????????????????????????????????
                       ?
                       ?
???????????????????????????????????????????????????????????
?  Converter (listening to ActionReplyEvent)              ?
?  ActionReplyEvent_OnCommandHandled() called             ?
?  ? ShowLogStatusJsonWriter.CanWrite(LogStatusEvent)?true?
?  ? CreateJsonMessage()                                  ?
?    ? JsonReplyMessage {                                 ?
?        Action:  "ShowLogMessage",                       ?
?        Message: { Status: "Success" }                   ?
?      }                                                  ?
?  ? calls ConnectionManager.SendMessage(reply)           ?
???????????????????????????????????????????????????????????
                       ?
                       ?
???????????????????????????????????????????????????????????
?  ConnectionManager                                      ?
?  Serializes reply to JSON string                        ?
?  ? WebViewWrapper.SendMessage(jsonString)               ?
???????????????????????????????????????????????????????????
                       ?
                       ?
???????????????????????????????????????????????????????????
?  ANGULAR APP                                            ?
?  Receives:                                              ?
?  {                                                      ?
?    "CallContext": { "Action": "ShowLogMessage" },       ?
?    "Payload": { "Status": "Success" }                   ?
?  }                                                      ?
?  ? shows success feedback to user                       ?
???????????????????????????????????????????????????????????
```

---

## Design Patterns Reference

| Pattern | Where Used | What Problem It Solves |
|---|---|---|
| **Command** | `ICommand` / `ICommandHandler` / `BusinessLogicModuleSetup` | Decouples "what to do" from "how to do it"; easy to add new actions |
| **Strategy** | `IJsonActionHandler` list in `JsonActionHandler` | Selects the right handler at runtime without if/else chains |
| **Observer / Event** | `OnMessageReceived`, `MessageReceived`, `OnCommandHandled` | Notifies interested parties without tight coupling |
| **Facade** | `Converter`, `ConnectionManager` | Hides internal complexity behind a simple interface |
| **Interface Segregation** | `IActionReply` vs `IActionReplyPrivate` | Different audiences get different views of the same object |
| **Layered Architecture** | The 5 projects themselves | Each layer has one responsibility; dependencies only flow inward |

---

## How to Add a New Action (Step-by-Step)

Say you want to add a new action called `"RestartService"`.

### Step 1 � Create the inbound message shape (ConverterModule)
```csharp
// ConverterModule/JsonMessage/RestartServiceMessage.cs
internal class RestartServiceMessage
{
    public string ServiceName { get; set; }
}
```

### Step 2 � Create the command (BusinessLogicModule)
```csharp
// BusinessLogicModule/Commands/RestartServiceCommand.cs
public class RestartServiceCommand : Infrastructure.ICommand
{
    public string ServiceName { get; set; }
    public RestartServiceCommand(string serviceName) => ServiceName = serviceName;
}
```

### Step 3 � Create the action handler (ConverterModule)
```csharp
// ConverterModule/RestartServiceJsonActionHandler.cs
internal class RestartServiceJsonActionHandler : IJsonActionHandler<RestartServiceMessage>
{
    public string ActionName => "RestartService";
    public ICommand HandleAction(string message)
    {
        var msg = JsonSerializer.Deserialize<RestartServiceMessage>(message);
        return new RestartServiceCommand(msg.ServiceName);
    }
}
```

### Step 4 � Create the command handler (BusinessLogicModule)
```csharp
// BusinessLogicModule/RestartServiceCommandHandler.cs
internal class RestartServiceCommandHandler : ICommandHandler
{
    public Type CommandType => typeof(RestartServiceCommand);
    public void HandleCommand(ICommand command)
    {
        var cmd = (RestartServiceCommand)command;
        // ... do the real work ...
    }
}
```

### Step 5 � Create the result event (BusinessLogicModule)
```csharp
// BusinessLogicModule/EventArgs/RestartServiceStatusEvent.cs
public class RestartServiceStatusEvent : System.EventArgs
{
    public bool Success { get; set; }
    public RestartServiceStatusEvent(bool success) => Success = success;
}
```

### Step 6 � Create the JSON writer (ConverterModule)
```csharp
// ConverterModule/JsonWriter/RestartServiceStatusJsonWriter.cs
internal class RestartServiceStatusJsonWriter : IJsonWriter
{
    public bool CanWrite(EventArgs e) => e is RestartServiceStatusEvent;
    public JsonReplyMessage CreateJsonMessage(EventArgs e)
    {
        var evt = (RestartServiceStatusEvent)e;
        return new JsonReplyMessage
        {
            Action  = "ShowRestartStatus",
            Message = new { Status = evt.Success ? "OK" : "Failed" }
        };
    }
}
```

### Step 7 � Register everything in the setup classes

In `BusinessLogicModuleSetup.cs`:
```csharp
AddCommandHandler(typeof(RestartServiceCommand), new RestartServiceCommandHandler(actionReply));
```

In `Converter.cs` constructor:
```csharp
AddJsonActionHandler(new RestartServiceJsonActionHandler());
AddJsonWriter(new RestartServiceStatusJsonWriter());
```

That's it. No other files need to change.

---

## Quick Reference Card

```
Angular sends a message
    ? WebViewWrapper catches it
    ? ConnectionManager parses it (JSON ? RawMessage ? event)
    ? Converter translates it (event ? ICommand)
    ? BusinessLogicModule executes it (ICommand ? work + result event)
    ? Converter translates reply (result event ? JsonReplyMessage)
    ? ConnectionManager sends it (JsonReplyMessage ? JSON string)
    ? WebViewWrapper delivers it to Angular
```
