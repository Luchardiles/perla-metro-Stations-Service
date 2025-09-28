using System.ComponentModel;

namespace perla_metro_Stations_Service.src.Models.Enums
{
    public enum StationType
    {
        [Description("Origen")]
        Origin = 0,
        
        [Description("Destino")]
        Destination = 1,
        
        [Description("Intermedia")]
        Intermediate = 2
    }
}