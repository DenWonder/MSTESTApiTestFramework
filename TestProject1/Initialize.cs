using TestProject1.Helpers;
using TestProject1.Models;

namespace TestProject1;

[TestClass]
public class Initialize
{

    private static readonly ApiHelper _apiHelper = new();
    private static readonly DeserializeHelper _deserializeHelper = new();
    
    public static int CountOfUsers;
    public static int MaxExistedUserId;
    public static int CountOfProducts;
    public static int MaxExistedProductId;
    public static int CountOfCarts;
    public static int MaxExistedCartId;
    public static UserModel RealUser;
    
    [AssemblyInitialize]
    public static async Task AssemblyInitialize(TestContext context)
    {
        await Initialize_Default_Count_of_Users();
        await Initialize_Default_Count_of_Products();
        await Initialize_Default_Count_of_Carts();
        
        await Initialize_Maximum_User_ID_Value();
        await Initialize_Maximum_Product_ID_Value();
        await Initialize_Maximum_Cart_ID_Value();
        
        await Initialize_Real_User_for_Tests();
        Console.WriteLine("Assembly Initialize");
    }
    

    private static async Task Initialize_Default_Count_of_Users()
    {
        var getUsersCountResponse = await _apiHelper.Make_Get_Request($"{Variables.UsersUrl}");
        var getUsersCountRequestResult = await getUsersCountResponse.JsonAsync();
        var gerUsersResponseData = _deserializeHelper.GetAllDeserializer(getUsersCountRequestResult);
        CountOfUsers = (int)gerUsersResponseData.Total;
        Console.WriteLine($"Actual count of Active Users in system = {CountOfUsers}");
    }
    
    private static async Task Initialize_Maximum_User_ID_Value()
    {
        var getUsersCountResponse = await _apiHelper.Make_Get_Request($"{Variables.UsersUrl}?limit={Initialize.CountOfUsers}");
        var getUsersCountRequestResult = await getUsersCountResponse.JsonAsync();
        var getUsersResponseData = _deserializeHelper.GetAllDeserializer(getUsersCountRequestResult);
        MaxExistedUserId = getUsersResponseData.Users.Select(x => x.Id).ToArray().Max();
        Console.WriteLine($"Maximum Users ID value in system = {MaxExistedUserId}");
    }

    private static async Task Initialize_Default_Count_of_Products()
    {
        var getProductsCountResponse = await _apiHelper.Make_Get_Request($"{Variables.ProductsUrl}");
        var getProductsCountRequestResult = await getProductsCountResponse.JsonAsync();
        var getProductsResponseData = _deserializeHelper.GetAllDeserializer(getProductsCountRequestResult);
        CountOfProducts = (int)getProductsResponseData.Total;
        Console.WriteLine($"Actual count of Products in system = {CountOfProducts}");
    }
    
    private static async Task Initialize_Maximum_Product_ID_Value()
    {
        var getProductsCountResponse = await _apiHelper.Make_Get_Request($"{Variables.ProductsUrl}?limit={Initialize.CountOfProducts}");
        var getProductsCountRequestResult = await getProductsCountResponse.JsonAsync();
        var getProductsResponseData = _deserializeHelper.GetAllDeserializer(getProductsCountRequestResult);
        MaxExistedProductId = getProductsResponseData.Products.Select(x => x.Id).ToArray().Max();
        Console.WriteLine($"Maximum Product ID value in system = {MaxExistedProductId}");
    }
    
    private static async Task Initialize_Default_Count_of_Carts()
    {
        var getCartsCountResponse = await _apiHelper.Make_Get_Request($"{Variables.CartsUrl}");
        var getCartsCountRequestResult = await getCartsCountResponse.JsonAsync();
        var getCartsResponseData = _deserializeHelper.GetAllDeserializer(getCartsCountRequestResult);
        CountOfCarts = (int)getCartsResponseData.Total;
        Console.WriteLine($"Actual count of Carts in system = {CountOfCarts}");
    }
    
    private static async Task Initialize_Maximum_Cart_ID_Value()
    {
        var getCartsCountResponse = await _apiHelper.Make_Get_Request($"{Variables.CartsUrl}?limit={Initialize.CountOfCarts}");
        var getCartsCountRequestResult = await getCartsCountResponse.JsonAsync();
        var getCartsResponseData = _deserializeHelper.GetAllDeserializer(getCartsCountRequestResult);
        MaxExistedCartId = getCartsResponseData.Carts.Select(x => x.Id).ToArray().Max();
        Console.WriteLine($"Maximum Cart ID value in system = {MaxExistedCartId}");
    }

    private static async Task Initialize_Real_User_for_Tests()
    {
        int numberOfRealUser = new Random().Next(1, CountOfUsers - 1);
        var getUsersCountResponse = await _apiHelper.Make_Get_Request($"{Variables.UsersUrl}?limit={Initialize.CountOfUsers}");
        var getUsersCountRequestResult = await getUsersCountResponse.JsonAsync();
        var getUsersResponseData = _deserializeHelper.GetAllDeserializer(getUsersCountRequestResult);
        int existedUserId = getUsersResponseData.Users.Select(x => x.Id).ToArray()[numberOfRealUser - 1];
        
        var response = await _apiHelper.Make_Get_Request($"{Variables.UsersUrl}/{existedUserId}")!;
        var requestResult = await response.JsonAsync();
        var userData = _deserializeHelper.UserModelDeserializer(requestResult);
        RealUser = (UserModel)userData;
        Console.WriteLine($"All authenticated request will be send as  {RealUser.Username}");
    }
    
    
    [AssemblyCleanup]
    public static void AssemblyCleanup()
    {
        Console.WriteLine("AssemblyCleanup");
    }
    
}