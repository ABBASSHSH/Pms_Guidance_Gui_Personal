# =============================================================================
# Makefile — Knowledge graph targets for this repository
# =============================================================================
#
# Requires GNU Make. On Windows without Make:
#   • Install via Chocolatey:  choco install make
#   • Or use the PowerShell equivalent:  .\scripts\Invoke-Graphify.ps1
#
# Usage:
#   make graphify PKG=<name>       Build graph for one package
#   make graphify-all              Build graphs for all packages + cross-package merge
#   make graphify-merge            (Re)merge existing per-package graphs only
#   make graphify-mcp              Start the MCP server on graphify-out/merged.json
#   make graphify PKG=X SEMANTIC=1 Enable LLM semantic extraction
#   make graphify-clean            Remove all graphify-out directories
#   make help                      Show available targets
# =============================================================================

SHELL     := bash
GRAPHIFY  := scripts/graphify.sh
MERGED    := graphify-out/merged.json

# Pass SEMANTIC=1 from command line to enable LLM extraction
SEMANTIC_FLAG := $(if $(filter 1,$(SEMANTIC)),--semantic)

.DEFAULT_GOAL := help

.PHONY: graphify graphify-all graphify-merge graphify-mcp graphify-clean graphify-list help

## graphify PKG=<name>  Build knowledge graph for a single package
##                      Optionally set SEMANTIC=1 for LLM extraction
graphify:
ifndef PKG
	$(error PKG is required. Example: make graphify PKG=ConverterModule)
endif
	@bash $(GRAPHIFY) PKG=$(PKG) $(SEMANTIC_FLAG)

## graphify-all         Build graphs for all packages + cross-package merge
##                      Optionally set SEMANTIC=1 for LLM extraction
graphify-all:
	@bash $(GRAPHIFY) $(SEMANTIC_FLAG)

## graphify-merge       Merge existing per-package graphs into graphify-out/merged.json
##                      (no re-extraction; fast, no API cost)
graphify-merge:
	@bash $(GRAPHIFY) --merge-only

## graphify-mcp         Start the MCP stdio server on graphify-out/merged.json
##                      Exposes query_graph, get_node, get_neighbors, shortest_path tools
##                      Prerequisites: pip install "graphifyy[mcp]"
graphify-mcp: $(MERGED)
	@echo "[graphify] Starting MCP server on $(MERGED)"
	@echo "[graphify] Tools: query_graph, get_node, get_neighbors, shortest_path"
	@python -m graphify.serve $(MERGED)

$(MERGED):
	@$(MAKE) graphify-merge

## graphify-clean       Remove graphify-out/ and all per-module graphify-out/ dirs
graphify-clean:
	@echo "[graphify] Cleaning all graphify-out directories..."
	@rm -rf graphify-out/
	@find . -maxdepth 2 -type d -name "graphify-out" \
	    -not -path "./graphify-out" \
	    -exec rm -rf {} + 2>/dev/null || true
	@echo "[graphify] Done."

## graphify-list        List auto-detected packages (dry-run detection)
graphify-list:
	@bash -c 'REPO_ROOT=.; for d in */; do \
	    name=$${d%/}; \
	    case "$$name" in \
	        .git|.github|.cursor|.vs|.vscode|.angular|.gemini|.claude|.codex) continue ;; \
	        node_modules|vendor|dist|build|bin|obj|graphify-out) continue ;; \
	        TestResults|TestResults2|pw-out|playwright-report|coverage) continue ;; \
	        Angular_Output|scripts) continue ;; \
	    esac; \
	    echo "  $$name"; \
	done'

## help                 Show this help message
help:
	@grep -E '^## ' $(MAKEFILE_LIST) | sed 's/## /  /'
