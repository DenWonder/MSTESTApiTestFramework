using System.Diagnostics;
using System.Text.Json.Nodes;
using TestProject1.Driver;
using TestProject1.Helpers;
using TestProject1.Models;

namespace TestProject1;

[TestClass]
public class UpdateCartsTests
{    

    private static ApiHelper _apiHelper;

    private static AuthHelper _authHelper;

    private static DeserializeHelper _deserializeHelper;

    private static CartsHelper _cartsHelper;
    
    private static TestContext _testContext;

    private static UserModel _realUser;

    [ClassInitialize]
    public static void ClassInit(TestContext testContext)
    {
        _testContext = testContext;
        _apiHelper = new ApiHelper();
        _authHelper = new AuthHelper();
        _cartsHelper = new CartsHelper();
        _deserializeHelper = new DeserializeHelper();
        _realUser = _authHelper.GetExistedUser();
    }

    /**
     * GIVEN: Authenticated User; Valid && Correct existed Cart Data; Merge parameter is TRUE
     * WHEN: User send PUT request to update current cart with data;
     * THEN: Response has code 200, response contain updated data, as Cart object;
     * THEN: Response Cart is different from the same cart before user request to update it
     */
    [TestMethod]
    public async Task 
        Put_Update_Valid_Existed_CardId_With_MergeIsTrue_And_CorrectData_As_AuthenticatedUser_Returns_CartObject_Test()
    {
        //Arrange
       
        var cartId = new Random().Next(1, _apiHelper.GetMaxExistedCartId());
        var requestData = new JsonObject
        {
            [$"{Variables.MergeCarts}"] = true,
            [$"{Variables.Products}"] = _cartsHelper.GetProductsInCartList(2)
        };

        //Act
        var getCartDataBeforeUpdateResponse = await _apiHelper.Make_Authenticated_Get_Request($"{Variables.CartsUrl}/{cartId}");
        var oldResponseData = await getCartDataBeforeUpdateResponse.JsonAsync();
        var deserializedOldResponseData = _deserializeHelper.CartModelDeserializer(oldResponseData);

        var getCartDataAfterUpdate = await _apiHelper.Make_Authenticated_Put_Request($"{Variables.CartsUrl}/{cartId}", requestData);
        var responseData = await getCartDataAfterUpdate.JsonAsync();
        var deserializedResponseData = _deserializeHelper.CartModelDeserializer(responseData);
        var responseStatus = getCartDataAfterUpdate.Status;

        //Assert
        /* Verify response code */
        Assert.AreEqual(200,responseStatus,  "Response status code is incorrect != 200");
        /* Verify response is valid */
        Assert.AreEqual(typeof(CartModel),deserializedResponseData.GetType());
        /* Verify response is correct */
        Assert.IsTrue(deserializedResponseData.TotalQuantity != deserializedOldResponseData.TotalQuantity, 
            "Merge option doesnt work correctly");
    }

    /**
     * GIVEN: Authenticated User; Valid && Correct existed Cart Data; Merge parameter is TRUE
     * WHEN: User send PUT request to update current cart with data;
     * THEN: Response has code 200, response contain updated data, as Cart object;
     */
    [TestMethod]
    public async Task Put_Update_Existed_CardId_With_MergeIsTrue_And_CorrectData_CartCalculation_Test()
    {
        //Arrange
        var cartId = new Random().Next(1, _apiHelper.GetMaxExistedCartId());
        var requestData = new JsonObject
        {
            [$"{Variables.MergeCarts}"] = true,
            [$"{Variables.Products}"] = _cartsHelper.GetProductsInCartList(new Random().Next(1, 10))
        };

        //Act
        var getCartDataAfterUpdate = await _apiHelper.Make_Authenticated_Put_Request($"{Variables.CartsUrl}/{cartId}", requestData);
        var responseData = await getCartDataAfterUpdate.JsonAsync();
        var deserializedResponseData = _deserializeHelper.CartModelDeserializer(responseData);
        var responseStatus = getCartDataAfterUpdate.Status;

        //Assert
        
        /* Verify response code */
        Assert.AreEqual(200,responseStatus,  "Response status code is incorrect != 200");
        /* Verify response calculations is correct */
        Assert.IsTrue(_cartsHelper.IsCartCalculationCorrect(deserializedResponseData), 
            "Carts calculation doesnt work correctly");
    }
    
