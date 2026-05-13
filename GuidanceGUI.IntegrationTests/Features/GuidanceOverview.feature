Feature: Guidance Overview Stepper Panel
  The Guidance Overview is a persistent left-hand stepper panel visible throughout the
  entire GuidanceUI flow. It shows all 6 steps with their labels, tracks which step is active,
  and marks completed steps as success/error using the SHUI sh-stepper-item [type] attribute.

  Background:
    Given the app is running and the guidance overview panel is visible

  # ── GO-A: Structure ────────────────────────────────────────────────────────

  @GO-A01
  Scenario: Stepper panel is visible on first load
    Then the element "GuidanceAppStepper" is visible

  @GO-A02
  Scenario: Stepper has vertical layout and primary color
    Then the element "GuidanceAppStepper" has attribute "vertical" equal to ""
    And  the element "GuidanceAppStepper" has attribute "color" equal to "primary"

  @GO-A03
  Scenario: Exactly 6 stepper items are rendered
    Then exactly 6 stepper items are rendered

  # ── GO-B: Step Labels ──────────────────────────────────────────────────────

  @GO-B04
  Scenario Outline: Each stepper item displays the correct translated label
    Then stepper item <step> has i18n label "<key>"

    Examples:
      | tag    | step | key                     |
      | GO-B04 | 1    | steps.introduction      |
      | GO-B05 | 2    | steps.verifyPrereq      |
      | GO-B06 | 3    | steps.verificationResult|
      | GO-B07 | 4    | steps.saveImages        |
      | GO-B08 | 5    | steps.driveToPark       |
      | GO-B09 | 6    | steps.installation      |

  # ── GO-C: Initial State ────────────────────────────────────────────────────

  @GO-C10
  Scenario: All steps start in clean state on first load
    # Step 1 is active — SHUI reflects type="active" in the DOM.
    # Steps 2-6 have never been set in the status map (undefined) so no type attribute is rendered.
    # This guards against state pollution from a previous test run.
    Then stepper item 1 is active
    And  stepper item 1 has type "active"
    And  stepper item 2 is not active
    And  stepper item 2 has no type attribute
    And  stepper item 3 is not active
    And  stepper item 3 has no type attribute
    And  stepper item 4 is not active
    And  stepper item 4 has no type attribute
    And  stepper item 5 is not active
    And  stepper item 5 has no type attribute
    And  stepper item 6 is not active
    And  stepper item 6 has no type attribute

  # ── GO-D: State After First Navigation ────────────────────────────────────

  @GO-D11
  Scenario: Clicking Proceed transitions stepper state correctly
    # Covers: step 2 becomes active (type="active"), step 1 deactivated and marked success,
    # steps 3-6 status never set (undefined → no type attribute), stepper stays visible,
    # only one step is active.
    When the user clicks Proceed on the Introduction screen
    Then the element "GuidanceAppStepper" is visible
    And  exactly 1 stepper item is active
    And  stepper item 1 has type "success"
    And  stepper item 1 is not active
    And  stepper item 2 is active
    And  stepper item 2 has type "active"
    And  stepper item 3 has no type attribute
    And  stepper item 4 has no type attribute
    And  stepper item 5 has no type attribute
    And  stepper item 6 has no type attribute

  # ── GO-E: Stepper State on Verification Success Path (steps 3, 4, 5, 6) ──
  # Each scenario navigates to that screen via its Background step so the stepper
  # state is always observed in the correct GuidanceUI phase.

  @GO-E12
  Scenario: Step 3 becomes active after successful prerequisite verification
    # Requires DefaultPrerequisiteStatus = "OK" in MainWindow.xaml.cs.
    Given the app is running and the verification result screen is visible with a success result
    Then exactly 1 stepper item is active
    And  stepper item 1 has type "success"
    And  stepper item 2 has type "success"
    And  stepper item 3 is active
    And  stepper item 3 has type "active"
    And  stepper item 4 has no type attribute
    And  stepper item 5 has no type attribute
    And  stepper item 6 has no type attribute

  @GO-E13
  Scenario: Step 4 becomes active after proceeding from Verification Result
    # Requires DefaultPrerequisiteStatus = "OK".
    Given the app is running and the save patient images screen is visible
    Then exactly 1 stepper item is active
    And  stepper item 1 has type "success"
    And  stepper item 2 has type "success"
    And  stepper item 3 has type "success"
    And  stepper item 4 is active
    And  stepper item 4 has type "active"
    And  stepper item 5 has no type attribute
    And  stepper item 6 has no type attribute

  @GO-E14
  Scenario: Step 5 becomes active after proceeding from Save Patient Images
    Given the app is running and the drive to park position screen is visible
    Then exactly 1 stepper item is active
    And  stepper item 1 has type "success"
    And  stepper item 2 has type "success"
    And  stepper item 3 has type "success"
    And  stepper item 4 has type "success"
    And  stepper item 5 is active
    And  stepper item 5 has type "active"
    And  stepper item 6 has no type attribute

  @GO-E15
  Scenario: Step 6 becomes active after proceeding from Drive to Park Position
    Given the app is running and the installation in progress screen is visible
    Then exactly 1 stepper item is active
    And  stepper item 1 has type "success"
    And  stepper item 2 has type "success"
    And  stepper item 3 has type "success"
    And  stepper item 4 has type "success"
    And  stepper item 5 has type "success"
    And  stepper item 6 is active
    And  stepper item 6 has type "active"

  # ── GO-F: Stepper State on Verification Error Path ─────────────────────
  # When the backend reports "Not Ok":
  #   step 1 — type="success" (completed successfully)
  #   step 2 — type="error"   (VerifyPrereq step that failed)
  #   step 3 — active, type="active" (current screen)
  #   steps 4-6 — no type attribute (never visited)

  @GO-F16
  Scenario: Step 3 is marked error after failed prerequisite verification
    # Requires SSIT_PREREQ_STATUS="Not Ok" when launching GuidanceHost.
    Given the app is running and the verification result screen is visible with an error result
    Then exactly 1 stepper item is active
    And  stepper item 1 has type "success"
    And  stepper item 2 has type "error"
    And  stepper item 3 is active
    And  stepper item 3 has type "active"
    And  stepper item 4 has no type attribute
    And  stepper item 5 has no type attribute
    And  stepper item 6 has no type attribute
