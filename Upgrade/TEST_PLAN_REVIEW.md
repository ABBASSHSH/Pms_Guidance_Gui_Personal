# Playwright Test Plan Review
**Date:** March 3, 2026  
**Reviewer:** GitHub Copilot  
**App:** Software Upgrade GuidanceUI — Angular 20, 6-step GuidanceUI, WebView2/SHUI  
**Total Tests:** 516 (passing 516/516 across Chromium, Firefox, WebKit)

---

## 1. Review Checklist Against Test-Plan Quality Standards

| Standard | Status | Notes |
|---|---|---|
| ✅ Happy path scenarios | **PASS** | Full 6-step happy path in `e2e-flows.spec.ts` E2E-01–05, plus per-step suites |
| ✅ Edge cases & boundary conditions | **PASS** | RB-01–08, PR-13–16, duplicate messages, rapid-fire messages |
| ✅ Error handling & validation | **PASS** | VRE-01–10, VP-13–17, PR-07–12, E2E-06 |
| ✅ Clear descriptive titles | **PASS** | All tests use `SUITE-NN: description` pattern |
| ✅ Step-by-step instructions | **PASS** | All test bodies follow navigate → interact → assert |
| ✅ Expected outcomes stated | **PASS** | Every test has at least one `expect()` assertion |
| ✅ Fresh state assumption | **PASS** | `bridgedPage` fixture reloads from `/` before each test |
| ✅ Success/failure criteria | **PASS** | Assertions are specific (attribute values, text content, counts) |
| ✅ Negative testing | **PASS** | Cancel flows, abort flows, invalid messages, missing fields |
| ✅ Tests independent / any order | **PASS** | Each test starts from fixture; no shared mutable state |
| ✅ Outbound messages verified | **PASS** | `__pw_messages_sent` spy used throughout Suite 10 + 14B |
| ✅ Inbound messages verified | **PASS** | `__pw_sendToFrontend` used in Suites 11, 12, 14C |
| ✅ Accessibility/theme | **PARTIAL** | Dark theme (AS-05), access bar label (AS-01) tested; keyboard nav not tested |
| ✅ i18n coverage | **PARTIAL** | English fully covered; German verified via HTTP response; no RTL |
| ✅ Formatted as markdown | **PASS** | All spec files use `test.describe` suites with clear IDs |

---

## 2. Suite-by-Suite Coverage Assessment

### Suite 1 — App Shell (`app-shell.spec.ts`) ✅
**6 tests / 6 tests — Complete**

| ID | Test | Happy | Negative | Edge |
|---|---|---|---|---|
| AS-01 | Access bar visible + label translated | ✅ | | |
| AS-02 | Stepper panel visible | ✅ | | |
| AS-03 | Exactly 6 stepper items | ✅ | | |
| AS-04 | Step 0 active on load; others inactive | ✅ | | |
| AS-05 | Dark theme applied | ✅ | | |
| AS-06 | Introduction content shown | ✅ | | |

**Gaps identified:** None critical. *Minor:* no test for light theme being absent, no test for page layout breakpoints.

---

### Suite 2 — Introduction (`introduction.spec.ts`) ✅
**8 tests / 8 tests — Complete**

| ID | Test | Happy | Negative | Edge |
|---|---|---|---|---|
| IN-01 | Title text | ✅ | | |
| IN-02 | Description text | ✅ | | |
| IN-03 | Info bar icon | ✅ | | |
| IN-04 | Cancel + Proceed buttons present | ✅ | | |
| IN-05 | Copyright text | ✅ | | |
| IN-06 | Proceed → Step 1 + stepper active | ✅ | | |
| IN-07 | Step 0 marked success in stepper | ✅ | | |
| IN-08 | Cancel → CloseApp sent | ✅ | ✅ | |

**Gaps identified:**
- No test for the **info bar label text** (`introduction.info` key → "Please click on Proceed...") — only icon presence tested (IN-03)
- No test that **Step 0 is not yet `type="success"`** before Proceed is clicked (boundary)

