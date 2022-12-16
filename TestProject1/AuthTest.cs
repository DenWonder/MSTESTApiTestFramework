using System.Diagnostics;
using System.Text.Json.Nodes;
using TestProject1.Driver;
using TestProject1.Helpers;
using TestProject1.Models;

namespace TestProject1;

[TestClass]
public class AuthTest
{    
    
    private static AuthHelper _authHelper;
    private static TestContext _testContext;
    private static UserModel _realUser;
    private static ApiHelper _apiHelper;
    private static DeserializeHelper _deserializeHelper;

    [ClassInitialize]
    public static void ClassInit(TestContext testContext)
    {
        _testContext = testContext;
        _authHelper = new AuthHelper();
        _apiHelper = new ApiHelper();
        _deserializeHelper = new DeserializeHelper();
        _realUser = _authHelper.GetExistedUser();
    }

    /**
     * GIVEN: Valid user & correct credentials data;
     * WHEN: Not Authenticated user send post auth request with prepared data; 
     * THEN: User become authenticated, response contain token, response code 200, response time is acceptable
     */
    [TestMethod]
    public async Task Authentication_With_CorrectCredentials_ReturnsUserModel_With_AuthToken_Test()
    {
        //Arrange
        var requestData = new JsonObject
        {
            [$"{Variables.Username}"] = _realUser.Username,
            [$"{Variables.Password}"] = _realUser.Password
        };
        var requestTimer = new Stopwatch();
        
        //Act
        requestTimer.Start();
        var response = await _apiHelper.Make_Post_Request($"{Variables.AuthLoginUrl}", requestData);
        requestTimer.Stop();
        var responseStatus = response.Status;
        var responseData = await response.JsonAsync();
        var deserializedResponseData = _deserializeHelper.UserModelDeserializer(responseData);

        //Assert
        /* Verify the correct of response status */
        Assert.AreEqual(200, responseStatus,
            "Response status != 200");
        /* Verify response is valid User object */
        Assert.AreEqual(typeof(UserModel), deserializedResponseData.GetType(),  
            "Response is not valid");
        /* Verify that verification response contains token */
        Assert.IsTrue(deserializedResponseData.Token.Length > 0, 
            "Response does not contain token");
        /* Verification that the user is logged in as the user with whose data he used.*/
        Assert.IsTrue(deserializedResponseData.Id == _realUser.Id, 
            "Authorization as another user");
        /* Check that the server response time is acceptable from the UX point of view. */
        Assert.IsTrue(requestTimer.ElapsedMilliseconds <= Variables.AcceptableServerResponseTime, 
            "Response time is too long");
    }

    /**
     * GIVEN: Valid & Correct credentials data, invalid or incorrect expiresIn data value;
     * WHEN: Not Authenticated user send post auth request with prepared data; 
     * THEN: User still unauthenticated, response contain error message, response code 500
     */
    [DataTestMethod]
    [DataRow(0)] // 0 minutes TTL for token should return error;
    [DataRow(-1)] // Negative value of TTL for token should return error;
    [DataRow(1441)] // Value more, than 24 hours for token is risky. Should return error;
    // Value which is too long for integer value. Should return error;
    [DataRow("-100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000")] 
    [DataRow("string value")] // TTL cannot be a string value. Should return error;
    public async Task Authentication_With_ValidCredentials_And_InvalidExpires_Values_Returns_ErrorMessage_Test(object expiresInMinsValue)
    {
        //Arrange 
        var requestData = new JsonObject
        {
            [$"{Variables.Username}"] = _realUser.Username,
            [$"{Variables.Password}"] = _realUser.Password,
            [$"{Variables.ExpiresIn}"] = expiresInMinsValue.ToString()
        };

        //Act
        var response = await _apiHelper.Make_Post_Request($"{Variables.AuthLoginUrl}", requestData);
        var responseStatus = response.Status;
        var responseData = await response.JsonAsync();
        var deserializedResponseData = _deserializeHelper.ResponseInfoMessageDeserializer(responseData);
        
        //Assert
        /* Verify the correct of response status */
        Assert.AreEqual(500, responseStatus, 
            "Response status != 500");
        /* Verify response is valid User object */
        Assert.IsTrue(deserializedResponseData.GetType() == typeof(ResponseInfoMessageSchema), 
            "Response is not valid");
    }

