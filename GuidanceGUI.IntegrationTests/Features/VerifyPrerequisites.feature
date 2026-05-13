Feature: Verify Installation Prerequisites
  The Verify Installation Prerequisites screen is step 2 of the GuidanceUI.
  On entry Angular sends VerifyInstallationPrerequisite to the WPF backend and
  animates a progress bar while waiting.  The operator may press Abort at any time,
  confirm or cancel in the modal.  All scenarios here are read-only or driven by
  a single UI action; backend-injected state transitions are not tested here.

  Background:
    Given the app is running and the verify prerequisites screen is visible

  # ── VVP-A: Screen presence ────────────────────────────────────────────────

  @VVP-A01
  Scenario: VVP-01 — Screen is visible
    Then the element "verify-prerequisites-screen" is visible

  # ── VVP-B: Static content ─────────────────────────────────────────────────

  @VVP-B02
  Scenario Outline: VVP-<tag> — Screen displays the correct static text
    Then the element "verify-prerequisites-screen" contains i18n text "<key>"

    Examples:
      | tag | key                          |
      | 02  | verification.title           |
      | 03  | verification.status.inProgress |
      | 04  | verification.info            |
      | 05  | common.copyright             |

  # ── VVP-C: Initial status text ───────────────────────────────────────────
  # Progress bar value is not asserted — the interval(250ms) fires immediately
  # and the value is non-deterministic by the time CDP connects.
  # Progress bar max=100 is a static binding and is always safe to assert.

  @VVP-C06
  Scenario: VVP-06 — Status text shows in-progress message on entry
    Then the status text contains i18n key "verification.status.inProgress"

  @VVP-C07
  Scenario: VVP-07 — Status text has no error styling on entry
    Then the status text does not have error styling

  @VVP-C08
  Scenario: VVP-08 — Progress bar max is 100
    Then the progress bar max is "100"

  # ── VVP-D: Abort button — visibility, color, label ────────────────────────

  @VVP-D08
  Scenario: VVP-08 — Abort button has the correct color
    Then the element "abort-btn" has attribute "color" equal to "secondary"

  @VVP-D09
  Scenario: VVP-09 — Abort button displays the correct label
    Then the element "abort-btn" has attribute "label" equal to i18n key "common.abort"

  @VVP-D10
  Scenario: VVP-10 — Abort button is visible
    Then the element "abort-btn" is visible

  # ── VVP-E: Abort modal — hidden by default, content when open ────────────
  # All assertions on the open modal are grouped in one scenario (Rule 3):
  # one app state, one CDP round-trip, no redundant restarts.

  @VVP-E11
  Scenario: VVP-11 — Abort modal is not visible before Abort is pressed
    Then the abort confirmation modal is not visible

  @VVP-E12
  Scenario: VVP-12 — Pressing Abort opens the modal with correct content and buttons
    When the element "abort-btn" is clicked
    Then the abort confirmation modal is visible
    And  the abort confirmation modal has i18n label "verification.abort.modal.label"
    And  the abort confirmation modal contains i18n text "verification.abort.modal.description"
    And  the element "abort-modal-cancel-btn" is visible
    And  the element "abort-modal-cancel-btn" has attribute "label" equal to i18n key "common.cancel"
    And  the element "abort-modal-cancel-btn" has attribute "color" equal to "secondary"
    And  the element "abort-modal-confirm-btn" is visible
    And  the element "abort-modal-confirm-btn" has attribute "label" equal to i18n key "common.abort"

  # ── VVP-F: Abort modal — Cancel path (modal closes, screen persists) ─────

  @VVP-F13
  Scenario: VVP-13 — Pressing Cancel in the modal closes it and keeps the screen
    When the element "abort-btn" is clicked
    And  the element "abort-modal-cancel-btn" is clicked
    Then the abort confirmation modal is not visible
    And  the verify prerequisites screen is still visible

  # ── VVP-G: Abort modal — Confirm path ────────────────────────────────────
  # TODO: Add VVP-G14 once the WPF backend abort-reset flow is implemented.
  #       The correct post-abort behaviour will be defined at that point.

  # ── VVP-H: Auto-navigation after backend response ──────────────────────
  # After the WPF backend sends ShowInstallationPrerequisite the Angular app
  # automatically navigates to the Verification Result screen
  # VerifyInstallationPrerequisite → ShowInstallationPrerequisite → auto-navigate
  # protocol path.

  @VVP-H15
  Scenario: VVP-15 — App auto-navigates to Verification Result after backend response
    # Timeout is generous (20 s) to cover the ~10.5 s WPF host delay.
    Then the app auto-navigates to the verification result screen
