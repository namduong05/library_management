using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Web.Models;

public class Loan
{
    public int Id { get; set; }

    [Display(Name = "Book")]
    public int BookId { get; set; }

    public Book? Book { get; set; }

    [Display(Name = "Borrower")]
    public int ApplicationUserId { get; set; }

    public ApplicationUser? ApplicationUser { get; set; }

    [Display(Name = "Borrowed At")]
    [DataType(DataType.Date)]
    public DateTime BorrowedAt { get; set; } = DateTime.UtcNow;

    [Display(Name = "Due Date")]
    [DataType(DataType.Date)]
    public DateTime DueAt { get; set; } = DateTime.UtcNow.AddDays(14);

    [Display(Name = "Returned At")]
    [DataType(DataType.Date)]
    public DateTime? ReturnedAt { get; set; }

    public LoanStatus Status { get; set; } = LoanStatus.Borrowed;
}
