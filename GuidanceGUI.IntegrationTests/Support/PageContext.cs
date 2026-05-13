using GuidanceHost.IntegrationTests.Support;
using Microsoft.Playwright;

namespace GuidanceHost.IntegrationTests.StepDefinitions;

/// <summary>
/// Holds the active Playwright page and shared services for the current scenario.
/// Injected by Reqnroll's built-in DI into every [Binding] class that needs it.
/// Lifecycle (Connect/Disconnect) is managed by AppLifecycle.
/// </summary>
public sealed class PageContext
{
    public IPage Page { get; set; } = null!;
    public IPlaywright? Playwright { get; set; }
    public IBrowser? Browser { get; set; }

    /// <summary>
    /// Loaded once per scenario by AppLifecycle after detecting the active language.
    /// Step definitions use this to resolve i18n keys to the language the app is
    /// actually rendering, making text assertions language-independent.
    /// </summary>
    public TranslationService Translations { get; } = new();
}
