namespace TestProject1;

public static class Variables
{
    /**
     * URLs
     */
    /* Base URL of API */
    public const string BaseUrl = "https://dummyjson.com/";

    /* */
    public const string AuthLoginUrl = "auth/login";

    /* */
    public const string UsersUrl = "users";

    /* */
    public const string ProductsUrl = "products";

    /* */
    public const string CartsUrl = "carts";

    /* */
    public const string CartsUserUrl = CartsUrl + "/user";

    /* */
    public const string CartsAddUrl = CartsUrl + "/add";

    /**
     * API Request parameter fields names
     */
    
    /* Auth/Login parameter names */
    /* Username parameter name */
    public const string Username = "username";
    /* Password parameter name */
    public const string Password = "password";
    /* Expire In Mins auth request value (token ttl) */
    public const string ExpiresIn = "expiresInMins";
    
    /* /Carts parameter fields names */
    /* User Id parameter name */
    public const string UserId = "userId";
    /* Products[] parameter name */
    public const string Products = "products";
    /* Products[].id parameter name */
    public const string ProductId = "id";
    /* Products[].quantity parameter name */
    public const string ProductQuantity = "quantity";
    /* Merge parameter name */
    public const string MergeCarts = "merge";

    
    /* Maximum acceptable time for awaiting of server response In milliseconds */
    public const int AcceptableServerResponseTime = 10000;
}