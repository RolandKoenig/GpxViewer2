using Avalonia;
using Avalonia.Controls;
using GpxViewer2.UiTestToolkit.Locators;

namespace GpxViewer2.UiTestToolkit.Actions;

public static class LocatorInteractions
{
    public static async Task<IEnumerable<Visual>> GetAllAsync(this ILocator locator, int expectedMinimumCount = 0)
    {
        return await InteractWithLocatorAsync(
            locator,
            checkConditionsMet: (visuals) => visuals.Length > expectedMinimumCount,
            generateResult: (visuals) => visuals,
            conditionsNeverMetExceptionFactory: () => new LocateElementException("Unable to get all Visuals. Locator returned no Visuals!"));
    }
    
    public static async Task<IEnumerable<string>> GetAllTextsAsync(this ILocator locator)
    {
        return await InteractWithLocatorAsync(
            locator,
            checkConditionsMet: (visuals) => visuals.Length > 0,
            generateResult: (visuals) =>
            {
                var resultTexts = new List<string>(visuals.Length);
                foreach (var actVisual in LocatorUtil.QueryChildVisualsDeepContainingSelf(visuals, LocatorOptions.Default))
                {
                    if ((actVisual is TextBlock { Text: not null } textBlock))
                    {
                        resultTexts.Add(textBlock.Text);
                    }
                    else if ((actVisual is TextBox { Text: not null } textBox))
                    {
                        resultTexts.Add(textBox.Text);
                    }
                }
                return resultTexts;
            },
            conditionsNeverMetExceptionFactory: () => new LocateElementException("Unable to get all texts. Locator returned no Visuals!"));
    }
    
    public static async Task ShouldExistAsync(this ILocator locator)
    {
        _ = await InteractWithLocatorAsync(
            locator,
            checkConditionsMet: (visuals) => visuals.Length > 0,
            generateResult: _ => true,
            conditionsNeverMetExceptionFactory: () => new LocateElementException("Locator returned no Visuals!"));
    }
    
    public static async Task ShouldBeVisibleAsync(this ILocator locator)
    {
        _ = await InteractWithLocatorAsync(
            locator,
            checkConditionsMet: (visuals) => (visuals.Length > 0) && visuals.All(x => x.IsVisible),
            generateResult: _ => true,
            conditionsNeverMetExceptionFactory: () => new LocateElementException("Locator returned no Visuals!"));
    }
    
    public static async Task ShouldHaveTextAsync(this ILocator locator, string text)
    {
        var texts = await locator.GetAllTextsAsync();
        if (!texts.Contains(text))
        {
            throw new LocateElementException($"Locator did not contain text '{text}'!");
        }
    }
    
    public static async Task<Visual> GetSingleAsync(this ILocator locator)
    {
        return await InteractWithLocatorAsync(
            locator,
            checkConditionsMet: (visuals) => visuals.Length > 0,
            generateResult: (visuals) =>
            {
                if (visuals.Length > 1)
                {
                    throw new LocateElementException($"Locator returned to many Visuals (Count: {visuals.Length})!");
                }
                
                return visuals[0];
            },
            conditionsNeverMetExceptionFactory: () => new LocateElementException("Locator returned no Visuals!"));
    }
    
    public static async Task ClickAsync(this ILocator locator)
    {
        _ = await InteractWithLocatorAsync(
            locator,
            checkConditionsMet: (visuals) => visuals.Length > 0,
            generateResult: (visuals) =>
            {
                if (visuals.Length > 1)
                {
                    throw new LocateElementException(
                        $"Unable to simulate click. Locator returned multiple Visuals (Count: {visuals.Length})!");
                }

                visuals[0].SimulateClick();
                return true;
            },
            conditionsNeverMetExceptionFactory: () => new LocateElementException("Unable to simulate click. Locator returned no Visuals!"));
    }
    
    private static async Task<TResult> InteractWithLocatorAsync<TResult>(
        this ILocator locator,
        Func<Visual[], bool> checkConditionsMet,
        Func<Visual[], TResult> generateResult,
        Func<Exception> conditionsNeverMetExceptionFactory)
    {
        var tryNumber = 0;
        while (tryNumber < 500)
        {
            tryNumber++;
            
            var visuals = locator.LocateAll().ToArray();
            if (!checkConditionsMet(visuals))
            {
                await Task.Delay(10);
                continue;
            }
            
            return generateResult(visuals);
        }

        throw conditionsNeverMetExceptionFactory();
    }
}