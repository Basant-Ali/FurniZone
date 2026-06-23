using FurniZone.BLL.ModelVM.Category;
using FurniZone.BLL.Services.Abstractions;
using FurniZone.DAL.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurniZone.PL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var result = await _categoryService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _categoryService.GetByIdAsync(id);
            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
        {
            var result = await _categoryService.CreateAsync(request);
            if (!result.Success)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryRequest request)
        {
            var result = await _categoryService.UpdateAsync(id, request);
            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _categoryService.DeleteAsync(id);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
