namespace TestProject1.Models;

public record ResponseInfoMessageSchema(string Message);

public record ResponseGetAllSchema(int Total, int Skip, int Limit, UserModel[]? Users, CartModel[]? Carts, ProductModel[]? Products);

