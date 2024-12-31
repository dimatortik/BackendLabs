using BackendLabs_2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendLabs_2.Controllers;
[ApiController]
[Route("category")]
public class CategoryController(AppDbContext context): ControllerBase
{
    
    [HttpGet]
    public async Task<ActionResult> GetCategories()
    {
        var category = await context.Categories.ToListAsync();
        return Ok(category);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult> GetCategory(int id)
    {
        var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (category == null)
        {
            return NotFound();
        }
        return Ok(category);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody]CreateCategoryRequest request)
    {
        var isCategoryExist = await context.Categories.AnyAsync(c => c.Name == request.Name);
        if (isCategoryExist)
        {
            return BadRequest("Category already exists");
        }
        var category = new Category()
        {
            Name = request.Name
        };
        await context.Categories.AddAsync(category);
        await context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCategory(int id)
    {
        var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (category == null)
        {
            return NotFound();
        }
        context.Categories.Remove(category);
        return NoContent();
    }
    
    public record CreateCategoryRequest(string Name);
    
}