using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using TestProject1.Driver;
using TestProject1.Helpers;
using TestProject1.Models;

namespace TestProject1;

[TestClass]
public class AddCartTests
{

    private static AuthHelper _authHelper;
    private static TestContext _testContext;
    private static UserModel _realUser;
    private static ApiHelper _apiHelper;
    private static CartsHelper _cartsHelper;
    private static DeserializeHelper _deserializeHelper;
    
    [ClassInitialize]
    public static void ClassInit(TestContext testContext)
    {
        _testContext = testContext;
        _authHelper = new AuthHelper();
        _apiHelper = new ApiHelper();
        _deserializeHelper = new DeserializeHelper();
        _cartsHelper = new CartsHelper();
        _realUser = _authHelper.GetExistedUser();
    }
    
    /**
     * Given: Prepared data: Authenticated user; Valid && Correct data (few cases for different products count) for request;
     * When: Authenticated user send request with prepared data;
     * Then: Response is valid Cart Object; Response code 200;
     */
    [DataTestMethod]
    [DataRow(1)]
    [DataRow(5)]
    [DataRow(10)]
    [DataRow(50)]
    public async Task Post_Add_NewCard_With_CorrectData_As_AuthenticatedUser_Returns_CartObject_Test(
        int countOfProductsInCart)
    {
        //Arrange
        var requestData = new JsonObject
        {
            [$"{Variables.UserId}"] = _realUser.Id,
            [$"{Variables.Products}"] = _cartsHelper.GetProductsInCartList(countOfProductsInCart)
        };

        //Act
        var response = await _apiHelper.Make_Authenticated_Post_Request($"{Variables.CartsAddUrl}", requestData);
        var responseStatus = response.Status;
        var responseData = await response.JsonAsync();
        var deserializedResponseData = _deserializeHelper.CartModelDeserializer(responseData);
        
        //Assert
        /* Verify the correct of response status */
        Assert.AreEqual(responseStatus, 200, "Response status != 200");
        /* Verify response is valid cart object */
        Assert.IsTrue(deserializedResponseData.GetType() == typeof(CartModel), "Response is not valid");
        /* Verify response is correct */
        Assert.IsTrue(_cartsHelper.IsCartCalculationCorrect(deserializedResponseData),
            "Response is not correct");
    }

    /**
     * Given: Prepared data: Valid && Correct data for request;
     * When: Not Authenticated user send request with prepared data;
     * Then: Response is valid Cart Object; Response code 200;
     */
    [TestMethod]
    public async Task Post_Add_NewCard_With_CorrectData_As_NotAuthenticatedUser_Returns_CartObject_Test()
    {
        //Arrange
        var requestData = new JsonObject
        {
            [$"{Variables.UserId}"] = _realUser.Id,
            [$"{Variables.Products}"] = _cartsHelper.GetProductsInCartList(new Random().Next(1, 5))
        };
 
        //Act
        var response = await _apiHelper.Make_Post_Request($"{Variables.CartsAddUrl}", requestData);
        var responseStatus = response.Status;
        var responseData = await response.JsonAsync();
        var deserializedResponseData = _deserializeHelper.CartModelDeserializer(responseData);

        //Assert
        /* Verify response status is 200 (ok) */
        Assert.AreEqual(responseStatus, 200, "Response status != 200");
        /* Verify response is valid cart object */
        Assert.IsTrue(deserializedResponseData.GetType() == typeof(CartModel), "Response is not valid");

    }
    
