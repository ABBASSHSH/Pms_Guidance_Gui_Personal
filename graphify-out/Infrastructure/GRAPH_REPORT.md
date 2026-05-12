# Graph Report - Infrastructure  (2026-05-12)

## Corpus Check
- 14 files · ~3,200 words
- Verdict: corpus is large enough that graph structure adds value.

## Summary
- 72 nodes · 62 edges · 13 communities (3 shown, 10 thin omitted)
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
1. `ApplicationLifecycleManager` - 12 edges
2. `IConfigurationProvider` - 5 edges
3. `ILogger` - 5 edges
4. `IApplicationLifecycleManager` - 4 edges
5. `IWebViewWrapper` - 4 edges
6. `IBusinessLogicModule` - 3 edges
7. `ILifeCycle` - 3 edges
8. `ICommandHandler` - 2 edges
9. `ISystemLanguageProvider` - 2 edges
10. `MessageReceivedEventArgs` - 2 edges

## Surprising Connections (you probably didn't know these)
- `IWebViewWrapper` --inherits--> `ILifeCycle`  [EXTRACTED]
  IWebViewWrapper.cs →   _Bridges community 1 → community 5_

## Communities (13 total, 10 thin omitted)

### Community 0 - "Community 0"
Cohesion: 0.16
Nodes (8): Action, bool, HashSet, IApplicationLifecycleManager, ICloseApplicationRequestSource, ApplicationLifecycleManager, Infrastructure, List

### Community 1 - "Community 1"
Cohesion: 0.2
Nodes (5): ILifeCycle, IApplicationLifecycleManager, Infrastructure, IBusinessLogicModule, Infrastructure

### Community 8 - "Community 8"
Cohesion: 0.5
Nodes (3): EventArgs, Infrastructure, MessageReceivedEventArgs

## Knowledge Gaps
- **23 isolated node(s):** `Infrastructure`, `List`, `HashSet`, `bool`, `ICloseApplicationRequestSource` (+18 more)
  These have ≤1 connection - possible missing edges or undocumented components.
- **10 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `IWebViewWrapper` connect `Community 5` to `Community 1`?**
  _High betweenness centrality (0.018) - this node is a cross-community bridge._
- **What connects `Infrastructure`, `List`, `HashSet` to the rest of the system?**
  _23 weakly-connected nodes found - possible documentation gaps or missing edges._