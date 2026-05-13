using Microsoft.Playwright;

namespace GuidanceHost.IntegrationTests.StepDefinitions;

/// <summary>
/// Generic reusable step definitions shared across all feature files.
/// These steps work on any element by ta-id and are used by all screen step files.
/// </summary>
[Binding]
public sealed class SharedSteps
{
    private readonly PageContext _ctx;

    public SharedSteps(PageContext ctx) => _ctx = ctx;

    // ── When ──────────────────────────────────────────────────────────────

    [When("the element {string} is clicked")]
    public async Task WhenElementIsClicked(string taId)
    {
        var el = _ctx.Page.GetByTestId(taId);
        await Expect(el).ToBeVisibleAsync();
        await el.ClickAsync();
    }

    // ── Then ──────────────────────────────────────────────────────────────

    [Then("the element {string} is visible")]
    public async Task ThenElementIsVisible(string taId)
    {
        await Expect(_ctx.Page.GetByTestId(taId)).ToBeVisibleAsync();
    }

    [Then("the element {string} contains text {string}")]
    public async Task ThenElementContainsText(string taId, string text)
    {
        await Expect(_ctx.Page.GetByTestId(taId)).ToContainTextAsync(text);
    }

    [Then("the element {string} has attribute {string} equal to {string}")]
    public async Task ThenElementHasAttribute(string taId, string attribute, string expected)
    {
        var el = _ctx.Page.GetByTestId(taId);
        await Expect(el).ToBeVisibleAsync();
        var actual = await el.GetAttributeAsync(attribute);
        Assert.AreEqual(expected, actual,
            $"[{taId}] expected attribute '{attribute}'='{expected}' but got '{actual}'");
    }

    [Then("the element {string} is not present")]
    public async Task ThenElementIsNotPresent(string taId)
    {
        await Expect(_ctx.Page.GetByTestId(taId)).ToHaveCountAsync(0);
    }

    [Then("the element {string} has no interactive button {string}")]
    public async Task ThenElementHasNoInteractiveButton(string screenTaId, string buttonTaId)
    {
        var screen = _ctx.Page.GetByTestId(screenTaId);
        await Expect(screen).ToBeVisibleAsync();
        await Expect(screen.GetByTestId(buttonTaId)).ToHaveCountAsync(0);
    }

    // ── i18n-aware steps ──────────────────────────────────────────────────

    [Then("the element {string} contains i18n text {string}")]
    public async Task ThenElementContainsI18nText(string taId, string i18nKey)
    {
        var expected = _ctx.Translations.Resolve(i18nKey);
        await Expect(_ctx.Page.GetByTestId(taId)).ToContainTextAsync(expected);
    }

    [Then("the element {string} has attribute {string} equal to i18n key {string}")]
    public async Task ThenElementHasAttributeI18nKey(string taId, string attribute, string i18nKey)
    {
        var expected = _ctx.Translations.Resolve(i18nKey);
        var el = _ctx.Page.GetByTestId(taId);
        await Expect(el).ToBeVisibleAsync();
        var actual = await el.GetAttributeAsync(attribute);
        Assert.AreEqual(expected, actual,
            $"[{taId}] attribute '{attribute}': expected i18n key '{i18nKey}'" +
            $" → '{expected}' but got '{actual}'");
    }
}
