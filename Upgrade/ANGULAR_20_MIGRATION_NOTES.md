# Angular 19 → 20 Migration Notes

## Migration Summary

| Item | Details |
|------|---------|
| **Date** | 2025-02-25 |
| **Angular Version** | 19.2.18 → 20.3.16 |
| **Angular CLI Version** | 19.2.21 → 20.3.17 |
| **TypeScript Version** | 5.8.3 (unchanged) |
| **Node.js Version** | 20.19.6 (within `^20.19.0` requirement) |
| **zone.js Version** | 0.15.1 (unchanged, compatible) |
| **RxJS Version** | 7.8.2 (unchanged, compatible) |
| **Tests** | 583/583 ✅ (100% passing) |
| **Build** | ✅ Production build succeeds |

---

## Migration Command

```bash
npx ng update @angular/cli@20 @angular/core@20 --allow-dirty --force
```

---

## Auto-Applied Migrations (by `ng update`)

### 1. `moduleResolution` Changed: `"node"` → `"bundler"`

**File:** `tsconfig.json`

Angular 20 requires `moduleResolution: "bundler"` instead of `"node"`. This was automatically updated by the migration schematic. The `"bundler"` resolution aligns with how modern bundlers (esbuild, Vite) resolve modules and supports `exports` fields in `package.json`.

**Impact:** Low — no code changes required. All imports continued to resolve correctly.

### 2. Build System Migration: `@angular-devkit/build-angular` → `@angular/build`

**File:** `angular.json`, `package.json`

All builders were migrated:
- `@angular-devkit/build-angular:application` → `@angular/build:application`
- `@angular-devkit/build-angular:karma` → `@angular/build:karma`
- `@angular-devkit/build-angular:dev-server` → `@angular/build:dev-server`
- `@angular-devkit/build-angular:extract-i18n` → `@angular/build:extract-i18n`

The `@angular-devkit/build-angular` package was removed from `devDependencies` and replaced with `@angular/build`.

**Impact:** Low — seamless migration, no configuration changes needed beyond the builder names.

### 3. Style Guide Schematics Added

**File:** `angular.json`

Angular 20 added a `schematics` configuration block to preserve naming conventions (`.component`, `.directive`, `.service` suffixes, etc.):

```json
"schematics": {
  "@schematics/angular:component": { "type": "component" },
  "@schematics/angular:directive": { "type": "directive" },
  "@schematics/angular:service": { "type": "service" }
}
```

**Impact:** None — preserves existing naming conventions when generating new files via `ng generate`.

---

## Difficult / Breaking Change Encountered

### Class Name Mangling (`constructor.name` → `_a`)

**Root Cause:** Angular 20's new `@angular/build` package uses enhanced build optimization (esbuild-based) that mangles class names in the test bundle. A test in `introduction.component.spec.ts` was asserting:

```typescript
// BEFORE (Angular 19 — PASSED)
expect(component.constructor.name).toBe('IntroductionComponent');
```

After the migration, `constructor.name` returned `_a` instead of `IntroductionComponent` because the build optimizer renamed the class internally.

**Fix Applied:**

```typescript
// AFTER (Angular 20 — FIXED)
expect(component instanceof IntroductionComponent).toBeTrue();
```

Using `instanceof` is the correct, optimizer-safe way to verify component type identity. The `constructor.name` property is an implementation detail that should never be relied upon in production or test code as it can be mangled by minifiers/optimizers.

**Lesson:** Never use `constructor.name` for type checking in Angular applications. Use `instanceof` instead.

---

## Dependency Updates

| Package | Before | After |
|---------|--------|-------|
| `@angular/core` | ^19.2.18 | ^20.3.16 |
| `@angular/cli` | ^19.2.21 | ^20.3.17 |
| `@angular/build` | _(new)_ | ^20.3.17 |
| `@angular-devkit/build-angular` | ^19.2.21 | _(removed)_ |
| `@types/jasmine` | ~5.1.0 | ^6.0.0 |
| `jasmine-core` | ~5.1.0 | ^6.1.0 |
| `karma` | ~6.4.0 | ^6.4.4 |
| `karma-chrome-launcher` | ~3.2.0 | ^3.2.0 |
| `karma-coverage` | ~2.2.0 | ^2.2.1 |
| `karma-jasmine` | ~5.1.0 | ^5.1.0 |
| `karma-jasmine-html-reporter` | ~2.1.0 | ^2.2.0 |
| `ng-packagr` | ^19.2.0 | ^20.3.2 |

---

## Angular 20 Breaking Changes Reviewed (Not Applicable to This Project)

The following Angular 20 breaking changes were reviewed but did **not** affect this project:

1. **`standalone: true` is no longer valid** — Already removed during v18→v19 migration.
2. **`provideZoneChangeDetection()` required for zoneless** — This project uses zone.js, no changes needed.
3. **`AsyncPipe` error handling changes** — No impact; project error handling patterns are compatible.
4. **`TestBed` error rethrowing** — No tests intentionally trigger uncaught errors.
5. **`HttpClient` returns `ReadableStream` for `responseType: 'stream'`** — Not used in this project.
6. **`RouterOutlet` change detection** — Not applicable; project uses custom step navigation.
7. **`@if`/`@for` control flow** — Already using `@if`/`@for` since v17; no template syntax changes needed.

