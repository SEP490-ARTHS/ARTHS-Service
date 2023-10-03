﻿using ARTHS_API.Configurations.Middleware;
using ARTHS_Data.Models.Requests.Filters;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Views;
using ARTHS_Service.Implementations;
using ARTHS_Service.Interfaces;
using ARTHS_Utility.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace ARTHS_API.Controllers
{
    [Route("api/motobike-products")]
    [ApiController]
    public class MotobikeProductController : ControllerBase
    {
        private readonly IMotobikeProductService _motobikeProductService;

        public MotobikeProductController(IMotobikeProductService motobikeProductService)
        {
            _motobikeProductService = motobikeProductService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(MotobikeProductViewModel), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get all motobike productss.")]
        public async Task<ActionResult<List<MotobikeProductViewModel>>> GetMotobikeProducts()
        {
            return await _motobikeProductService.GetMotobikeProducts();
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(MotobikeProductDetailViewModel), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get motobike products by id.")]
        public async Task<ActionResult<MotobikeProductDetailViewModel>> GetMotobikeProduct([FromRoute] Guid id)
        {
            return await _motobikeProductService.GetMotobikeProduct(id);
        }

        [HttpPost]
        [Authorize(UserRole.Owner)]
        [ProducesResponseType(typeof(MotobikeProductDetailViewModel), StatusCodes.Status201Created)]
        [SwaggerOperation(Summary = "Create motobike product.")]
        public async Task<ActionResult<MotobikeProductDetailViewModel>> CreateMotobileProduct([FromForm][Required] CreateMotobikeProductModel model)
        {
            var motobikeProduct = await _motobikeProductService.CreateMotobikeProduct(model);
            return CreatedAtAction(nameof(GetMotobikeProduct), new { id = motobikeProduct.Id }, motobikeProduct);
        }

        [HttpPut]
        [Authorize(UserRole.Owner)]
        [Route("{id}")]
        [ProducesResponseType(typeof(MotobikeProductDetailViewModel), StatusCodes.Status201Created)]
        [SwaggerOperation(Summary = "Update motobike product.")]
        public async Task<ActionResult<MotobikeProductDetailViewModel>> UpdateMotobileProduct([FromRoute] Guid id, [FromForm] UpdateMotobikeProductModel model)
        {
            var motobikeProduct = await _motobikeProductService.UpdateMotobikeProduct(id, model);
            return CreatedAtAction(nameof(GetMotobikeProduct), new { id = motobikeProduct.Id }, motobikeProduct);
        }
    }
}