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
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        [Route("{id}")]
        [SwaggerOperation(Summary = "Get Category by id.")]
        public async Task<ActionResult> GetCategory([FromRoute] Guid id)
        {
            try
            {
                var result = await _categoryService.GetCategory(id);
                if (result == null)
                {
                    return NotFound("ko tim thay danh muc");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Det all Category or search by name.")]
        public async Task<ActionResult> GetCategories([FromQuery] CategoryFilterModel filter)
        {
            try
            {
                var result = await _categoryService.GetCategories(filter);
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
        [SwaggerOperation(Summary = "Create new Category.")]
        public async Task<ActionResult<CategoryViewModel>> CreateCategory([FromBody] CreateCategoryRequest request)
        {
            try
            {
                var result = await _categoryService.CreateCategory(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }

        [HttpPut]
        [Route("{id}")]
        [SwaggerOperation(Summary = "Update Category.")]
        public async Task<IActionResult> UpdateCategory([FromRoute] Guid id,
                                                        [FromBody] UpdateCategoryRequest request)
        {
            try
            {
                var result = await _categoryService.UpdateCategory(id, request);
                if (result == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, "Not found this category");

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
        [SwaggerOperation(Summary = "Delete Category.")]
        public async Task<IActionResult> DeleteCategory([FromRoute] Guid id)
        {
            try
            {
                var result = await _categoryService.DeleteCategory(id);
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