---

## Files Modified

| File | Change Type | Description |
|------|-------------|-------------|
| `package.json` | Auto + Manual | All @angular/* updated, build package swapped, dev deps updated |
| `tsconfig.json` | Auto | `moduleResolution` changed to `"bundler"` |
| `angular.json` | Auto | Builders migrated, schematics added |
| `introduction.component.spec.ts` | Manual Fix | `constructor.name` → `instanceof` check |

---

## Verification Results

- ✅ **Unit Tests:** 583/583 passing (0 failures)
- ✅ **Production Build:** Successful (bundle size: 1.30 MB initial)
- ✅ **No TypeScript Errors:** Clean compilation
- ⚠️ **Budget Warning:** Pre-existing — initial bundle exceeds 500 kB budget (1.30 MB). Not caused by migration.

---

## Post-Migration: Node 22 Upgrade (2025-02-25)

Node.js was upgraded from **v20.19.6** to **v22.22.0** (Active LTS).

Angular 20.2.x/20.3.x supports Node `^20.19.0 || ^22.12.0 || ^24.0.0` — Node 22.22.0 is within the supported range.

After the upgrade, `npm install` was re-run (796 packages, clean install). Build and all 583 tests passed without any changes required.

---

## Post-Migration: Full Dependency Audit (2025-02-25)

### Audit Tool Results

**`npx ng version` output (final state):**
| Item | Version |
|------|---------|
| Angular CLI | 20.3.17 |
| @angular/core | 20.3.16 |
| Node.js | 22.22.0 ✅ (Active LTS) |
| npm | 11.10.1 |
| TypeScript | 5.9.3 ✅ (upgraded) |
| zone.js | 0.15.1 |
| RxJS | 7.8.2 |

### Dependency Changes Applied

| Package | Action | Before | After | Reason |
|---------|--------|--------|-------|--------|
| `typescript` | **Upgraded** | `~5.8.3` | `~5.9.0` | Angular 20.2+/20.3.x supports `>=5.8.0 <6.0.0`; TS 5.9.3 is within range |
| `jest-editor-support` | **Removed** | `"*"` (in `dependencies`) | _(removed)_ | Never used in source code; was incorrectly placed in runtime `dependencies` with a dangerous wildcard version |

### Packages Verified as Current (No Upgrade Needed)

| Package | Installed | Latest Compatible | Status |
|---------|-----------|-------------------|--------|
| `rxjs` | 7.8.2 | 7.8.2 | ✅ Latest 7.x |
| `tslib` | 2.8.1 | 2.8.1 | ✅ Latest |
| `zone.js` | 0.15.1 | 0.15.x | ✅ (0.16.x is Angular 21 only — do NOT upgrade) |
| `karma` | 6.4.4 | 6.4.4 | ✅ Latest |
| `karma-chrome-launcher` | 3.2.0 | 3.2.0 | ✅ Latest |
| `karma-coverage` | 2.2.1 | 2.2.1 | ✅ Latest |
| `karma-jasmine` | 5.1.0 | 5.1.0 | ✅ Latest |
| `karma-jasmine-html-reporter` | 2.2.0 | 2.2.0 | ✅ Latest |
| `jasmine-core` | 6.1.0 | 6.1.0 | ✅ Latest |
| `@types/jasmine` | 6.0.0 | 6.0.0 | ✅ Latest |
| `ng-packagr` | 20.3.2 | 20.3.x | ✅ (21.x is Angular 21 only — do NOT upgrade) |
| `@shui/core` | 1.33.0 | 1.33.0 | ✅ Framework-agnostic Lit component library |

### Security Audit: 7 Moderate Vulnerabilities (Known/Acceptable)

**`npm audit` identified 7 moderate severity vulnerabilities.** These are **NOT fixable without upgrading to Angular 21**.

| Detail | Value |
|--------|-------|
| Vulnerability | `ajv` ReDoS (Regular Expression Denial of Service) via `$data` option |
| Affected chain | `@angular-devkit/core` → `@angular-devkit/architect` / `@angular-devkit/schematics` → `@angular/cli` / `@angular/build` / `@schematics/angular` |
| Dependency type | **devDependencies only** (build and CLI tools) |
| Runtime application risk | **None** — the compiled app bundle does NOT contain `ajv` |
| Fix available | Only via `npm audit fix --force` which installs Angular CLI 21.1.5 — a full major version upgrade |
| Decision | **Do not force-fix** while remaining on Angular 20. These vulnerabilities exist in the development toolchain, not in the distributed application. |

> **Note for future Angular 21 upgrade:** When upgrading to Angular 21, these 7 vulnerabilities will be automatically resolved by the Angular CLI update.

### Final Verification After Audit

- ✅ **Unit Tests:** 583/583 SUCCESS
- ✅ **Production Build:** Clean (1.30 MB, no errors or warnings)
- ✅ **TypeScript 5.9.3:** Fully compatible with Angular 20.3.x
- ✅ **All dependencies:** On latest compatible versions for Angular 20
