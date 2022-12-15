using System.Text.Json;
using TestProject1.Models;

namespace TestProject1.Helpers;

public class DeserializeHelper
{
    public CartModel CartModelDeserializer(JsonElement? responseBodyData = null)
    {
        return responseBodyData?.Deserialize<CartModel>(new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }
    
    public ResponseInfoMessageSchema ResponseInfoMessageDeserializer(JsonElement? responseBodyData = null)
    {
        return responseBodyData?.Deserialize<ResponseInfoMessageSchema>(new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }
    
    public ProductModel ProductModelDeserializer(JsonElement? responseBodyData = null)
    {
        return responseBodyData?.Deserialize<ProductModel>(new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }
    
    public UserModel UserModelDeserializer(JsonElement? responseBodyData = null)
    {
        return responseBodyData?.Deserialize<UserModel>(new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }
    
    public ResponseGetAllSchema GetAllDeserializer(JsonElement? responseBodyData = null)
    {
        return responseBodyData?.Deserialize<ResponseGetAllSchema>(new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }
    
    
}