using Microsoft.EntityFrameworkCore;
using perla_metro_Stations_Service.src.Models;
using perla_metro_Stations_Service.src.Models.Enums;
     
/// <summary>
/// Clase estática para sembrar datos iniciales en la base de datos.
namespace perla_metro_Stations_Service.src.Data.Seeders
{
    public static class StationSeeder
    {
        public static async Task SeedStationsAsync(MysqlDbContext context)
        {
            // Verificar si ya existen estaciones
            if (await context.Stations.AnyAsync())
            {
                return; // Ya hay datos, no necesitamos hacer seed
            }

            var stations = new List<Station>
            {
                new Station
                {
                    Id = Guid.NewGuid(),
                    Name = "Estación Central",
                    Location = "Av. Libertador Bernardo O'Higgins 3322, Santiago",
                    Type = StationType.Origin,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Station
                {
                    Id = Guid.NewGuid(),
                    Name = "Plaza de Armas",
                    Location = "Plaza de Armas s/n, Santiago Centro",
                    Type = StationType.Intermediate,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Station
                {
                    Id = Guid.NewGuid(),
                    Name = "Universidad de Chile",
                    Location = "Alameda 1058, Santiago",
                    Type = StationType.Intermediate,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Station
                {
                    Id = Guid.NewGuid(),
                    Name = "Santa Lucía",
                    Location = "Alameda con Santa Lucía, Santiago",
                    Type = StationType.Intermediate,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Station
                {
                    Id = Guid.NewGuid(),
                    Name = "Universidad Católica",
                    Location = "Alameda 390, Santiago",
                    Type = StationType.Intermediate,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Station
                {
                    Id = Guid.NewGuid(),
                    Name = "Baquedano",
                    Location = "Plaza Baquedano s/n, Providencia",
                    Type = StationType.Intermediate,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Station
                {
                    Id = Guid.NewGuid(),
                    Name = "Salvador",
                    Location = "Av. Providencia 1445, Providencia",
                    Type = StationType.Intermediate,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Station
                {
                    Id = Guid.NewGuid(),
                    Name = "Manuel Montt",
                    Location = "Av. Providencia 2330, Providencia",
                    Type = StationType.Intermediate,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Station
                {
                    Id = Guid.NewGuid(),
                    Name = "Pedro de Valdivia",
                    Location = "Av. Providencia 2834, Providencia",
                    Type = StationType.Intermediate,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Station
                {
                    Id = Guid.NewGuid(),
                    Name = "Los Leones",
                    Location = "Av. Providencia 3560, Las Condes",
                    Type = StationType.Destination,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                // Algunas estaciones inactivas para pruebas
                new Station
                {
                    Id = Guid.NewGuid(),
                    Name = "Estación Mantenimiento",
                    Location = "Zona Industrial, Santiago",
                    Type = StationType.Intermediate,
                    IsActive = false,
                    CreatedAt = DateTime.UtcNow
                }
            };

            context.Stations.AddRange(stations);
            await context.SaveChangesAsync();
        }
    }
}