# Graph Report - Upgrade  (2026-05-12)

## Corpus Check
- 58 files · ~27,896 words
- Verdict: corpus is large enough that graph structure adds value.

## Summary
- 490 nodes · 744 edges · 30 communities (28 shown, 2 thin omitted)
- Extraction: 100% EXTRACTED · 0% INFERRED · 0% AMBIGUOUS · INFERRED: 1 edges (avg confidence: 0.8)
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
- [[_COMMUNITY_Community 5|Community 5]]
- [[_COMMUNITY_Community 6|Community 6]]
- [[_COMMUNITY_Community 7|Community 7]]
- [[_COMMUNITY_Community 8|Community 8]]
- [[_COMMUNITY_Community 9|Community 9]]
- [[_COMMUNITY_Community 10|Community 10]]
- [[_COMMUNITY_Community 11|Community 11]]
- [[_COMMUNITY_Community 12|Community 12]]
- [[_COMMUNITY_Community 13|Community 13]]
- [[_COMMUNITY_Community 14|Community 14]]
- [[_COMMUNITY_Community 15|Community 15]]
- [[_COMMUNITY_Community 16|Community 16]]
- [[_COMMUNITY_Community 17|Community 17]]
- [[_COMMUNITY_Community 18|Community 18]]
- [[_COMMUNITY_Community 19|Community 19]]
- [[_COMMUNITY_Community 20|Community 20]]
- [[_COMMUNITY_Community 21|Community 21]]
- [[_COMMUNITY_Community 22|Community 22]]
- [[_COMMUNITY_Community 23|Community 23]]
- [[_COMMUNITY_Community 24|Community 24]]
- [[_COMMUNITY_Community 25|Community 25]]
- [[_COMMUNITY_Community 26|Community 26]]

## God Nodes (most connected - your core abstractions)
1. `LogService` - 28 edges
2. `UiStateManagementService` - 23 edges
3. `CommunicationService` - 22 edges
4. `2.2 Programming Recommendations` - 18 edges
5. `SSIT Test Guide — Writing Integration Tests from Scratch` - 18 edges
6. `2. Suite-by-Suite Coverage Assessment` - 15 edges
7. `LogManager` - 12 edges
8. `StepId` - 12 edges
9. `ConnectionManager` - 11 edges
10. `Angular 19 → 20 Migration Notes` - 11 edges

## Surprising Connections (you probably didn't know these)
- `main()` --calls--> `bootstrapShui()`  [EXTRACTED]
  src/main.ts → src/app/core/shui/shui.bootstrap.ts

## Communities (30 total, 2 thin omitted)

### Community 0 - "Community 0"
Cohesion: 0.08
Nodes (22): order, CommunicationService, DriveToParkPositionComponent, order, GuidanceOverviewComponent, ids, statuses, InstallationInProgressComponent (+14 more)

### Community 1 - "Community 1"
Cohesion: 0.06
Nodes (29): call, circular, largePayload, payload, sentMessage, specialActions, testDate, testMessages (+21 more)

