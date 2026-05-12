## graphify

This project has a knowledge graph at graphify-out/ with god nodes, community structure, and cross-file relationships.

Rules:
- ALWAYS read graphify-out/GRAPH_REPORT.md before reading any source files, running grep/glob searches, or answering codebase questions. The graph is your primary map of the codebase.
- IF graphify-out/wiki/index.md EXISTS, navigate it instead of reading raw files
- For cross-module "how does X relate to Y" questions, prefer `graphify query "<question>"`, `graphify path "<A>" "<B>"`, or `graphify explain "<concept>"` over grep — these traverse the graph's EXTRACTED + INFERRED edges instead of scanning files
- After modifying code, run `graphify update .` to keep the graph current (AST-only, no API cost).

## graphify — per-package graphs

Per-package graphs are stored in graphify-out/<package>/. Read the module-specific
GRAPH_REPORT.md when working in that package, then use graphify query for deeper traversal.
The composite index at graphify-out/GRAPH_REPORT.md lists all packages.

Rebuild: `bash scripts/graphify.sh` or `.\scripts\Invoke-Graphify.ps1` (Windows).

## graphify — cross-package merged graph

For cross-module architecture questions use `graphify-out/merged.json`:
```
python -m graphify query "<question>" --graph graphify-out/merged.json
```
Rebuild: `bash scripts/graphify.sh --merge-only`

## graphify — MCP server

An MCP server is configured in `.mcp.json`.
Start: `python -m graphify.serve graphify-out/merged.json`
Tools: `query_graph`, `get_node`, `get_neighbors`, `shortest_path`
