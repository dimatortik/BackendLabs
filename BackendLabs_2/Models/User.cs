using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendLabs_2.Models;

public class User
{
    [Key]
    public int Id { get; set; }
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    [ForeignKey("Currency")]
    public int DefaultCurrencyId { get; set; }
    public virtual Currency? DefaultCurrency { get; set; } = null!;
    
    [Required]
    [DataType(DataType.Password)]
    public string PasswordHash { get; set; }
    
    public static User Create(string name, Currency currency)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be empty");
        }
        if (currency == null)
        {
            throw new ArgumentException("Currency cannot be null");
        }
        return new User()
        {
            Name = name,
            DefaultCurrency = currency
        };
        
    }
    
}

