using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using perla_metro_Stations_Service.src.Configurations;
using perla_metro_Stations_Service.src.Data.Seeders;
using perla_metro_Stations_Service.src.Middlewares;
using perla_metro_Stations_Service.src.Services;
using perla_metro_Stations_Service.src.Data; 
using System.Reflection;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);


Env.Load();

// Configuración de servicios
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        // Personalizar respuestas de validación
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(new
            {
                message = "Errores de validación",
                errors = errors,
                timestamp = DateTime.UtcNow
            });
        };
    });

// Configuración de base de datos
builder.Services.AddDatabaseConfiguration(builder.Configuration);

// Registrar servicios de aplicación
builder.Services.AddScoped<IStationService, StationService>();

// Configuración de Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = builder.Configuration["ApiSettings:Title"],
        Version = builder.Configuration["ApiSettings:Version"],
        Description = builder.Configuration["ApiSettings:Description"],
        Contact = new OpenApiContact
        {
            Name = "Perla Metro Development Team",
            Email = "dev@perlametro.cl"
        }
    });

    // Incluir comentarios XML
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Configurar esquemas para enums
    c.UseAllOfForInheritance();
    c.UseOneOfForPolymorphism();
});

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowedOrigins", policy =>
    {
        policy.WithOrigins(
            "http://localhost:3000",
            "http://localhost:5000",
            "http://localhost:5173",
            "https://perla-metro-frontend.vercel.app"
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

// Configuración de logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

if (builder.Environment.IsProduction() && OperatingSystem.IsWindows())
{
    builder.Logging.AddEventLog();
}

var app = builder.Build();

// Configuración del pipeline de middlewares
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Station Service v1");
        c.RoutePrefix = string.Empty; // Swagger en la raíz
    });
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Middleware personalizado para manejo de excepciones
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AllowedOrigins");

app.UseAuthorization();

app.MapControllers();

// Endpoint de información del servicio
app.MapGet("/info", () => new
{
    service = "Station Service",
    version = "1.0.0",
    environment = app.Environment.EnvironmentName,
    timestamp = DateTime.UtcNow,
    database = "MySQL"
});

// Migración automática y seeding en desarrollo
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<MysqlDbContext>();
        
        try
        {
            // Aplicar migraciones pendientes
            await context.Database.MigrateAsync();
            
            // Ejecutar seeder
            await StationSeeder.SeedStationsAsync(context);
            
            app.Logger.LogInformation("Base de datos inicializada correctamente");
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "Error al inicializar la base de datos");
        }
    }
}

app.Logger.LogInformation("Station Service iniciado en el entorno: {Environment}", app.Environment.EnvironmentName);

app.Run();