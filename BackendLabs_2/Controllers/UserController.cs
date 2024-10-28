using BackendLabs_2.Models;
using Microsoft.AspNetCore.Mvc;

namespace BackendLabs_2.Controllers;
[ApiController]
public class UserController : ControllerBase
{
    private static List<User> _users =
    [
        new User{ Id = 1, Name = "John" },
        new User{ Id = 2, Name = "Doe" },
        new User { Id = 3, Name = "Jane" },
        new User{ Id = 4, Name = "Daniel" },

    ];
    
    [HttpGet("/users")]
    public ActionResult<IEnumerable<User>> GetUsers()
    {
        return _users;
    }
    
    [HttpGet("/user/{id}")]
    public ActionResult<User> GetUser(int id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            return NotFound();
        }
        return user;
    }
    
    [HttpPost("/user")]
    public ActionResult<User> CreateUser([FromBody]CreateUserRequest request)
    {
        var user = new User
        {
            Id = _users.Count + 1,
            Name = request.Name
        };
        _users.Add(user);
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }
    
    [HttpDelete("/user/{id}")]
    public ActionResult DeleteUser(int id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            return NotFound();
        }
        _users.Remove(user);
        return NoContent();
    }
}
public record CreateUserRequest(string Name);