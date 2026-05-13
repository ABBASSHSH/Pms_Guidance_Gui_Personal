using System.Text.Json;

namespace GuidanceHost.IntegrationTests.Support;

/// <summary>
/// Loads the i18n JSON files shipped with the Angular app and resolves
/// translation keys to the string the UI will actually render.
///
/// Usage (injected via Reqnroll DI):
///   var label = _translations.Resolve("common.proceed");   // "Proceed" or "Weiter"
///
/// The language is set once per scenario by AppLifecycle after it detects
/// which language the WPF host sent to Angular.
///
/// Supported languages: "English", "German"  (same identifiers that
/// GuidanceHost sends in the ShowSystemLanguage message).
/// </summary>
public sealed class TranslationService
{
    // Path to i18n folder relative to the test assembly output directory.
    // The Angular build copies assets to Angular_Output/browser/assets/i18n/.
    // The project copies that folder to the test bin via a post-build step;
    // fall back to the source tree when running from within VS / Rider.
    private static readonly string[] I18nSearchRoots =
    [
        // 1. Beside the test DLL (when Angular assets are copied to test output)
        Path.Combine(AppContext.BaseDirectory, "assets", "i18n"),
        // 2. Source tree — works when running dotnet test from the repo
        Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..",   // up to solution root
            "Upgrade", "src", "assets", "i18n")),
        Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..", "..",  // one extra level for nested bin dirs
            "Upgrade", "src", "assets", "i18n")),
    ];

    private static readonly Dictionary<string, string> LanguageToFile = new(StringComparer.OrdinalIgnoreCase)
    {
        ["English"] = "en.json",
        ["German"]  = "de.json",
    };

    private Dictionary<string, string> _translations = [];

    /// <summary>The language currently loaded. Null until <see cref="Load"/> is called.</summary>
    public string? Language { get; private set; }

    /// <summary>
    /// Loads the translation file for the given language.
    /// Called once per scenario by <see cref="AppLifecycle"/> after detecting
    /// the active language from the DOM.
    /// </summary>
    /// <param name="language">"English" or "German"</param>
    public void Load(string language)
    {
        if (!LanguageToFile.TryGetValue(language, out var fileName))
            throw new ArgumentException(
                $"Unsupported language '{language}'. Supported: {string.Join(", ", LanguageToFile.Keys)}");

        var path = FindI18nFile(fileName)
            ?? throw new FileNotFoundException(
                $"i18n file '{fileName}' not found. Searched:\n" +
                string.Join("\n", I18nSearchRoots.Select(r => Path.Combine(r, fileName))));

        var json = File.ReadAllText(path);
        _translations = JsonSerializer.Deserialize<Dictionary<string, string>>(json)
            ?? throw new InvalidDataException($"Failed to parse i18n file: {path}");

        Language = language;
    }

    /// <summary>
    /// Returns the translated string for <paramref name="key"/>.
    /// Throws <see cref="KeyNotFoundException"/> with a clear message if the key is absent.
    /// </summary>
    public string Resolve(string key)
    {
        if (Language is null)
            throw new InvalidOperationException(
                "TranslationService has not been loaded. " +
                "Ensure AppLifecycle.Connect() has run before any step that uses i18n keys.");

        if (_translations.TryGetValue(key, out var value))
            return value;

        throw new KeyNotFoundException(
            $"i18n key '{key}' not found in '{Language}' translations. " +
            "Check the key name against the JSON files in Upgrade/src/assets/i18n/.");
    }

    // ── private helpers ──────────────────────────────────────────────────

    private static string? FindI18nFile(string fileName)
    {
        foreach (var root in I18nSearchRoots)
        {
            var candidate = Path.Combine(root, fileName);
            if (File.Exists(candidate))
                return candidate;
        }
        return null;
    }
}
