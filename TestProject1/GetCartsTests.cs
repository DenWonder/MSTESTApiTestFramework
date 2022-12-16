using TestProject1.Driver;
using TestProject1.Helpers;
using TestProject1.Models;

namespace TestProject1;

[TestClass]
public class GetCartsTests
{

    private static ApiHelper _apiHelper;
    private static DeserializeHelper _deserializeHelper;
    private static AuthHelper _authHelper;
    private static TestContext _testContext;
    private static UserModel _realUser;

    [ClassInitialize]
    public static void ClassInit(TestContext testContext)
    {
        _testContext = testContext;
        _authHelper = new AuthHelper();
        _apiHelper = new ApiHelper();
        _deserializeHelper = new DeserializeHelper();
        _realUser = _authHelper.GetExistedUser();
    }
    
    /*
     * ======================================================================================================
     * Tests for Get all carts (dummyjson.com/carts) endpoint;
     * ======================================================================================================
     */
    
    /**
     * GIVEN: Authenticated user
     * WHEN: Send GET request to get All carts
     * THEN: Response code is 200, response contain all carts in the system;
     */
    [TestMethod]
    public async Task Get_AllCarts_As_AuthenticatedUser_Returns_CartsArray_Test()
    {
        //Arrange
        int cartsCount = _apiHelper.GetCountOfCarts();

        //Act
        var response = await _apiHelper.Make_Authenticated_Get_Request(Variables.CartsUrl);
        var responseStatus = response.Status;
        var responseData = await response.JsonAsync();
        var deserializedResponseData = _deserializeHelper.GetAllDeserializer(responseData);
        
        //Assert
        
        /* Verify response status is correct */
        Assert.IsTrue(responseStatus == 200, "Response status != 200");
        /* Verify response is valid */
        Assert.IsTrue(deserializedResponseData.GetType() == typeof(ResponseGetAllSchema));
        /* Verify response is correct */
        Assert.IsTrue(deserializedResponseData.Total == cartsCount);
    }

    /**
     * GIVEN: NotAuthenticated user
     * WHEN: Send GET request to get All carts
     * THEN: Response code is 200, response contain all carts in the system;
     */
    [TestMethod]
    public async Task Get_AllCarts_As_Not_AuthenticatedUser_ReturnsCartsArray_Test()
    {
        //Arrange
        
        int cartsCount = _apiHelper.GetCountOfCarts();
        
        //Act
        
        var response = await _apiHelper.Make_Get_Request(Variables.CartsUrl);
        var responseStatus = response.Status;
        var responseData = await response.JsonAsync();
        var deserializedResponseData = _deserializeHelper.GetAllDeserializer(responseData);
        
        //Assert
        
        /* Verify response status is correct */
        Assert.IsTrue(responseStatus == 200, "Response status != 200");
        /* Verify response is valid */
        Assert.IsTrue(deserializedResponseData.GetType() == typeof(ResponseGetAllSchema));
        /* Verify response is correct */
        Assert.IsTrue(deserializedResponseData.Total == cartsCount);
    }
    
    /**
     * GIVEN: Authenticated user, valid limit parameter
     * WHEN: Send GET request to get All carts
     * THEN: Response code is 200, response contain carts array;
     */
    [DataTestMethod]
    [DataRow(5)]
    [DataRow(0)]
    [DataRow(10)]
    [DataRow(30)]
    public async Task Get_AllCarts_With_ValidLimitParameter_As_AuthenticatedUser_ReturnsCartsArray_Test(
        object limitParameterValue)
    {
        //Arrange
        
        //Act

        var response = await _apiHelper.Make_Authenticated_Get_Request($"{Variables.CartsUrl}?limit={limitParameterValue}");
        var responseStatus = response.Status;
        var responseData = await response.JsonAsync();
        var deserializedResponseData = _deserializeHelper.GetAllDeserializer(responseData);

        //Assert
        
