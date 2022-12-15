using System.Diagnostics;
using System.Text.Json.Nodes;
using Microsoft.Playwright;
using TestProject1.Driver;

namespace TestProject1.Helpers;

public class ApiHelper
{

    private readonly ApiDriver _apiDriver = new ApiDriver();
    private static readonly AuthHelper _authHelper = new AuthHelper();

    private readonly int CountOfUsers = Initialize.CountOfUsers;
    private readonly int CountOfProducts = Initialize.CountOfProducts;
    private readonly int CountOfCarts = Initialize.CountOfCarts;
    
    private readonly int MaxExistedUserId = Initialize.MaxExistedUserId;
    private readonly int MaxExistedProductId = Initialize.MaxExistedProductId;
    private readonly int MaxExistedCartId = Initialize.MaxExistedCartId;

    public int GetCountOfUsers()
    {
        return CountOfUsers;
    }
    
    public int GetCountOfProducts()
    {
        return CountOfProducts;
    }

    public int GetCountOfCarts()
    {
        return CountOfCarts;
    }

    public int GetMaxExistedUserId()
    {
        return MaxExistedUserId;
    }

    public int GetMaxExistedProductId()
    {
        return MaxExistedProductId;
    }
    
    public int GetMaxExistedCartId()
    {
        return MaxExistedCartId;
    }

    public async Task<IAPIResponse> Make_Authenticated_Get_Request(string url)
    {
        return await Make_Get_Request(url, await _authHelper.GetAuthenticatedUserHeaders());
    }

    public async Task<IAPIResponse> Make_Authenticated_Post_Request(string url, JsonObject? requestBodyData = null)
    {
        return await Make_Post_Request(url, requestBodyData, await _authHelper.GetAuthenticatedUserHeaders());
    }
    
    public async Task<IAPIResponse> Make_Authenticated_Put_Request(string url, JsonObject? requestBodyData = null)
    {
        return await Make_Put_Request(url, requestBodyData, await _authHelper.GetAuthenticatedUserHeaders());
    }
    
    public async Task<IAPIResponse> Make_Authenticated_Delete_Request(string url, JsonObject? requestBodyData = null)
    {
        return await Make_Delete_Request(url, requestBodyData, await _authHelper.GetAuthenticatedUserHeaders());
    }

    public async Task<IAPIResponse> Make_Post_Request(
        string url, 
        JsonObject? requestBodyData = null,
        IEnumerable<KeyValuePair<string, string>>? requestHeadersData = null)
    {
        var requestTimer = new Stopwatch();
        requestTimer.Start();
        var response = await _apiDriver.ApiRequestContext?.PostAsync(url, new APIRequestContextOptions
        {
            Headers = requestHeadersData,
            DataObject = requestBodyData?.ToJsonString()
        })!;
        requestTimer.Stop();
        Trace.WriteLine($"Post Request to {url} was completed in {requestTimer.ElapsedMilliseconds} ms");
        response.Headers.Add("requestTimeInMilliseconds", requestTimer.ElapsedMilliseconds.ToString());
        return response;
    }

    public async Task<IAPIResponse> Make_Get_Request(
        string url,
        IEnumerable<KeyValuePair<string, string>>? requestHeadersData = null)
    {
        var requestTimer = new Stopwatch();
        requestTimer.Start();
        var response = await _apiDriver.ApiRequestContext?.GetAsync(url, new APIRequestContextOptions
        {
            Headers = requestHeadersData,
        })!;
        requestTimer.Stop();
        Trace.WriteLine($"Get Request to {url} was completed in {requestTimer.ElapsedMilliseconds} ms");
        return response;
    }

    public async Task<IAPIResponse> Make_Put_Request(
        string url, 
        JsonObject? requestBodyData = null,
        IEnumerable<KeyValuePair<string, string>>? requestHeadersData = null
        )
    {
        var requestTimer = new Stopwatch();
        requestTimer.Start();
        var response = await _apiDriver.ApiRequestContext?.PutAsync(url, new APIRequestContextOptions
        {
            Headers = requestHeadersData,
            DataObject = requestBodyData?.ToJsonString()
        })!;
        requestTimer.Stop();
        Trace.WriteLine($"Put Request to {url} was completed in {requestTimer.ElapsedMilliseconds} ms");
        return response;
    }
    
    public async Task<IAPIResponse> Make_Delete_Request(
        string url, 
        JsonObject? requestBodyData = null,
        IEnumerable<KeyValuePair<string, string>>? requestHeadersData = null
        )
    {
        var requestTimer = new Stopwatch();
        requestTimer.Start();
        var response = await _apiDriver.ApiRequestContext?.DeleteAsync(url, new APIRequestContextOptions
        {
            Headers = requestHeadersData,
            DataObject = requestBodyData?.ToJsonString()
        })!;
        requestTimer.Stop();
        Trace.WriteLine($"Delete Request to {url} was completed in {requestTimer.ElapsedMilliseconds} ms");
        return response;
    }
    
    
}