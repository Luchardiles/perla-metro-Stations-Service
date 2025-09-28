using System.ComponentModel.DataAnnotations;
using perla_metro_Stations_Service.src.Models.Enums;

namespace perla_metro_Stations_Service.src.Models
{
    public class Station
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Location { get; set; } = string.Empty;

        [Required]
        public StationType Type { get; set; }

        public bool IsActive { get; set; } = true;

        // Campos de auditoría
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}