    /**
     * GIVEN: Authenticated User; Valid && Correct existed Cart Data; Merge parameter is FALSE
     * WHEN: User send PUT request to update current cart with data;
     * THEN: CALCULATION OF RESPONSE FIELDS ARE CORRECT;
     */
    [TestMethod]
    public async Task Put_Update_Existed_CardId_With_MergeIsFalse_And_CorrectData_CartCalculation_Test()
    {
        //Arrange
        var cartId = new Random().Next(1, _apiHelper.GetMaxExistedCartId());
        var requestData = new JsonObject
        {
            [$"{Variables.MergeCarts}"] = false,
            [$"{Variables.Products}"] = _cartsHelper.GetProductsInCartList(new Random().Next(1, 10))
        };

        //Act
        var getCartDataAfterUpdate = await _apiHelper.Make_Authenticated_Put_Request($"{Variables.CartsUrl}/{cartId}", requestData);
        var responseData = await getCartDataAfterUpdate.JsonAsync();
        var deserializedResponseData = _deserializeHelper.CartModelDeserializer(responseData);

        //Assert
        
        /* Verify, that backend makes calculation of cart correctly */
        Assert.IsTrue(_cartsHelper.IsCartCalculationCorrect(deserializedResponseData), "Carts calculation doesnt work correct");
    }
    
    /**
     * GIVEN: NotAuthenticated User; Valid && Correct existed Cart Data; Merge parameter is TRUE
     * WHEN: User send PUT request to update current cart with data;
     * THEN: Response has code 200, response contain updated data, as Cart object;
     * THEN: Request is executed in acceptable time
     */
    [TestMethod]
    public async Task 
        Put_Update_Valid_Existed_CardId_With_MergeIsTrue_And_CorrectData_As_NotAuthenticatedUser_Returns_CartObject_Test()
    {
        //Arrange
        
        var cartId = new Random().Next(1, _apiHelper.GetMaxExistedCartId());
        var requestData = new JsonObject
        {
            [$"{Variables.MergeCarts}"] = true,
            [$"{Variables.Products}"] = _cartsHelper.GetProductsInCartList(new Random().Next(1, 5))
        };
        var requestTimer = new Stopwatch();

        //Act
        var getCartDataBeforeUpdateResponse = await _apiHelper.Make_Get_Request($"{Variables.CartsUrl}/{cartId}");
        var oldResponseData = await getCartDataBeforeUpdateResponse.JsonAsync();
        var deserializedOldResponseData = _deserializeHelper.CartModelDeserializer(oldResponseData);
        
        requestTimer.Start();
        var getCartDataAfterUpdate = await _apiHelper.Make_Put_Request($"{Variables.CartsUrl}/{cartId}", requestData);
        requestTimer.Stop();
        
        var responseData = await getCartDataAfterUpdate.JsonAsync();
        var deserializedResponseData = _deserializeHelper.CartModelDeserializer(responseData);
        var responseStatus = getCartDataAfterUpdate.Status;
        
        //Assert
        /* Verify response code */
        Assert.AreEqual(200,responseStatus,  "Response status code is incorrect != 200");
        /* Verify response is valid */
        Assert.AreEqual(typeof(CartModel), deserializedResponseData.GetType());
        /* Verify response is correct */
        Assert.IsTrue(deserializedResponseData.TotalQuantity != deserializedOldResponseData.TotalQuantity, 
            "Merge option doesnt work correctly");
    }
    
    /**
     * GIVEN: Authenticated User; Valid && Correct existed Cart Data; Merge parameter is FALSE
     * WHEN: User send PUT request to update current cart with data;
     * THEN: Response has code 200, response contain updated data, as Cart object;
     * THEN: Response contain ONLY NEW data, without merging with old
     */
    [DataTestMethod]
    [DataRow(1)]
    [DataRow(5)]
    [DataRow(10)]
    [DataRow(20)]
    public async Task 
        Put_Update_Valid_Existed_CardId_With_MergeIsFalse_And_CorrectData_As_AuthenticatedUser_Returns_CartObject_Test(
            int countOfProducts)
    {
          //Arrange
          var cartId = new Random().Next(1, _apiHelper.GetMaxExistedCartId());
          var requestData = new JsonObject
          {
              [$"{Variables.MergeCarts}"] = false,
              [$"{Variables.Products}"] = _cartsHelper.GetProductsInCartList(countOfProducts)
          };

        //Act
        var getCartDataAfterUpdate = await _apiHelper.Make_Authenticated_Put_Request($"{Variables.CartsUrl}/{cartId}", requestData);
        var responseData = await getCartDataAfterUpdate.JsonAsync();
        var deserializedResponseData = _deserializeHelper.CartModelDeserializer(responseData);
        var responseStatus = getCartDataAfterUpdate.Status;
        
        //Assert
        /* Verify response code */
        Assert.AreEqual(200,responseStatus,  "Response status code is incorrect != 200");
        /* Verify response is valid */
        Assert.AreEqual(typeof(CartModel),deserializedResponseData.GetType());
        /* Verify response is correct */
        Assert.IsTrue(deserializedResponseData.TotalProducts <= countOfProducts, 
            "Merge option doesnt work correctly");
    }