---

### Suite 3 — Verify Prerequisites (`verify-prereq.spec.ts`) ✅
**23 tests — Very thorough**

Covers: initial state (VP-01–07), OK response (VP-08–12), NotOk response (VP-13–17), abort modal (VP-18–23).

**Gaps identified:**
- `VP-03` tests in-progress status text but there is no test for the **info bar label** (`verification.info` → "Once done, the process automatically switches to the next step")
- No test for **Abort button label text** = "Abort" (only visibility tested in VP-05)

---

### Suite 4 — Verification Result Success (`verification-result-success.spec.ts`) ✅
**11 tests — Complete**

Covers: title, success notification type/label/subtitle/body, info bar, buttons, navigation, stepper, CloseApp.

**Gaps identified:**
- `VRE/VRS`: **copyright text** not tested on the Verification Result page (the template has `<sh-text>{{ 'common.copyright' | t }}</sh-text>` in the footer)

---

### Suite 5 — Verification Result Error (`verification-result-error.spec.ts`) ✅
**10 tests — Complete**

Covers: title, error notification type/label/subtitle/body, info bar, buttons, OK/ShowReport clickable.

**Gaps identified:**
- Same as Suite 4: **copyright text** not tested
- **Cancel does not exist on error path** — the template renders "Show Report" + "OK" on error, not Cancel. This is correctly tested (VRE-08: no Proceed with Installation). However there is no test that **CloseApp is NOT sent** when OK is clicked on the error path.

---

### Suite 6 — Save Patient Images (`save-patient-images.spec.ts`) ✅
**8 tests — Complete**

Covers: title, description, info bar, buttons, copyright, navigation, stepper success, Cancel → CloseApp.

**Gaps identified:** None critical.

---

### Suite 7 — Drive to Park Position (`drive-to-park.spec.ts`) ✅
**11 tests — Complete**

Covers: title, all 4 instruction steps, info bar, buttons, copyright, navigation, stepper success, Cancel → CloseApp.

**Gaps identified:** None critical.

---

### Suite 8 — Installation In Progress (`installation-in-progress.spec.ts`) ✅
**6 tests — Complete**

Covers: spinner visible, spinner label text, no buttons, stepper active, InstallSoftware sent once.

**Gaps identified:**
- No test for the **copyright text** absence (installation page does not have a footer with copyright — this is a valid design difference worth explicitly testing to catch regressions)
- No test that navigation **away from Step 5 is not possible** (no buttons → no outbound trigger)

---

### Suite 9 — Guidance Overview (`guidance-overview.spec.ts`) ✅
**31 tests — Most comprehensive suite**

Covers: structure, all 6 labels, German i18n HTTP fetch, active state tracking at every step, terminal type attributes at every step, state persistence through full flow, stepper always visible.

