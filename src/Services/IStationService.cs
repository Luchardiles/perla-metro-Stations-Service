using perla_metro_Stations_Service.src.Models;
using perla_metro_Stations_Service.src.DTOs;
using perla_metro_Stations_Service.src.Data;
using perla_metro_Stations_Service.src.Models.Enums;

namespace perla_metro_Stations_Service.src.Services
{
    public interface IStationService
    {
        Task<StationResponseDto?> CreateStationAsync(CreateStationDto createStationDto);
        Task<IEnumerable<GetAllStationsDto>> GetAllStationsAsync(string? nameFilter = null, bool? isActive = null, StationType? type = null);
        Task<StationResponseDto?> GetStationByIdAsync(Guid id);
        Task<StationResponseDto?> UpdateStationAsync(Guid id, UpdateStationDto updateStationDto);
        Task<bool> DeleteStationAsync(Guid id);
        Task<bool> StationExistsAsync(Guid id);
        Task<bool> StationNameExistsAsync(string name, Guid? excludeId = null);
    }
}