using BackendLabs_2.Models;
using Microsoft.AspNetCore.Mvc;

namespace BackendLabs_2.Controllers;
[ApiController]
[Route("category")]
public class CategoryController: ControllerBase
{
    private static List<Category> _categories =
    [
        new Category { Id = 1, Name = "Fora Sandwich" },
        new Category { Id = 2, Name = "Fora Burger" },
    ];
    
    [HttpGet]
    public ActionResult<Category> GetCategories()
    {
        return Ok(_categories);
    }
    
    [HttpGet("{id}")]
    public ActionResult<Category> GetCategory(int id)
    {
        var category = _categories.FirstOrDefault(c => c.Id == id);
        if (category == null)
        {
            return NotFound();
        }
        return category;
    }
    
    [HttpPost]
    public ActionResult<Category> CreateCategory([FromBody]CreateCategoryRequest request)
    {
        var category = new Category
        {
            Id = _categories.Count + 1,
            Name = request.Name
        };
        _categories.Add(category);
        return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
    }
    
    [HttpDelete("{id}")]
    public ActionResult DeleteCategory(int id)
    {
        var category = _categories.FirstOrDefault(c => c.Id == id);
        if (category == null)
        {
            return NotFound();
        }
        _categories.Remove(category);
        return NoContent();
    }
    
    public record CreateCategoryRequest(string Name);
    
}