**Gaps identified:**
- `GO-20` checks pending steps have no `[type]` on first load — but only checks items 1–5. **Step 0 (Introduction)** should also not have `[type]` on first load (it's active, not yet completed).
- No test for `sh-stepper-item` when **type="warning"** — the `getType()` method in guidance-overview supports "warning" but it is never exercised in the GuidanceUI flow. This is acceptable if "warning" is unused.

---

### Suite 10 — Outbound Messages (`messaging-outbound.spec.ts`) ✅
**13 tests — Complete**

Covers: boot sends no domain messages, VerifyInstallationPrerequisite, CloseApp from every screen, InstallSoftware, idempotency, full path message count, every message has Action field.

**Gaps identified:**
- No test for **LogMessage format** — the `Action: 'LogMessage'` messages are filtered in OUT-01/OUT-12 but their content structure (`Message: '[level] source: text'`) is never verified
- **OUT-05**: Cancel on Verification Result **error path** (VRE scenario) is not tested — only the success Cancel path (OUT-05 covers success)

---

### Suite 11 — ShowSystemLanguage (`messaging-system-language.spec.ts`) ✅
**6 tests — Appropriately scoped given Zone.js limitation**

Covers: English keeps English, German fetches correct bundle (via HTTP response), German pre-boot, unknown language graceful fallback, missing Language field graceful, mid-flow label update via HTTP verification.

**Note on SL-03:** The test uses a broken `handlers_ref` closure reference (the variable is declared inside the `if` block after being used in the closure). This test passes because the `chrome.webview.addEventListener` override doesn't actually work correctly — the test uses `not.toContainText` which happens to pass if the title hasn't loaded in time. **This test may give a false positive.**

**Gaps identified:**
- **SL-03** has a subtle implementation bug — see Note above
- No test for switching **back from German to English** (bidirectional language switching)

---

### Suite 12 — ShowInstallationPrerequisite (`messaging-prereq-result.spec.ts`) ✅
**16 tests — Complete (adapted for synchronous navigation)**

Covers: OK/NotOk outcomes verified via stepper + notification (post-navigation), no navigation on invalid status, no crash on missing status, idempotency, message while modal open.

**Note:** PR-01/07 are re-mapped from "progress bar" assertions to stepper type assertions due to synchronous navigation. The test names remain "progress bar reaches 100%" but actually test the stepper. The test intent is preserved but the names are misleading.

**Gaps identified:**
- PR-01 title says "progress bar reaches 100%" but tests `sh-stepper-item type="success"` — **test name/body mismatch**
- PR-07 title says "progress bar reaches 75%" but tests `sh-stepper-item type="error"` — **test name/body mismatch**
- PR-09 title says "error styling applied" but also tests `sh-stepper-item type="error"` — **duplicate of PR-07/PR-11**

---

### Suite 13 — Communication Robustness (`messaging-robustness.spec.ts`) ✅
**8 tests — Complete**

Covers: empty object, no Action field, unknown Action, malformed JSON, null data, rapid OK messages, case sensitivity, large payload.

**Gaps identified:**
- **RB-04** implementation is incomplete — it creates a `__pw_sendToFrontend_raw` function but never actually dispatches the malformed JSON to the registered handlers. The test only verifies the app doesn't crash because the malformed data is never actually sent through the real handler pipeline. **This test may give a false positive.**

---

### Suite 14 — Full E2E Flows (`e2e-flows.spec.ts`) ✅
**15 tests — Excellent integration coverage**

Covers: full happy path (all 6 steps), stepper tracking, all steps success, message order, no console errors, error path, Cancel from every step, Abort confirmed/cancelled, language pre-boot, language mid-flow, round-trip, no crash.

**Gaps identified:**
- **E2E-12**: Same Zone.js limitation as SL-03 — uses `not.toContainText` on `.title` after sending German synchronously. This may pass vacuously if the title hasn't resolved yet.
- No test for the **@default case** in `app.component.html` — when `getActiveStepId(state)` returns an unexpected step ID, the app falls back to `app-introduction`.

---

## 3. Critical Issues Found

### 🔴 Issue 1: SL-03 — Broken `handlers_ref` closure (may give false positive)

**File:** `tests/messaging-system-language.spec.ts`, line 43–58  
**Problem:** `handlers_ref` is declared *inside* the `if (chrome?.webview)` block but used *in the closure* before that declaration in the execution order. The override `chrome.webview.addEventListener` never actually pushes to a real array. The German language message is sent synchronously but dispatched to an empty array — Angular never receives it. The `not.toContainText` assertion passes because the title text hasn't resolved yet (no i18n), not because it's German.

**Fix:** Rewrite the test to use the `waitForResponse` pattern (same as SL-02/SL-06) rather than a broken pre-boot injection.

### 🟡 Issue 2: RB-04 — Malformed JSON never actually reaches the handler (may give false positive)

**File:** `tests/messaging-robustness.spec.ts`, line 22–33  
**Problem:** The test creates a `__pw_sendToFrontend_raw` helper in the page context but never calls it. It then calls `chrome.webview.addEventListener('message', () => {})` which pushes an empty no-op handler — this doesn't send anything. The app is verified stable because nothing actually happens.

**Fix:** Use `page.evaluate` to dispatch a raw `MessageEvent` with bad JSON directly via `window.__pw_sendToFrontend` equivalent or dispatch to the actual registered handlers.

### 🟡 Issue 3: PR-01/PR-07 — Test name/body mismatch

**File:** `tests/messaging-prereq-result.spec.ts`, lines 7 and 49  
**Problem:** PR-01 is named "progress bar reaches 100%" but asserts `sh-stepper-item type="success"`. PR-07 is named "progress bar reaches 75%" but asserts `sh-stepper-item type="error"`. The test IDs/names are misleading for future maintainers.

**Fix:** Rename tests to match their actual assertions.

---

## 4. Missing Test Scenarios

The following scenarios from the test-plan quality standards are **not yet covered**:

| ID | Missing Scenario | Priority | Suggested Suite |
|---|---|---|---|
| M-01 | Info bar label text verified (not just icon presence) on Introduction, SaveImages, DriveToPark, VerifyPrereq, VerificationResult | Low | Per-step suites |
| M-02 | Copyright text on Verification Result (success + error) | Low | Suite 4 & 5 |
| M-03 | CloseApp NOT sent when OK clicked on error verification result | Medium | Suite 5 / Suite 10 |
| M-04 | Step 0 has no `[type]` attribute on first load | Low | Suite 9 (GO-20 extension) |
| M-05 | Language switch back from German → English | Low | Suite 11 |
| M-06 | `@default` case in app router (unknown step ID → shows Introduction) | Low | Suite 14 |
| M-07 | Installation page has no copyright footer (regression guard) | Low | Suite 8 |
| M-08 | OUT-05 equivalent for error path (Cancel not present on error page — verify ShowReport+OK do NOT send CloseApp) | Medium | Suite 10 |
| M-09 | Abort button label text "Abort" is visible in footer (not just modal visible) | Low | Suite 3 |

---

## 5. Test Quality Observations

### ✅ What is done well

1. **Fixture isolation** — `bridgedPage` fully resets state before each test via page reload; no shared mutable state between tests.
2. **Selector robustness** — Ambiguous selectors (dual Abort buttons) are correctly scoped to `.footer__actions`.
3. **Zone.js limitation handling** — German language DOM limitation properly documented and worked around with `waitForResponse` + response body assertion.
4. **BOM-free JSON** — `de.json` has no UTF-8 BOM after fix, ensuring cross-browser `response.json()` compatibility.
5. **LogMessage awareness** — OUT-01/OUT-12 correctly filter infrastructure log messages from domain assertions.
6. **Idempotency testing** — Duplicate message handling tested (PR-15, RB-06).
7. **Stepper state persistence** — GO-28/GO-29 verify completed steps stay in terminal state as the GuidanceUI advances.
8. **Cross-browser** — All 516 tests passing on Chromium, Firefox, and WebKit.

### ⚠️ What could be improved

1. **Test name accuracy** — PR-01 and PR-07 names do not match their assertions (see Issue 3).
2. **False-positive risk** — SL-03 and RB-04 may pass vacuously (see Issues 1 and 2).
3. **Missing copyright assertions** — Verification Result page copyright not tested.
4. **No keyboard navigation tests** — Tab order and keyboard-only flow not covered.
5. **No accessibility assertions** — `aria-label`, `role`, focus management not tested.

---

## 6. Recommended Fixes (Priority Order)

### P1 — Fix false positives

**Fix SL-03** (broken pre-boot German injection):
```typescript
test('SL-03: language applied before first render via addInitScript', async ({ page }) => {
  // Verify German bundle is fetched synchronously on first connect
  const deJsonPromise = page.waitForResponse(
    resp => resp.url().includes('/assets/i18n/de.json') && resp.status() === 200,
    { timeout: 10000 }
  );
  await page.addInitScript(() => {
    const handlers: Array<(e: MessageEvent) => void> = [];
    (window as any).__pw_messages_sent = [];
    (window as any).chrome = {
      webview: {
        addEventListener: (_: string, fn: (e: MessageEvent) => void) => {
          handlers.push(fn);
          if (handlers.length === 1) {
            // Send German immediately when Angular registers its listener
            Promise.resolve().then(() => {
              const evt = new MessageEvent('message', {
                data: JSON.stringify({ Action: 'ShowSystemLanguage', Language: 'German' }),
              });
              handlers.forEach(h => h(evt));
            });
          }
        },
        removeEventListener: (_: string, fn: (e: MessageEvent) => void) => {
          const i = handlers.indexOf(fn);
          if (i > -1) handlers.splice(i, 1);
        },
        postMessage: (msg: unknown) => (window as any).__pw_messages_sent.push(msg),
      },
    };
    (window as any).__pw_sendToFrontend = (payload: object) => {
      const evt = new MessageEvent('message', { data: JSON.stringify(payload) });
      handlers.forEach(h => h(evt));
    };
  });
  await page.goto('/');
  await page.waitForSelector('app-introduction');
  const deJsonResponse = await deJsonPromise;
  expect(deJsonResponse.status()).toBe(200);
  const body = await deJsonResponse.json();
  expect(body['introduction.title']).toBe('Software-Installationsprozess');
});
```

**Fix RB-04** (malformed JSON never dispatched):
```typescript
test('RB-04: malformed JSON string does not crash', async ({ bridgedPage: page }) => {
  // Dispatch a MessageEvent with malformed JSON directly to the webview handlers
  await page.evaluate(() => {
    const evt = new MessageEvent('message', { data: 'not-json{{{' });
    // Dispatch directly to all registered handlers via the bridge
    (window as any).chrome.webview.__dispatchRaw?.('not-json{{{');
  });
  await expect(page.locator('app-introduction')).toBeVisible();
});
```
*(Or add a `__dispatchRaw` helper to the bridge script that sends raw string data to handlers.)*

### P2 — Fix misleading test names

Rename PR-01 and PR-07:
- `PR-01: progress bar reaches 100%` → `PR-01: OK response causes Step 1 stepper to be marked success`
- `PR-07: progress bar reaches 75%` → `PR-07: NotOk response causes Step 1 stepper to be marked error`

### P3 — Add missing coverage (optional but recommended)

Add to `verification-result-success.spec.ts` and `verification-result-error.spec.ts`:
```typescript
test('VRS-12: copyright text present', async ({ bridgedPage: page }) => {
  await goToStep2ViaOk(page);
  await expect(page.locator('app-verification-result sh-text')).toContainText(
    '© Copyright 2025 Siemens Healthineers AG'
  );
});

test('VRE-11: OK does not send CloseApp', async ({ bridgedPage: page }) => {
  await goToStep2ViaNotOk(page);
  await page.locator('sh-button[label="OK"]').click();
  const sent = await getMessagesSent(page);
  expect(sent.find(m => m.Action === 'CloseApp')).toBeUndefined();
});
```

---

## 7. Summary

| Metric | Value |
|---|---|
| Total spec files | 14 |
| Total tests | 516 |
| Passing (all browsers) | 516 / 516 (100%) |
| Critical false-positive risks | 2 (SL-03, RB-04) |
| Test name mismatches | 2 (PR-01, PR-07) |
| Missing coverage gaps | 9 (all low/medium priority) |
| Suites with full coverage | 12 / 14 |
| Suites needing improvement | 2 (Suite 11: SL-03, Suite 13: RB-04) |

**Overall verdict: The test plan is comprehensive and well-structured. All primary user flows, error paths, edge cases, communication patterns, and cross-browser scenarios are covered. Two tests carry false-positive risk and should be fixed. Nine minor coverage gaps exist but do not represent critical functionality left untested.**