    /**
     * GIVEN: Valid & Not Correct credentials data;
     * WHEN: Not Authenticated user send post auth request with prepared data; 
     * THEN: User still unauthenticated, response contain error message with "Invalid credentials" str, response code 400
     */
    [DataTestMethod]
    [DataRow("************", "wrong_password")]
    [DataRow("", "wrong_password")]
    [DataRow("***********", "")]
    [DataRow("___", "******")]
    public async Task Authentication_With_InvalidCredentials_ReturnsErrorMessage_Test(string username, string password)
    {
        //Arrange
        var requestData = new JsonObject
        {
            [$"{Variables.Username}"] = username,
            [$"{Variables.Password}"] = password
        };
        
        //Act
        var response = await _apiHelper.Make_Post_Request($"{Variables.AuthLoginUrl}", requestData);
        var responseStatus = response.Status;
        var responseData = await response.JsonAsync();
        var deserializedResponseData = _deserializeHelper.ResponseInfoMessageDeserializer(responseData);
        
        //Assert
        Assert.AreEqual(400, responseStatus);
        Assert.IsTrue(deserializedResponseData.Message.Contains("Invalid credentials"));
    }

    /**
     * GIVEN: Credentials data without password;
     * WHEN: Not Authenticated user send post auth request with prepared data; 
     * THEN: User still unauthenticated, response contain error message, response code 400
     */
    [TestMethod]
    public async Task Authentication_Without_PasswordCredential_ReturnsErrorMessage_Test()
    {
        //Arrange
        var requestData = new JsonObject
        {
            [$"{Variables.Username}"] = _realUser.Username,
            [$"{Variables.Password}"] = ""
        };
        
        //Act
        var response = await _apiHelper.Make_Post_Request($"{Variables.AuthLoginUrl}", requestData);
        var responseStatus = response.Status;
        var responseData = await response.JsonAsync();
        var deserializedResponseData = _deserializeHelper.ResponseInfoMessageDeserializer(responseData);
        
        //Assert
        Assert.AreEqual(400, responseStatus);
        Assert.IsTrue(deserializedResponseData.Message is { Length: > 0 });
    }

    /**
     * GIVEN: Credentials data without password, incorrect fields names of request body;
     * WHEN: Not Authenticated user send post auth request with prepared data; 
     * THEN: User still unauthenticated, response contain error message, response code 400
     */
    [DataTestMethod]
    [DataRow("user_name", "password")]
    [DataRow("Username", "Password")]
    [DataRow("LOGIN", "KEY")]
    public async Task Authentication_With_IncorrectFieldsOfRequest_ReturnsErrorMessage_Test(string usernameFieldName, string passwordFieldName)
    {
        //Arrange
        var requestData = new JsonObject
        {
            [usernameFieldName] = _realUser.Username,
            [passwordFieldName] = _realUser.Password
        };

        //Act
        var response = await _apiHelper.Make_Post_Request($"{Variables.AuthLoginUrl}", requestData);
        var responseStatus = response.Status;
        var responseData = await response.JsonAsync();
        var deserializedResponseData = _deserializeHelper.ResponseInfoMessageDeserializer(responseData);
        
        //Assert
        Assert.AreEqual(400, responseStatus);
        Assert.IsTrue(deserializedResponseData.Message is { Length: > 0 });
    }
    
    /**
     * GIVEN: Correct && Valid user credentials data
     * WHEN: Not Authenticated user send GET auth request with prepared data; 
     * THEN: User still unauthenticated, response contain error message, response code 403
     */
    [TestMethod]
    public async Task Authentication_With_IncorrectRequestMethod_ReturnsErrorMessage_Test()
    {
        //Arrange
        var requestData = new JsonObject
        {
            [$"{Variables.Username}"] = _realUser.Username,
            [$"{Variables.Password}"] = _realUser.Password
        };
      
        //Act
        var response = await _apiHelper.Make_Put_Request($"{Variables.AuthLoginUrl}", requestData);
        var responseStatus = response.Status;
        var responseData = await response.JsonAsync();
        var deserializedResponseData = _deserializeHelper.ResponseInfoMessageDeserializer(responseData);
        
        //Assert
        /* Verify the correct of response status */
        Assert.AreEqual(403, responseStatus, 
            "Response status != 403");
        /* Verify response is valid User object */
        Assert.IsTrue(deserializedResponseData.GetType() == typeof(ResponseInfoMessageSchema), 
            "Response is not valid");
        Assert.IsTrue(deserializedResponseData.Message is { Length: > 0 });
    }


    [ClassCleanup]
    public static void ClassCleanup()
    {
    }
}