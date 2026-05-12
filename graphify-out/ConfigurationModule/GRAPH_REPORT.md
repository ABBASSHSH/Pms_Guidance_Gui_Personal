# Graph Report - ConfigurationModule  (2026-05-12)

## Corpus Check
- 2 files · ~725 words
- Verdict: corpus is large enough that graph structure adds value.

## Summary
- 21 nodes · 24 edges · 4 communities (2 shown, 2 thin omitted)
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

## God Nodes (most connected - your core abstractions)
1. `GuidanceConfigurationProvider` - 12 edges
2. `SystemLanguageProvider` - 7 edges
3. `ILogger` - 2 edges
4. `ConfigurationModule` - 1 edges
5. `string` - 1 edges
6. `Dictionary` - 1 edges
7. `ConfigurationModule` - 1 edges

## Surprising Connections (you probably didn't know these)
- `GuidanceConfigurationProvider` --references--> `ILogger`  [EXTRACTED]
  GuidanceConfigurationProvider.cs → SystemLanguageProvider.cs

## Communities (4 total, 2 thin omitted)

### Community 0 - "Community 0"
Cohesion: 0.24
Nodes (4): GuidanceConfigurationProvider, Dictionary, IConfigurationProvider, string

### Community 1 - "Community 1"
Cohesion: 0.33
Nodes (4): SystemLanguageProvider, ILifeCycle, ILogger, ISystemLanguageProvider

## Knowledge Gaps
- **4 isolated node(s):** `ConfigurationModule`, `string`, `Dictionary`, `ConfigurationModule`
  These have ≤1 connection - possible missing edges or undocumented components.
- **2 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `GuidanceConfigurationProvider` connect `Community 0` to `Community 1`, `Community 2`?**
  _High betweenness centrality (0.795) - this node is a cross-community bridge._
- **Why does `SystemLanguageProvider` connect `Community 1` to `Community 3`?**
  _High betweenness centrality (0.513) - this node is a cross-community bridge._
- **Why does `ILogger` connect `Community 1` to `Community 0`?**
  _High betweenness centrality (0.221) - this node is a cross-community bridge._
- **What connects `ConfigurationModule`, `string`, `Dictionary` to the rest of the system?**
  _4 weakly-connected nodes found - possible documentation gaps or missing edges._