### Community 2 - "Community 2"
Cohesion: 0.06
Nodes (33): 1. Review Checklist Against Test-Plan Quality Standards, 2. Suite-by-Suite Coverage Assessment, 3. Critical Issues Found, 4. Missing Test Scenarios, 5. Test Quality Observations, 6. Recommended Fixes (Priority Order), 7. Summary, code:typescript (test('SL-03: language applied before first render via addIni) (+25 more)

### Community 3 - "Community 3"
Cohesion: 0.13
Nodes (18): ShowInstallationPrereqHandler, msg, ShowSystemLanguageHandler, msg, handler, handlerSpy, installSpy, langSpy (+10 more)

### Community 4 - "Community 4"
Cohesion: 0.07
Nodes (29): 1.1 Purpose, 1.2 Scope, 1.3.1 Definitions, 1.3.2 Acronyms and Abbreviations, 1.3 Definitions, Acronyms and Abbreviations, 1.4 References, 1. INTRODUCTION, 2.1 Naming Conventions (+21 more)

### Community 5 - "Community 5"
Cohesion: 0.07
Nodes (28): 2.2.10 Use the using directive for disposable classes, 2.2.11 Use Managed equivalents where available, 2.2.12 Do not initialize variables unnecessarily, 2.2.13 Class code Complexity, 2.2.14 Create Shortcut Names, 2.2.15 Use Generics, 2.2.16.1 Use DOM Parser, 2.2.16.2 Use STAX Parser (+20 more)

### Community 6 - "Community 6"
Cohesion: 0.12
Nodes (12): LogBus, LOG_LEVEL_ORDER, LogManager, fresh, freshComm, long, start, ts (+4 more)

### Community 7 - "Community 7"
Cohesion: 0.08
Nodes (24): 1. `moduleResolution` Changed: `"node"` → `"bundler"`, 2. Build System Migration: `@angular-devkit/build-angular` → `@angular/build`, 3. Style Guide Schematics Added, Angular 19 → 20 Migration Notes, Angular 20 Breaking Changes Reviewed (Not Applicable to This Project), Audit Tool Results, Auto-Applied Migrations (by `ng update`), Class Name Mangling (`constructor.name` → `_a`) (+16 more)

### Community 8 - "Community 8"
Cohesion: 0.09
Nodes (23): 12. Complete worked example — adding a brand new test, code:html (<!-- Before -->), code:gherkin (@TC08 @TC09), code:gherkin (@TC_NEW01), code:block29 ("the element {string} is visible"), code:csharp ([Then("the element {string} is visible")]), code:csharp ([Then("the screen title reads i18n key {string}")]), code:gherkin (@TC_NEW02) (+15 more)

### Community 9 - "Community 9"
Cohesion: 0.15
Nodes (8): I18nService, fallbackReq, mockDe, mockEn, req, staleReq, result, TranslatePipe

### Community 10 - "Community 10"
Cohesion: 0.14
Nodes (14): 6.1 File Header, 6.2 Function Header, 6.3 Hazard Code, 6.4 AI Generated code header, 6. APPENDIX, C#, TypeScript, JavaScript, Java, Kotlin, Go, code:csharp (#region Copyright), code:csharp (//----------------------------------------------------------) (+6 more)

### Community 11 - "Community 11"
Cohesion: 0.15
Nodes (13): 13. Running the tests, code:powershell (# Terminal 1 — keep this running), code:powershell (dotnet test ... --filter "TestCategory=TC01"), code:powershell (dotnet test ... --filter "TestCategory=TC01|TestCategory=TC0), code:powershell (dotnet test ... --filter "TestCategory=TC01|TestCategory=TC0), code:powershell (dotnet test ... --filter "FullyQualifiedName~Introduction"), code:powershell (dotnet test "d:\WS_1\Sources\Pms\nPSW\Pms_GuidanceGUI\Guidan), Launch GuidanceHost in English (recommended for test runs) (+5 more)

### Community 12 - "Community 12"
Cohesion: 0.17
Nodes (12): 8. Writing step definitions (C#), code:gherkin (Then the element "proceed-btn" is visible         → {string}), code:csharp ([Binding]), code:csharp (public sealed class PageContext), code:csharp (// Find an element by ta-id attribute), code:csharp ([Then("the element {string} is visible")]), PageContext — the shared state object, The binding attribute (+4 more)

### Community 13 - "Community 13"
Cohesion: 0.17
Nodes (11): 1. What are SSIT tests?, 2. How the whole system fits together, 3. The technology stack, 4. Project structure, 5. How a test run actually works — step by step, code:block1 (┌─────────────────────────────────────────────────────┐), code:block2 (WEBVIEW2_ADDITIONAL_BROWSER_ARGUMENTS=--remote-debugging-por), code:block3 (GuidanceGUI.IntegrationTests/) (+3 more)

### Community 14 - "Community 14"
Cohesion: 0.17
Nodes (12): 14. When a test fails — how to read the error, code:block44 (System.AggregateException: connect ECONNREFUSED 127.0.0.1:92), code:block45 (TimeoutError: Locator: GetByTestId("introduction-screen") — ), code:block46 (AssertFailedException: [proceed-btn] expected attribute 'lab), code:gherkin (# Before), code:block48 (Reqnroll: No matching step definition found for:), code:block49 (Reqnroll: Ambiguous step definitions. Multiple step definiti), Error type 1 — CDP connection refused (+4 more)

### Community 15 - "Community 15"
Cohesion: 0.18
Nodes (11): 15. Rules and conventions used in this project, code:gherkin (# BAD — mixed concerns, hard to understand what failed), code:gherkin (# NOTE: Cancel sends CloseApp to the WPF host which terminat), code:xml (<Parallelize>), code:gherkin (# TODO: Add VVP-G14 once the WPF backend abort-reset flow is), Rule 1 — One assertion per concern, Rule 2 — Do not test what Playwright cannot observe, Rule 3 — One CDP connection, one scenario at a time (+3 more)

### Community 16 - "Community 16"
Cohesion: 0.27
Nodes (4): AppComponent, appConfig, bootstrapShui(), main()

### Community 17 - "Community 17"
Cohesion: 0.2
Nodes (10): 10. i18n — making tests language-independent, code:gherkin (# BAD — breaks in German), code:csharp (var expected = _ctx.Translations.Resolve("common.proceed");), code:json ({), code:gherkin (# color is a component attribute value, not a translation), Finding the right key, How the key resolves, The problem (+2 more)

### Community 18 - "Community 18"
Cohesion: 0.22
Nodes (9): 9. The infrastructure files you must not break, code:csharp (public sealed class PageContext), code:csharp ([BeforeScenario(Order = 0)]), code:csharp (// Loads en.json or de.json from Upgrade/src/assets/i18n/), code:csharp (global using Microsoft.VisualStudio.TestTools.UnitTesting;  ), `ImplicitUsings.cs`, `Support/AppLifecycle.cs`, `Support/PageContext.cs` (+1 more)

### Community 19 - "Community 19"
Cohesion: 0.25
Nodes (7): Build, Code scaffolding, Development server, Further help, Running end-to-end tests, Running unit tests, Upgrade

### Community 20 - "Community 20"
Cohesion: 0.25
Nodes (8): 11. The ta-id system — how tests find elements, code:html (<sh-button ta-id="proceed-btn" [label]="'common.proceed' | t), code:csharp (// Finds the element with ta-id="proceed-btn"), code:csharp (_ctx.Playwright.Selectors.SetTestIdAttribute("ta-id");), code:html (<sh-button ta-id="proceed-btn" ...></sh-button>), How to find a ta-id for an element you want to test, In Angular HTML templates, In Playwright (C#)

### Community 22 - "Community 22"
Cohesion: 0.29
Nodes (7): 16. Quick reference cheat sheet, Adding a new scenario (checklist), code:powershell (# Build), Finding a ta-id, Finding an i18n key, Step definition patterns already available (SharedSteps.cs), Terminal commands

### Community 24 - "Community 24"
Cohesion: 0.33
Nodes (6): 6. The BDD language — Gherkin, A minimal example, code:gherkin (Feature: Introduction Screen), code:gherkin (@TC02 @TC03 @TC04 @TC05), Scenario Outline — testing multiple values, The keywords

### Community 25 - "Community 25"
Cohesion: 0.33
Nodes (6): 7. Writing a feature file, code:gherkin (Feature: [Screen Name]            ← What area of the app thi), code:block8 (A — Screen presence (is the screen visible?)), File organisation pattern, Tag naming convention, The anatomy of a feature file

### Community 26 - "Community 26"
Cohesion: 0.4
Nodes (4): c, inner, payload, w

## Knowledge Gaps
- **226 isolated node(s):** `w`, `c`, `payload`, `inner`, `order` (+221 more)
  These have ≤1 connection - possible missing edges or undocumented components.
- **2 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `SSIT Test Guide — Writing Integration Tests from Scratch` connect `Community 13` to `Community 8`, `Community 11`, `Community 12`, `Community 14`, `Community 15`, `Community 17`, `Community 18`, `Community 20`, `Community 22`, `Community 24`, `Community 25`?**
  _High betweenness centrality (0.062) - this node is a cross-community bridge._
- **Why does `LogService` connect `Community 0` to `Community 9`, `Community 3`, `Community 6`?**
  _High betweenness centrality (0.025) - this node is a cross-community bridge._
- **Why does `12. Complete worked example — adding a brand new test` connect `Community 8` to `Community 13`?**
  _High betweenness centrality (0.021) - this node is a cross-community bridge._
- **What connects `w`, `c`, `payload` to the rest of the system?**
  _226 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Community 0` be split into smaller, more focused modules?**
  _Cohesion score 0.08 - nodes in this community are weakly interconnected._
- **Should `Community 1` be split into smaller, more focused modules?**
  _Cohesion score 0.06 - nodes in this community are weakly interconnected._
- **Should `Community 2` be split into smaller, more focused modules?**
  _Cohesion score 0.06 - nodes in this community are weakly interconnected._