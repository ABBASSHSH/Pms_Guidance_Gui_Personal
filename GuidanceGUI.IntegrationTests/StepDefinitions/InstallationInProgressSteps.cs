namespace GuidanceHost.IntegrationTests.StepDefinitions;

/// <summary>
/// Step definitions for the Installation In Progress screen.
/// Generic steps (element visible, click, attribute, contains text) live in SharedSteps.cs.
/// Lifecycle (CDP connect / navigate / disconnect) lives in AppLifecycle.cs.
///
/// Navigation path to reach this screen:
///   1. Introduction          — click proceed-btn
///   2. VerifyPrereq          — wait for backend auto-navigate (~10–20 s)
///   3. VerificationResult    — click proceed-install-btn  (success path)
///   4. SavePatientImages     — click proceed-btn
///   5. DriveToParkPosition   — click proceed-btn
///   6. InstallationInProgress — this screen (no user interaction — passive spinner)
///
/// ta-id values used by the feature file:
///   installation-in-progress-screen — host component wrapper (app.component.html @case 5)
///   installation-spinner            — sh-spinner element inside the screen
///
/// Note: This screen has NO proceed-btn or cancel-btn.
///       IIP-D08 and IIP-D09 use a dedicated step to assert absence.
/// </summary>
[Binding]
public sealed class InstallationInProgressSteps
{
    private readonly PageContext _ctx;

    public InstallationInProgressSteps(PageContext ctx) => _ctx = ctx;

    /// <summary>
    /// Background step: drives the app from cold start through the full
    /// Introduction → VerifyPrereq → VerificationResult (success) →
    /// SavePatientImages → DriveToParkPosition → InstallationInProgress path.
    /// </summary>
    [Given("the app is running and the installation in progress screen is visible")]
    public async Task GivenInstallationInProgressIsVisible()
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

        // Step 5 — DriveToParkPosition: click Proceed to start the installation.
        await Expect(_ctx.Page.GetByTestId("drive-to-park-position-screen")).ToBeVisibleAsync();

        proceedBtn = _ctx.Page.GetByTestId("proceed-btn");
        await Expect(proceedBtn).ToBeVisibleAsync();
        await proceedBtn.ClickAsync();

        // Step 6 — Arrive at Installation In Progress.
        await Expect(_ctx.Page.GetByTestId("installation-in-progress-screen")).ToBeVisibleAsync();
    }

    // ── IIP-D: Assert no interactive button is present ────────────────────
    // NOTE: "the element {string} has no interactive button {string}" is defined
    //       in SharedSteps.cs and is reused here via Reqnroll's shared binding resolution.
}
