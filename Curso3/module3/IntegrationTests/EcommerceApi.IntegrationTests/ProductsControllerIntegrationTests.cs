using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using WebApplicationFactoryDemo.Controllers;
using ECommerceAPI.IntegrationTests;
using WebApplicationFactoryDemo.Models;
using System.Net;

namespace EcommerceApi.IntegrationTests;

public class ProductsControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _httpClient;
    private readonly CustomWebApplicationFactory<Program> _factory;


    public ProductsControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _httpClient = factory.CreateClient();
    }

    // ─────────────────────────────────────────────────────────────
    // HAPPY PATH — baseline, so failures elsewhere are unambiguous
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetProducts_Returns200AndJsonArray()
    {
        var response = await _httpClient.GetAsync("/api/products");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);

        var products = await response.Content.ReadFromJsonAsync<List<Product>>();
        Assert.NotNull(products);
    }

    [Fact]
    public async Task CreateProduct_ValidPayload_Returns201WithLocation()
    {
        var payload = new Product { Name = "Monitor", Price = 199.90m, Stock = 3 };

        var response = await _httpClient.PostAsJsonAsync("/api/products", payload);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var created = await response.Content.ReadFromJsonAsync<Product>();
        Assert.NotNull(created);
        Assert.True(created!.Id > 0);
        Assert.Equal(payload.Name, created.Name);
    }

    // ─────────────────────────────────────────────────────────────
    // DOWNSTREAM DOMAIN EXCEPTION → mapped to a specific status code
    // (ProductNotFoundException → 404 problem+json)
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetProduct_UnknownId_Returns404ProblemDetails()
    {
        var response = await _httpClient.GetAsync("/api/products/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json",
            response.Content.Headers.ContentType?.MediaType);

        var problem = await response.Content
            .ReadFromJsonAsync<Microsoft.AspNetCore.Mvc.ProblemDetails>();
        Assert.NotNull(problem);
        Assert.Equal(404, problem!.Status);
        Assert.Contains("99999", problem.Detail);
    }

    // ─────────────────────────────────────────────────────────────
    // VALIDATION / ARGUMENT EXCEPTION → 400
    // Proves the middleware's catch chain discriminates by type
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateProduct_EmptyName_Returns400()
    {
        var payload = new Product { Name = "", Price = 10m, Stock = 1 };

        var response = await _httpClient.PostAsJsonAsync("/api/products", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ─────────────────────────────────────────────────────────────
    // UNHANDLED EXCEPTION → 500 with sanitized body
    // (the "boom" endpoint always throws InvalidOperationException)
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task Boom_Returns500WithGenericMessage()
    {
        var response = await _httpClient.GetAsync("/api/products/boom");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        // Sensitive internals must NOT leak to the client
        Assert.DoesNotContain("InvalidOperationException", body);
        Assert.DoesNotContain("Simulated downstream failure", body);
    }

    // ─────────────────────────────────────────────────────────────
    // EXCEPTION LOGGING — the middleware logged the exception
    // with the right level and carried the exception object
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetProduct_UnknownId_LogsWarningWithException()
    {
        _factory.LoggerProvider.Logs.Clear();

        await _httpClient.GetAsync("/api/products/424242");

        var captured = _factory.LoggerProvider.Logs
            .Where(l => l.Exception is ProductNotFoundException)
            .ToList();

        Assert.NotEmpty(captured);
        Assert.Contains(captured, l => l.Level == LogLevel.Warning);
    }

    [Fact]
    public async Task Boom_LogsErrorWithTheOriginalException()
    {
        _factory.LoggerProvider.Logs.Clear();

        await _httpClient.GetAsync("/api/products/boom");

        var captured = _factory.LoggerProvider.Logs
            .Where(l => l.Exception is InvalidOperationException)
            .ToList();

        Assert.NotEmpty(captured);
        Assert.Contains(captured, l => l.Level == LogLevel.Error);
    }

    // ─────────────────────────────────────────────────────────────
    // MIDDLEWARE ORDER — proves the exception middleware must wrap
    // the pipeline. We can't easily reorder in the same factory, so
    // we demonstrate the *positive* contract: the middleware's
    // problem+json shape reaches the client (not Kestrel's default
    // empty 500). This is the assertion that would fail if the
    // middleware were registered after MapControllers().
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task Boom_ResponseIsFormattedByOurMiddleware_NotKestrelDefault()
    {
        var response = await _httpClient.GetAsync("/api/products/boom");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal("application/problem+json",
            response.Content.Headers.ContentType?.MediaType);
    }

    // ─────────────────────────────────────────────────────────────
    // CANCELLATION — client disconnect must surface as an exception
    // in the middleware, and the middleware must not try to write
    // a response to an aborted request (HasStarted guard).
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task Request_CancelledByClient_DoesNotCrashHost()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // The client will throw OperationCanceledException; that's the
        // client-side view. The real assertion is that the host survives.
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            _httpClient.GetAsync("/api/products", cts.Token));

        // Host still responds afterwards
        var response = await _httpClient.GetAsync("/api/products");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