        /* Verify response status is correct */
        Assert.IsTrue(responseStatus == 200, "Response status != 200");
        /* Verify response is valid */
        Assert.IsTrue(deserializedResponseData.GetType() == typeof(ResponseGetAllSchema));
        /* Verify response is correct */
        Assert.IsTrue(deserializedResponseData.Carts.Length <= (int)limitParameterValue);
    }

    /**
     * GIVEN: Authenticated user, invalid limit parameter
     * WHEN: Send GET request to get All carts
     * THEN: Response code is 400, response contain error message;
     */
    [DataTestMethod]
    [DataRow("string_value")] // limit should be positive integer only
    public async Task Get_AllCarts_With_InvalidLimitParameter_As_AuthenticatedUser_Returns_ErrorMessage_Test(
        object limitParameterValue)
    {
        //Arrange

        //Act
        var response = await _apiHelper.Make_Authenticated_Get_Request($"{Variables.CartsUrl}?limit={limitParameterValue}");
        var responseStatus = response.Status;
        var responseData = await response.JsonAsync();
        var deserializedResponseData = _deserializeHelper.ResponseInfoMessageDeserializer(responseData);
        
        //Assert
        
        /* Verify response status is correct */
        Assert.IsTrue(responseStatus == 400, "Response status != 400");
        /* Verify response is valid */
        Assert.IsTrue(deserializedResponseData.GetType() == typeof(ResponseInfoMessageSchema));
    }
    
    /**
     * GIVEN: Authenticated user, invalid limit parameter
     * WHEN: Send GET request to get All carts
     * THEN: Response code is 500, response contain error message;
     */
    [DataTestMethod]
    [DataRow(-5)] // -5 is not correct value for LIMIT parameter
    public async Task Get_AllCarts_With_IncorrectLimitParameter_As_AuthenticatedUser_Returns_ErrorMessage_Test(
        object limitParameterValue)
    {
        //Arrange

        //Act
        var response = await _apiHelper.Make_Authenticated_Get_Request($"{Variables.CartsUrl}?limit={limitParameterValue}");
        var responseStatus = response.Status;
        var responseData = await response.JsonAsync();
        var deserializedResponseData = _deserializeHelper.ResponseInfoMessageDeserializer(responseData);
        
        //Assert
        
        /* Verify response status is correct */
        Assert.IsTrue(responseStatus == 500, "Response status != 500");
        /* Verify response is valid */
        Assert.IsTrue(deserializedResponseData.GetType() == typeof(ResponseInfoMessageSchema));
    }

    /*
     * =========================================================================================================
     * Tests for Get carts of a user (dummyjson.com/carts/user/*) endpoint;
     * =========================================================================================================
     */
    
    /**
     * GIVEN: Authenticated user with carts, valid & correct existed ID
     * WHEN: Send GET request to get carts
     * THEN: Response code is 200, response contain users carts;
     */
    [TestMethod]
    public async Task Get_AllCarts_Of_RealUser_As_Current_AuthenticatedUser_Returns_CartsArray_Test()
    {
        //Arrange

        //Act
        var response = await _apiHelper.Make_Authenticated_Get_Request($"{Variables.CartsUserUrl}/{_realUser.Id}");
        var responseStatus = response.Status;
        var responseData = await response.JsonAsync();
        var deserializedResponseData = _deserializeHelper.GetAllDeserializer(responseData);
        
        //Assert
        /* Verify response status is correct */
        Assert.IsTrue(responseStatus == 200, "Response status != 200");
        /* Verify response is valid */
        Assert.IsTrue(deserializedResponseData.GetType() == typeof(ResponseGetAllSchema));
    }
    
