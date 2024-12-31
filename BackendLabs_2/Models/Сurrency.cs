using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace BackendLabs_2.Models;

[Index(nameof(Symbol), IsUnique = true)]
public class Currency
{
    [Key]
    public int Id { get; set; }
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(3)]
    public string Symbol { get; set; } = string.Empty;
    
    public static Currency DefualtCurrency => new Currency()
    {
        Id = 1,
        Name = "American Dollar",
        Symbol = "USD"
    };
}