using System;
using System.ComponentModel.DataAnnotations;

namespace perla_metro_Stations_Service.src.Models
{
    public enum StationType { Origen = 0, Destino = 1, Intermedia = 2 }

    public class Station
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Nombre { get; set; } = null!;

        [Required]
        public string Ubicacion { get; set; } = null!;

        [Required]
        public StationType TipoParada { get; set; }

        // Soft delete flag
        public bool Activa { get; set; } = true;

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    }
}