    /**
     * GIVEN: Authenticated User; Valid && Correct existed Cart Data without MERGE value
     * WHEN: User send PUT request to update current cart with data;
     * THEN: Response has code 200, response contain updated data, as Cart object;
     * THEN: Response contain ONLY NEW data, without merging with old
     */
    [TestMethod]
    public async Task
        Put_Update_Valid_Existed_CardId_Without_MergeValue_And_CorrectData_As_AuthenticatedUser_Returns_CartObject_Test()
    {
        //Arrange
        var cartId = new Random().Next(1, _apiHelper.GetMaxExistedCartId());
        int countOfProducts = new Random().Next(1, 10);
        var requestData = new JsonObject
        {
            [$"{Variables.Products}"] = _cartsHelper.GetProductsInCartList(countOfProducts)
        };
        
        //Act
        var getCartDataAfterUpdate = await _apiHelper.Make_Authenticated_Put_Request($"{Variables.CartsUrl}/{cartId}", requestData);
        var responseData = await getCartDataAfterUpdate.JsonAsync();
        var deserializedResponseData = _deserializeHelper.CartModelDeserializer(responseData);
        var responseStatus = getCartDataAfterUpdate.Status;
        
        //Assert
        
        /* Verify response code */
        Assert.AreEqual(200,responseStatus,  "Response status code is incorrect != 200");
        /* Verify response is valid */
        Assert.AreEqual(typeof(CartModel), deserializedResponseData.GetType());
        /* Verify response is correct */
        Assert.IsTrue(deserializedResponseData.TotalProducts <= countOfProducts, 
            "Merge option doesnt work correctly");
    }

    /**
     * GIVEN: Authenticated User; Valid && Correct data, not existed Cart ID
     * WHEN: User send PUT request to update current cart with data;
     * THEN: Response has code 404, response contain error message;
     */
    [TestMethod]
    public async Task
        Put_Update_Valid_NotExisted_CardId_With_MergeIsFalse_And_CorrectData_As_AuthenticatedUser_Returns_ErrorMessage_Test()
    {
        //Arrange
        
        var cartId = _apiHelper.GetMaxExistedCartId()+1;
        int countOfProducts = new Random().Next(1, 5);
        var requestData = new JsonObject
        {
            [$"{Variables.MergeCarts}"] = false,
            [$"{Variables.Products}"] = _cartsHelper.GetProductsInCartList(countOfProducts)
        };

        //Act
        
        var getCartDataAfterUpdate = await _apiHelper.Make_Authenticated_Put_Request($"{Variables.CartsUrl}/{cartId}", requestData);
        var responseData = await getCartDataAfterUpdate.JsonAsync();
        var deserializedResponseData = _deserializeHelper.ResponseInfoMessageDeserializer(responseData);
        var responseStatus = getCartDataAfterUpdate.Status;
        
        //Assert
        
        /* Verify response code */
        Assert.AreEqual(404, responseStatus, "Response status code is incorrect != 404");
        /* Verify response is valid */
        Assert.AreEqual(typeof(ResponseInfoMessageSchema),deserializedResponseData.GetType());
        /* Verify response contain message */
        Assert.IsTrue(!string.IsNullOrEmpty(deserializedResponseData.Message), 
            "Response does not contain erro message text");
    }
    
