using Microsoft.EntityFrameworkCore;
using perla_metro_Stations_Service.src.Data;
using perla_metro_Stations_Service.src.Models;
using perla_metro_Stations_Service.src.DTOs;
using perla_metro_Stations_Service.src.Models.Enums;
/// <summary>   
/// Implementación del servicio de estaciones.
namespace perla_metro_Stations_Service.src.Services
{
    public class StationService : IStationService
    {
        private readonly MysqlDbContext _context;

        public StationService(MysqlDbContext context)
        {
            _context = context;
        }

        public async Task<StationResponseDto?> CreateStationAsync(CreateStationDto createStationDto)
        {
            // Verificar si ya existe una estación con el mismo nombre
            var existingStation = await _context.Stations
                .FirstOrDefaultAsync(s => s.Name.ToLower() == createStationDto.Name.ToLower());

            if (existingStation != null)
            {
                throw new InvalidOperationException("Ya existe una estación con este nombre");
            }

            var station = new Station
            {
                Id = Guid.NewGuid(), // UUID v4
                Name = createStationDto.Name.Trim(),
                Location = createStationDto.Location.Trim(),
                Type = createStationDto.Type,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Stations.Add(station);
            await _context.SaveChangesAsync();

            return MapToStationResponseDto(station);
        }

        public async Task<IEnumerable<GetAllStationsDto>> GetAllStationsAsync(string? nameFilter = null, bool? isActive = null, StationType? type = null)
        {
            var query = _context.Stations.AsQueryable();

            // Aplicar filtros
            if (!string.IsNullOrWhiteSpace(nameFilter))
            {
                query = query.Where(s => s.Name.ToLower().Contains(nameFilter.ToLower()));
            }

            if (isActive.HasValue)
            {
                query = query.Where(s => s.IsActive == isActive.Value);
            }

            if (type.HasValue)
            {
                query = query.Where(s => s.Type == type.Value);
            }

            var stations = await query
                .OrderBy(s => s.Name)
                .Select(s => new GetAllStationsDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Location = s.Location,
                    Type = GetStationTypeDescription(s.Type),
                    Status = s.IsActive ? "Activa" : "Inactiva"
                })
                .ToListAsync();

            return stations;
        }

        public async Task<StationResponseDto?> GetStationByIdAsync(Guid id)
        {
            var station = await _context.Stations
                .FirstOrDefaultAsync(s => s.Id == id);

            if (station == null)
            {
                return null;
            }

            return MapToStationResponseDto(station);
        }

        public async Task<StationResponseDto?> UpdateStationAsync(Guid id, UpdateStationDto updateStationDto)
        {
            var station = await _context.Stations
                .FirstOrDefaultAsync(s => s.Id == id);

            if (station == null)
            {
                return null;
            }

            // Verificar si se está actualizando el nombre y si ya existe
            if (!string.IsNullOrWhiteSpace(updateStationDto.Name) && 
                updateStationDto.Name.ToLower() != station.Name.ToLower())
            {
                var existingStation = await _context.Stations
                    .FirstOrDefaultAsync(s => s.Name.ToLower() == updateStationDto.Name.ToLower() && s.Id != id);

                if (existingStation != null)
                {
                    throw new InvalidOperationException("Ya existe otra estación con este nombre");
                }
            }

            // Actualizar campos si se proporcionan
            if (!string.IsNullOrWhiteSpace(updateStationDto.Name))
            {
                station.Name = updateStationDto.Name.Trim();
            }

            if (!string.IsNullOrWhiteSpace(updateStationDto.Location))
            {
                station.Location = updateStationDto.Location.Trim();
            }

            if (updateStationDto.Type.HasValue)
            {
                station.Type = updateStationDto.Type.Value;
            }

            if (updateStationDto.IsActive.HasValue)
            {
                station.IsActive = updateStationDto.IsActive.Value;
            }

            station.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapToStationResponseDto(station);
        }

        public async Task<bool> DeleteStationAsync(Guid id)
        {
            var station = await _context.Stations
                .IgnoreQueryFilters() // Ignora el filtro de soft delete
                .FirstOrDefaultAsync(s => s.Id == id);

            if (station == null)
            {
                return false;
            }
            
            if (station.IsDeleted)
            {
                return false; // Ya está eliminada
            }

            // Soft delete
            station.IsDeleted = true;
            station.DeletedAt = DateTime.UtcNow;
            station.IsActive = false;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> StationExistsAsync(Guid id)
        {
            return await _context.Stations
                .AnyAsync(s => s.Id == id);
        }

        public async Task<bool> StationNameExistsAsync(string name, Guid? excludeId = null)
        {
            var query = _context.Stations
                .Where(s => s.Name.ToLower() == name.ToLower());

            if (excludeId.HasValue)
            {
                query = query.Where(s => s.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        private static StationResponseDto MapToStationResponseDto(Station station)
        {
            return new StationResponseDto
            {
                Id = station.Id,
                Name = station.Name,
                Location = station.Location,
                Type = GetStationTypeDescription(station.Type),
                IsActive = station.IsActive,
                CreatedAt = station.CreatedAt,
                UpdatedAt = station.UpdatedAt
            };
        }

        private static string GetStationTypeDescription(StationType type)
        {
            return type switch
            {
                StationType.Origin => "Origen",
                StationType.Destination => "Destino",
                StationType.Intermediate => "Intermedia",
                _ => "Desconocido"
            };
        }
    }
}