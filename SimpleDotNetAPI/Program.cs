using SimpleDotNetAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IUserService, UserService>();
//builder.Services.AddSingleton<IMyService, MyService>();  // Singleton
//builder.Services.AddScoped<IMyService, MyService>();    // Scoped
builder.Services.AddTransient<IMyService, MyService>();   // Transient
builder.Services.AddControllers();

var app = builder.Build();

// Middleware de logging
app.Use(async (context, next) =>
{
    await next();
    if (context.Response.StatusCode >= 400)
    {
        Console.WriteLine($"Security Event: {context.Request.Path} - Status Code: {context.Response.StatusCode}");
    }
});

//Custom middleware 1
app.Use(async (context, next) => {
    Console.WriteLine("Custom middleware 1");
    var startTime = DateTime.UtcNow;
    await next.Invoke();
    var duration = DateTime.UtcNow - startTime;
    Console.WriteLine($"Duration: {duration}");
});

//Custom middleware 2
app.Use(async (context, next) => {
    Console.WriteLine("Custom middleware 2");
    Console.WriteLine(context.Request.Path);
    await next.Invoke();
    Console.WriteLine(context.Response.StatusCode);
});

// Simulación de HTTPS
app.Use(async (context, next) =>
{
    if (context.Request.Query["secure"] != "true")
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsync("Simulated HTTPS Required");
        return;
    }
    await next();
});

//Custom middleware con condicional
//para pedir una password en la solicitud
app.UseWhen(
    context => context.Request.Method != "GET",
    appBuilder => appBuilder.Use(async (context, next) => {
        var extractedPassword = context.Request.Headers["X-Api-Key"];
        if (extractedPassword == "thisIsABadPassword") {
            await next.Invoke();
        } else {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Invalid API Key");
        }
    })
);

// Validación de entrada
app.Use(async (context, next) =>
{
    var input = context.Request.Query["input"];
    Console.WriteLine($"Input: {input}");

    // Permitir la solicitud si "input" no está presente o está vacío
    if (!string.IsNullOrEmpty(input) && !IsValidInput(input))
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsync("Invalid Input");
        return;
    }

    await next();
});

static bool IsValidInput(string? input)
{
    return !string.IsNullOrEmpty(input) && input.All(char.IsLetterOrDigit) && !input.Contains("<script>");
}

// Configure middleware for error handling
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Global exception caught: {ex.Message}");
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync("An unexpected error occurred. Please try again later.");
    }
});

// Autenticación simulada
app.Use(async (context, next) =>
{
    if (context.Request.Query["authenticated"] != "true")
    {
        context.Response.StatusCode = 403;
        await context.Response.WriteAsync("Access Denied");
        return;
    }

    context.Response.Cookies.Append("SecureCookie", "SecureData", new CookieOptions
    {
        HttpOnly = true,
        Secure = true
    });

    await next();
});

// Routing y Endpoints
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

// Configurar Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware to demonstrate lifecycle in multiple parts of the pipeline
app.Use(async (context, next) =>
{
    var myService = context.RequestServices.GetRequiredService<IMyService>();
    myService.LogCreation("First Middleware"); 
    await next();
});

app.Use(async (context, next) =>
{
    var myService = context.RequestServices.GetRequiredService<IMyService>();
    myService.LogCreation("Second Middleware"); 
    await next();
});

// Final endpoint to demonstrate service lifecycle in the request
app.MapGet("/service", (IMyService myService) =>
{
    myService.LogCreation("Root"); 
    return Results.Ok("Check the console for service creation logs.");
});

// Manejo de errores global
app.UseExceptionHandler("/Inicio/Error");

app.Run();
