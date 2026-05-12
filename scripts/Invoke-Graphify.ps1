<#
.SYNOPSIS
    Package-level knowledge graph generation for Windows (equivalent of scripts/graphify.sh).

.DESCRIPTION
    Builds graphify knowledge graphs per top-level package in this repository.

    Default mode  : structural AST extraction only (no LLM required).
    With -Semantic: full extraction including LLM semantic edges (requires an
                    API key: GEMINI_API_KEY, GOOGLE_API_KEY, ANTHROPIC_API_KEY,
                    or OPENAI_API_KEY).

    Output per package:
        graphify-out\<package>\graph.json       — queryable graph (committed)
        graphify-out\<package>\GRAPH_REPORT.md  — god nodes + report (committed)
        graphify-out\<package>\graph.html       — browser graph (gitignored)

    A composite index is written to graphify-out\GRAPH_REPORT.md.

.PARAMETER Package
    Process only this named package. Default: all detected packages.

.PARAMETER Semantic
    Enable LLM semantic extraction (requires an API key in the environment).

.PARAMETER Force
    Overwrite graphs even if fewer nodes are detected on the rebuild.

.PARAMETER NoHtml
    Skip graph.html generation (faster; recommended for CI).

.PARAMETER MergeOnly
    Skip extraction; only merge existing per-package graphs into
    graphify-out\merged.json.

.PARAMETER NoMerge
    Skip the cross-package merge step after extraction.

.PARAMETER PythonCmd
    Override the Python executable. Default: the GRAPHIFY_PYTHON environment
    variable, falling back to 'python'.

.EXAMPLE
    .\scripts\Invoke-Graphify.ps1
    .\scripts\Invoke-Graphify.ps1 -Package ConverterModule
    .\scripts\Invoke-Graphify.ps1 -Semantic
    .\scripts\Invoke-Graphify.ps1 -MergeOnly
    .\scripts\Invoke-Graphify.ps1 -Package Upgrade -NoHtml
    make graphify-all          # via Makefile (Linux/macOS/Git Bash with make)
#>

