namespace TestProject1.Models;

public class ProductModel
{
    public ProductModel(int id, string? title, decimal price, int quantity, int total, decimal discountPercentage, decimal discountedPrice)
    {
        Id = id;
        Title = title;
        Price = price;
        Quantity = quantity;
        Total = total;
        DiscountPercentage = discountPercentage;
        DiscountedPrice = discountedPrice;
    }

    public int Id { get; set; }
    public string? Title { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public int Total { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal DiscountedPrice { get; set; }
}