namespace GuidanceHost.IntegrationTests.StepDefinitions;

/// <summary>
/// Step definitions for the Guidance Overview stepper panel.
/// Generic steps (element visible, click, attribute) live in SharedSteps.cs.
/// Lifecycle (connect/navigate/disconnect) lives in AppLifecycle.cs.
///
/// ta-id values used:
///   GuidanceAppStepper          — the sh-stepper container
///   GuidanceAppStepperStep1..6  — individual sh-stepper-item elements (1-based)
/// </summary>
[Binding]
public sealed class GuidanceOverviewSteps
{
    private readonly PageContext _ctx;

    public GuidanceOverviewSteps(PageContext ctx) => _ctx = ctx;

    // ── Helper ────────────────────────────────────────────────────────────

    private Microsoft.Playwright.ILocator StepperItem(int stepNumber)
        => _ctx.Page.Locator($"sh-stepper-item[ta-id='GuidanceAppStepperStep{stepNumber}']");

    // ── Given ─────────────────────────────────────────────────────────────

    [Given("the app is running and the guidance overview panel is visible")]
    public async Task GivenGuidanceOverviewIsVisible()
    {
        await Expect(_ctx.Page.GetByTestId("GuidanceAppStepper")).ToBeVisibleAsync();
    }

    // ── When ──────────────────────────────────────────────────────────────

    [When("the user clicks Proceed on the Introduction screen")]
    public async Task WhenUserClicksProceedOnIntroduction()
    {
        var btn = _ctx.Page.GetByTestId("proceed-btn");
        await Expect(btn).ToBeVisibleAsync();
        await btn.ClickAsync();
        await Expect(_ctx.Page.Locator("app-verify-installation-prerequisites")).ToBeVisibleAsync();
    }

    // ── Then: stepper count ───────────────────────────────────────────────

    [Then("exactly {int} stepper items are rendered")]
    public async Task ThenExactlyNStepperItemsRendered(int count)
    {
        await Expect(_ctx.Page.Locator("sh-stepper-item")).ToHaveCountAsync(count);
    }

    [Then("exactly {int} stepper item is active")]
    public async Task ThenExactlyNStepperItemsActive(int count)
    {
        await Expect(_ctx.Page.Locator("sh-stepper-item[active]")).ToHaveCountAsync(count);
    }

    // ── Then: stepper item label ──────────────────────────────────────────

    [Then("stepper item {int} has label {string}")]
    public async Task ThenStepperItemHasLabel(int stepNumber, string expectedLabel)
    {
        var item = StepperItem(stepNumber);
        await Expect(item).ToBeVisibleAsync();
        var actual = await item.GetAttributeAsync("label");
        Assert.AreEqual(expectedLabel, actual,
            $"Step {stepNumber}: expected label '{expectedLabel}' but got '{actual}'");
    }

    [Then("stepper item {int} has i18n label {string}")]
    public async Task ThenStepperItemHasI18nLabel(int stepNumber, string i18nKey)
    {
        var expected = _ctx.Translations.Resolve(i18nKey);
        var item = StepperItem(stepNumber);
        await Expect(item).ToBeVisibleAsync();
        var actual = await item.GetAttributeAsync("label");
        Assert.AreEqual(expected, actual,
            $"Step {stepNumber}: expected label i18n key '{i18nKey}' → '{expected}' but got '{actual}'");
    }

    // ── Then: stepper item active state ──────────────────────────────────

    [Then("stepper item {int} is active")]
    public async Task ThenStepperItemIsActive(int stepNumber)
    {
        await Expect(StepperItem(stepNumber)).ToHaveAttributeAsync("active", "");
    }

    [Then("stepper item {int} is not active")]
    public async Task ThenStepperItemIsNotActive(int stepNumber)
    {
        await Expect(StepperItem(stepNumber)).Not.ToHaveAttributeAsync("active", "");
    }

    // ── Then: stepper item type ───────────────────────────────────────────

    [Then("stepper item {int} has type {string}")]
    public async Task ThenStepperItemHasType(int stepNumber, string expectedType)
    {
        var item = StepperItem(stepNumber);
        await Expect(item).ToBeVisibleAsync();
        var actual = await item.GetAttributeAsync("type");
        Assert.AreEqual(expectedType, actual,
            $"Step {stepNumber}: expected type='{expectedType}' but got '{actual}'");
    }

    [Then("stepper item {int} has no type attribute")]
    public async Task ThenStepperItemHasNoType(int stepNumber)
    {
        var item = StepperItem(stepNumber);
        await Expect(item).ToBeVisibleAsync();
        var actual = await item.GetAttributeAsync("type");
        Assert.IsNull(actual,
            $"Step {stepNumber}: expected no [type] attribute but got '{actual}'");
    }
}
