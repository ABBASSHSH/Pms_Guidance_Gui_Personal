#!/usr/bin/env bash
# =============================================================================
# scripts/graphify.sh — Package-level knowledge graph generation
# =============================================================================
#
# Builds graphify knowledge graphs per top-level package in this repository.
#
# Usage:
#   bash scripts/graphify.sh [PKG=<name>] [--semantic] [--force] [--no-html]
#                            [--merge-only] [--no-merge] [--help]
#
# Arguments:
#   PKG=<name>    Process only this package (default: all detected packages)
#   --semantic    Enable LLM semantic extraction (requires an API key:
#                 GEMINI_API_KEY, GOOGLE_API_KEY, ANTHROPIC_API_KEY, or OPENAI_API_KEY)
#   --force       Overwrite graphs even if the rebuild has fewer nodes
#   --no-html     Skip graph.html generation (faster; useful in CI)
#   --merge-only  Skip extraction; only merge existing per-package graphs
#   --no-merge    Skip the cross-package merge step after extraction
#   --help, -h    Show this help message
#
# Output:
#   graphify-out/<package>/
#     graph.json       Queryable knowledge graph (committed to git)
#     GRAPH_REPORT.md  God nodes, communities, surprises (committed to git)
#     graph.html       Interactive browser graph (gitignored)
#
#   graphify-out/GRAPH_REPORT.md   Composite index of all package reports
#   graphify-out/merged.json       Whole-repo cross-package graph (committed to git)
#
# Examples:
#   bash scripts/graphify.sh                        # all packages + cross-package merge
#   bash scripts/graphify.sh PKG=ConverterModule    # single package (no merge)
#   bash scripts/graphify.sh --semantic             # all + LLM semantic edges + merge
#   bash scripts/graphify.sh --merge-only           # (re)merge existing graphs only
#   bash scripts/graphify.sh PKG=Upgrade --no-html  # Upgrade, no HTML
#
# On Windows:  use scripts/Invoke-Graphify.ps1 (PowerShell) or run this
#              script through Git Bash.
#
# Requires:    Python 3.10+, graphifyy (auto-installed if missing)
# =============================================================================

set -euo pipefail

# ---------------------------------------------------------------------------
# Configuration
# ---------------------------------------------------------------------------
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "${SCRIPT_DIR}/.." && pwd)"
GRAPHIFY_OUT="${REPO_ROOT}/graphify-out"
PYTHON_CMD="${GRAPHIFY_PYTHON:-python}"

# Directories excluded from package auto-detection
readonly -a EXCLUDE_DIRS=(
  .git .github .cursor .vs .vscode .angular
  .gemini .claude .codex .kiro .agents
  node_modules vendor dist build bin obj
  graphify-out TestResults TestResults2
  pw-out playwright-report test-results
  coverage Angular_Output scripts
)

# ---------------------------------------------------------------------------
# Logging helpers
# ---------------------------------------------------------------------------
log()      { printf '\033[0;36m[graphify]\033[0m %s\n' "$*" >&2; }
log_ok()   { printf '\033[0;32m[graphify] \xe2\x9c\x93\033[0m %s\n' "$*" >&2; }
log_warn() { printf '\033[0;33m[graphify] WARNING:\033[0m %s\n' "$*" >&2; }
fail()     { printf '\033[0;31m[graphify] ERROR:\033[0m %s\n' "$*" >&2; exit 1; }
section()  { printf '\n\033[0;35m[graphify] === %s ===\033[0m\n' "$*" >&2; }

# ---------------------------------------------------------------------------
# Help
# ---------------------------------------------------------------------------
usage() {
  sed -n '/^# Usage:/,/^# Requires:/p' "${BASH_SOURCE[0]}" | sed 's/^# \?//'
  exit 0
}

# ---------------------------------------------------------------------------
# Argument parsing
# ---------------------------------------------------------------------------
PKG=""
SEMANTIC=false
FORCE=false
NO_HTML=false
MERGE_ONLY=false
NO_MERGE=false

for arg in "$@"; do
  case "$arg" in
    PKG=*)       PKG="${arg#PKG=}" ;;
    --semantic)  SEMANTIC=true ;;
    --force)     FORCE=true ;;
    --no-html)   NO_HTML=true ;;
    --merge-only) MERGE_ONLY=true ;;
    --no-merge)  NO_MERGE=true ;;
    --help|-h)   usage ;;
    *) fail "Unknown argument: '${arg}'. Run with --help for usage." ;;
  esac
done

# ---------------------------------------------------------------------------
# Dependency check — auto-install graphify if missing
# ---------------------------------------------------------------------------
ensure_graphify() {
  if ! "$PYTHON_CMD" -m graphify --version &>/dev/null 2>&1; then
    log "graphify not found — installing via pip..."
    "$PYTHON_CMD" -m pip install graphifyy --quiet \
      || fail "pip install graphifyy failed. Install manually: pip install graphifyy"
  fi
  local ver
  ver="$("$PYTHON_CMD" -m graphify --version 2>&1 | head -1)"
  log "Using ${ver}"
}

