using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyModel;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// add DbContext Configuration
//This configuration register our context with Dependency Injection Container and configures to use Sqlite with file library.db
builder.Services.AddDbContext<LibraryContext>(options =>
    options.UseSqlite("Data Source=library.db"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.Run();

