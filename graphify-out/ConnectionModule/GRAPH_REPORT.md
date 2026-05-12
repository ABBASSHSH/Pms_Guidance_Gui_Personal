# Graph Report - ConnectionModule  (2026-05-12)

## Corpus Check
- 4 files · ~909 words
- Verdict: corpus is large enough that graph structure adds value.

## Summary
- 23 nodes · 20 edges · 5 communities (2 shown, 3 thin omitted)
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

## God Nodes (most connected - your core abstractions)
1. `ConnectionManager` - 10 edges
2. `IConnectionManager` - 3 edges
3. `ConnectionModule` - 1 edges
4. `IWebViewWrapper` - 1 edges
5. `ILogger` - 1 edges
6. `bool` - 1 edges
7. `ConnectionModule` - 1 edges
8. `ConnectionModule.JsonMessage` - 1 edges
9. `CallContext` - 1 edges
10. `ConnectionModule.JsonMessage` - 1 edges

## Surprising Connections (you probably didn't know these)
- None detected - all connections are within the same source files.

## Communities (5 total, 3 thin omitted)

### Community 0 - "Community 0"
Cohesion: 0.22
Nodes (5): bool, ConnectionManager, IConnectionManager, ILogger, IWebViewWrapper

### Community 1 - "Community 1"
Cohesion: 0.4
Nodes (3): ConnectionModule, IConnectionManager, ILifeCycle

## Knowledge Gaps
- **9 isolated node(s):** `ConnectionModule`, `IWebViewWrapper`, `ILogger`, `bool`, `ConnectionModule` (+4 more)
  These have ≤1 connection - possible missing edges or undocumented components.
- **3 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `ConnectionManager` connect `Community 0` to `Community 4`?**
  _High betweenness centrality (0.229) - this node is a cross-community bridge._
- **What connects `ConnectionModule`, `IWebViewWrapper`, `ILogger` to the rest of the system?**
  _9 weakly-connected nodes found - possible documentation gaps or missing edges._