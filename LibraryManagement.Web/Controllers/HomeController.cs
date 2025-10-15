using LibraryManagement.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace LibraryManagement.Web.Controllers;

public class HomeController : Controller
{
    private readonly LibraryContext _context;

    public HomeController(LibraryContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var bookCount = await _context.Books.CountAsync();
        var readerCount = await _context.ApplicationUsers.CountAsync(u => u.Role == Models.UserRole.Reader);
        var librarianCount = await _context.ApplicationUsers.CountAsync(u => u.Role == Models.UserRole.Librarian);
        var activeLoans = await _context.Loans.CountAsync(l => l.Status == Models.LoanStatus.Borrowed || l.Status == Models.LoanStatus.Overdue);

        var recentLoans = await _context.Loans
            .Include(l => l.Book)
            .Include(l => l.ApplicationUser)
            .OrderByDescending(l => l.BorrowedAt)
            .Take(5)
            .ToListAsync();

        ViewBag.BookCount = bookCount;
        ViewBag.ReaderCount = readerCount;
        ViewBag.LibrarianCount = librarianCount;
        ViewBag.ActiveLoanCount = activeLoans;
        ViewBag.RecentLoans = recentLoans;

        return View();
    }
}
