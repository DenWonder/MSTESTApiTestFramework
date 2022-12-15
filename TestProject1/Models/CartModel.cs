namespace TestProject1.Models;

public class CartModel
{
    public CartModel(int id, int total, int discountedTotal, int userId, int totalProducts, int totalQuantity, ProductModel[]? products)
    {
        Id = id;
        Total = total;
        DiscountedTotal = discountedTotal;
        UserId = userId;
        TotalProducts = totalProducts;
        TotalQuantity = totalQuantity;
        Products = products;
    }

    public int Id { get; set; }
    public int Total { get; set; }
    public int DiscountedTotal { get; set; }
    public int UserId { get; set; }
    public int TotalProducts { get; set; }
    public int TotalQuantity { get; set; }
    public ProductModel[]? Products { get; set; }
    
    public bool? IsDeleted { get; set; }
}