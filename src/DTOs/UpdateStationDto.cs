using System.ComponentModel.DataAnnotations;
using perla_metro_Stations_Service.src.Models.Enums;
/// <summary>   
/// DTO para actualizar una estación.
 
namespace perla_metro_Stations_Service.src.DTOs
{
    public class UpdateStationDto
    {
        public string? Name { get; set; }

        public string? Location { get; set; }

        [EnumDataType(typeof(StationType), ErrorMessage = "Tipo de estación inválido")]
        public StationType? Type { get; set; }

        public bool? IsActive { get; set; }
    }
}