    /**
     * Given: Prepared data: Valid && Correct data for request, include user id of another user;
     * When: Authenticated user send request with prepared data;
     * Then: Response is valid Cart Object with user id of another user; Response code 200;
     */
    [TestMethod]
    public async Task Post_Add_NewCard_With_CorrectData_As_AnotherAuthenticatedUser_Returns_CartObject_Test()
    {
        //Arrange
        var requestData = new JsonObject
        {
            [$"{Variables.UserId}"] = _realUser.Id+1,
            [$"{Variables.Products}"] = _cartsHelper.GetProductsInCartList(1)
        };

        //Act
        var response = await _apiHelper.Make_Authenticated_Post_Request($"{Variables.CartsAddUrl}", requestData);
        var responseStatus = response.Status;
        var responseData = await response.JsonAsync();
        var deserializedResponseData = _deserializeHelper.CartModelDeserializer(responseData);

        //Assert
        /* Verify response status is 200 (ok) */
        Assert.AreEqual(responseStatus, 200, "Response status != 200");
        /* Verify response is valid cart object */
        Assert.IsTrue(deserializedResponseData.GetType() == typeof(CartModel), "Response is not valid");
        /* Verify is cart updated for the right user */
        Assert.IsTrue(deserializedResponseData.UserId != _realUser.Id, 
            "Add cart for another user action added cart to current user");
    }

