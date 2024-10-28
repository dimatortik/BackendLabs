using BackendLabs_2.Models;
using Microsoft.AspNetCore.Mvc;

namespace BackendLabs_2.Controllers;

[ApiController]
[Route("record")]
public class RecordController : ControllerBase
{
    private static List<Record> _records =
    [
        new Record{ Id = 1, UserId = 1, CategoryId = 1, TotalAmount = 50 },
        new Record{ Id = 2, UserId = 2, CategoryId = 2, TotalAmount = 100 },
        new Record{ Id = 3, UserId = 3, CategoryId = 1, TotalAmount = 150 },
        new Record{ Id = 4, UserId = 4, CategoryId = 2, TotalAmount = 200 },
    ];
    
    [HttpGet("{id}")]
    public ActionResult<Record> GetRecord(int id)
    {
        var record = _records.FirstOrDefault(r => r.Id == id);
        if (record == null)
        {
            return NotFound();
        }
        return record;
    }
    
    [HttpPost]
    public ActionResult<Record> CreateRecord(
        [FromBody]CreateRecordRequest request)
    {
        var record = new Record
        {
            Id = _records.Count + 1,
            UserId = request.UserId,
            CategoryId = request.CategoryId,
            TotalAmount = request.TotalAmount
        };
        _records.Add(record);
        return CreatedAtAction(nameof(GetRecord), new { id = record.Id }, record);
    }
    
    [HttpDelete("{id:int}")]
    public ActionResult DeleteRecord(int id)
    {
        var record = _records.FirstOrDefault(r => r.Id == id);
        if (record == null)
        {
            return NotFound();
        }
        _records.Remove(record);
        return NoContent();
    }
    
    [HttpGet]
    public ActionResult<IEnumerable<Record>> GetRecords([FromQuery]int? userId, [FromQuery]int? categoryId)
    {
        if (userId == null && categoryId == null)
        {
            return BadRequest();
        }
        
        var filteredRecords = _records.AsQueryable();
        
        if (categoryId != null)
        {
            filteredRecords = filteredRecords.Where(p => p.CategoryId == categoryId);
        }
        
        if (userId != null)
        {
            filteredRecords = filteredRecords.Where(p => p.UserId == userId);
        }

        return filteredRecords.Any() ?  Ok(filteredRecords) : NotFound("No records found");
    }
    
}

public record CreateRecordRequest(int UserId, int CategoryId, decimal TotalAmount);