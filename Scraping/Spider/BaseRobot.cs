using Models.Settings;
using Microsoft.Playwright;

namespace Scraping.Spider;

public class BaseRobot
{
    private readonly BrowserTypeLaunchOptions _browserOptions;
    public BaseRobot(BrowserOptions options)
    {
        _browserOptions = _parseBrowserOptions(options);
    }

    /// <summary>
    /// Run browser with page operation as callback function
    /// </summary>
    /// <param name="callback"></param>
    public async Task RunBrowser(Func<IPage, Task> callback)
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(_browserOptions);
        var page = await browser.NewPageAsync();
        await callback(page);
    }
    
    /// <summary>
    /// Create browser options
    /// </summary>
    /// <returns></returns>
    private BrowserTypeLaunchOptions _parseBrowserOptions(BrowserOptions options)
    {
        return new BrowserTypeLaunchOptions
        {
            Headless = options.Headless,
            SlowMo = options.SlowMo
        };
    }
}