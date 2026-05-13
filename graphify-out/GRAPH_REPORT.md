# Graph Report - HybridWebApps  (2026-05-12)

## Corpus Check
- 143 files · ~68,350 words
- Verdict: corpus is large enough that graph structure adds value.

## Summary
- 455 nodes · 642 edges · 18 communities (11 shown, 7 thin omitted)
- Extraction: 100% EXTRACTED · 0% INFERRED · 0% AMBIGUOUS
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `6664fd4f`
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
- [[_COMMUNITY_Community 15|Community 15]]
- [[_COMMUNITY_Community 16|Community 16]]
- [[_COMMUNITY_Community 17|Community 17]]

## God Nodes (most connected - your core abstractions)
1. `BackendFlowComponentTests` - 51 edges
2. `ConverterTests` - 47 edges
3. `BusinessLogicModuleTests` - 40 edges
4. `ConnectionManagerTests` - 40 edges
5. `CommandHandlerTests` - 33 edges
6. `LoggingModuleTests` - 33 edges
7. `ApplicationLifecycleManagerTests` - 29 edges
8. `ConfigurationProviderTests` - 23 edges
9. `JsonWriterManagerTests` - 20 edges
10. `JsonActionHandlerManagerTests` - 16 edges

## Surprising Connections (you probably didn't know these)
- `JsonWriterManagerTests` --references--> `JsonWriterManager`  [EXTRACTED]
  Unit/JsonWriterManagerTests.cs → ConverterModule/Converter.cs
- `BackendFlowComponentTests` --references--> `ConnectionManager`  [EXTRACTED]
  Component/BackendFlowComponentTests.cs → Pms_GuidanceGUI.Tests/Unit/ConnectionManagerTests.cs
- `BackendFlowComponentTests` --references--> `List`  [EXTRACTED]
  Component/BackendFlowComponentTests.cs → Pms_GuidanceGUI.Tests/Unit/ConverterTests.cs
- `BackendFlowComponentTests` --references--> `Converter`  [EXTRACTED]
  Component/BackendFlowComponentTests.cs → Pms_GuidanceGUI.Tests/Unit/ConverterTests.cs
- `BusinessLogicModuleSetup` --inherits--> `IBusinessLogicModule`  [EXTRACTED]
  BusinessLogicModule/BusinessLogicModuleSetup.cs → ConverterModule/Converter.cs

## Communities (18 total, 7 thin omitted)

### Community 0 - "Community 0"
Cohesion: 0.06
Nodes (4): Converter, List, ConverterTests, Pms_GuidanceGUI.Tests.Unit

### Community 2 - "Community 2"
Cohesion: 0.05
Nodes (4): ICommand, BusinessLogicModuleTests, Pms_GuidanceGUI.Tests.Unit, UnregisteredCommand

### Community 3 - "Community 3"
Cohesion: 0.06
Nodes (18): bool, BusinessLogicModule, BusinessLogicModuleSetup, ConnectionManager, ConnectionModule, Converter, ConverterModule, Dictionary (+10 more)

### Community 5 - "Community 5"
Cohesion: 0.06
Nodes (3): BusinessLogicModuleSetup, CommandHandlerTests, Pms_GuidanceGUI.Tests.Unit

### Community 6 - "Community 6"
Cohesion: 0.07
Nodes (4): ICloseApplicationRequestSource, ApplicationLifecycleManagerTests, Pms_GuidanceGUI.Tests.Unit, TestCloseApplicationRequestSource

### Community 8 - "Community 8"
Cohesion: 0.09
Nodes (4): ConcreteJsonWriterTests, JsonWriterManagerTests, Pms_GuidanceGUI.Tests.Unit, StubOutboundMessage

### Community 9 - "Community 9"
Cohesion: 0.11
Nodes (4): Mock, ConcreteJsonActionHandlerTests, JsonActionHandlerManagerTests, Pms_GuidanceGUI.Tests.Unit

### Community 11 - "Community 11"
Cohesion: 0.13
Nodes (7): ConfigurationModule, GuidanceConfigurationProvider, ConfigurationModule, SystemLanguageProvider, ILifeCycle, ISystemLanguageProvider, string

### Community 12 - "Community 12"
Cohesion: 0.21
Nodes (7): AbstractCommandHandler, BusinessLogicModule, InstallSoftwareCommandHandler, BusinessLogicModule, VerifyInstallationPrerequisitesCommandHandler, IConfigurationProvider, int

### Community 14 - "Community 14"
Cohesion: 0.53
Nodes (4): OutboundMessage, BadSerializationMessage, Pms_GuidanceGUI.Tests.Unit, TestOutboundMessage

### Community 16 - "Community 16"
Cohesion: 0.4
Nodes (3): AbstractJsonActionHandler, ConverterModule, UIAppStartedJsonActionHandler

## Knowledge Gaps
- **18 isolated node(s):** `BusinessLogicModule`, `BusinessLogicModule`, `BusinessLogicModule`, `ConfigurationModule`, `ConfigurationModule` (+13 more)
  These have ≤1 connection - possible missing edges or undocumented components.
- **7 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `Mock` connect `Community 9` to `Community 0`, `Community 1`, `Community 2`, `Community 4`, `Community 5`, `Community 8`, `Community 10`?**
  _High betweenness centrality (0.631) - this node is a cross-community bridge._
- **Why does `ConfigurationProviderTests` connect `Community 10` to `Community 9`, `Community 11`?**
  _High betweenness centrality (0.278) - this node is a cross-community bridge._
- **Why does `string` connect `Community 11` to `Community 10`, `Community 7`?**
  _High betweenness centrality (0.249) - this node is a cross-community bridge._
- **What connects `BusinessLogicModule`, `BusinessLogicModule`, `BusinessLogicModule` to the rest of the system?**
  _18 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Community 0` be split into smaller, more focused modules?**
  _Cohesion score 0.06 - nodes in this community are weakly interconnected._
- **Should `Community 1` be split into smaller, more focused modules?**
  _Cohesion score 0.09 - nodes in this community are weakly interconnected._
- **Should `Community 2` be split into smaller, more focused modules?**
  _Cohesion score 0.05 - nodes in this community are weakly interconnected._