using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace TechShop.Tests;

public class CartControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public CartControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task AddToCart_ValidProduct_ReturnsRedirectToProductDetails()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        // 1) Cria um HttpContext só para gerar o par cookie + token (sem parse do HTML)
        using var scope = _factory.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var antiforgery = scope.ServiceProvider.GetRequiredService<IAntiforgery>();

        var httpContext = new DefaultHttpContext
        {
            RequestServices = scope.ServiceProvider
        };
        // Alguns middlewares/antiforgery usam Scheme/Host ao definir cookies
        httpContext.Request.Scheme = "https";
        httpContext.Request.Host = new HostString("localhost");

        var tokenSet = antiforgery.GetAndStoreTokens(httpContext);
        Assert.NotNull(tokenSet.RequestToken);

        var cookieHeader = GetAntiforgeryCookieHeader(httpContext);
        Assert.False(string.IsNullOrEmpty(cookieHeader), "Antiforgery cookie should be present after GetAndStoreTokens.");

        const int productId = 1;
        const int quantity = 2;

        // 2) POST como um browser: mesmo cookie + campo __RequestVerificationToken.
        using var request = new HttpRequestMessage(HttpMethod.Post, "/Cart/AddToCart")
        {
            Content = new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>("productId", productId.ToString()),
                new KeyValuePair<string, string>("quantity", quantity.ToString()),
                new KeyValuePair<string, string>("__RequestVerificationToken", tokenSet.RequestToken)
            ])
        };
        request.Headers.Add("Cookie", cookieHeader);
        request.Content!.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

        using var postResponse = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Found, postResponse.StatusCode);
        var location = postResponse.Headers.Location?.ToString() ?? string.Empty;
        Assert.Contains("Product", location, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Details", location, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Pega o header Cookie (name=value) correspondente ao cookie de antiforgery emitido no fake HttpContext.
    /// </summary>
    private static string GetAntiforgeryCookieHeader(HttpContext context)
    {
        // GetAndStoreTokens grava em Response.Headers["Set-Cookie"] ou nas features de cookies.
        if (context.Response.Headers.TryGetValue("Set-Cookie", out var setCookies))
        {
            foreach (var line in setCookies)
            {
                if (string.IsNullOrEmpty(line))
                    continue;
                // Primeiro segmento é "Nome=valor"; o resto são atributos (; HttpOnly; Path=...)
                var nameValue = line.Split(';')[0].Trim();
                if (nameValue.StartsWith(".AspNetCore.Antiforgery.", StringComparison.OrdinalIgnoreCase))
                    return nameValue;
            }
        }

        return string.Empty;
    }
}