    /**
     * GIVEN: NotAuthenticated user with carts, valid & correct existed ID
     * WHEN: Send GET request to get carts
     * THEN: Response code is 200, response contain users carts;
     */
    [TestMethod]
    public async Task Get_AllCarts_Of_RealUser_As_NotAuthenticatedUser_Returns_CartsArray_Test()
    {
        //Arrange
        int userIdValue = new Random().Next(1, _apiHelper.GetMaxExistedUserId());
        
        //Act
        var response = await _apiHelper.Make_Get_Request($"{Variables.CartsUserUrl}/{userIdValue}");
        var responseStatus = response.Status;
        var responseData = await response.JsonAsync();
        var deserializedResponseData = _deserializeHelper.GetAllDeserializer(responseData);
        
        //Assert
        /* Verify response status is correct */
        Assert.IsTrue(responseStatus == 200, "Response status != 200");
        /* Verify response is valid */
        Assert.IsTrue(deserializedResponseData.GetType() == typeof(ResponseGetAllSchema));
    }
    
    /**
     * GIVEN: Authenticated user, UserId of another User with carts, valid & correct existed ID
     * WHEN: Send GET request to get carts
     * THEN: Response code is 200, response contain all user carts;
     */
    [TestMethod]
    public async Task Get_AllCarts_Of_RealUser_As_Another_AuthenticatedUser_Returns_CartsArray_Test()
    {
        //Arrange

        //Act
        var response = await _apiHelper.Make_Authenticated_Get_Request($"{Variables.CartsUserUrl}/{_realUser.Id+1}");
        var responseStatus = response.Status;
        var responseData = await response.JsonAsync();
        var deserializedResponseData = _deserializeHelper.GetAllDeserializer(responseData);

        
        //Assert
        /* Verify response status is correct */
        Assert.IsTrue(responseStatus == 200, "Response status != 200");
        /* Verify response is valid */
        Assert.IsTrue(deserializedResponseData.GetType() == typeof(ResponseGetAllSchema));
    }

    /**
     * GIVEN: Authenticated user, Invalid User Id value
     * WHEN: Send GET request to get carts
     * THEN: Response code is 400, response error message;
     */
    [DataTestMethod]
    [DataRow(-1)] // negative number is invalid value for ID
    [DataRow("string_value")] // string id format is not provided according to the documentation
    public async Task Get_AllCarts_With_InvalidUserId_As_AuthenticatedUser_Returns_ErrorMessage_Test(object userIdValue)
    {
        //Arrange

        //Act

        var response = await _apiHelper.Make_Authenticated_Get_Request($"{Variables.CartsUserUrl}/{userIdValue}");
        var responseStatus = response.Status;
        var responseData = await response.JsonAsync();
        var deserializedResponseData = _deserializeHelper.ResponseInfoMessageDeserializer(responseData);

        //Assert
        /* Verify response status is correct */
        Assert.IsTrue(responseStatus == 400, "Response status != 400");
        /* Verify response is valid */
        Assert.IsTrue(deserializedResponseData.GetType() == typeof(ResponseInfoMessageSchema));
        /* Verify response is correct */
        Assert.IsTrue(deserializedResponseData.Message.Length > 0);
    }

    /**
     * GIVEN: Authenticated user, unexisted User Id value
     * WHEN: Send GET request to get carts
     * THEN: Response code is 404, response error message;
     */
    [TestMethod]
    public async Task Get_AllCarts_With_NotExisted_UserId_As_AuthenticatedUser_Returns_ErrorMessage_Test()
    {
        //Arrange

        int userIdValue = _apiHelper.GetMaxExistedUserId() + 1;
        
        //Act

        var response = await _apiHelper.Make_Authenticated_Get_Request($"{Variables.CartsUserUrl}/{userIdValue}");
        var responseStatus = response.Status;
        var responseData = await response.JsonAsync();
        var deserializedResponseData = _deserializeHelper.ResponseInfoMessageDeserializer(responseData);

        //Assert
        /* Verify response status is correct */
        Assert.IsTrue(responseStatus == 404, "Response status != 404");
        /* Verify response is valid */
        Assert.IsTrue(deserializedResponseData.GetType() == typeof(ResponseInfoMessageSchema));
        /* Verify response is correct */
        Assert.IsTrue(deserializedResponseData.Message.Length > 0);
    }
    
    [ClassCleanup]
    public static void ClassCleanup()
    {
    }
    
}