using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendLabs_2.Models;

public class Record
{
    [Key]
    public int Id { get; set; }
    [ForeignKey("User")]
    public int UserId { get; set; }
    [ForeignKey("Category")]
    public int CategoryId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; } = 0;
    [ForeignKey("Currency")]
    public int CurrencyId { get; set; }
    public virtual Currency Currency { get; set; } = null!;
    public User User { get; set; } = null!;
    public Category Category { get; set; } = null!;
    
    public static Record Create(User user, Category category, decimal totalAmount, Currency currency)
    {
        if (totalAmount <= 0)
        {
            throw new ArgumentException("Total amount should be greater than 0");
        }
        if (currency == null)
        {
            throw new ArgumentException("Currency cannot be null");
        }
        return new Record()
        {
            User = user,
            Category = category,
            TotalAmount = totalAmount,
            Currency = currency
        };
    }
}