    /**
     * Given: Prepared data: Authenticated user, Valid && Correct data for request;
     * When: Authenticated user send request with prepared data;
     * Then: Response is valid Cart Object; Cart object fields value calculate is correct; Response code 200;
     */
    [DataTestMethod]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(5)]
    [DataRow(10)]
    [DataRow(50)]
    [DataRow(100)]
    public async Task Post_Add_NewCard_With_CorrectData_CartCalculation_IsCorrect_Test(int countOfProductsInCart)
    {
        //Arrange
        var requestData = new JsonObject
        {
            [$"{Variables.UserId}"] = _realUser.Id,
            [$"{Variables.Products}"] = _cartsHelper.GetProductsInCartList(countOfProductsInCart)
        };

        //Act
        var response = await _apiHelper.Make_Authenticated_Post_Request($"{Variables.CartsAddUrl}", requestData);
        var responseStatus = response.Status;
        var responseData = await response.JsonAsync();
        var deserializedResponseData = _deserializeHelper.CartModelDeserializer(responseData);

        //Assert
        /* Verify response status is 200 (ok) */
        Assert.AreEqual(responseStatus, 200, "Response status != 200");
        /* Verify response is valid cart object */
        Assert.IsTrue(deserializedResponseData.GetType() == typeof(CartModel), "Response is not valid");
        /* Verify response is correct */
        Assert.IsTrue(_cartsHelper.IsCartCalculationCorrect(deserializedResponseData),
            "Response is not correct");
    }
    
    /**
     * Given: Prepared data: Authenticated user, Valid data for request with not existed User ID Value;
     * When: Authenticated user send request with prepared data;
     * Then: Response is Error message; Response code 404;
     */
    [TestMethod]
    public async Task Post_Add_NewCard_With_NotExisted_UserIdValue_As_AuthenticatedUser_Returns_ErrorMessage_Test()
    {
        //Arrange
        var requestData = new JsonObject
        {
            [$"{Variables.UserId}"] = _apiHelper.GetMaxExistedUserId()*2,
            [$"{Variables.Products}"] = _cartsHelper.GetProductsInCartList(new Random().Next(1, 5))
        };
        var requestTimer = new Stopwatch();

        //Act
        requestTimer.Start();
        var response = await _apiHelper.Make_Authenticated_Post_Request($"{Variables.CartsAddUrl}", requestData);
        requestTimer.Stop();
        
        var responseStatus = response.Status;
        var responseData = await response.JsonAsync();
        var deserializedResponseData = _deserializeHelper.ResponseInfoMessageDeserializer(responseData);
        
        //Assert
        Assert.IsTrue(responseStatus == 404, 
            "Response code should be equal to 404, cause user from request doesnt exist");
        Assert.IsTrue(deserializedResponseData.Message.Length > 0, 
            "Response does not contain error message");
        Assert.IsTrue(requestTimer.ElapsedMilliseconds <= Variables.AcceptableServerResponseTime, 
            "Request execution time is too long");
    }
    
    /**
     * Given: Prepared data: Authenticated user, InValid data for request with not valid User ID Value;
     * When: Authenticated user send request with prepared data;
     * Then: Response is Error message; Response code 400; Response should be get with acceptable time;
     */
    [DataTestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    [DataRow("")]
    [DataRow("string_value")]
    [DataRow(null)]
    public async Task Post_Add_NewCard_With_Invalid_UserIdValue_As_AuthenticatedUser_Returns_ErrorMessage_Test(
        object userIdValue)
    {
        //Arrange
        var requestData = new JsonObject
        {
            [$"{Variables.UserId}"] = JsonSerializer.Serialize(userIdValue),
            [$"{Variables.Products}"] = _cartsHelper.GetProductsInCartList(1)
        };
        var requestTimer = new Stopwatch();


        //Act
        requestTimer.Start();
        var response = await _apiHelper.Make_Authenticated_Post_Request($"{Variables.CartsAddUrl}", requestData);
        requestTimer.Stop();

        var responseStatus = response.Status;
        var responseData = await response.JsonAsync();
        var deserializedResponseData = _deserializeHelper.ResponseInfoMessageDeserializer(responseData);
        
        //Assert
        Assert.IsTrue(responseStatus == 400, "Response status should be equal to 400");
        Assert.IsTrue(deserializedResponseData.GetType() == typeof(ResponseInfoMessageSchema), 
            "Response is not valid");
        Assert.IsTrue(deserializedResponseData.Message.Length > 0, 
            "Response does not contain error message");
        Assert.IsTrue(requestTimer.ElapsedMilliseconds <= Variables.AcceptableServerResponseTime, 
            "Request execution time is too long");
    }

    /**
     * Given: Prepared data: Authenticated user, InValid data for request without User ID Value;
     * When: Authenticated user send request with prepared data;
     * Then: Response is Error message; Response code 400;
     */
    [TestMethod]
    public async Task Post_Add_NewCard_Without_UserIdValue_As_AuthenticatedUser_Returns_ErrorMessage_Test()
    {
        //Arrange
        var requestData = new JsonObject
        {
            [$"{Variables.Products}"] = _cartsHelper.GetProductsInCartList(1)
        };

        //Act
        var response = await _apiHelper.Make_Authenticated_Post_Request($"{Variables.CartsAddUrl}", requestData);
        var responseStatus = response.Status;
        var responseData = await response.JsonAsync();
        var deserializedResponseData = _deserializeHelper.ResponseInfoMessageDeserializer(responseData);
    
        //Assert
        Assert.IsTrue(responseStatus == 400, "Response status should be equal to 400");
        Assert.IsTrue(deserializedResponseData.GetType() == typeof(ResponseInfoMessageSchema), 
            "Response is not valid");
        Assert.IsTrue(deserializedResponseData.Message.Length > 0, 
            "Response does not contain error message");
    }
    
    /**
     * Given: Prepared data: Authenticated user, InValid data for request with not existed Product ID Value;
     * When: Authenticated user send request with prepared data;
     * Then: Response is Error message; Response code 400 or 404;
     */
    [DataTestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    
    public async Task Post_Add_NewCard_With_NotExisted_ProductIdValues_As_AuthenticatedUser_Returns_ErrorMessage_Test(
        int productIdValue)
    {
        //Arrange
        var requestData = new JsonObject
        {
            [$"{Variables.UserId}"] = _realUser.Id,
            [$"{Variables.Products}"] = _cartsHelper.Generate_ProductList_With_CustomParameters(productIdValue, 1)
        };

        //Act
        var response = await _apiHelper.Make_Authenticated_Post_Request($"{Variables.CartsAddUrl}", requestData);
        var responseStatus = response.Status;
        var responseData = await response.JsonAsync();
        var deserializedResponseData = _deserializeHelper.ResponseInfoMessageDeserializer(responseData);
        
        //Assert
        Assert.IsTrue(responseStatus is 404 or 400, "responseStatus is not 404 or 400");
        Assert.IsTrue(deserializedResponseData.GetType() == typeof(ResponseInfoMessageSchema), 
            "Response is not valid");
        Assert.IsTrue(deserializedResponseData.Message.Length > 0, 
            "Response does not contain error message");
    }

    /**
     * Given: Prepared data: Authenticated user, InValid data for request with not valid Product ID Value;
     * When: Authenticated user send request with prepared data;
     * Then: Response is Error message; Response code 400;
     */
    [DataTestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    [DataRow("")]
    [DataRow("string_value")]
    [DataRow(null)]
    public async Task Post_Add_NewCard_With_Invalid_ProductIdValues_As_AuthenticatedUser_Returns_ErrorMessage_Test(
        object productIdValue)
    {
        //Arrange
        var requestData = new JsonObject
        {
            [$"{Variables.UserId}"] = _realUser.Id,
            [$"{Variables.Products}"] = _cartsHelper.Generate_ProductList_With_CustomParameters(productIdValue, 1)
        };

        //Act
        var response = await _apiHelper.Make_Authenticated_Post_Request($"{Variables.CartsAddUrl}", requestData);
        var responseStatus = response.Status;
        var responseData = await response.JsonAsync();
        var deserializedResponseData = _deserializeHelper.ResponseInfoMessageDeserializer(responseData);

        //Assert
        Assert.IsTrue(responseStatus is 400, "responseStatus is not 400");
        Assert.IsTrue(deserializedResponseData.GetType() == typeof(ResponseInfoMessageSchema), 
            "Response is not valid");
        Assert.IsTrue(deserializedResponseData.Message.Length > 0, 
            "Response does not contain error message");
    }

    /**
     * Given: Prepared data: Authenticated user, InValid data for request withoutProduct ID Value;
     * When: Authenticated user send request with prepared data;
     * Then: Response is Error message; Response code 400;
     */
    [TestMethod]
    public async Task Post_Add_NewCard_Without_ProductIdValue_As_AuthenticatedUser_Returns_ErrorMessage_Test()
    {
        //Arrange
        var products = new JsonArray();
        products.Add(new JsonObject {
            [$"{Variables.ProductQuantity}"] = 1
        });
        var requestData = new JsonObject
        {
            [$"{Variables.UserId}"] = _realUser.Id,
            [$"{Variables.Products}"] = products
        };
        
        //Act
        var response = await _apiHelper.Make_Authenticated_Post_Request($"{Variables.CartsAddUrl}", requestData);
        var responseStatus = response.Status;
        var responseData = await response.JsonAsync();
        var deserializedResponseData = _deserializeHelper.ResponseInfoMessageDeserializer(responseData);

        //Assert
        Assert.IsTrue(responseStatus is 400, "responseStatus is not 400");
        Assert.IsTrue(deserializedResponseData.GetType() == typeof(ResponseInfoMessageSchema), 
            "Response is not valid");
        Assert.IsTrue(deserializedResponseData.Message.Length > 0, 
            "Response does not contain error message");
    }

    /**
     * Given: Prepared data: Authenticated user, InValid data for request with invalid Product quantity Value;
     * When: Authenticated user send request with prepared data;
     * Then: Response is Error message; Response code 400;
     */
    [DataTestMethod]
    [DataRow(0.5)]
    [DataRow(0)]
    [DataRow(-1)]
    [DataRow("")]
    [DataRow("string_value")]
    [DataRow(null)]
    public async Task Post_Add_NewCard_With_NotValid_ProductQuantityValues_As_AuthenticatedUser_Returns_ErrorMessage_Test(
        object productQuantityValue)
    {
        //Arrange
        var productIdValue = new Random().Next(1, _apiHelper.GetMaxExistedProductId());
        var requestData = new JsonObject
        {
            [$"{Variables.UserId}"] = _realUser.Id,
            [$"{Variables.Products}"] = _cartsHelper.Generate_ProductList_With_CustomParameters(productIdValue, productQuantityValue)
        };

        //Act
        var response = await _apiHelper.Make_Authenticated_Post_Request($"{Variables.CartsAddUrl}", requestData);
        var responseStatus = response.Status;
        var responseData = await response.JsonAsync();
        var deserializedResponseData = _deserializeHelper.ResponseInfoMessageDeserializer(responseData);

        //Assert
        Assert.IsTrue(responseStatus is 400, "responseStatus is not 400");
        Assert.IsTrue(deserializedResponseData.GetType() == typeof(ResponseInfoMessageSchema), 
            "Response is not valid");
        Assert.IsTrue(deserializedResponseData.Message.Length > 0, 
            "Response does not contain error message");
    }

    /**
     * Given: Prepared data: Authenticated user, InValid data for request without Product quantity Value;
     * When: Authenticated user send request with prepared data;
     * Then: Response is Error message; Response code 400;
     */
    [TestMethod]
    public async Task Post_Add_NewCard_Without_ProductQuantityValues_As_AuthenticatedUser_Returns_ErrorMessage_Test()
    {
        //Arrange
        var products = new JsonArray();
        products.Add(new JsonObject {
            [$"{Variables.ProductId}"] = new Random().Next(1, _apiHelper.GetMaxExistedProductId())
        });
        var requestData = new JsonObject
        {
            [$"{Variables.UserId}"] = _realUser.Id,
            [$"{Variables.Products}"] = products
        };

        //Act
        var response = await _apiHelper.Make_Authenticated_Post_Request($"{Variables.CartsAddUrl}", requestData);
        var responseStatus = response.Status;
        var responseData = await response.JsonAsync();
        var deserializedResponseData = _deserializeHelper.ResponseInfoMessageDeserializer(responseData);

        //Assert
        Assert.IsTrue(responseStatus is 400, "responseStatus is not 400");
        Assert.IsTrue(deserializedResponseData.GetType() == typeof(ResponseInfoMessageSchema), 
            "Response is not valid");
        Assert.IsTrue(deserializedResponseData.Message.Length > 0, 
            "Response does not contain error message");
    }
    

    /**
     * Given: Prepared data: Authenticated user, InValid data with incorrect field names;
     * When: Authenticated user send request with prepared data;
     * Then: Response is Error message; Response code 400;
     */
    [DataTestMethod]
    [DynamicData(nameof(OBJ_Get_NotValid_FieldNames_Values_List), DynamicDataSourceType.Method)]
    public async Task Post_Add_NewCard_With_IncorrectFieldsNames_As_AuthenticatedUser_Returns_ErrorMessage_Test(
        string userIdFieldName, 
        string productsFieldName)
    {
        //Arrange
        var requestData = new JsonObject
        {
            [userIdFieldName] = _realUser.Id,
            [productsFieldName] = _cartsHelper.GetProductsInCartList(1)
        };

        //Act
        var response = await _apiHelper.Make_Authenticated_Post_Request($"{Variables.CartsAddUrl}", requestData);
        var responseStatus = response.Status;
        var responseData = await response.JsonAsync();
        var deserializedResponseData = _deserializeHelper.ResponseInfoMessageDeserializer(responseData);
        
        //Assert
        Assert.IsTrue(responseStatus is 400, "responseStatus is not 400");
        Assert.IsTrue(deserializedResponseData.GetType() == typeof(ResponseInfoMessageSchema), 
            "Response is not valid");
        Assert.IsTrue(deserializedResponseData.Message.Length > 0, 
            "Response does not contain error message");
    }
    
    public static IEnumerable<object[]> OBJ_Get_NotValid_FieldNames_Values_List()
    {
        yield return new object[] { "userId", "Products"};
        yield return new object[] { "userId", "products_ids" };
        yield return new object[] { "UserId", "Products" };
        yield return new object[] { "UserId", "products_ids" };
        yield return new object[] { "user_id", "products_ids" };
        yield return new object[] { "firstField", "secondField"};
        yield return new object[] { "", "" };
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
    }
}