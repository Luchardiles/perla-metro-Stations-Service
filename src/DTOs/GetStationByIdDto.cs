using perla_metro_Stations_Service.src.Models.Enums;

namespace perla_metro_Stations_Service.src.DTOs
{
    public class GetStationByIdDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
