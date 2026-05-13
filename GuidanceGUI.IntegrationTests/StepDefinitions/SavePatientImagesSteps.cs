namespace GuidanceHost.IntegrationTests.StepDefinitions;

/// <summary>
/// Step definitions for the Save Patient Images screen.
/// Generic steps (element visible, click, attribute, contains text) live in SharedSteps.cs.
/// Lifecycle (CDP connect / navigate / disconnect) lives in AppLifecycle.cs.
///
/// Navigation path to reach this screen:
///   1. Introduction  — click proceed-btn
///   2. VerifyPrereq  — wait for backend auto-navigate (~10–20 s)
///   3. VerificationResult (success) — click proceed-install-btn
///   4. SavePatientImages — this screen
///
/// ta-id values used by the feature file:
///   save-patient-images-screen  — the host component wrapper (app.component.html @case 3)
///   proceed-btn                 — Proceed button
///   cancel-btn                  — Cancel button
///   drive-to-park-position-screen — target screen after Proceed (app.component.html @case 4)
/// </summary>
[Binding]
public sealed class SavePatientImagesSteps
{
    private readonly PageContext _ctx;

    public SavePatientImagesSteps(PageContext ctx) => _ctx = ctx;

    /// <summary>
    /// Background step: drives the app from cold start through the full
    /// Introduction → VerifyPrereq → VerificationResult (success) → SavePatientImages path.
    /// </summary>
    [Given("the app is running and the save patient images screen is visible")]
    public async Task GivenSavePatientImagesIsVisible()
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

        // Step 4 — Arrive at Save Patient Images.
        await Expect(_ctx.Page.GetByTestId("save-patient-images-screen")).ToBeVisibleAsync();
    }
}
