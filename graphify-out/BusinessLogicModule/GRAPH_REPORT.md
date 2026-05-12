# Graph Report - BusinessLogicModule  (2026-05-12)

## Corpus Check
- 18 files · ~3,304 words
- Verdict: corpus is large enough that graph structure adds value.

## Summary
- 94 nodes · 81 edges · 23 communities (17 shown, 6 thin omitted)
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
- [[_COMMUNITY_Community 10|Community 10]]
- [[_COMMUNITY_Community 11|Community 11]]
- [[_COMMUNITY_Community 12|Community 12]]

## God Nodes (most connected - your core abstractions)
1. `BusinessLogicModuleSetup` - 11 edges
2. `AbstractCommandHandler` - 6 edges
3. `ActionReplyHandler` - 5 edges
4. `InstallSoftwareCommandHandler` - 5 edges
5. `VerifyInstallationPrerequisitesCommandHandler` - 5 edges
6. `CloseAppCommandHandler` - 4 edges
7. `UIAppStartedCommandHandler` - 4 edges
8. `LogActionCommandHandler` - 3 edges
9. `IActionReplyPrivate` - 2 edges
10. `ILogger` - 2 edges

## Surprising Connections (you probably didn't know these)
- `BusinessLogicModuleSetup` --references--> `ILogger`  [EXTRACTED]
  BusinessLogicModuleSetup.cs → CommandHandlers/AbstractCommandHandler.cs
- `ActionReplyHandler` --inherits--> `IActionReplyPrivate`  [EXTRACTED]
  ActionReplyHandler.cs → CommandHandlers/AbstractCommandHandler.cs
- `InstallSoftwareCommandHandler` --references--> `IConfigurationProvider`  [EXTRACTED]
  CommandHandlers/InstallSoftwareCommandHandler.cs → CommandHandlers/VerifyInstallationPrerequisitesCommandHandler.cs

## Communities (23 total, 6 thin omitted)

### Community 0 - "Community 0"
Cohesion: 0.12
Nodes (11): BusinessLogicModule.Commands, CloseAppCommand, BusinessLogicModule.Commands, InstallSoftwareCommand, BusinessLogicModule.Commands, LogCommand, BusinessLogicModule.Commands, UIAppStartedCommand (+3 more)

### Community 1 - "Community 1"
Cohesion: 0.17
Nodes (6): bool, BusinessLogicModule, BusinessLogicModuleSetup, Dictionary, IBusinessLogicModule, ICloseApplicationRequestSource

### Community 2 - "Community 2"
Cohesion: 0.2
Nodes (6): AbstractCommandHandler, Action, BusinessLogicModule, CloseAppCommandHandler, BusinessLogicModule, LogActionCommandHandler

### Community 3 - "Community 3"
Cohesion: 0.33
Nodes (4): ActionReplyHandler, BusinessLogicModule, IActionReply, IActionReplyPrivate

### Community 4 - "Community 4"
Cohesion: 0.33
Nodes (4): AbstractCommandHandler, BusinessLogicModule, ICommandHandler, ILogger

### Community 5 - "Community 5"
Cohesion: 0.4
Nodes (3): BusinessLogicModule, InstallSoftwareCommandHandler, IConfigurationProvider

### Community 6 - "Community 6"
Cohesion: 0.4
Nodes (3): BusinessLogicModule, UIAppStartedCommandHandler, ISystemLanguageProvider

## Knowledge Gaps
- **26 isolated node(s):** `BusinessLogicModule`, `BusinessLogicModule`, `Dictionary`, `bool`, `BusinessLogicModule` (+21 more)
  These have ≤1 connection - possible missing edges or undocumented components.
- **6 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `BusinessLogicModuleSetup` connect `Community 1` to `Community 4`?**
  _High betweenness centrality (0.049) - this node is a cross-community bridge._
- **Why does `AbstractCommandHandler` connect `Community 4` to `Community 3`?**
  _High betweenness centrality (0.047) - this node is a cross-community bridge._
- **Why does `ILogger` connect `Community 4` to `Community 1`?**
  _High betweenness centrality (0.036) - this node is a cross-community bridge._
- **What connects `BusinessLogicModule`, `BusinessLogicModule`, `Dictionary` to the rest of the system?**
  _26 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Community 0` be split into smaller, more focused modules?**
  _Cohesion score 0.12 - nodes in this community are weakly interconnected._