using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;
using ARTHS_Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ARTHS_API.Controllers
{
    [Route("api/configurations")]
    [ApiController]
    public class ConfigurationsController : ControllerBase
    {
        private readonly IConfigurationService _configurationService;

        public ConfigurationsController(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ConfigurationViewModel), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get settings of project.")]
        public async Task<ActionResult<ConfigurationViewModel>> GetSettings()
        {
            return await _configurationService.GetSetting();
        }

        [HttpPut]
        [ProducesResponseType(typeof(ConfigurationViewModel), StatusCodes.Status201Created)]
        [SwaggerOperation(Summary = "Update settings.")]
        public async Task<ActionResult<ConfigurationViewModel>> UpdateBookingSetting([FromBody] UpdateConfigurationModel model)
        {
            var setting = await _configurationService.UpdateSetting(model);
            return CreatedAtAction(nameof(GetSettings), setting);
        }
    }
}
