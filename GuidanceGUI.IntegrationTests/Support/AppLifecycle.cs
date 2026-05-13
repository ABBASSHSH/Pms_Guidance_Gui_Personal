using Microsoft.Playwright;

namespace GuidanceHost.IntegrationTests.Support;

/// <summary>
/// Shared CDP lifecycle for all scenarios.
///
/// [BeforeScenario] — connects to the running WPF host via CDP,
///                    navigates to https://app.local/index.html,
///                    waits for i18n translations to resolve, then
///                    auto-detects the active language (English/German)
///                    and loads the matching i18n file into PageContext.Translations
///                    so every step definition can resolve keys at runtime.
/// [AfterScenario]  — disconnects CDP (does NOT close the WPF host).
///
/// PageContext is injected by Reqnroll DI and shared with every
/// [Binding] class in the same scenario.
///
/// Pre-requisite: GuidanceHost.exe running with --remote-debugging-port=9222
///   $env:WEBVIEW2_ADDITIONAL_BROWSER_ARGUMENTS="--remote-debugging-port=9222"
///   dotnet run --project GuidanceHost\GuidanceHost.csproj
///
/// Language is auto-detected — tests run correctly in both English and German.
///
/// Success-path tests (default):  launch host without SSIT_PREREQ_STATUS (defaults to "OK").
/// Error-path tests:               launch host with $env:SSIT_PREREQ_STATUS="Not Ok".
/// </summary>
[Binding]
public sealed class AppLifecycle
{
    private const string CdpEndpoint = "http://127.0.0.1:9222/";
    private const string AppUrl      = "https://app.local/index.html";

    private readonly StepDefinitions.PageContext _ctx;

    public AppLifecycle(StepDefinitions.PageContext ctx) => _ctx = ctx;

    // ── CDP connect / disconnect ─────────────────────────────────────────

    [BeforeScenario(Order = 0)]
    public async Task Connect()
    {
        _ctx.Playwright = await Playwright.CreateAsync();
        _ctx.Playwright.Selectors.SetTestIdAttribute("ta-id");

        _ctx.Browser = await _ctx.Playwright.Chromium.ConnectOverCDPAsync(CdpEndpoint);

        var context = _ctx.Browser.Contexts.FirstOrDefault()
            ?? throw new InvalidOperationException(
                $"No browser context found at {CdpEndpoint}. " +
                "Is GuidanceHost.exe running with --remote-debugging-port=9222?");

        _ctx.Page = context.Pages.FirstOrDefault()
            ?? throw new InvalidOperationException("No open page found in WebView2 context.");

        await _ctx.Page.GotoAsync(AppUrl);
        await _ctx.Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Detect active language by checking a German-only string in the rendered body.
        // Playwright auto-waits for NetworkIdle above, so translations are resolved by here.
        var bodyText = await _ctx.Page.Locator("body").InnerTextAsync();
        var detectedLanguage = bodyText.Contains("Einführung") ? "German" : "English";

        // Load the matching i18n file so step definitions can resolve keys at runtime.
        // This makes all text assertions language-independent — the same feature files
        // run correctly in both English and German without any hardcoded string changes.
        _ctx.Translations.Load(detectedLanguage);
    }

    [AfterScenario]
    public async Task Disconnect()
    {
        if (_ctx.Browser is not null)
            await _ctx.Browser.CloseAsync();
        _ctx.Playwright?.Dispose();
    }
}
