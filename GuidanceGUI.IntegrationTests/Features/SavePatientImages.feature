Feature: Save Patient Images
  The Save Patient Images screen is step 4 of the GuidanceUI.
  It instructs the user to archive or export patient images before installation begins,
  then lets them Proceed (advance to Drive to Park Position) or Cancel (send CloseApp).

  Background:
    Given the app is running and the save patient images screen is visible

  # ── SPI-A: Screen presence ─────────────────────────────────────────────────

  @SPI-A01
  Scenario: Save Patient Images screen is visible
    Then the element "save-patient-images-screen" is visible

  # ── SPI-B: Static content ──────────────────────────────────────────────────

  @SPI-B02
  Scenario Outline: Screen displays the correct static text
    Then the element "save-patient-images-screen" contains i18n text "<key>"

    Examples:
      | tag    | key                              |
      | SPI-B02 | savePatientImages.title         |
      | SPI-B03 | savePatientImages.description   |
      | SPI-B04 | savePatientImages.info          |
      | SPI-B05 | common.copyright                |

  # ── SPI-C: Buttons — visibility, color and label ───────────────────────────

  @SPI-C06
  Scenario Outline: Action buttons are visible
    Then the element "<element>" is visible

    Examples:
      | tag    | element     |
      | SPI-C06 | proceed-btn |
      | SPI-C07 | cancel-btn  |

  @SPI-C08
  Scenario Outline: Action buttons have the correct color
    Then the element "<element>" has attribute "color" equal to "<color>"

    Examples:
      | tag    | element     | color     |
      | SPI-C08 | proceed-btn | primary   |
      | SPI-C09 | cancel-btn  | secondary |

  @SPI-C10
  Scenario Outline: Action buttons display the correct label
    Then the element "<element>" has attribute "label" equal to i18n key "<key>"

    Examples:
      | tag    | element     | key            |
      | SPI-C10 | proceed-btn | common.proceed |
      | SPI-C11 | cancel-btn  | common.cancel  |

  # ── SPI-D: Info-bar hint ───────────────────────────────────────────────────

  @SPI-D12
  Scenario: Info-bar hint displays the correct text
    Then the element "save-patient-images-info-bar-hint" has attribute "label" equal to i18n key "savePatientImages.info"

  # ── SPI-E: Navigation ──────────────────────────────────────────────────────

  @SPI-E13
  Scenario: Clicking Proceed navigates to the Drive to Park Position screen
    When the element "proceed-btn" is clicked
    Then the element "drive-to-park-position-screen" is visible

  # NOTE: Cancel sends CloseApp to the WPF host which terminates the WebView2 session.
  # This drops the CDP connection so the outcome cannot be asserted from the test runner.
  # Cancel is therefore covered by unit tests only (Rule 2 — untestable navigation).
