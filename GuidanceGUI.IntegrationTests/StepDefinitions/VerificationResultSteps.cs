namespace GuidanceHost.IntegrationTests.StepDefinitions;

/// <summary>
/// Step definitions for the Verification Result screen.
/// Generic steps (element visible, click, attribute, contains text) live in SharedSteps.cs.
/// Lifecycle (CDP connect / navigate / disconnect) lives in AppLifecycle.cs.
///
/// Navigation path to reach this screen:
///   1. Introduction  — click proceed-btn
///   2. VerifyPrereq  — wait for WPF backend to auto-navigate (~10–20 s)
///   3. VerificationResult — this screen (success OR error path, driven by backend config)
///
/// ta-id values used by the feature file:
///   verification-result-screen  — host component wrapper (app.component.html @case 2)
///   result-status               — sh-notification-item (success or error banner)
///   cancel-btn                  — Cancel button (success path only)
///   proceed-install-btn         — Proceed with Installation button (success path only)
///   show-report-btn             — Show Report button (error path only)
///   ok-btn                      — OK button (error path only)
///   save-patient-images-screen  — target screen after clicking Proceed with Installation
/// </summary>
[Binding]
public sealed class VerificationResultSteps
{
    private readonly PageContext _ctx;

    public VerificationResultSteps(PageContext ctx) => _ctx = ctx;

    // ── Given — success path ───────────────────────────────────────────────

    [Given("the app is running and the verification result screen is visible with a success result")]
    public async Task GivenVerificationResultSuccessIsVisible()
    {
        await NavigateToVerificationResultAsync();
    }

    // ── Given — error path ────────────────────────────────────────────────

    [Given("the app is running and the verification result screen is visible with an error result")]
    public async Task GivenVerificationResultErrorIsVisible()
    {
        await NavigateToVerificationResultAsync();
    }

    // ── Shared navigation helper ──────────────────────────────────────────

    private async Task NavigateToVerificationResultAsync()
    {
        // Step 1 — Introduction: click Proceed to trigger prerequisite verification.
        var proceedBtn = _ctx.Page.GetByTestId("proceed-btn");
        await Expect(proceedBtn).ToBeVisibleAsync();
        await proceedBtn.ClickAsync();

        // Step 2 — VerifyPrereq: wait up to 20 s for the WPF backend to respond
        // (~10.5 s delay) and Angular to auto-navigate to the verification result screen.
        await Expect(_ctx.Page.Locator("app-verification-result"))
            .ToBeVisibleAsync(new() { Timeout = 20_000 });
    }

    // ── Then: element not present ──────────────────────────────────────────
    // NOTE: "the element {string} is not present" is defined in SharedSteps.cs
    //       and is reused here via Reqnroll's shared binding resolution.
}
