using ARTHS_Data.Models.Requests.Filters;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;
using ARTHS_Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ARTHS_API.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<CategoryViewModel>), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get all category.")]
        public async Task<ActionResult> GetCategories([FromQuery] CategoryFilterModel filter)
        {
            try
            {
                var result = await _categoryService.GetCategories(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(CategoryViewModel), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get category by id.")]
        public async Task<ActionResult> GetCategory([FromRoute] Guid id)
        {
            try
            {
                var result = await _categoryService.GetCategory(id);
                return result != null ? Ok(result) : NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        
        [HttpPost]
        [ProducesResponseType(typeof(CategoryViewModel), StatusCodes.Status201Created)]
        [SwaggerOperation(Summary = "Create category.")]
        public async Task<ActionResult<CategoryViewModel>> CreateCategory([FromBody] CreateCategoryRequest request)
        {
            try
            {
                var result = await _categoryService.CreateCategory(request);
                return CreatedAtAction(nameof(GetCategory), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }

        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(typeof(CategoryViewModel), StatusCodes.Status201Created)]
        [SwaggerOperation(Summary = "Update category.")]
        public async Task<IActionResult> UpdateCategory([FromRoute] Guid id,
                                                        [FromBody] UpdateCategoryRequest request)
        {
            try
            {
                var result = await _categoryService.UpdateCategory(id, request);
                return CreatedAtAction(nameof(GetCategory), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("{id}")]
        [SwaggerOperation(Summary = "Delete category.")]
        public async Task<IActionResult> DeleteCategory([FromRoute] Guid id)
        {
            try
            {
                var result = await _categoryService.DeleteCategory(id);
                return result != null ? Ok("xóa thành công") : NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
