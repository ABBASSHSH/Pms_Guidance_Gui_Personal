namespace GuidanceHost.IntegrationTests.StepDefinitions;

/// <summary>
/// Step definitions specific to the Introduction screen feature.
///
/// Coverage summary (Introduction.feature):
///   TC01        — introduction-screen is visible                  [SharedSteps]
///   TC02–TC05   — screen contains correct static text             [SharedSteps]
///   TC06–TC07   — proceed-btn / cancel-btn are visible            [SharedSteps]
///   TC08–TC09   — button color attributes (primary / secondary)   [SharedSteps]
///   TC11–TC12   — button label attributes (Proceed / Cancel)      [SharedSteps]
///   TC13        — info-bar hint label attribute                   [SharedSteps]
///   TC10        — Proceed navigates to verify-prerequisites-screen [SharedSteps]
///   Cancel      — sends CloseApp (untestable via CDP, Rule 2)
///
/// All Then/When steps are generic and live in SharedSteps.cs.
/// The Background Given step is defined here because it is specific
/// to the Introduction screen entry condition.
/// Lifecycle (CDP connect / navigate / disconnect) lives in AppLifecycle.cs.
/// </summary>
[Binding]
public sealed class IntroductionSteps
{
    private readonly PageContext _ctx;

    public IntroductionSteps(PageContext ctx) => _ctx = ctx;

    /// <summary>
    /// Background step: asserts the app has launched and the Introduction
    /// screen is the active view before each scenario runs.
    /// </summary>
    [Given("the app is running and the introduction screen is visible")]
    public async Task GivenIntroductionScreenIsVisible()
    {
        await Expect(_ctx.Page.GetByTestId("introduction-screen")).ToBeVisibleAsync();
    }
}
