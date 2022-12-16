using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Playwright;
using TestProject1.Driver;
using TestProject1.Models;

namespace TestProject1.Helpers;

public class AuthHelper
{
    private readonly ApiHelper _apiHelper = new ApiHelper();

    private readonly DeserializeHelper _deserializeHelper = new DeserializeHelper();
    
    private readonly UserModel _realUser = Initialize.RealUser;
    
    /**
     * A method of making an authorization request and receiving a token.
     * Accepts string parameters for @{username} and @{password}.
     * Returns a string - token or error message.
     */
    public async Task<string?> LoginRequest(string username, string password)
    {
        
        var requestData = new JsonObject
        {
            [$"{Variables.Username}"] = username,
            [$"{Variables.Password}"] = password
        };
        
        var response = await _apiHelper.Make_Post_Request($"{Variables.AuthLoginUrl}", requestData);

        var responseData = await response.JsonAsync();
        var deserializedResponseData = _deserializeHelper.UserModelDeserializer(responseData);
        
        return deserializedResponseData.Token;
        
    }

    
    /**
     * A method for generating a request header requiring
     * authentication by token. Accepts a string - token.
     * Returns dictionary - request headers. Used directly
     * in the body of APIRequestContextOptions method,
     * after "Headers =" declaration 
     */
    public Dictionary<string, string> GetRequestHeadersWithToken(string? token = "")
    {
        return new Dictionary<string, string>
        {
            { "Authorization", $"Bearer {token}" },
            { "Content-Type", "application/json" }
        };
    }


    /**
     * A method that returns a user model object.
     */
    public UserModel GetExistedUser()
    {
        return _realUser;
    }
    
    public Dictionary<string, string> GetAuthenticatedUserHeaders()
    {
        var token = _realUser.Token;
        return GetRequestHeadersWithToken(token);
    }

}