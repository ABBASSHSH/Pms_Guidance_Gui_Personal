# Graph Report - Pms_GuidanceGUI.Tests  (2026-05-12)

## Corpus Check
- 11 files · ~13,440 words
- Verdict: corpus is large enough that graph structure adds value.

## Summary
- 372 nodes · 547 edges · 14 communities (5 shown, 9 thin omitted)
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

## God Nodes (most connected - your core abstractions)
1. `BusinessLogicModuleTests` - 52 edges
2. `BackendFlowComponentTests` - 51 edges
3. `ConverterTests` - 44 edges
4. `ConnectionManagerTests` - 38 edges
5. `LoggingModuleTests_Legacy` - 33 edges
6. `LoggingModuleTests` - 33 edges
7. `ApplicationLifecycleManagerTests` - 23 edges
8. `ConfigurationProviderTests` - 21 edges
9. `JsonWriterManagerTests` - 20 edges
10. `JsonActionHandlerManagerTests` - 15 edges

## Surprising Connections (you probably didn't know these)
- `BackendFlowComponentTests` --references--> `List`  [EXTRACTED]
  Component/BackendFlowComponentTests.cs → Unit/ConverterTests.cs
- `BackendFlowComponentTests` --references--> `ConnectionManager`  [EXTRACTED]
  Component/BackendFlowComponentTests.cs → Unit/ConnectionManagerTests.cs
- `BackendFlowComponentTests` --references--> `BusinessLogicModuleSetup`  [EXTRACTED]
  Component/BackendFlowComponentTests.cs → Unit/BusinessLogicModuleTests.cs
- `BackendFlowComponentTests` --references--> `Converter`  [EXTRACTED]
  Component/BackendFlowComponentTests.cs → Unit/ConverterTests.cs
- `BackendFlowComponentTests` --references--> `Mock`  [EXTRACTED]
  Component/BackendFlowComponentTests.cs → Unit/JsonWriterManagerTests.cs

## Communities (14 total, 9 thin omitted)

### Community 2 - "Community 2"
Cohesion: 0.07
Nodes (4): Converter, List, ConverterTests, Pms_GuidanceGUI.Tests.Unit

### Community 7 - "Community 7"
Cohesion: 0.11
Nodes (5): JsonActionHandlerManager, Mock, ConcreteJsonActionHandlerTests, JsonActionHandlerManagerTests, Pms_GuidanceGUI.Tests.Unit

### Community 8 - "Community 8"
Cohesion: 0.13
Nodes (3): string, ConfigurationProviderTests, Pms_GuidanceGUI.Tests.Unit

### Community 10 - "Community 10"
Cohesion: 0.12
Nodes (7): OutboundMessage, BadSerializationMessage, Pms_GuidanceGUI.Tests.Unit, TestOutboundMessage, ConcreteJsonWriterTests, Pms_GuidanceGUI.Tests.Unit, StubOutboundMessage

### Community 13 - "Community 13"
Cohesion: 0.5
Nodes (3): ICommand, Pms_GuidanceGUI.Tests.Unit, UnregisteredCommand

## Knowledge Gaps
- **13 isolated node(s):** `Pms_GuidanceGUI.Tests.Component`, `Pms_GuidanceGUI.Tests.Unit`, `Pms_GuidanceGUI.Tests.Unit`, `Pms_GuidanceGUI.Tests.Unit`, `Pms_GuidanceGUI.Tests.Unit` (+8 more)
  These have ≤1 connection - possible missing edges or undocumented components.
- **9 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `Mock` connect `Community 7` to `Community 0`, `Community 1`, `Community 2`, `Community 3`, `Community 8`, `Community 9`?**
  _High betweenness centrality (0.635) - this node is a cross-community bridge._
- **Why does `ConfigurationProviderTests` connect `Community 8` to `Community 7`?**
  _High betweenness centrality (0.359) - this node is a cross-community bridge._
- **Why does `string` connect `Community 8` to `Community 4`, `Community 5`?**
  _High betweenness centrality (0.302) - this node is a cross-community bridge._
- **What connects `Pms_GuidanceGUI.Tests.Component`, `Pms_GuidanceGUI.Tests.Unit`, `Pms_GuidanceGUI.Tests.Unit` to the rest of the system?**
  _13 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Community 0` be split into smaller, more focused modules?**
  _Cohesion score 0.04 - nodes in this community are weakly interconnected._
- **Should `Community 1` be split into smaller, more focused modules?**
  _Cohesion score 0.09 - nodes in this community are weakly interconnected._
- **Should `Community 2` be split into smaller, more focused modules?**
  _Cohesion score 0.07 - nodes in this community are weakly interconnected._