    /**
     * GIVEN: Authenticated User; Valid && Correct data, not valid Cart ID value
     * WHEN: User send PUT request to update current cart with data;
     * THEN: Response has code 400, response contain error message;
     */
    [DataTestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    [DataRow(null)]
    [DataRow("string_value")]
    public async Task 
        Put_Update_NotValid_CardId_With_MergeIsFalse_And_CorrectData_As_AuthenticatedUser_Returns_ErrorMessage_Test(
            object cardIdValue)
    {
        //Arrange
        
        int countOfProducts = new Random().Next(1, 5);
        var requestData = new JsonObject
        {
            [$"{Variables.MergeCarts}"] = false,
            [$"{Variables.Products}"] = _cartsHelper.GetProductsInCartList(countOfProducts)
        };
        
        //Act
        
        var getCartDataAfterUpdate = await _apiHelper.Make_Authenticated_Put_Request($"{Variables.CartsUrl}/{cardIdValue}", requestData);
        var responseData = await getCartDataAfterUpdate.JsonAsync();
        var deserializedResponseData = _deserializeHelper.ResponseInfoMessageDeserializer(responseData);
        var responseStatus = getCartDataAfterUpdate.Status;

        //Assert
        
        /* Verify response code */
        Assert.AreEqual(400, responseStatus, "Response status code is incorrect != 400");
        /* Verify response is valid */
        Assert.AreEqual(typeof(ResponseInfoMessageSchema), deserializedResponseData.GetType());
        /* Verify response contain message */
        Assert.IsTrue(!string.IsNullOrEmpty(deserializedResponseData.Message), 
            "Response does not contain erro message text");
    }
    
    /**
     * GIVEN: Authenticated User; correct cart id, not existed product id value
     * WHEN: User send PUT request to update current cart with data;
     * THEN: Response has code 400, response contain error message;
     */
    [TestMethod]
    public async Task
        Put_Update_Valid_CardId_With_MergeIsFalse_And_NotExisted_ProductId_Value_As_AuthenticatedUser_Returns_ErrorMessage_Test()
    {
        //Arrange

        int cartId = new Random().Next(1, _apiHelper.GetMaxExistedCartId());
        var requestData = new JsonObject
        {
            [$"{Variables.MergeCarts}"] = false,
            [$"{Variables.Products}"] = _cartsHelper.Generate_ProductList_With_CustomParameters((_apiHelper.GetMaxExistedProductId()*2), 1)
        };

        //Act
        
        var getCartDataAfterUpdate = await _apiHelper.Make_Authenticated_Put_Request($"{Variables.CartsUrl}/{cartId}", requestData);
        var responseData = await getCartDataAfterUpdate.JsonAsync();
        var deserializedResponseData = _deserializeHelper.ResponseInfoMessageDeserializer(responseData);
        var responseStatus = getCartDataAfterUpdate.Status;
        
        //Assert
        /* Verify response code */
        Assert.AreEqual(400, responseStatus, "Response status code is incorrect != 400");
        /* Verify response is valid */
        Assert.AreEqual(typeof(ResponseInfoMessageSchema),deserializedResponseData.GetType());
        /* Verify response contain message */
        Assert.IsTrue(!string.IsNullOrEmpty(deserializedResponseData.Message), 
            "Response does not contain erro message text");
    }
    
    /**
     * GIVEN: Authenticated User; correct cart id, not valid product id value
     * WHEN: User send PUT request to update current cart with data;
     * THEN: Response has code 400, response contain error message;
     */
    [DataTestMethod]
    [DataRow(-1)]
    [DataRow(null)]
    [DataRow("string_value")]
    [DataRow("")]
    public async Task
        Put_Update_Valid_CardId_With_MergeIsFalse_And_NotValid_ProductId_Value_As_AuthenticatedUser_Returns_ErrorMessage_Test(
            object productIdValue)
    {
        //Arrange
        
        var cartId = new Random().Next(1, _apiHelper.GetMaxExistedCartId());
        var requestData = new JsonObject
        {
            [$"{Variables.MergeCarts}"] = false,
            [$"{Variables.Products}"] = _cartsHelper.Generate_ProductList_With_CustomParameters((productIdValue), 1)
        };

        //Act
        
        var getCartDataAfterUpdate = await _apiHelper.Make_Authenticated_Put_Request($"{Variables.CartsUrl}/{cartId}", requestData);
        var responseData = await getCartDataAfterUpdate.JsonAsync();
        var deserializedResponseData = _deserializeHelper.ResponseInfoMessageDeserializer(responseData);
        var responseStatus = getCartDataAfterUpdate.Status;
        
        //Assert
        /* Verify response code */
        Assert.AreEqual(400,responseStatus,  "Response status code is incorrect != 400");
        /* Verify response is valid */
        Assert.AreEqual(typeof(ResponseInfoMessageSchema), deserializedResponseData.GetType());
        /* Verify response contain message */
        Assert.IsTrue(!string.IsNullOrEmpty(deserializedResponseData.Message), 
            "Response does not contain erro message text");
    }

    /**
     * GIVEN: Authenticated User; correct cart id, without product id value
     * WHEN: User send PUT request to update current cart with data;
     * THEN: Response has code 400, response contain error message;
     */
    [TestMethod]
    public async Task
        Put_Update_Valid_CardId_With_MergeIsFalse_And_Without_ProductId_Value_As_AuthenticatedUser_Returns_ErrorMessage_Test()
    {
        //Arrange
        
        var cartId = new Random().Next(1, _apiHelper.GetMaxExistedCartId());
        var productsList = new JsonArray();
        productsList.Add(new JsonObject
        {
            [$"{Variables.ProductQuantity}"] = new Random().Next(1,5)
        });
        var requestData = new JsonObject
        {
            [$"{Variables.MergeCarts}"] = false,
            [$"{Variables.Products}"] = productsList
        };

        //Act
        
        var getCartDataAfterUpdate = await _apiHelper.Make_Authenticated_Put_Request($"{Variables.CartsUrl}/{cartId}", requestData);
        var responseData = await getCartDataAfterUpdate.JsonAsync();
        var deserializedResponseData = _deserializeHelper.ResponseInfoMessageDeserializer(responseData);
        var responseStatus = getCartDataAfterUpdate.Status;
        
        //Assert
     
        /* Verify response code */
        Assert.AreEqual(400, responseStatus, "Response status code is incorrect != 400");
        /* Verify response is valid */
        Assert.AreEqual(typeof(ResponseInfoMessageSchema), deserializedResponseData.GetType());
        /* Verify response contain message */
        Assert.IsTrue(!string.IsNullOrEmpty(deserializedResponseData.Message), 
            "Response does not contain erro message text");
    }

    /**
     * GIVEN: Authenticated User; correct cart id, with invalid product quantity value
     * WHEN: User send PUT request to update current cart with data;
     * THEN: Response has code 400, response contain error message;
     */
    [DataTestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("string_value")]
    public async Task
        Put_Update_Valid_CardId_With_MergeIsFalse_And_NotValid_Quantity_Value_As_AuthenticatedUser_Returns_ErrorMessage_Test(object quantityValue)
    {
        //Arrange
        
        var cartId = new Random().Next(1, _apiHelper.GetMaxExistedCartId());
        var requestData = new JsonObject
        {
            [$"{Variables.MergeCarts}"] = false,
            [$"{Variables.Products}"] = _cartsHelper.Generate_ProductList_With_CustomParameters((
                new Random().Next(1, _apiHelper.GetMaxExistedProductId())), quantityValue)
        };

        //Act
        
        var getCartDataAfterUpdate = await _apiHelper.Make_Authenticated_Put_Request($"{Variables.CartsUrl}/{cartId}", requestData);
        var responseData = await getCartDataAfterUpdate.JsonAsync();
        var deserializedResponseData = _deserializeHelper.ResponseInfoMessageDeserializer(responseData);
        var responseStatus = getCartDataAfterUpdate.Status;
        
        //Assert
        
        /* Verify response code */
        Assert.AreEqual(400,responseStatus,  "Response status code is incorrect != 400");
        /* Verify response is valid */
        Assert.AreEqual(typeof(ResponseInfoMessageSchema), deserializedResponseData.GetType());
        /* Verify response contain message */
        Assert.IsTrue(!string.IsNullOrEmpty(deserializedResponseData.Message), 
            "Response does not contain erro message text");
    }

    /**
     * GIVEN: Authenticated User; correct cart id, without product quantity value
     * WHEN: User send PUT request to update current cart with data;
     * THEN: Response has code 400, response contain error message;
     */
    [TestMethod]
    public async Task
        Put_Update_Valid_CardId_With_MergeIsFalse_And_Without_Quantity_Value_As_AuthenticatedUser_Returns_ErrorMessage_Test()
    {
        //Arrange
        
        var cartId = new Random().Next(1, _apiHelper.GetMaxExistedCartId());
        var productsList = new JsonArray();
        productsList.Add(new JsonObject
        {
            [$"{Variables.ProductId}"] = new Random().Next(1, _apiHelper.GetMaxExistedProductId())
        });
        var requestData = new JsonObject
        {
            [$"{Variables.MergeCarts}"] = false,
            [$"{Variables.Products}"] = productsList
        };

        //Act
        
        var getCartDataAfterUpdate = await _apiHelper.Make_Authenticated_Put_Request($"{Variables.CartsUrl}/{cartId}", requestData);
        var responseData = await getCartDataAfterUpdate.JsonAsync();
        var deserializedResponseData = _deserializeHelper.ResponseInfoMessageDeserializer(responseData);
        var responseStatus = getCartDataAfterUpdate.Status;
        
        //Assert
       
        /* Verify response code */
        Assert.AreEqual(400,responseStatus,  "Response status code is incorrect != 400");
        /* Verify response is valid */
        Assert.AreEqual(typeof(ResponseInfoMessageSchema), deserializedResponseData.GetType());
        /* Verify response contain message */
        Assert.IsTrue(!string.IsNullOrEmpty(deserializedResponseData.Message), 
            "Response does not contain erro message text");
    }

    /**
     * GIVEN: Authenticated User; correct cart id, without products value
     * WHEN: User send PUT request to update current cart with data;
     * THEN: Response has code 400, response contain error message;
     * THEN: Request execution time is acceptable
     */
    [TestMethod]
    public async Task
        Put_Update_Valid_CardId_With_MergeIsFalse_And_Without_Products_As_AuthenticatedUser_Returns_ErrorMessage_Test()
    {
        //Arrange
        
        var cartId = new Random().Next(1, _apiHelper.GetMaxExistedCartId());
        var requestData = new JsonObject
        {
            [$"{Variables.MergeCarts}"] = false,
        };
        var requestTimer = new Stopwatch();
        
        //Act
        
        requestTimer.Start();
        var getCartDataAfterUpdate = await _apiHelper.Make_Authenticated_Put_Request($"{Variables.CartsUrl}/{cartId}", requestData);
        requestTimer.Stop();
        
        Trace.WriteLine($"{requestTimer.ElapsedMilliseconds} ms");
        var responseData = await getCartDataAfterUpdate.JsonAsync();
        var deserializedResponseData = _deserializeHelper.ResponseInfoMessageDeserializer(responseData);
        var responseStatus = getCartDataAfterUpdate.Status;
        
        //Assert
        
        /* Verify response code */
        Assert.AreEqual(400, responseStatus, "Response status code is incorrect != 400");
        /* Verify response is valid */
        Assert.AreEqual(typeof(ResponseInfoMessageSchema),deserializedResponseData.GetType());
        /* Verify response contain message */
        Assert.IsTrue(!string.IsNullOrEmpty(deserializedResponseData.Message), 
            "Response does not contain erro message text");
        /* Verify response data is acceptable */
        Assert.IsTrue(requestTimer.ElapsedMilliseconds <= Variables.AcceptableServerResponseTime);
    }

    /**
     * GIVEN: Authenticated User; correct cart id, with invalid products field value
     * WHEN: User send PUT request to update current cart with data;
     * THEN: Response has code 400, response contain error message;
     * THEN: Request execution time is acceptable
     */
    [DataTestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow("string")]
    [DataRow("")]
    public async Task
        Put_Update_Valid_CardId_With_MergeIsFalse_And_Invalid_Products_As_AuthenticatedUser_Returns_ErrorMessage_Test(object productListValue)
    {
        //Arrange
        
        var cartId = new Random().Next(1, _apiHelper.GetMaxExistedCartId());
        var requestData = new JsonObject
        {
            [$"{Variables.MergeCarts}"] = true,
            [$"{Variables.Products}"]= productListValue.ToString()
        };
        var requestTimer = new Stopwatch();
        
        //Act
        
        requestTimer.Start();
        var getCartDataAfterUpdate = await _apiHelper.Make_Authenticated_Put_Request($"{Variables.CartsUrl}/{cartId}", requestData);
        requestTimer.Stop();
        
        var responseData = await getCartDataAfterUpdate.JsonAsync();
        var deserializedResponseData = _deserializeHelper.ResponseInfoMessageDeserializer(responseData);
        var responseStatus = getCartDataAfterUpdate.Status;
        
        //Assert
        
        /* Verify response code */
        Assert.AreEqual(400,responseStatus,  "Response status code is incorrect != 400");
        /* Verify response is valid */
        Assert.AreEqual(typeof(ResponseInfoMessageSchema), deserializedResponseData.GetType());
        /* Verify response contain message */
        Assert.IsTrue(!string.IsNullOrEmpty(deserializedResponseData.Message), 
            "Response does not contain error message text");
        /* Verify response data is acceptable */
        Assert.IsTrue(requestTimer.ElapsedMilliseconds <= Variables.AcceptableServerResponseTime);
    }
    
    [ClassCleanup]
    public static void ClassCleanup()
    {
    }
}