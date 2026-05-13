Feature: Drive To Park Position
  The Drive To Park Position screen is step 5 of the GuidanceUI.
  It presents 4 numbered instructions the operator must follow before installation,
  then lets them Proceed (advance to Installation) or Cancel (send CloseApp).

  Background:
    Given the app is running and the drive to park position screen is visible

  # ── DTP-A: Screen presence ────────────────────────────────────────────────

  @DTP-A01
  Scenario: Drive to Park Position screen is visible
    Then the element "drive-to-park-position-screen" is visible

  # ── DTP-B: Static content ─────────────────────────────────────────────────

  @DTP-B02
  Scenario Outline: Screen displays the correct static text
    Then the element "drive-to-park-position-screen" contains i18n text "<key>"

    Examples:
      | tag     | key                  |
      | DTP-B02 | driveToPark.title    |
      | DTP-B03 | driveToPark.step1    |
      | DTP-B04 | driveToPark.step2    |
      | DTP-B05 | driveToPark.step3    |
      | DTP-B06 | driveToPark.step4    |
      | DTP-B07 | driveToPark.info     |
      | DTP-B08 | common.copyright     |

  # ── DTP-C: Buttons — visibility, color and label ──────────────────────────

  @DTP-C09
  Scenario Outline: Action buttons are visible
    Then the element "<element>" is visible

    Examples:
      | tag     | element     |
      | DTP-C09 | proceed-btn |
      | DTP-C10 | cancel-btn  |

  @DTP-C11
  Scenario Outline: Action buttons have the correct color
    Then the element "<element>" has attribute "color" equal to "<color>"

    Examples:
      | tag     | element     | color     |
      | DTP-C11 | proceed-btn | primary   |
      | DTP-C12 | cancel-btn  | secondary |

  @DTP-C13
  Scenario Outline: Action buttons display the correct label
    Then the element "<element>" has attribute "label" equal to i18n key "<key>"

    Examples:
      | tag     | element     | key            |
      | DTP-C13 | proceed-btn | common.proceed |
      | DTP-C14 | cancel-btn  | common.cancel  |

  # ── DTP-D: Info-bar hint ──────────────────────────────────────────────────

  @DTP-D15
  Scenario: Info-bar hint displays the correct text
    Then the element "drive-to-park-info-bar-hint" has attribute "label" equal to i18n key "driveToPark.info"

  # ── DTP-E: Navigation ─────────────────────────────────────────────────────

  @DTP-E16
  Scenario: Clicking Proceed navigates to the Installation screen
    When the element "proceed-btn" is clicked
    Then the element "installation-in-progress-screen" is visible

  # NOTE: Cancel sends CloseApp to the WPF host which terminates the WebView2 session.
  # This drops the CDP connection so the outcome cannot be asserted from the test runner.
  # Cancel is therefore covered by unit tests only (Rule 2 — untestable navigation).
