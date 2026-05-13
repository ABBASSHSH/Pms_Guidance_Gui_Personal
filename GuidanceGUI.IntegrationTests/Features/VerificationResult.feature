Feature: Verification Result
  The Verification Result screen is step 3 of the GuidanceUI.
  It is reached automatically after the WPF backend responds to the
  VerifyInstallationPrerequisite command and Angular auto-navigates here.
  The screen renders two entirely different layouts depending on whether the
  backend sent Status="OK" (prereqOk=true) or Status="Not Ok" (prereqOk=false).
  Both paths are tested here; each suite requires the WPF host to be configured
  with the matching DefaultPrerequisiteStatus value in MainWindow.xaml.cs.

  # ════════════════════════════════════════════════════════════════════════
  # SUITE 1 — SUCCESS PATH  (DefaultPrerequisiteStatus = "OK")
  # ════════════════════════════════════════════════════════════════════════

  Rule: When the backend reports OK the success layout is rendered

    Background:
      Given the app is running and the verification result screen is visible with a success result

    # ── VRS-A: Screen presence ──────────────────────────────────────────────

    @VRS-A01
    Scenario: VRS-01 — Verification Result screen is visible
      Then the element "verification-result-screen" is visible

    # ── VRS-B: Static content ───────────────────────────────────────────────

    @VRS-B02
    Scenario Outline: VRS-<tag> — Screen displays the correct static text
      Then the element "verification-result-screen" contains i18n text "<key>"

      Examples:
        | tag | key                                    |
        | 02  | verificationResult.title               |
        | 03  | verificationResult.success.subtitle    |
        | 04  | verificationResult.success.body        |
        | 05  | verificationResult.success.info        |
        | 06  | common.copyright                       |

    # ── VRS-C: Success notification banner ─────────────────────────────────

    @VRS-C07
    Scenario: VRS-07 — Result status notification is visible
      Then the element "result-status" is visible

    @VRS-C08
    Scenario Outline: VRS-<tag> — Result status notification has the correct attribute
      Then the element "result-status" has attribute "<attribute>" equal to "<value>"

      Examples:
        | tag | attribute | value   |
        | 08  | type      | success |

    @VRS-C09
    Scenario: VRS-09 — Result status notification label matches i18n key
      Then the element "result-status" has attribute "label" equal to i18n key "verificationResult.success.title"

    # ── VRS-D: Action buttons ───────────────────────────────────────────────

    @VRS-D10
    Scenario Outline: VRS-<tag> — Success-path button is visible
      Then the element "<element>" is visible

      Examples:
        | tag | element             |
        | 10  | cancel-btn          |
        | 11  | proceed-install-btn |

    @VRS-D12
    Scenario Outline: VRS-<tag> — Success-path button has the correct color
      Then the element "<element>" has attribute "color" equal to "<color>"

      Examples:
        | tag | element             | color     |
        | 12  | cancel-btn          | secondary |
        | 13  | proceed-install-btn | primary   |

    @VRS-D14
    Scenario Outline: VRS-<tag> — Success-path button has the correct label
      Then the element "<element>" has attribute "label" equal to i18n key "<key>"

      Examples:
        | tag | element             | key                                       |
        | 14  | cancel-btn          | common.cancel                             |
        | 15  | proceed-install-btn | verificationResult.success.proceedInstall |

    # ── VRS-E: Error-path buttons are absent ────────────────────────────────

    @VRS-E16
    Scenario Outline: VRS-<tag> — Error-path button is not present on the success path
      Then the element "<element>" is not present

      Examples:
        | tag | element         |
        | 16  | show-report-btn |
        | 17  | ok-btn          |

    # ── VRS-F: Info-bar hint ─────────────────────────────────────────────────

    @VRS-F18
    Scenario: VRS-18 — Info-bar hint displays the correct text on success path
      Then the element "verification-result-info-bar-hint" has attribute "label" equal to i18n key "verificationResult.success.info"

    # ── VRS-G: Navigation (last — changes screen) ────────────────────────────

    @VRS-G19
    Scenario: VRS-19 — Clicking Proceed with Installation navigates to Save Patient Images
      When the element "proceed-install-btn" is clicked
      Then the element "save-patient-images-screen" is visible

  # ════════════════════════════════════════════════════════════════════════
  # SUITE 2 — ERROR PATH  (DefaultPrerequisiteStatus = "Not Ok")
  # ════════════════════════════════════════════════════════════════════════

  Rule: When the backend reports Not Ok the error layout is rendered

    Background:
      Given the app is running and the verification result screen is visible with an error result

    # ── VRE-A: Screen presence ──────────────────────────────────────────────

    @VRE-A01
    Scenario: VRE-01 — Verification Result screen is visible on error path
      Then the element "verification-result-screen" is visible

    # ── VRE-B: Static content ───────────────────────────────────────────────

    @VRE-B02
    Scenario Outline: VRE-<tag> — Screen displays the correct static text on error path
      Then the element "verification-result-screen" contains i18n text "<key>"

      Examples:
        | tag | key                                   |
        | 02  | verificationResult.title              |
        | 03  | verificationResult.error.subtitle     |
        | 04  | verificationResult.error.body         |
        | 05  | verificationResult.error.info         |
        | 06  | common.copyright                      |

    # ── VRE-C: Error notification banner ────────────────────────────────────

    @VRE-C07
    Scenario: VRE-07 — Result status notification is visible on error path
      Then the element "result-status" is visible

    @VRE-C08
    Scenario Outline: VRE-<tag> — Result status notification has the correct attribute on error path
      Then the element "result-status" has attribute "<attribute>" equal to "<value>"

      Examples:
        | tag | attribute | value |
        | 08  | type      | error |

    @VRE-C09
    Scenario: VRE-09 — Result status notification label matches i18n key on error path
      Then the element "result-status" has attribute "label" equal to i18n key "verificationResult.error.title"

    # ── VRE-D: Action buttons ───────────────────────────────────────────────

    @VRE-D10
    Scenario Outline: VRE-<tag> — Error-path button is visible
      Then the element "<element>" is visible

      Examples:
        | tag | element         |
        | 10  | show-report-btn |
        | 11  | ok-btn          |

    @VRE-D12
    Scenario Outline: VRE-<tag> — Error-path button has the correct color
      Then the element "<element>" has attribute "color" equal to "<color>"

      Examples:
        | tag | element         | color     |
        | 12  | show-report-btn | secondary |
        | 13  | ok-btn          | primary   |

    @VRE-D14
    Scenario Outline: VRE-<tag> — Error-path button has the correct label
      Then the element "<element>" has attribute "label" equal to i18n key "<key>"

      Examples:
        | tag | element         | key                                  |
        | 14  | show-report-btn | verificationResult.error.showReport  |
        | 15  | ok-btn          | common.ok                            |

    # ── VRE-E: Success-path buttons are absent ──────────────────────────────

    @VRE-E16
    Scenario Outline: VRE-<tag> — Success-path button is not present on the error path
      Then the element "<element>" is not present

      Examples:
        | tag | element             |
        | 16  | cancel-btn          |
        | 17  | proceed-install-btn |

    # ── VRE-F: Info-bar hint ─────────────────────────────────────────────────

    @VRE-F18
    Scenario: VRE-18 — Info-bar hint displays the correct text on error path
      Then the element "verification-result-info-bar-hint" has attribute "label" equal to i18n key "verificationResult.error.info"

    # ── VRE-G: Navigation — error path ──────────────────────────────────────
    # TODO: Add VRE-G19 once the WPF backend OK-button reset flow is implemented.
    #       The correct post-error navigation behaviour will be defined at that point.

