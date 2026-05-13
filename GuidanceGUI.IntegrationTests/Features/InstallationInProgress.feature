Feature: Installation In Progress
  The Installation In Progress screen is a passive, non-interactive screen that
  is displayed after the user clicks Proceed on the Drive To Park Position screen.
  Angular sends an InstallSoftware command to the WPF backend on entry, then waits
  for the backend to drive further navigation.  The screen presents a large spinner
  with a status label and an informational message — the user has no buttons to
  interact with.

  Background:
    Given the app is running and the installation in progress screen is visible

  # ── IIP-A: Screen presence ──────────────────────────────────────────────

  @IIP-A01
  Scenario: Installation In Progress screen is visible
    Then the element "installation-in-progress-screen" is visible

  # ── IIP-B: Static text content ──────────────────────────────────────────

  @IIP-B02
  Scenario: Screen displays the spinner status label
    Then the element "installation-in-progress-screen" contains i18n text "installation.status.inProgress"

  # ── IIP-C: Spinner element ──────────────────────────────────────────────

  @IIP-C05
  Scenario: The installation spinner is visible
    Then the element "installation-spinner" is visible

  @IIP-C06
  Scenario: The spinner has the medium size (SHUI sh-spinner reflects "l" binding as "m")
    Then the element "installation-spinner" has attribute "size" equal to "m"

  @IIP-C07
  Scenario: The spinner displays the correct status label
    Then the element "installation-spinner" has attribute "label" equal to i18n key "installation.status.inProgress"

  # ── IIP-D: No interactive controls present ──────────────────────────────

  @IIP-D08
  Scenario: There is no Proceed button on the screen
    Then the element "installation-in-progress-screen" has no interactive button "proceed-btn"

  @IIP-D09
  Scenario: There is no Cancel button on the screen
    Then the element "installation-in-progress-screen" has no interactive button "cancel-btn"
