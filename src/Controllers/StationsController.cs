using Microsoft.AspNetCore.Mvc;
using perla_metro_Stations_Service.src.DTOs;
using perla_metro_Stations_Service.src.Models.Enums;
using perla_metro_Stations_Service.src.Services;


namespace perla_metro_Stations_Service.src.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class StationsController : ControllerBase
    {
        private readonly IStationService _stationService;
        private readonly ILogger<StationsController> _logger;

        public StationsController(IStationService stationService, ILogger<StationsController> logger)
        {
            _stationService = stationService;
            _logger = logger;
        }

        /// <summary>
        /// Crear una nueva estación
        /// </summary>
        /// <param name="createStationDto">Datos de la nueva estación</param>
        /// <returns>Información de la estación creada</returns>
        [HttpPost]
        [ProducesResponseType(typeof(StationResponseDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        public async Task<ActionResult<StationResponseDto>> CreateStation([FromBody] CreateStationDto createStationDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var station = await _stationService.CreateStationAsync(createStationDto);
                if (station == null)
                {
                    _logger.LogWarning("No se pudo crear la estación. El resultado es nulo.");
                    return StatusCode(500, new { message = "No se pudo crear la estación" });
                }                
                _logger.LogInformation("Estación creada exitosamente: {StationId} - {StationName}", 
                    station.Id, station.Name);

                return CreatedAtAction(nameof(GetStationById), new { id = station.Id }, station);
            }
            
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Error al crear estación: {Message}", ex.Message);
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno al crear estación");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtener todas las estaciones (solo para administradores)
        /// </summary>
        /// <param name="nameFilter">Filtro por nombre (opcional)</param>
        /// <param name="isActive">Filtro por estado activo/inactivo (opcional)</param>
        /// <param name="type">Filtro por tipo de estación (opcional)</param>
        /// <returns>Lista de estaciones</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<GetAllStationsDto>), 200)]
        public async Task<ActionResult<IEnumerable<GetAllStationsDto>>> GetAllStations(
            [FromQuery] string? nameFilter = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] StationType? type = null)
        {
            try
            {
                var stations = await _stationService.GetAllStationsAsync(nameFilter, isActive, type);
                
                _logger.LogInformation("Consulta de estaciones realizada. Filtros - Nombre: {NameFilter}, Activo: {IsActive}, Tipo: {Type}", 
                    nameFilter, isActive, type);

                return Ok(stations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de estaciones");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtener una estación por su ID
        /// </summary>
        /// <param name="id">ID de la estación</param>
        /// <returns>Información detallada de la estación</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(StationResponseDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<StationResponseDto>> GetStationById([FromRoute] Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest(new { message = "ID de estación inválido" });
                }

                var station = await _stationService.GetStationByIdAsync(id);
                
                if (station == null)
                {
                    _logger.LogWarning("Estación no encontrada: {StationId}", id);
                    return NotFound(new { message = "Estación no encontrada" });
                }

                return Ok(station);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estación por ID: {StationId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Actualizar una estación existente
        /// </summary>
        /// <param name="id">ID de la estación</param>
        /// <param name="updateStationDto">Datos a actualizar</param>
        /// <returns>Información actualizada de la estación</returns>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(StationResponseDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<ActionResult<StationResponseDto>> UpdateStation(
            [FromRoute] Guid id, 
            [FromBody] UpdateStationDto updateStationDto)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest(new { message = "ID de estación inválido" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updatedStation = await _stationService.UpdateStationAsync(id, updateStationDto);
                
                if (updatedStation == null)
                {
                    _logger.LogWarning("Intento de actualizar estación inexistente: {StationId}", id);
                    return NotFound(new { message = "Estación no encontrada" });
                }

                _logger.LogInformation("Estación actualizada exitosamente: {StationId} - {StationName}", 
                    updatedStation.Id, updatedStation.Name);

                return Ok(updatedStation);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Error al actualizar estación {StationId}: {Message}", id, ex.Message);
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno al actualizar estación: {StationId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Eliminar una estación (soft delete - solo administradores)
        /// </summary>
        /// <param name="id">ID de la estación</param>
        /// <returns>Confirmación de eliminación</returns>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteStation([FromRoute] Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest(new { message = "ID de estación inválido" });
                }

                var deleted = await _stationService.DeleteStationAsync(id);
                
                if (!deleted)
                {
                    _logger.LogWarning("Intento de eliminar estación inexistente: {StationId}", id);
                    return NotFound(new { message = "Estación no encontrada" });
                }

                _logger.LogInformation("Estación eliminada (soft delete) exitosamente: {StationId}", id);
                
                return Ok(new { message = "Estación eliminada exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno al eliminar estación: {StationId}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Verificar si una estación existe
        /// </summary>
        /// <param name="id">ID de la estación</param>
        /// <returns>Estado de existencia de la estación</returns>
        [HttpHead("{id:guid}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> CheckStationExists([FromRoute] Guid id)
        {
            try
            {
                var exists = await _stationService.StationExistsAsync(id);
                return exists ? Ok() : NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar existencia de estación: {StationId}", id);
                return StatusCode(500);
            }
        }
    }
}