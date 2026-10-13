var builder = WebApplication.CreateBuilder(args);

// 2. Register this new custom Service
// Podemos substituir qualquer MyService por algum outro Service que implementa a interface IMyService.
// Sendo mto mais fácil de manter - Singleton cria apenas 1 instância do objeto, 
// não importa quantas vezes é chamado
// 4. Mudar de Singleton para AddScoped -> Aqui é uma nova instanciação por Requests
// ou seja, o ServiceID vai ser diferente para cada requisição.
builder.Services.AddSingleton<IMyService, MyService>(); // Singleton
// builder.Services.AddScoped<IMyService, MyService>(); // Scoped
// builder.Services.AddTransient<IMyService, MyService>(); Transient
var app = builder.Build();

// middleware to use in other contexts
app.Use(async(context, next) =>
    {
    var myService = context.RequestServices.GetRequiredService<IMyService>();
    myService.LogCreation("First Middleware");
    await next.Invoke(); 
});

app.Use(async(context, next) =>
    {
    var myService = context.RequestServices.GetRequiredService<IMyService>();
    myService.LogCreation("Second Middleware");
    await next.Invoke(); 
});

// 3. Inject on the Route the new Service
app.MapGet("/", (IMyService myService) =>
{
    myService.LogCreation("Root");
    return Results.Ok("Check the console for service creation logs");
});

app.Run();


// 1. Creating a custom Interface / Service
public interface IMyService
{
    void LogCreation(string message);
}

public class MyService: IMyService
{
    private readonly int _serviceId;
    public MyService()
    {
        _serviceId = new Random().Next(100000, 999999);
    }

    public void LogCreation(string message)
    {
        Console.WriteLine($"{message} - Service ID: {_serviceId}");
    }
}