
Dependency Injection, ou **Injeção de Dependência**, é um dos conceitos mais importantes do ASP.NET Core. Ele permite que uma classe receba os objetos de que precisa, em vez de criá-los diretamente.

Como você já tem experiência com Java e está aprendendo ASP.NET Core com .NET 10, vou usar exemplos práticos e fazer algumas comparações com Java.

## 1. O problema que o Dependency Injection resolve

Imagine que você está desenvolvendo seu projeto `PersonalPortfolio` e precisa criar uma página que exibe projetos do seu portfólio.

Você poderia criar um controller assim:
```cs
public class ProjectsController : Controller
{
    public IActionResult Index()
    {
        var service = new ProjectService();

        var projects = service.GetProjects();

        return View(projects);
    }
}
```

E o serviço:
```cs
public class ProjectService
{
    public List<string> GetProjects()
    {
        return new List<string>
        {
            "Personal Portfolio",
            "Task API",
            "Blog API"
        };
    }
}
```

Esse código funciona. Porém, existe um problema:
```cs
var service = new ProjectService();
```

O controller é responsável por criar sua própria dependência.

Isso gera alguns problemas:

- **Acoplamento:** o controller depende diretamente de como `ProjectService` é criado.
- **Testabilidade:** fica mais difícil substituir o serviço por um objeto falso durante os testes.
- **Flexibilidade:** se o serviço precisar de outras dependências, você terá que gerenciar sua criação manualmente.

Imagine que, futuramente, `ProjectService` precise de um repositório que acessa o banco de dados. A criação manual dos objetos começa a ficar mais complexa.

É aqui que entra o Dependency Injection.

---

## 2. Como funciona o Dependency Injection?

A ideia é simples:

**Em vez de a classe criar suas dependências, o ASP.NET Core fornece essas dependências para ela.**

Observe o mesmo controller usando DI:

```cs
public class ProjectsController : Controller
{
    private readonly ProjectService _projectService;

    public ProjectsController(ProjectService projectService)
    {
        _projectService = projectService;
    }

    public IActionResult Index()
    {
        var projects = _projectService.GetProjects();

        return View(projects);
    }
}
```

Observe o construtor:
```cs
public ProjectsController(ProjectService projectService)
{
    _projectService = projectService;
}
```

O controller declara que precisa de um `ProjectService`.

Mas ele não executa:
```cs
new ProjectService()
```

Quem fornece o serviço é o container de Dependency Injection do ASP.NET Core.

O fluxo é:

1. Uma requisição chega ao ASP.NET Core.
2. O framework identifica que precisa criar um `ProjectsController`.
3. Ele verifica quais dependências o construtor exige.
4. O container resolve e fornece um `ProjectService`.
5. O controller utiliza esse serviço.

Visualmente:

```
HTTP Request
     |
     v
ASP.NET Core
     |
     v
ProjectsController
     |
     | Precisa de ProjectService
     v
DI Container
     |
     | Cria ou recupera a instância
     v
ProjectService
```

O container é responsável por fornecer as dependências registradas e gerenciar seus tempos de vida.

### Mas como o ASP.NET Core sabe criar um ProjectService?

Você precisa registrá-lo no container.

Abra seu `Program.cs`.

Atualmente, você tem:
```cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var app = builder.Build();
```

Adicione o registro do serviço antes de `builder.Build()`:
```cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddTransient<ProjectService>();

var app = builder.Build();
```

A linha importante é:
```cs
builder.Services.AddTransient<ProjectService>();
```
Ela informa ao container que `ProjectService` pode ser criado e fornecido quando alguma classe precisar dele.

**Regra importante:** registre os serviços antes de chamar `builder.Build()`.
