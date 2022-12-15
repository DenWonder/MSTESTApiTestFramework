using System.Text.Json.Nodes;
using System.Text.Json;
using System.Text.Json.Nodes;
using TestProject1.Models;

namespace TestProject1.Helpers;

public class CartsHelper
{
    
    private ApiHelper _apiHelper = new ApiHelper();
    public JsonArray GetProductsInCartList(int productsListLength)
    {
        int countOfProducts = _apiHelper.GetCountOfProducts();

        var productsList = new JsonArray();
        int tempId = 0;
        for (int i = 0; i < productsListLength; i++)
        {
            if (productsListLength <= countOfProducts)
            {
                tempId = i + 1;
            }
            else
            {
                tempId = new Random().Next(1, countOfProducts - 1);
            }
            productsList.Add(new JsonObject
            {
                [$"{Variables.ProductId}"] = tempId,
                [$"{Variables.ProductQuantity}"] = new Random().Next(1, 10)
            });
        }
        return productsList;
    }

    public JsonArray Generate_ProductList_With_CustomParameters(object idValue, object quantityValue)
    {
        var productsList = new JsonArray();
        productsList.Add(new JsonObject
        {
            [$"{Variables.ProductId}"] = JsonSerializer.Serialize(idValue),
            [$"{Variables.ProductQuantity}"] = JsonSerializer.Serialize(quantityValue)
        });
    
        return productsList;
    }
    
    public bool IsCartCalculationCorrect(CartModel cart)
    {
        bool isCalculationCorrectFlag = true;
        int totalProductsCounter = 0;
        int totalQuantityCounter = 0;
        decimal totalPriceCounter = 0;
        decimal totalDiscountedCounter = 0;
        for (int i = 0; i < cart.Products.Length; i++)
        {
            if (((cart.Products[i].Quantity * cart.Products[i].Price) != cart.Products[i].Total) ||
                (Math.Round(cart.Products[i].Quantity * cart.Products[i].Price * (((100 - cart.Products[i].DiscountPercentage)/100))) != cart.Products[i].DiscountedPrice)
                )
            {
                isCalculationCorrectFlag = false;
            }

            totalProductsCounter++;
            totalQuantityCounter += cart.Products[i].Quantity;
            totalPriceCounter += cart.Products[i].Total;
            totalDiscountedCounter += cart.Products[i].DiscountedPrice;
        }

        if (cart.Total != totalPriceCounter ||
            cart.TotalProducts != totalProductsCounter ||
            cart.DiscountedTotal != totalDiscountedCounter ||
            cart.TotalQuantity != totalQuantityCounter)
        {
            isCalculationCorrectFlag = false;
        }
        
        return isCalculationCorrectFlag;
    }

}