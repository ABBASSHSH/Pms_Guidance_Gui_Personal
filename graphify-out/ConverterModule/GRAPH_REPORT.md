# Graph Report - ConverterModule  (2026-05-12)

## Corpus Check
- 21 files · ~3,929 words
- Verdict: corpus is large enough that graph structure adds value.

## Summary
- 103 nodes · 94 edges · 15 communities (7 shown, 8 thin omitted)
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
- [[_COMMUNITY_Community 13|Community 13]]
- [[_COMMUNITY_Community 14|Community 14]]

## God Nodes (most connected - your core abstractions)
1. `Converter` - 13 edges
2. `JsonActionHandlerManager` - 6 edges
3. `JsonWriterManager` - 6 edges
4. `AbstractJsonActionHandler` - 6 edges
5. `ShowSystemLanguageJsonWriter` - 5 edges
6. `ShowVerifyInstallationPrerequisitesJsonWriter` - 5 edges
7. `ILogger` - 4 edges
8. `IJsonWriter` - 3 edges
9. `UIAppStartedJsonActionHandler` - 3 edges
10. `CloseAppJsonActionHandler` - 3 edges

## Surprising Connections (you probably didn't know these)
- `Converter` --references--> `ILogger`  [EXTRACTED]
  Converter.cs → JsonActionHandlers/AbstractJsonActionHandler.cs
- `JsonActionHandlerManager` --references--> `ILogger`  [EXTRACTED]
  JsonActionHandlerManager.cs → JsonActionHandlers/AbstractJsonActionHandler.cs
- `JsonWriterManager` --references--> `ILogger`  [EXTRACTED]
  JsonWriterManager.cs → JsonActionHandlers/AbstractJsonActionHandler.cs
- `JsonActionHandlerManager` --references--> `List`  [EXTRACTED]
  JsonActionHandlerManager.cs → JsonWriterManager.cs
- `ShowSystemLanguageJsonWriter` --references--> `Type`  [EXTRACTED]
  JsonWriter/ShowSystemLanguageJsonWriter.cs → JsonWriter/ShowVerifyInstallationPrerequisitesJsonWriter.cs

## Communities (15 total, 8 thin omitted)

### Community 0 - "Community 0"
Cohesion: 0.12
Nodes (9): AbstractJsonActionHandler, ConverterModule, UIAppStartedJsonActionHandler, ConverterModule, InstallSoftwareJsonActionHandler, ConverterModule, LogJsonActionHandler, ConverterModule (+1 more)

### Community 1 - "Community 1"
Cohesion: 0.14
Nodes (8): bool, Converter, ConverterModule, IBusinessLogicModule, IConnectionManager, IConverter, JsonActionHandlerManager, JsonWriterManager

### Community 2 - "Community 2"
Cohesion: 0.15
Nodes (5): ConverterModule, JsonActionHandlerManager, ConverterModule, JsonWriterManager, List

### Community 3 - "Community 3"
Cohesion: 0.18
Nodes (6): IJsonWriter, ConverterModule.JsonWriter, ShowSystemLanguageJsonWriter, ConverterModule.JsonWriter, ShowVerifyInstallationPrerequisitesJsonWriter, Type

### Community 4 - "Community 4"
Cohesion: 0.29
Nodes (4): IJsonActionHandler, ILogger, AbstractJsonActionHandler, ConverterModule

### Community 5 - "Community 5"
Cohesion: 0.29
Nodes (5): ConverterModule.JsonMessage, ShowSystemLanguageMessage, ConverterModule.JsonMessage, ShowVerifyInstallationPrerequisitesMessage, OutboundMessage

### Community 7 - "Community 7"
Cohesion: 0.5
Nodes (3): ConverterModule, IConverter, ILifeCycle

## Knowledge Gaps
- **31 isolated node(s):** `ConverterModule`, `JsonWriterManager`, `IBusinessLogicModule`, `IConnectionManager`, `JsonActionHandlerManager` (+26 more)
  These have ≤1 connection - possible missing edges or undocumented components.
- **8 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `ILogger` connect `Community 4` to `Community 1`, `Community 2`?**
  _High betweenness centrality (0.076) - this node is a cross-community bridge._
- **Why does `Converter` connect `Community 1` to `Community 4`?**
  _High betweenness centrality (0.068) - this node is a cross-community bridge._
- **What connects `ConverterModule`, `JsonWriterManager`, `IBusinessLogicModule` to the rest of the system?**
  _31 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Community 0` be split into smaller, more focused modules?**
  _Cohesion score 0.12 - nodes in this community are weakly interconnected._
- **Should `Community 1` be split into smaller, more focused modules?**
  _Cohesion score 0.14 - nodes in this community are weakly interconnected._