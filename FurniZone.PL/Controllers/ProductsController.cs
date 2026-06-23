using FurniZone.BLL.ModelVM.Product;
using FurniZone.BLL.Services.Abstractions;
using FurniZone.DAL.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurniZone.PL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] ProductFilterRequest request)
        {
            var result = await _productService.GetAllAsync(request);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _productService.GetByIdAsync(id);
            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> Create([FromForm] CreateProductRequest request)
        {
            var result = await _productService.CreateAsync(request);
            if (!result.Success)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> Update(Guid id, [FromForm] UpdateProductRequest request)
        {
            var result = await _productService.UpdateAsync(id, request);
            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPatch("{id:guid}")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> Patch(Guid id, [FromBody] PatchProductRequest request)
        {
            var result = await _productService.PatchAsync(id, request);
            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _productService.DeleteAsync(id);
            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }
    }
}
