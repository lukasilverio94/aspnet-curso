using System.Text.Json;
using Microsoft.AspNetCore.Http;
using TechShop.Controllers;
using TechShop.Models;

namespace TechShop.Services;

/// <summary>
/// Reads/writes the session cart using the same JSON settings as <see cref="CartController"/>.
/// </summary>
public static class CartSessionHelper
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public static Cart GetCart(ISession session)
    {
        var json = session.GetString(CartController.SessionKey);
        if (string.IsNullOrEmpty(json))
            return new Cart();

        try
        {
            return JsonSerializer.Deserialize<Cart>(json, JsonOptions) ?? new Cart();
        }
        catch (JsonException)
        {
            return new Cart();
        }
    }

    public static void SaveCart(ISession session, Cart cart)
    {
        var json = JsonSerializer.Serialize(cart, JsonOptions);
        session.SetString(CartController.SessionKey, json);
    }

    public static void ClearCart(ISession session)
    {
        session.Remove(CartController.SessionKey);
    }
}
