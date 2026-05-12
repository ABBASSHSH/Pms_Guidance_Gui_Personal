## graphify

This project has a knowledge graph at graphify-out/ with god nodes, community structure, and cross-file relationships.

Rules:
- ALWAYS read graphify-out/GRAPH_REPORT.md before reading any source files, running grep/glob searches, or answering codebase questions. The graph is your primary map of the codebase.
- IF graphify-out/wiki/index.md EXISTS, navigate it instead of reading raw files
- For cross-module "how does X relate to Y" questions, prefer `graphify query "<question>"`, `graphify path "<A>" "<B>"`, or `graphify explain "<concept>"` over grep — these traverse the graph's EXTRACTED + INFERRED edges instead of scanning files
- After modifying code, run `graphify update .` to keep the graph current (AST-only, no API cost).

## graphify — per-package graphs

This repository uses **package-level** knowledge graphs. Each top-level package has its own graph:

| Package | Graph | Report |
|---------|-------|--------|
| BusinessLogicModule | graphify-out/BusinessLogicModule/graph.json | graphify-out/BusinessLogicModule/GRAPH_REPORT.md |
| ConfigurationModule | graphify-out/ConfigurationModule/graph.json | graphify-out/ConfigurationModule/GRAPH_REPORT.md |
| ConnectionModule | graphify-out/ConnectionModule/graph.json | graphify-out/ConnectionModule/GRAPH_REPORT.md |
| ConverterModule | graphify-out/ConverterModule/graph.json | graphify-out/ConverterModule/GRAPH_REPORT.md |
| Infrastructure | graphify-out/Infrastructure/graph.json | graphify-out/Infrastructure/GRAPH_REPORT.md |
| LoggingModule | graphify-out/LoggingModule/graph.json | graphify-out/LoggingModule/GRAPH_REPORT.md |
| Pms_GuidanceGUI.exe | graphify-out/Pms_GuidanceGUI.exe/graph.json | graphify-out/Pms_GuidanceGUI.exe/GRAPH_REPORT.md |
| Pms_GuidanceGUI.Tests | graphify-out/Pms_GuidanceGUI.Tests/graph.json | graphify-out/Pms_GuidanceGUI.Tests/GRAPH_REPORT.md |
| Upgrade | graphify-out/Upgrade/graph.json | graphify-out/Upgrade/GRAPH_REPORT.md |

Navigation strategy:
1. Read `graphify-out/GRAPH_REPORT.md` for a cross-package index.
2. For questions about a specific module, read that module's `GRAPH_REPORT.md` first.
3. Use `python -m graphify query "<question>" --graph graphify-out/<pkg>/graph.json` for deep traversal.
4. Rebuild after code changes: `bash scripts/graphify.sh PKG=<name>` or `python -m graphify update <pkg-dir>`.

## graphify — cross-package merged graph

A whole-repo graph merging all packages is at `graphify-out/merged.json`.
Use it for cross-module architecture questions:
```
python -m graphify query "how does ConverterModule connect to BusinessLogicModule" \
  --graph graphify-out/merged.json
python -m graphify path "IConverter" "ActionReplyHandler" \
  --graph graphify-out/merged.json
```
Rebuild the merged graph: `bash scripts/graphify.sh --merge-only`

## graphify — MCP server

The graph is also exposed as an MCP server (`.mcp.json` at repo root).
Server name: `graphify` → `graphify-out/merged.json`
Tools: `query_graph`, `get_node`, `get_neighbors`, `shortest_path`
Start: `make graphify-mcp` or `python -m graphify.serve graphify-out/merged.json`
Prerequisite: `pip install "graphifyy[mcp]"`
