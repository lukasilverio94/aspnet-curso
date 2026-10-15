var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

var blogs = new List<Blog>
{
    new Blog { Title = "My First Post", Body = "This is my first post"},
    new Blog { Title = "My Second Post", Body = "This is my second post"}
};

// Tell how longs take to make a request
app.Use(async (context, next) =>
{
    var startTime = DateTime.UtcNow;
    await next.Invoke();
    var duration = DateTime.UtcNow - startTime;
    Console.WriteLine($"Duration: {duration}");
});

/*
    1. Adding middleware
*/
app.Use(async (context, next) =>
{
    Console.WriteLine(context.Request.Path); // runs before
    await next.Invoke();
    Console.WriteLine(context.Response.StatusCode);
});

// runs a middleware only for specific requests
// Not for every request — only when your condition matches.
app.UseWhen(
    context => context.Request.Method != "GET",
    appBuilder => appBuilder.Use(async (context, next) =>
    {
        var extractedPassword = context.Request.Headers["X-Api-Key"];
        if (extractedPassword == "thisIsABadPassword")
        {
            await next.Invoke();
        }
        else
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Invalid API Key");
        }
    })
);

app.MapGet("/", () => "I am root!");

app.MapGet("/blogs", () =>
{
    return Results.Ok(blogs);
});

app.MapGet("/blogs/{id}", (int id) =>
{
    if (id < 0 || id >= blogs.Count)
    {
        return Results.NotFound();
    }
    else
    {
        return Results.Ok(blogs[id]);
    }
});

app.MapPost("/blogs", (Blog blog) =>
{
    blogs.Add(blog);
    return Results.Created($"/blogs/{blogs.Count - 1}", blog);
});

app.Run();


public class Blog
{
    public required string Title { get; set; } = string.Empty;
    public required string Body { get; set; } = string.Empty;

}