using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Web.Models;

public class ApplicationUser
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Full Name")]
    [StringLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Phone Number")]
    [Phone]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Address")]
    public string? Address { get; set; }

    public UserRole Role { get; set; } = UserRole.Reader;

    [Display(Name = "Registered At")]
    [DataType(DataType.Date)]
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
}
