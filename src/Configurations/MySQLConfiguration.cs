using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using perla_metro_Stations_Service.src.Data;    
/// <summary>   
/// Configuración de la base de datos MySQL
/// </summary>
namespace perla_metro_Stations_Service.src.Configurations
{
    public static class MySQLConfiguration
    {
        public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            Env.Load();
            var connectionString = Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING");
            
            services.AddDbContext<MysqlDbContext>(options =>
            {
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), mySqlOptions =>
                {
                    mySqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                });
                
                // Solo en desarrollo
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    options.EnableSensitiveDataLogging();
                    options.EnableDetailedErrors();
                }
            });

            return services;
        }
    }
}