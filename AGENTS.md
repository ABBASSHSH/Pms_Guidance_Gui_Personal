## graphify

This project has a knowledge graph at graphify-out/ with god nodes, community structure, and cross-file relationships.

Rules:
- ALWAYS read graphify-out/GRAPH_REPORT.md before reading any source files, running grep/glob searches, or answering codebase questions. The graph is your primary map of the codebase.
- IF graphify-out/wiki/index.md EXISTS, navigate it instead of reading raw files
- For cross-module "how does X relate to Y" questions, prefer `graphify query "<question>"`, `graphify path "<A>" "<B>"`, or `graphify explain "<concept>"` over grep — these traverse the graph's EXTRACTED + INFERRED edges instead of scanning files
- After modifying code, run `graphify update .` to keep the graph current (AST-only, no API cost).

## graphify — per-package graphs

This repository uses **package-level** knowledge graphs. `graphify-out/GRAPH_REPORT.md` is the composite index; each package also has its own graph:

- BusinessLogicModule  → graphify-out/BusinessLogicModule/GRAPH_REPORT.md
- ConfigurationModule  → graphify-out/ConfigurationModule/GRAPH_REPORT.md
- ConnectionModule     → graphify-out/ConnectionModule/GRAPH_REPORT.md
- ConverterModule      → graphify-out/ConverterModule/GRAPH_REPORT.md
- Infrastructure       → graphify-out/Infrastructure/GRAPH_REPORT.md
- LoggingModule        → graphify-out/LoggingModule/GRAPH_REPORT.md
- Pms_GuidanceGUI.exe  → graphify-out/Pms_GuidanceGUI.exe/GRAPH_REPORT.md
- Pms_GuidanceGUI.Tests→ graphify-out/Pms_GuidanceGUI.Tests/GRAPH_REPORT.md
- Upgrade              → graphify-out/Upgrade/GRAPH_REPORT.md

Rebuild: `bash scripts/graphify.sh` (Linux/macOS/Git Bash) or `.\scripts\Invoke-Graphify.ps1` (Windows PowerShell).

## graphify — cross-package merged graph

For cross-module questions use `graphify-out/merged.json` (whole-repo graph):
```
python -m graphify query "<question>" --graph graphify-out/merged.json
```
Rebuild merged graph: `bash scripts/graphify.sh --merge-only`

## graphify — MCP server

The graph is exposed as an MCP server via `.mcp.json`.
Start with: `python -m graphify.serve graphify-out/merged.json`
Tools: `query_graph`, `get_node`, `get_neighbors`, `shortest_path`
