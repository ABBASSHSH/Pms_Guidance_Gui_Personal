# Graph Report - Pms_GuidanceGUI.exe  (2026-05-12)

## Corpus Check
- 5 files · ~4,277 words
- Verdict: corpus is large enough that graph structure adds value.

## Summary
- 103 nodes · 99 edges · 11 communities
- Extraction: 100% EXTRACTED · 0% INFERRED · 0% AMBIGUOUS
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `9d5b08a5`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- [[_COMMUNITY_Community 0|Community 0]]
- [[_COMMUNITY_Community 1|Community 1]]
- [[_COMMUNITY_Community 2|Community 2]]
- [[_COMMUNITY_Community 3|Community 3]]
- [[_COMMUNITY_Community 4|Community 4]]
- [[_COMMUNITY_Community 5|Community 5]]
- [[_COMMUNITY_Community 6|Community 6]]
- [[_COMMUNITY_Community 7|Community 7]]
- [[_COMMUNITY_Community 8|Community 8]]
- [[_COMMUNITY_Community 9|Community 9]]

## God Nodes (most connected - your core abstractions)
1. `HybridWebApps � Complete Solution Guide` - 13 edges
2. `WebViewWrapper` - 10 edges
3. `MainWindow` - 8 edges
4. `How to Add a New Action (Step-by-Step)` - 8 edges
5. `Project 4 � ConverterModule (The Translator)` - 7 edges
6. `Project 3 � ConnectionModule (The Message Bridge)` - 5 edges
7. `Project 5 � BusinessLogicModule (The Brain)` - 5 edges
8. `Project 2 � Pms_GuidanceGUI (The Shell / Entry Point)` - 4 edges
9. `WebViewWrapper.cs � The Browser Host` - 4 edges
10. `Project 1 � Infrastructure (The Shared Language)` - 3 edges

## Surprising Connections (you probably didn't know these)
- `WebViewWrapper` --inherits--> `IWebViewWrapper`  [EXTRACTED]
  WebViewWrapper.cs →   _Bridges community 4 → community 3_

## Communities (11 total, 0 thin omitted)

### Community 0 - "Community 0"
Cohesion: 0.12
Nodes (15): code:block1 (Solution), code:block2 (Pms_GuidanceGUI), code:block23 (???????????????????????????????????????????????????????????), code:block32 (Angular sends a message), Complete End-to-End Flow, Data Classes, Design Patterns Reference, HybridWebApps � Complete Solution Guide (+7 more)

### Community 1 - "Community 1"
Cohesion: 0.12
Nodes (16): code:csharp (// ConverterModule/JsonMessage/RestartServiceMessage.cs), code:csharp (// BusinessLogicModule/Commands/RestartServiceCommand.cs), code:csharp (// ConverterModule/RestartServiceJsonActionHandler.cs), code:csharp (// BusinessLogicModule/RestartServiceCommandHandler.cs), code:csharp (// BusinessLogicModule/EventArgs/RestartServiceStatusEvent.c), code:csharp (// ConverterModule/JsonWriter/RestartServiceStatusJsonWriter), code:csharp (AddCommandHandler(typeof(RestartServiceCommand), new Restart), code:csharp (AddJsonActionHandler(new RestartServiceJsonActionHandler());) (+8 more)

### Community 2 - "Community 2"
Cohesion: 0.15
Nodes (13): code:csharp (internal interface IJsonActionHandler), code:csharp (internal interface IJsonWriter), code:block13 (JsonActionHandler), code:block16 (Inbound payload JSON:), code:block17 (LogStatusEvent(IsLogged: true) arrives), Concrete Handler: LogJsonActionHandler, Concrete Writer: ShowLogStatusJsonWriter, Files (+5 more)

### Community 3 - "Community 3"
Cohesion: 0.18
Nodes (5): bool, object, Pms_GuidanceGUI, WebViewWrapper, WebView2

### Community 4 - "Community 4"
Cohesion: 0.2
Nodes (6): IApplicationLifecycleManager, ILogger, IWebViewWrapper, MainWindow, WebAppWrapper, Window

### Community 5 - "Community 5"
Cohesion: 0.2
Nodes (10): BusinessLogicModuleSetup.cs � The Command Dispatcher, code:block18 (BusinessLogicModuleSetup), code:block19 (receives LogCommand), code:csharp (// What it does when LogCommand arrives:), code:block21 (ActionReplyHandler), code:block22 (Outside world (Converter)         Inside world (LogActionCom), Files, LogActionCommandHandler.cs � The Worker (+2 more)

### Community 6 - "Community 6"
Cohesion: 0.22
Nodes (9): code:csharp (connectionManager.SendMessage("ShowLogMessage", payloadObjec), code:json ({), code:block8 (RawMessage), code:block9 (Raw JSON string arrives from browser), Files, Inbound Flow (Angular ? C#), Outbound Flow (C# ? Angular), Project 3 � ConnectionModule (The Message Bridge) (+1 more)

### Community 7 - "Community 7"
Cohesion: 0.25
Nodes (8): code:block3 (MainWindow constructor), code:block4 (WebView2 (Chromium browser inside WPF)), code:csharp (myWebView.CoreWebView2.PostWebMessageAsJson(jsonString);), code:block6 (https://pmsGuidanceFrontendApp/index.html), Files, MainWindow Startup Sequence, Project 2 � Pms_GuidanceGUI (The Shell / Entry Point), WebViewWrapper.cs � The Browser Host

### Community 8 - "Community 8"
Cohesion: 0.4
Nodes (5): code:block14 (ConnectionManager raises MessageReceived("LogMessage", paylo), code:block15 (BusinessLogicModule raises OnCommandHandled(LogStatusEvent)), Converter.cs � The Orchestrator, Inbound path through Converter:, Outbound path through Converter:

### Community 9 - "Community 9"
Cohesion: 0.5
Nodes (3): Application, App, WebAppWrapper

## Knowledge Gaps
- **48 isolated node(s):** `WebAppWrapper`, `WebAppWrapper`, `ILogger`, `IApplicationLifecycleManager`, `Pms_GuidanceGUI` (+43 more)
  These have ≤1 connection - possible missing edges or undocumented components.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `HybridWebApps � Complete Solution Guide` connect `Community 0` to `Community 1`, `Community 2`, `Community 5`, `Community 6`, `Community 7`?**
  _High betweenness centrality (0.477) - this node is a cross-community bridge._
- **Why does `Project 4 � ConverterModule (The Translator)` connect `Community 2` to `Community 0`, `Community 8`?**
  _High betweenness centrality (0.217) - this node is a cross-community bridge._
- **Why does `How to Add a New Action (Step-by-Step)` connect `Community 1` to `Community 0`?**
  _High betweenness centrality (0.196) - this node is a cross-community bridge._
- **What connects `WebAppWrapper`, `WebAppWrapper`, `ILogger` to the rest of the system?**
  _48 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Community 0` be split into smaller, more focused modules?**
  _Cohesion score 0.12 - nodes in this community are weakly interconnected._
- **Should `Community 1` be split into smaller, more focused modules?**
  _Cohesion score 0.12 - nodes in this community are weakly interconnected._