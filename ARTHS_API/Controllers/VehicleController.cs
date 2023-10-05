using ARTHS_Data.Models.Requests.Filters;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;
using ARTHS_Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ARTHS_API.Controllers
{
    [Route("api/vehicles")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;
        public VehicleController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<VehicleViewModel>), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get all vehicles.")]
        public async Task<ActionResult> GetVehicles([FromQuery] VehicleFilterModel filter)
        {
            try
            {
                var result = await _vehicleService.GetVehicles(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(VehicleViewModel), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get vehicle by id.")]
        public async Task<ActionResult> GetVehicle([FromRoute] Guid id)
        {
            try
            {
                var result = await _vehicleService.GetVehicle(id);
                return result != null ? Ok(result) : NotFound();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        
        [HttpPost]
        [ProducesResponseType(typeof(VehicleViewModel), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Create vehicle.")]
        public async Task<ActionResult<VehicleViewModel>> CreateVehicle([FromBody] CreateVehicleRequest request)
        {
            try
            {
                var result = await _vehicleService.CreateVehicle(request);
                return CreatedAtAction(nameof(GetVehicle), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }

        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(typeof(VehicleViewModel), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Update vehicle.")]
        public async Task<IActionResult> UpdateVehicle([FromRoute] Guid id,
                                                        [FromBody] UpdateVehicleRequest request)
        {
            try
            {
                var result = await _vehicleService.UpdateVehicle(id, request);
                return CreatedAtAction(nameof(GetVehicle), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("{id}")]
        [SwaggerOperation(Summary = "Delete vehicle.")]
        public async Task<IActionResult> DeleteVehicle([FromRoute] Guid id)
        {
            try
            {
                var result = await _vehicleService.DeleteVehicle(id);
                return result != null ? Ok("xóa thành công") : NotFound();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
