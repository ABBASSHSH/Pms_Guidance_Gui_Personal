# Graph Report - LoggingModule  (2026-05-12)

## Corpus Check
- 5 files · ~1,313 words
- Verdict: corpus is large enough that graph structure adds value.

## Summary
- 27 nodes · 23 edges · 5 communities (2 shown, 3 thin omitted)
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
1. `SourceLogger` - 7 edges
2. `FileLogWriter` - 5 edges
3. `AppLoggerSetup` - 2 edges
4. `ILogWriter` - 2 edges
5. `ILogWriter` - 2 edges
6. `LogEntryFormatter` - 2 edges
7. `LoggingModule` - 1 edges
8. `LoggingModule` - 1 edges
9. `string` - 1 edges
10. `object` - 1 edges

## Surprising Connections (you probably didn't know these)
- `FileLogWriter` --inherits--> `ILogWriter`  [EXTRACTED]
  FileLogWriter.cs → SourceLogger.cs

## Communities (5 total, 3 thin omitted)

### Community 0 - "Community 0"
Cohesion: 0.25
Nodes (3): ILogger, LoggingModule, SourceLogger

### Community 1 - "Community 1"
Cohesion: 0.29
Nodes (5): ILogWriter, FileLogWriter, LoggingModule, object, string

## Knowledge Gaps
- **7 isolated node(s):** `LoggingModule`, `LoggingModule`, `string`, `object`, `LoggingModule` (+2 more)
  These have ≤1 connection - possible missing edges or undocumented components.
- **3 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `SourceLogger` connect `Community 0` to `Community 1`?**
  _High betweenness centrality (0.212) - this node is a cross-community bridge._
- **Why does `ILogWriter` connect `Community 1` to `Community 0`?**
  _High betweenness centrality (0.148) - this node is a cross-community bridge._
- **What connects `LoggingModule`, `LoggingModule`, `string` to the rest of the system?**
  _7 weakly-connected nodes found - possible documentation gaps or missing edges._