namespace GuidanceHost.IntegrationTests.StepDefinitions;

/// <summary>
/// Step definitions for the Verify Installation Prerequisites screen.
/// Generic steps (element visible, click, attribute) live in SharedSteps.cs.
/// Lifecycle (connect/navigate/disconnect) lives in AppLifecycle.cs.
///
/// This class navigates from Introduction to Verify Prerequisites in its Given step,
/// then asserts state specific to this screen.
///
/// ta-id values used:
///   abort-btn               — Abort button in the footer
///   abort-modal-cancel-btn  — Cancel button inside the abort confirmation modal
///   abort-modal-confirm-btn — Abort confirm button inside the modal
/// </summary>
[Binding]
public sealed class VerifyPrerequisitesSteps
{
    private readonly PageContext _ctx;

    public VerifyPrerequisitesSteps(PageContext ctx) => _ctx = ctx;

    // ── Given ─────────────────────────────────────────────────────────────

    [Given("the app is running and the verify prerequisites screen is visible")]
    public async Task GivenVerifyPrerequisitesIsVisible()
    {
        var proceedBtn = _ctx.Page.GetByTestId("proceed-btn");
        await Expect(proceedBtn).ToBeVisibleAsync();
        await proceedBtn.ClickAsync();

        await Expect(_ctx.Page.Locator("app-verify-installation-prerequisites"))
            .ToBeVisibleAsync();
    }

    // ── Then: content ─────────────────────────────────────────────────────

    [Then("the element with text {string} is visible")]
    public async Task ThenElementWithTextIsVisible(string text)
    {
        await Expect(_ctx.Page.Locator("app-verify-installation-prerequisites .title"))
            .ToContainTextAsync(text);
    }

    [Then("the progress label shows {string}")]
    public async Task ThenProgressLabelShows(string expected)
    {
        await Expect(_ctx.Page.Locator(".progress-label")).ToContainTextAsync(expected);
    }

    [Then("the progress bar has value {string}")]
    public async Task ThenProgressBarHasValue(string expected)
    {
        await Expect(_ctx.Page.Locator("sh-progress")).ToHaveAttributeAsync("value", expected);
    }

    [Then("the progress bar max is {string}")]
    public async Task ThenProgressBarMaxIs(string expected)
    {
        await Expect(_ctx.Page.Locator("sh-progress")).ToHaveAttributeAsync("max", expected);
    }

    [Then("the status text contains {string}")]
    public async Task ThenStatusTextContains(string text)
    {
        await Expect(_ctx.Page.Locator("app-verify-installation-prerequisites .text"))
            .ToContainTextAsync(text);
    }

    [Then("the status text contains i18n key {string}")]
    public async Task ThenStatusTextContainsI18nKey(string i18nKey)
    {
        var expected = _ctx.Translations.Resolve(i18nKey);
        var firstLine = expected.Split('\n')[0].Trim();
        await Expect(_ctx.Page.Locator("app-verify-installation-prerequisites .text"))
            .ToContainTextAsync(firstLine);
    }

    [Then("the status text does not have error styling")]
    public async Task ThenStatusTextHasNoErrorStyling()
    {
        await Expect(_ctx.Page.Locator("app-verify-installation-prerequisites .text--error"))
            .ToHaveCountAsync(0);
    }

    // ── Then: backend response / navigation ──────────────────────────────

    [Then("the app auto-navigates to the verification result screen")]
    public async Task ThenAutoNavigatesToVerificationResult()
    {
        // Wait up to 20 s — the WPF backend has a ~10.5 s delay before responding.
        await Expect(_ctx.Page.Locator("app-verification-result"))
            .ToBeVisibleAsync(new() { Timeout = 20_000 });
    }

    // ── Then: abort modal ─────────────────────────────────────────────────

    [Then("the abort confirmation modal is visible")]
    public async Task ThenAbortModalIsVisible()
    {
        await Expect(_ctx.Page.Locator("sh-modal#abort-confirm-modal")).ToBeVisibleAsync();
    }

    [Then("the abort confirmation modal is not visible")]
    public async Task ThenAbortModalIsNotVisible()
    {
        await Expect(_ctx.Page.Locator("sh-modal#abort-confirm-modal")).Not.ToBeVisibleAsync();
    }

    [Then("the abort confirmation modal has label {string}")]
    public async Task ThenAbortModalHasLabel(string expectedLabel)
    {
        var actual = await _ctx.Page.Locator("sh-modal#abort-confirm-modal")
            .GetAttributeAsync("label");
        Assert.AreEqual(expectedLabel, actual,
            $"Abort modal: expected label='{expectedLabel}' but got '{actual}'");
    }

    [Then("the abort confirmation modal has i18n label {string}")]
    public async Task ThenAbortModalHasI18nLabel(string i18nKey)
    {
        var expected = _ctx.Translations.Resolve(i18nKey);
        var actual = await _ctx.Page.Locator("sh-modal#abort-confirm-modal")
            .GetAttributeAsync("label");
        Assert.AreEqual(expected, actual,
            $"Abort modal: expected label i18n key '{i18nKey}' → '{expected}' but got '{actual}'");
    }

    [Then("the abort confirmation modal contains text {string}")]
    public async Task ThenAbortModalContainsText(string text)
    {
        await Expect(_ctx.Page.Locator("sh-modal#abort-confirm-modal")).ToContainTextAsync(text);
    }

    [Then("the abort confirmation modal contains i18n text {string}")]
    public async Task ThenAbortModalContainsI18nText(string i18nKey)
    {
        var expected = _ctx.Translations.Resolve(i18nKey);
        await Expect(_ctx.Page.Locator("sh-modal#abort-confirm-modal")).ToContainTextAsync(expected);
    }

    [Then("the verify prerequisites screen is still visible")]
    public async Task ThenVerifyPrerequisitesIsStillVisible()
    {
        await Expect(_ctx.Page.Locator("app-verify-installation-prerequisites")).ToBeVisibleAsync();
    }
}