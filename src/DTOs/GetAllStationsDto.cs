using perla_metro_Stations_Service.src.Models.Enums;    

namespace perla_metro_Stations_Service.src.DTOs
{
    public class GetAllStationsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // "Activa" o "Inactiva"
    }
}