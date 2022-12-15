using Microsoft.Playwright;

namespace TestProject1.Driver;

public class ApiDriver : IDisposable
{

    private readonly Task<IAPIRequestContext?>? _requestContext;

    public ApiDriver()
    {
        _requestContext = CreateApiContext();
    }

    public IAPIRequestContext? ApiRequestContext => _requestContext?.GetAwaiter().GetResult();

    public void Dispose()
    {
        _requestContext?.Dispose();
    }
    
    private async Task<IAPIRequestContext?> CreateApiContext()
    {
        var playwright = await Playwright.CreateAsync();

        return await playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions
        {
            BaseURL = Variables.BaseUrl,
            IgnoreHTTPSErrors = true
        });
    }
}