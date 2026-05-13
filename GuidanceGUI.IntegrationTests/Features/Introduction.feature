Feature: Introduction Screen
  The Introduction screen is the first screen shown when the Guidance app launches.

  Background:
    Given the app is running and the introduction screen is visible

  # ---------------------------------------------------------------------------
  # Screen content
  # ---------------------------------------------------------------------------

  @TC01
  Scenario: Introduction screen is visible
    Then the element "introduction-screen" is visible

  @TC02 @TC03 @TC04 @TC05
  Scenario Outline: Introduction screen displays the correct static text
    Then the element "introduction-screen" contains i18n text "<key>"

    Examples:
      | tag  | key                     |
      | TC02 | introduction.title      |
      | TC03 | introduction.description|
      | TC04 | introduction.info       |
      | TC05 | common.copyright        |

  # ---------------------------------------------------------------------------
  # Buttons — visibility, labels, and colors
  # ---------------------------------------------------------------------------

  @TC06 @TC07
  Scenario Outline: Action buttons are visible
    Then the element "<element>" is visible

    Examples:
      | tag  | element     |
      | TC06 | proceed-btn |
      | TC07 | cancel-btn  |

  @TC08 @TC09
  Scenario Outline: Action buttons have the correct color
    Then the element "<element>" has attribute "color" equal to "<color>"

    Examples:
      | tag  | element     | color     |
      | TC08 | proceed-btn | primary   |
      | TC09 | cancel-btn  | secondary |

  @TC11 @TC12
  Scenario Outline: Action buttons display the correct label
    Then the element "<element>" has attribute "label" equal to i18n key "<key>"

    Examples:
      | tag  | element     | key            |
      | TC11 | proceed-btn | common.proceed |
      | TC12 | cancel-btn  | common.cancel  |

  # ---------------------------------------------------------------------------
  # Info-bar hint
  # ---------------------------------------------------------------------------

  @TC13
  Scenario: Info-bar hint displays the correct text
    # introduction.info is a full sentence containing the translated "Proceed" button label.
    Then the element "introduction-info-bar-hint" has attribute "label" equal to i18n key "introduction.info"

  # ---------------------------------------------------------------------------
  # Navigation
  # ---------------------------------------------------------------------------

  @TC10
  Scenario: Clicking Proceed navigates to the Verify Prerequisites screen
    When the element "proceed-btn" is clicked
    Then the element "verify-prerequisites-screen" is visible

  # NOTE: Cancel sends CloseApp to the WPF host which terminates the WebView2 session.
  # This drops the CDP connection so the outcome cannot be asserted from the test runner.
  # Cancel is therefore covered by unit tests only (Rule 2 — untestable navigation).
