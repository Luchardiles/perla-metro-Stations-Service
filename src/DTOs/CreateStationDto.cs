using System.ComponentModel.DataAnnotations;
using perla_metro_Stations_Service.src.Models.Enums;

/// <summary>
/// DTO para crear una nueva estación.  

namespace perla_metro_Stations_Service.src.DTOs
{
    public class CreateStationDto
    {
        [Required(ErrorMessage = "El nombre de la estación es obligatorio")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "La ubicación es obligatoria")]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de estación es obligatorio")]
        [EnumDataType(typeof(StationType), ErrorMessage = "Tipo de estación inválido")]
        public StationType Type { get; set; }
    }
}