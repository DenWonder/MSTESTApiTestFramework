using System.Diagnostics;
using TestProject1.Driver;
using TestProject1.Helpers;
using TestProject1.Models;

namespace TestProject1;

[TestClass]
public class DeleteCartsTests
{    
    private static ApiDriver _apiDriver;
    
    private static ApiHelper _apiHelper;

    private static DeserializeHelper _deserializeHelper;

    private static TestContext _testContext;
    
    [ClassInitialize]
    public static void ClassInit(TestContext testContext)
    {
        _testContext = testContext;
        _apiDriver = new ApiDriver();
        _apiHelper = new ApiHelper();
        _deserializeHelper = new DeserializeHelper();
    }
    

    /**
     * GIVEN: Authenticated user, Valid & Correct existed cart ID;
     * WHEN: User send DELETE request with prepared data;
     * THEN: Response has status 200, response contain Cart Model object with IsDeleted = true parameter
     */
    [TestMethod]
    public async Task Delete_Card_With_Valid_Existed_CardID_As_AuthenticatedUser_Returns_CartObject_Test()
    {
        //Arrange
        
        var cartToRemoveId = new Random().Next(1, _apiHelper.GetCountOfCarts());

        //Act
        
        var response = await _apiHelper.Make_Authenticated_Delete_Request($"{Variables.CartsUrl}/{cartToRemoveId}");
        var responseStatus = response.Status;
        var responseData = await response.JsonAsync();
        var deserializedResponseData = _deserializeHelper.CartModelDeserializer(responseData);
        
        //Assert
        
        /* Verify correct response status */
        Assert.AreEqual(responseStatus, 200, "Response status is != 200");
        /* Verify valid response format */
        Assert.IsTrue(deserializedResponseData.GetType() == typeof(CartModel), "Response is not valid");
        /* Verify correct operation result */
        Assert.IsTrue(deserializedResponseData.IsDeleted, "Cart did not removed");
        /* Verify cart id is the same */
        Assert.AreEqual(cartToRemoveId, deserializedResponseData.Id, "Wrong cart id removed");
    }

    /**
     * GIVEN: Not Authenticated user, Valid & Correct existed cart ID;
     * WHEN: User send DELETE request with prepared data;
     * THEN: Response has status 200, response contain Cart Model object with IsDeleted = true parameter
     */
    [TestMethod]
    public async Task Delete_Card_With_Valid_Existed_CardID_As_NotAuthenticatedUser_Returns_CartObject_Test()
    {
        //Arrange
        
        var cartToRemoveId = new Random().Next(1, _apiHelper.GetCountOfCarts());

        //Act
        
        var response = await _apiHelper.Make_Delete_Request($"{Variables.CartsUrl}/{cartToRemoveId}");
        var responseStatus = response.Status;
        var responseData = await response.JsonAsync();
        var deserializedResponseData = _deserializeHelper.CartModelDeserializer(responseData);
        
        //Assert
        
        /* Verify correct response status */
        Assert.AreEqual(responseStatus, 200, "Response status is != 200");
        /* Verify valid response format */
        Assert.IsTrue(deserializedResponseData.GetType() == typeof(CartModel), "Response is not valid");
        /* Verify correct operation result */
        Assert.IsTrue(deserializedResponseData.IsDeleted, "Cart did not removed");
        /* Verify cart id is the same */
        Assert.AreEqual(cartToRemoveId, deserializedResponseData.Id, "Wrong cart id removed");
    }

    /**
     * GIVEN: Authenticated user, Valid & Not Correct (Not existed) cart ID;
     * WHEN: User send DELETE request with prepared data;
     * THEN: Response has status 404, response contain error message, response time is acceptable;
     */
    [TestMethod]
    public async Task Delete_Card_With_Valid_NotExisted_CardID_As_AuthenticatedUser_Returns_ErrorMessage_Test()
    {
        //Arrange

        var cartToRemoveId = _apiHelper.GetMaxExistedCartId() + 1;
        var requestTimer = new Stopwatch();
        
        //Act
        requestTimer.Start();
        var response = await _apiHelper.Make_Delete_Request($"{Variables.CartsUrl}/{cartToRemoveId}");
        requestTimer.Stop();
        var responseStatus = response.Status;
        var responseData = await response.JsonAsync();
        var deserializedResponseData = _deserializeHelper.ResponseInfoMessageDeserializer(responseData);
        
        //Assert
        
        /* Verify correct response status */
        Assert.AreEqual(responseStatus, 404, "Response status is != 404");
        /* Verify valid response format */
        Assert.IsTrue(deserializedResponseData.GetType() == typeof(ResponseInfoMessageSchema), 
            "Response is not valid");
        /* Verify valid response format */
        Assert.IsTrue((deserializedResponseData.Message.Length > 0), "Response contain error message");
        /* Verify response time is acceptable */
        Assert.IsTrue(requestTimer.ElapsedMilliseconds == Variables.AcceptableServerResponseTime);
    }

    /**
     * GIVEN: Authenticated user, InValid  cart ID;
     * WHEN: User send DELETE request with prepared data;
     * THEN: Response has status 400, response contain error message;
     */
    [DataTestMethod]
    [DataRow(0)] // ID with value 0 is unsafety
    [DataRow(-1)] // ID with negative integer value is invalid
    [DataRow("string_value")] // string id is not provided according to the documentation
    [DataRow(null)] // null id is invalid
    public async Task Delete_Card_With_InValid_CardID_As_AuthenticatedUser_Returns_Errormessage_Test(object cartToRemoveId)
    {
        //Arrange

        //Act
        var response = await _apiHelper.Make_Delete_Request($"{Variables.CartsUrl}/{cartToRemoveId}");
        var responseStatus = response.Status;
        var responseData = await response.JsonAsync();
        var deserializedResponseData = _deserializeHelper.ResponseInfoMessageDeserializer(responseData);

        //Assert
        /* Verify correct response status */
        Assert.AreEqual(responseStatus, 400, "Response status is != 400");
        /* Verify valid response format */
        Assert.IsTrue(deserializedResponseData.GetType() == typeof(ResponseInfoMessageSchema), 
            "Response is not valid");
        /* Verify valid response format */
        Assert.IsTrue((deserializedResponseData.Message.Length > 0), "Response contain error message");
    }

    /**
     * GIVEN: Authenticated user;
     * WHEN: User send DELETE request;
     * THEN: Response has status 400, response contain error message;
     */
    [TestMethod]
    public async Task Delete_Card_Without_CardID_As_AuthenticatedUser_Returns_Errormessage_Test()
    {
        //Arrange

        //Act
        var response = await _apiHelper.Make_Delete_Request($"{Variables.CartsUrl}/");
        var responseStatus = response.Status;
        var responseData = await response.JsonAsync();
        var deserializedResponseData = _deserializeHelper.ResponseInfoMessageDeserializer(responseData);
        
        //Assert
        /* Verify correct response status */
        Assert.AreEqual(responseStatus, 400, "Response status is != 400");
        /* Verify valid response format */
        Assert.IsTrue(deserializedResponseData.GetType() == typeof(ResponseInfoMessageSchema), 
            "Response is not valid");
        /* Verify valid response format */
        Assert.IsTrue((deserializedResponseData.Message.Length > 0), "Response contain error message");
    }
    
    
    [ClassCleanup]
    public static void ClassCleanup()
    {
        _apiDriver.Dispose();
    }
}