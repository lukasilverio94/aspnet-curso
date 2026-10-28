using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Step 1: Adding HTTPS Redirection Service
/*
Port Configuration in Production
In production environments, HTTPS is often handled by reverse proxies like Nginx or IIS. The port configuration may differ from the development setting of 5001.
The reverse proxy typically handles SSL termination and forwards requests to your application.
*/
builder.Services.AddHttpsRedirection(options =>
{
    options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
    options.HttpsPort = 5001;
}
);

// Step 2: Adding HSTS Service Configuration (HTTP Strict Transport Security)
/*
HTTP Strict Transport Security (HSTS) is a web security policy mechanism that helps protect websites against protocol downgrade attacks and cookie hijacking.
Once a browser receives the HSTS header, it will automatically convert all HTTP requests to HTTPS for the specified duration, even if the user types "http://"
in the address bar.
  */
builder.Services.AddHsts(options =>
{
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(60);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // Step 3: Adding HSTS Middleware(Production Only)
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
