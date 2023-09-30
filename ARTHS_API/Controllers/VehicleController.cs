using ARTHS_Data.Models.Requests.Filters;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;
using ARTHS_Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ARTHS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;
        public VehicleController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [HttpGet]
        [Route("{id}")]
        [SwaggerOperation(Summary = "Get Vehicle by id.")]
        public async Task<ActionResult> GetVehicle([FromRoute] Guid id)
        {
            try
            {
                var result = await _vehicleService.GetVehicle(id);
                if (result == null)
                {
                    return NotFound("ko tim thay hãng xe");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get all Vehicle or search by name.")]
        public async Task<ActionResult> GetVehicles([FromQuery] VehicleFilterModel filter)
        {
            try
            {
                var result = await _vehicleService.GetVehicles(filter);
                if (result != null)
                {
                    return Ok(result);
                }
                return BadRequest("Something wrong!!!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("create")]
        [SwaggerOperation(Summary = "Create new Vehicle.")]
        public async Task<ActionResult<VehicleViewModel>> CreateVehicle([FromBody] CreateVehicleRequest request)
        {
            try
            {
                var result = await _vehicleService.CreateVehicle(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }

        [HttpPut]
        [Route("{id}")]
        [SwaggerOperation(Summary = "Update Vehicle.")]
        public async Task<IActionResult> UpdateVehicle([FromRoute] Guid id,
                                                        [FromBody] UpdateVehicleRequest request)
        {
            try
            {
                var result = await _vehicleService.UpdateVehicle(id, request);
                if (result == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, "không tìm thấy hãng xe");

                }
                return StatusCode(StatusCodes.Status201Created, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("{id}")]
        [SwaggerOperation(Summary = "Delete Vehicle.")]
        public async Task<IActionResult> DeleteVehicle([FromRoute] Guid id)
        {
            try
            {
                var result = await _vehicleService.DeleteVehicle(id);
                if (result != null)
                {
                    return Ok("xóa thành công");
                }
                return BadRequest("Somethings wrong!!!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
