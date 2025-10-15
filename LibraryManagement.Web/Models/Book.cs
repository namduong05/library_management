using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Web.Models;

public class Book
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    public string Author { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Category { get; set; }

    [StringLength(20)]
    public string? Isbn { get; set; }

    [Display(Name = "Total Copies")]
    [Range(0, 1000)]
    public int TotalCopies { get; set; }

    [Display(Name = "Available Copies")]
    [Range(0, 1000)]
    public int AvailableCopies { get; set; }

    [Display(Name = "Cover Image URL")]
    [Url]
    public string? CoverUrl { get; set; }

    [DataType(DataType.MultilineText)]
    public string? Description { get; set; }

    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
}
