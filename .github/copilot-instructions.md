## graphify

For any question about this repo's architecture, structure, components, or how to add/modify/find
code, your **first tool call must be** to read `graphify-out/GRAPH_REPORT.md` (if it exists).

Triggers: "how do I…", "where is…", "what does … do", "add/modify a <component>",
"explain the architecture", or anything that depends on how files or classes relate.

After reading the report (and `graphify-out/wiki/index.md` for deep questions), answer from the
graph. Only read source files when (a) modifying/debugging specific code, (b) the graph lacks
the needed detail, or (c) the graph is missing or stale.

Type `/graphify` in Copilot Chat to build or update the graph.

## graphify — per-package graphs

This repository uses **package-level** knowledge graphs. The composite index is at
`graphify-out/GRAPH_REPORT.md`. Each top-level package also has its own graph:

| Package | Report |
|---------|--------|
| BusinessLogicModule | graphify-out/BusinessLogicModule/GRAPH_REPORT.md |
| ConfigurationModule | graphify-out/ConfigurationModule/GRAPH_REPORT.md |
| ConnectionModule | graphify-out/ConnectionModule/GRAPH_REPORT.md |
| ConverterModule | graphify-out/ConverterModule/GRAPH_REPORT.md |
| Infrastructure | graphify-out/Infrastructure/GRAPH_REPORT.md |
| LoggingModule | graphify-out/LoggingModule/GRAPH_REPORT.md |
| Pms_GuidanceGUI.exe | graphify-out/Pms_GuidanceGUI.exe/GRAPH_REPORT.md |
| Pms_GuidanceGUI.Tests | graphify-out/Pms_GuidanceGUI.Tests/GRAPH_REPORT.md |
| Upgrade | graphify-out/Upgrade/GRAPH_REPORT.md |

Workflow:
1. Read `graphify-out/GRAPH_REPORT.md` first (cross-package index).
2. For questions about one module, read that module's `GRAPH_REPORT.md`.
3. Deep queries: `python -m graphify query "<question>" --graph graphify-out/<pkg>/graph.json`
4. Cross-module queries: use the merged graph `graphify-out/merged.json`
5. After editing code: `bash scripts/graphify.sh PKG=<name>` to refresh that package's graph.

## graphify — MCP server

An MCP server is configured in `.mcp.json` (server name: `graphify`).
It serves `graphify-out/merged.json` with tools: `query_graph`, `get_node`, `get_neighbors`, `shortest_path`.
Start: `make graphify-mcp` or `python -m graphify.serve graphify-out/merged.json`
Prerequisite: `pip install "graphifyy[mcp]"`