# ---------------------------------------------------------------------------
# Package detection helpers
# ---------------------------------------------------------------------------

# Returns 0 (true) if a name should be excluded, 1 (false) otherwise
is_excluded() {
  local name="$1"
  # Always exclude hidden directories
  [[ "$name" == .* ]] && return 0
  for ex in "${EXCLUDE_DIRS[@]}"; do
    [[ "$name" == "$ex" ]] && return 0
  done
  return 1
}

# Returns 0 (true) if directory contains recognisable source files
has_source_files() {
  local dir="$1"
  find "$dir" -maxdepth 4 \
    \( -name "*.csproj" -o -name "package.json" \
       -o -name "*.cs"  -o -name "*.ts" \
       -o -name "*.js"  -o -name "*.cmd" \) \
    -not -path "*/node_modules/*" \
    -not -path "*/bin/*" \
    -not -path "*/obj/*" \
    -not -path "*/dist/*" \
    2>/dev/null | grep -q .
}

# Print detected package names, one per line
detect_packages() {
  local -a packages=()
  for dir in "${REPO_ROOT}"/*/; do
    [[ -d "$dir" ]] || continue
    local name
    name="$(basename "$dir")"
    is_excluded "$name" && continue
    has_source_files "$dir" && packages+=("$name")
  done
  # Guard: handle empty array without triggering set -u
  printf '%s\n' "${packages[@]+"${packages[@]}"}"
}

# ---------------------------------------------------------------------------
# Sync graphify outputs from per-module dir → centralised graphify-out/<pkg>/
# ---------------------------------------------------------------------------
sync_outputs() {
  local source="$1"
  local target="$2"
  local pkg="$3"

  [[ -d "$source" ]] \
    || fail "graphify did not create expected output directory: ${source}"

  mkdir -p "$target"

  local file
  for file in graph.json GRAPH_REPORT.md graph.html; do
    if [[ -f "${source}/${file}" ]]; then
      cp -f "${source}/${file}" "${target}/${file}"
    else
      log_warn "${file} not generated for package '${pkg}'"
    fi
  done
}

# ---------------------------------------------------------------------------
# Run graphify on a single package
# ---------------------------------------------------------------------------
run_package() {
  local pkg="$1"
  local pkg_dir="${REPO_ROOT}/${pkg}"
  local out_dir="${GRAPHIFY_OUT}/${pkg}"
  local source_out="${pkg_dir}/graphify-out"

  [[ -d "$pkg_dir" ]] || fail "Package directory not found: ${pkg_dir}"

  log "Processing package: ${pkg}"
  mkdir -p "$out_dir"

  # Build optional flags array
  local -a extra_flags=()
  [[ "$FORCE"   == "true" ]] && extra_flags+=(--force)
  [[ "$NO_HTML" == "true" ]] && extra_flags+=(--no-viz)

  if [[ "$SEMANTIC" == "true" ]]; then
    log "  Mode: semantic (AST + LLM extraction)"
    "$PYTHON_CMD" -m graphify extract "$pkg_dir" \
      "${extra_flags[@]+"${extra_flags[@]}"}" 2>&1 \
      | sed "s/^/  [${pkg}] /" >&2 \
      || fail "graphify extract failed for package '${pkg}'"
  else
    log "  Mode: structural (AST only, no LLM)"
    "$PYTHON_CMD" -m graphify update "$pkg_dir" \
      "${extra_flags[@]+"${extra_flags[@]}"}" 2>&1 \
      | sed "s/^/  [${pkg}] /" >&2 \
      || fail "graphify update failed for package '${pkg}'"
  fi

  sync_outputs "$source_out" "$out_dir" "$pkg"
  log_ok "Done: graphify-out/${pkg}/"
}

# ---------------------------------------------------------------------------
# Generate composite index at graphify-out/GRAPH_REPORT.md
# ---------------------------------------------------------------------------
generate_index() {
  local -a packages=("$@")
  local index="${GRAPHIFY_OUT}/GRAPH_REPORT.md"
  local ts
  ts="$(date -u '+%Y-%m-%dT%H:%M:%SZ' 2>/dev/null || date -u)"

  {
    printf '# Repository Knowledge Graph — Package Index\n\n'
    printf '_Generated by [graphify](https://github.com/safishamsi/graphify) on %s_\n\n' "$ts"
    printf '## Packages\n\n'
    printf '| Package | Nodes | Graph | Report |\n'
    printf '|---------|-------|-------|--------|\n'

    local pkg report nodes
    for pkg in "${packages[@]}"; do
      report="${GRAPHIFY_OUT}/${pkg}/GRAPH_REPORT.md"
      nodes="—"
      if [[ -f "$report" ]]; then
        # Extract node count from the report (e.g. "94 nodes")
        nodes="$(grep -oP '\d+ nodes' "$report" 2>/dev/null | head -1 || true)"
        [[ -z "$nodes" ]] && nodes="(see report)"
      fi
      printf '| [%s](%s/GRAPH_REPORT.md) | %s | [graph.json](%s/graph.json) | [GRAPH_REPORT.md](%s/GRAPH_REPORT.md) |\n' \
        "$pkg" "$pkg" "$nodes" "$pkg" "$pkg"
    done

    printf '\n## Query examples\n\n'
    printf '```bash\n'
    printf '# Query a specific package graph\n'
    printf 'python -m graphify query "explain the architecture" \\\n'
    printf '  --graph graphify-out/ConverterModule/graph.json\n\n'
    printf '# Find shortest path between two concepts\n'
    printf 'python -m graphify path "IConverter" "JsonWriterManager" \\\n'
    printf '  --graph graphify-out/ConverterModule/graph.json\n\n'
    printf '# Explain a symbol\n'
    printf 'python -m graphify explain "ActionReplyHandler" \\\n'
    printf '  --graph graphify-out/BusinessLogicModule/graph.json\n\n'
    printf '# Rebuild all package graphs\n'
    printf 'bash scripts/graphify.sh\n'
    printf '# or on Windows:\n'
    printf '.\\scripts\\Invoke-Graphify.ps1\n'
    printf '```\n\n'

    printf '## Per-package report locations\n\n'
    for pkg in "${packages[@]}"; do
      printf -- '- **%s** -> graphify-out/%s/GRAPH_REPORT.md\n' "$pkg" "$pkg"
    done
  } > "$index"

  log_ok "Composite index written → graphify-out/GRAPH_REPORT.md"
}

# ---------------------------------------------------------------------------
# Merge all per-package graphs into graphify-out/merged.json
# ---------------------------------------------------------------------------
run_merge() {
  local -a graphs=()
  local g
  for g in "${GRAPHIFY_OUT}"/*/graph.json; do
    [[ -f "$g" ]] && graphs+=("$g")
  done

  if [[ ${#graphs[@]} -lt 2 ]]; then
    log_warn "Fewer than 2 package graphs found in graphify-out/; skipping cross-package merge."
    return 0
  fi

  local out="${GRAPHIFY_OUT}/merged.json"
  log "Merging ${#graphs[@]} package graphs into graphify-out/merged.json"
  PYTHONIOENCODING=utf-8 "$PYTHON_CMD" -m graphify merge-graphs "${graphs[@]}" --out "$out" 2>&1 \
    | sed 's/^/  [merge] /' >&2 \
    || fail "merge-graphs failed (exit code $?)"
  log_ok "Cross-package graph written -> graphify-out/merged.json"
}

# ---------------------------------------------------------------------------
# Main
# ---------------------------------------------------------------------------
main() {
  section "graphify — package knowledge graph generator"
  log "Repo root : ${REPO_ROOT}"
  log "Output dir: ${GRAPHIFY_OUT}"
  [[ "$SEMANTIC" == "true" ]] && log "Semantic extraction: ENABLED (LLM required)"
  [[ "$SEMANTIC" == "false" ]] && log "Semantic extraction: disabled (structural AST only)"

  ensure_graphify
  mkdir -p "${GRAPHIFY_OUT}"

  # ── Merge-only mode ──────────────────────────────────────────────────────
  if [[ "$MERGE_ONLY" == "true" ]]; then
    section "Cross-package merge (no extraction)"
    run_merge
    section "Complete"
    log_ok "Merged graph at graphify-out/merged.json"
    log "  Tip: python -m graphify.serve graphify-out/merged.json"
    return 0
  fi

  if [[ -n "$PKG" ]]; then
    # ── Single-package mode ──────────────────────────────────────────────
    section "Extracting: ${PKG}"
    run_package "$PKG"
    generate_index "$PKG"

  else
    # ── All-packages mode ────────────────────────────────────────────────
    section "Package detection"
    mapfile -t PACKAGES < <(detect_packages)

    if [[ ${#PACKAGES[@]} -eq 0 ]]; then
      fail "No packages detected under ${REPO_ROOT}. Check EXCLUDE_DIRS in this script."
    fi
    log "Detected ${#PACKAGES[@]} package(s): ${PACKAGES[*]}"

    section "Extraction"
    local failed=0
    for pkg in "${PACKAGES[@]}"; do
      run_package "$pkg" || {
        log_warn "FAILED: ${pkg}"
        (( failed++ )) || true
      }
    done

    section "Index"
    generate_index "${PACKAGES[@]}"

    section "Cross-package merge"
    if [[ "$NO_MERGE" == "false" ]]; then
      run_merge
    else
      log "Skipping cross-package merge (--no-merge)"
    fi

    section "Summary"
    log "${#PACKAGES[@]} package(s) processed, ${failed} failed"
    [[ $failed -eq 0 ]] \
      || fail "${failed} package(s) failed extraction. See warnings above."
  fi

  section "Complete"
  log_ok "Graphs available in graphify-out/"
  log "  Tip: python -m graphify query '<question>' --graph graphify-out/merged.json"
  log "  MCP: python -m graphify.serve graphify-out/merged.json"
}

main "$@"
