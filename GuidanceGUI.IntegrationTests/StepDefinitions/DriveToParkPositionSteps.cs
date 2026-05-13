namespace GuidanceHost.IntegrationTests.StepDefinitions;

/// <summary>
/// Step definitions for the Drive To Park Position screen.
/// Generic steps (element visible, click, attribute, contains text) live in SharedSteps.cs.
/// Lifecycle (CDP connect / navigate / disconnect) lives in AppLifecycle.cs.
///
/// Navigation path to reach this screen:
///   1. Introduction          — click proceed-btn
///   2. VerifyPrereq          — wait for backend auto-navigate (~10–20 s)
///   3. VerificationResult    — click proceed-install-btn  (success path)
///   4. SavePatientImages     — click proceed-btn
///   5. DriveToParkPosition   — this screen
///
/// ta-id values used by the feature file:
///   drive-to-park-position-screen    — host component wrapper (app.component.html @case 4)
///   proceed-btn                      — Proceed button
///   cancel-btn                       — Cancel button
///   installation-in-progress-screen  — target screen after Proceed (@case 5)
/// </summary>
[Binding]
public sealed class DriveToParkPositionSteps
{
    private readonly PageContext _ctx;

    public DriveToParkPositionSteps(PageContext ctx) => _ctx = ctx;

    /// <summary>
    /// Background step: drives the app from cold start through the full
    /// Introduction → VerifyPrereq → VerificationResult (success) →
    /// SavePatientImages → DriveToParkPosition path.
    /// </summary>
    [Given("the app is running and the drive to park position screen is visible")]
    public async Task GivenDriveToParkPositionIsVisible()
    {
        // Step 1 — Introduction: click Proceed to start prerequisite verification.
        var proceedBtn = _ctx.Page.GetByTestId("proceed-btn");
        await Expect(proceedBtn).ToBeVisibleAsync();
        await proceedBtn.ClickAsync();

        // Step 2 — VerifyPrereq: wait up to 20 s for the WPF backend to respond
        // (~10.5 s delay) and Angular to auto-navigate to the verification result screen.
        await Expect(_ctx.Page.Locator("app-verification-result"))
            .ToBeVisibleAsync(new() { Timeout = 20_000 });

        // Step 3 — VerificationResult (success path): click Proceed with Installation.
        var proceedInstallBtn = _ctx.Page.GetByTestId("proceed-install-btn");
        await Expect(proceedInstallBtn).ToBeVisibleAsync();
        await proceedInstallBtn.ClickAsync();

        // Step 4 — SavePatientImages: click Proceed to advance.
        await Expect(_ctx.Page.GetByTestId("save-patient-images-screen")).ToBeVisibleAsync();

        proceedBtn = _ctx.Page.GetByTestId("proceed-btn");
        await Expect(proceedBtn).ToBeVisibleAsync();
        await proceedBtn.ClickAsync();

        // Step 5 — Arrive at Drive To Park Position.
        await Expect(_ctx.Page.GetByTestId("drive-to-park-position-screen")).ToBeVisibleAsync();
    }
}