[CmdletBinding()]
param(
    [string] $Package   = "",
    [switch] $Semantic,
    [switch] $Force,
    [switch] $NoHtml,
    [switch] $MergeOnly,
    [switch] $NoMerge,
    [string] $PythonCmd = $(if ($env:GRAPHIFY_PYTHON) { $env:GRAPHIFY_PYTHON } else { "python" })
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

# ---------------------------------------------------------------------------
# Configuration
# ---------------------------------------------------------------------------
$ScriptDir   = Split-Path $PSCommandPath -Parent
$RepoRoot    = Split-Path $ScriptDir -Parent
$GraphifyOut = Join-Path $RepoRoot "graphify-out"

$ExcludeDirs = @(
    '.git', '.github', '.cursor', '.vs', '.vscode', '.angular',
    '.gemini', '.claude', '.codex', '.kiro', '.agents',
    'node_modules', 'vendor', 'dist', 'build', 'bin', 'obj',
    'graphify-out', 'TestResults', 'TestResults2',
    'pw-out', 'playwright-report', 'test-results',
    'coverage', 'Angular_Output', 'scripts'
)

$SourceExtensions = @('.csproj', '.cs', '.ts', '.js', '.cmd')
$SourceNames      = @('package.json')

# ---------------------------------------------------------------------------
# Logging
# ---------------------------------------------------------------------------
function Write-Log     { param([string]$Msg) Write-Host "[graphify] $Msg"           -ForegroundColor Cyan    }
function Write-Ok      { param([string]$Msg) Write-Host "[graphify] OK  $Msg"       -ForegroundColor Green   }
function Write-Warn    { param([string]$Msg) Write-Host "[graphify] WARNING: $Msg"  -ForegroundColor Yellow  }
function Write-Fail    { param([string]$Msg) Write-Host "[graphify] ERROR: $Msg"    -ForegroundColor Red; exit 1 }
function Write-Section { param([string]$Msg) Write-Host "`n[graphify] === $Msg ===" -ForegroundColor Magenta }

# ---------------------------------------------------------------------------
# Dependency check — auto-install graphify if missing
# ---------------------------------------------------------------------------
function Ensure-Graphify {
    $verOutput = & $PythonCmd -m graphify --version 2>&1
    if ($LASTEXITCODE -ne 0) {
        Write-Log "graphify not found — installing via pip..."
        & $PythonCmd -m pip install graphifyy --quiet
        if ($LASTEXITCODE -ne 0) {
            Write-Fail "pip install graphifyy failed. Install manually: pip install graphifyy"
        }
    }
    $ver = (& $PythonCmd -m graphify --version 2>&1 | Select-Object -First 1)
    Write-Log "Using $ver"
}

# ---------------------------------------------------------------------------
# Package detection
# ---------------------------------------------------------------------------
function Get-Packages {
    $packages = [System.Collections.Generic.List[string]]::new()

    Get-ChildItem -Path $RepoRoot -Directory -ErrorAction SilentlyContinue |
        Where-Object {
            $n = $_.Name
            $n -notlike '.*' -and ($n -notin $ExcludeDirs)
        } |
        ForEach-Object {
            $dir = $_.FullName
            $found = Get-ChildItem -Path $dir -Recurse -File -ErrorAction SilentlyContinue |
                Where-Object {
                    ($_.Extension -in $SourceExtensions -or $_.Name -in $SourceNames) -and
                    $_.FullName -notlike "*\node_modules\*" -and
                    $_.FullName -notlike "*\bin\*"          -and
                    $_.FullName -notlike "*\obj\*"          -and
                    $_.FullName -notlike "*\dist\*"
                } |
                Select-Object -First 1
            if ($found) { $packages.Add($_.Name) }
        }

    return $packages.ToArray()
}

# ---------------------------------------------------------------------------
# Sync outputs from per-module dir → centralised graphify-out\<pkg>\
# ---------------------------------------------------------------------------
function Sync-Outputs {
    param(
        [string] $Source,
        [string] $Target,
        [string] $Pkg
    )
    if (-not (Test-Path $Source)) {
        Write-Fail "graphify did not create expected output directory: $Source"
    }
    New-Item -ItemType Directory -Path $Target -Force | Out-Null

    foreach ($file in @('graph.json', 'GRAPH_REPORT.md', 'graph.html')) {
        $src = Join-Path $Source $file
        if (Test-Path $src) {
            Copy-Item -Path $src -Destination (Join-Path $Target $file) -Force
        }
        else {
            Write-Warn "$file not generated for package '$Pkg'"
        }
    }
}

# ---------------------------------------------------------------------------
# Run graphify on a single package
# ---------------------------------------------------------------------------
function Invoke-Package {
    param([string] $Pkg)

    $pkgDir    = Join-Path $RepoRoot $Pkg
    $outDir    = Join-Path $GraphifyOut $Pkg
    $sourceOut = Join-Path $pkgDir "graphify-out"

    if (-not (Test-Path $pkgDir)) {
        Write-Fail "Package directory not found: $pkgDir"
    }

    Write-Log "Processing package: $Pkg"
    New-Item -ItemType Directory -Path $outDir -Force | Out-Null

    $extraFlags = [System.Collections.Generic.List[string]]::new()
    if ($Force)  { $extraFlags.Add('--force')  }
    if ($NoHtml) { $extraFlags.Add('--no-viz') }

    if ($Semantic) {
        Write-Log "  Mode: semantic (AST + LLM extraction)"
        & $PythonCmd -m graphify extract $pkgDir @extraFlags
    }
    else {
        Write-Log "  Mode: structural (AST only, no LLM)"
        & $PythonCmd -m graphify update $pkgDir @extraFlags
    }

    if ($LASTEXITCODE -ne 0) {
        Write-Fail "graphify failed for package '$Pkg' (exit code $LASTEXITCODE)"
    }

    Sync-Outputs -Source $sourceOut -Target $outDir -Pkg $Pkg
    Write-Ok "Done: graphify-out\$Pkg\"
}

# ---------------------------------------------------------------------------
# Generate composite index at graphify-out\GRAPH_REPORT.md
# ---------------------------------------------------------------------------
function New-CompositeIndex {
    param([string[]] $Packages)

    $indexPath = Join-Path $GraphifyOut "GRAPH_REPORT.md"
    $ts        = (Get-Date -Format "yyyy-MM-ddTHH:mm:ssZ")

    $sb = [System.Text.StringBuilder]::new()
    $null = $sb.AppendLine("# Repository Knowledge Graph — Package Index")
    $null = $sb.AppendLine("")
    $null = $sb.AppendLine("_Generated by [graphify](https://github.com/safishamsi/graphify) on ${ts}_")
    $null = $sb.AppendLine("")
    $null = $sb.AppendLine("## Packages")
    $null = $sb.AppendLine("")
    $null = $sb.AppendLine("| Package | Nodes | Graph | Report |")
    $null = $sb.AppendLine("|---------|-------|-------|--------|")

    foreach ($pkg in $Packages) {
        $reportPath = Join-Path $GraphifyOut "$pkg\GRAPH_REPORT.md"
        $nodes = "—"
        if (Test-Path $reportPath) {
            $match = Select-String -Path $reportPath -Pattern '\d+ nodes' -ErrorAction SilentlyContinue |
                     Select-Object -First 1
            if ($match) { $nodes = $match.Matches[0].Value }
        }
        $null = $sb.AppendLine("| [$pkg]($pkg/GRAPH_REPORT.md) | $nodes | [graph.json]($pkg/graph.json) | [GRAPH_REPORT.md]($pkg/GRAPH_REPORT.md) |")
    }

    $null = $sb.AppendLine("")
    $null = $sb.AppendLine("## Query examples")
    $null = $sb.AppendLine("")
    $null = $sb.AppendLine('```powershell')
    $null = $sb.AppendLine("# Query a specific package graph")
    $null = $sb.AppendLine('python -m graphify query "explain the architecture" `')
    $null = $sb.AppendLine("  --graph graphify-out\ConverterModule\graph.json")
    $null = $sb.AppendLine("")
    $null = $sb.AppendLine("# Find shortest path between two concepts")
    $null = $sb.AppendLine('python -m graphify path "IConverter" "JsonWriterManager" `')
    $null = $sb.AppendLine("  --graph graphify-out\ConverterModule\graph.json")
    $null = $sb.AppendLine("")
    $null = $sb.AppendLine("# Rebuild all package graphs")
    $null = $sb.AppendLine(".\scripts\Invoke-Graphify.ps1")
    $null = $sb.AppendLine('```')
    $null = $sb.AppendLine("")
    $null = $sb.AppendLine("## Per-package report locations")
    $null = $sb.AppendLine("")

    foreach ($pkg in $Packages) {
        $null = $sb.AppendLine("- **$pkg** → [graphify-out/$pkg/GRAPH_REPORT.md](graphify-out/$pkg/GRAPH_REPORT.md)")
    }

    [System.IO.File]::WriteAllText($indexPath, $sb.ToString(), [System.Text.Encoding]::UTF8)
    Write-Ok "Composite index written → graphify-out\GRAPH_REPORT.md"
}

# ---------------------------------------------------------------------------
# Merge all per-package graphs into graphify-out\merged.json
# ---------------------------------------------------------------------------
function Invoke-GraphifyMerge {
    $graphs = Get-ChildItem -Path (Join-Path $GraphifyOut '*\graph.json') -ErrorAction SilentlyContinue |
              Select-Object -ExpandProperty FullName

    if ($null -eq $graphs -or @($graphs).Count -lt 2) {
        Write-Warn "Fewer than 2 package graphs found in graphify-out\; skipping cross-package merge."
        return
    }

    $out = Join-Path $GraphifyOut "merged.json"
    Write-Log "Merging $(@($graphs).Count) package graphs into graphify-out\merged.json"

    & $PythonCmd -m graphify merge-graphs @graphs --out $out
    if ($LASTEXITCODE -ne 0) {
        Write-Fail "merge-graphs failed (exit code $LASTEXITCODE)"
    }
    Write-Ok "Cross-package graph written → graphify-out\merged.json"
}

# ---------------------------------------------------------------------------
# Main
# ---------------------------------------------------------------------------
Write-Section "graphify — package knowledge graph generator"
Write-Log "Repo root : $RepoRoot"
Write-Log "Output dir: $GraphifyOut"
if ($Semantic)   { Write-Log "Semantic extraction: ENABLED (LLM required)" }
else             { Write-Log "Semantic extraction: disabled (structural AST only)" }
if ($MergeOnly)  { Write-Log "Mode: merge-only (no extraction)" }

Ensure-Graphify
New-Item -ItemType Directory -Path $GraphifyOut -Force | Out-Null

# ── Merge-only mode ────────────────────────────────────────────────────────
if ($MergeOnly) {
    Write-Section "Cross-package merge (no extraction)"
    Invoke-GraphifyMerge
    Write-Section "Complete"
    Write-Ok "Merged graph at graphify-out\merged.json"
    Write-Log "  MCP: python -m graphify.serve graphify-out\merged.json"
    return
}

if ($Package) {
    # ── Single-package mode ────────────────────────────────────────────────
    Write-Section "Extracting: $Package"
    Invoke-Package -Pkg $Package
    New-CompositeIndex -Packages @($Package)
}
else {
    # ── All-packages mode ──────────────────────────────────────────────────
    Write-Section "Package detection"
    $packages = Get-Packages

    if ($packages.Count -eq 0) {
        Write-Fail "No packages detected under $RepoRoot. Check `$ExcludeDirs."
    }
    Write-Log "Detected $($packages.Count) package(s): $($packages -join ', ')"

    Write-Section "Extraction"
    $failed = 0
    foreach ($pkg in $packages) {
        try   { Invoke-Package -Pkg $pkg }
        catch { Write-Warn "FAILED: $pkg — $_"; $failed++ }
    }

    Write-Section "Index"
    New-CompositeIndex -Packages $packages

    Write-Section "Cross-package merge"
    if (-not $NoMerge) {
        Invoke-GraphifyMerge
    } else {
        Write-Log "Skipping cross-package merge (-NoMerge)"
    }

    Write-Section "Summary"
    Write-Log "$($packages.Count) package(s) processed, $failed failed"
    if ($failed -gt 0) {
        Write-Fail "$failed package(s) failed extraction. See warnings above."
    }
}

Write-Section "Complete"
Write-Ok "Graphs available in graphify-out\"
Write-Log "  Tip: python -m graphify query '<question>' --graph graphify-out\merged.json"
Write-Log "  MCP: python -m graphify.serve graphify-out\merged.json"
