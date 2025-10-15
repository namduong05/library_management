using LibraryManagement.Web.Data;
using LibraryManagement.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace LibraryManagement.Web.Controllers;

public class LoansController : Controller
{
    private readonly LibraryContext _context;

    public LoansController(LibraryContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? status)
    {
        var query = _context.Loans
            .Include(l => l.Book)
            .Include(l => l.ApplicationUser)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<LoanStatus>(status, out var parsedStatus))
        {
            query = query.Where(l => l.Status == parsedStatus);
        }

        var loans = await query.OrderByDescending(l => l.BorrowedAt).ToListAsync();

        var now = DateTime.UtcNow;
        var updated = false;
        foreach (var loan in loans.Where(l => l.Status == LoanStatus.Borrowed && l.DueAt < now))
        {
            loan.Status = LoanStatus.Overdue;
            _context.Loans.Update(loan);
            updated = true;
        }

        if (updated)
        {
            await _context.SaveChangesAsync();
        }

        ViewBag.Status = status;
        ViewBag.StatusOptions = Enum.GetValues<LoanStatus>()
            .Select(s => new SelectListItem(s.ToString(), s.ToString()))
            .ToList();
        return View(loans);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var loan = await _context.Loans
            .Include(l => l.Book)
            .Include(l => l.ApplicationUser)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (loan == null)
        {
            return NotFound();
        }

        return View(loan);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new Loan { BorrowedAt = DateTime.UtcNow, DueAt = DateTime.UtcNow.AddDays(14) });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("BookId,ApplicationUserId,BorrowedAt,DueAt")] Loan loan)
    {
        if (!await _context.Books.AnyAsync(b => b.Id == loan.BookId && b.AvailableCopies > 0))
        {
            ModelState.AddModelError(nameof(Loan.BookId), "Selected book is not available for borrowing.");
        }

        if (!await _context.ApplicationUsers.AnyAsync(u => u.Id == loan.ApplicationUserId))
        {
            ModelState.AddModelError(nameof(Loan.ApplicationUserId), "Selected reader does not exist.");
        }

        if (loan.DueAt < loan.BorrowedAt)
        {
            ModelState.AddModelError(nameof(Loan.DueAt), "Due date must be after borrowed date.");
        }

        if (ModelState.IsValid)
        {
            loan.Status = LoanStatus.Borrowed;
            _context.Loans.Add(loan);

            var book = await _context.Books.FindAsync(loan.BookId);
            if (book != null)
            {
                book.AvailableCopies = Math.Max(0, book.AvailableCopies - 1);
                _context.Books.Update(book);
            }

            await _context.SaveChangesAsync();
            TempData["Message"] = "Loan created successfully.";
            return RedirectToAction(nameof(Index));
        }

        await PopulateDropdownsAsync();
        return View(loan);
    }

    public async Task<IActionResult> Return(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var loan = await _context.Loans
            .Include(l => l.Book)
            .Include(l => l.ApplicationUser)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (loan == null)
        {
            return NotFound();
        }

        if (loan.Status == LoanStatus.Returned)
        {
            TempData["Message"] = "Loan has already been returned.";
            return RedirectToAction(nameof(Index));
        }

        return View(loan);
    }

    [HttpPost, ActionName("Return")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ReturnConfirmed(int id)
    {
        var loan = await _context.Loans.FindAsync(id);
        if (loan == null)
        {
            return NotFound();
        }

        if (loan.Status == LoanStatus.Returned)
        {
            return RedirectToAction(nameof(Index));
        }

        loan.ReturnedAt = DateTime.UtcNow;
        loan.Status = LoanStatus.Returned;
        _context.Loans.Update(loan);

        var book = await _context.Books.FindAsync(loan.BookId);
        if (book != null)
        {
            book.AvailableCopies += 1;
            if (book.AvailableCopies > book.TotalCopies)
            {
                book.AvailableCopies = book.TotalCopies;
            }
            _context.Books.Update(book);
        }

        await _context.SaveChangesAsync();
        TempData["Message"] = "Loan marked as returned successfully.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var loan = await _context.Loans
            .Include(l => l.Book)
            .Include(l => l.ApplicationUser)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (loan == null)
        {
            return NotFound();
        }

        return View(loan);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var loan = await _context.Loans.FindAsync(id);
        if (loan != null)
        {
            if (loan.Status != LoanStatus.Returned)
            {
                var book = await _context.Books.FindAsync(loan.BookId);
                if (book != null)
                {
                    book.AvailableCopies = Math.Min(book.TotalCopies, book.AvailableCopies + 1);
                    _context.Books.Update(book);
                }
            }
            _context.Loans.Remove(loan);
            await _context.SaveChangesAsync();
            TempData["Message"] = "Loan record deleted.";
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync()
    {
        var availableBooks = await _context.Books
            .Where(b => b.AvailableCopies > 0)
            .OrderBy(b => b.Title)
            .Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = $"{b.Title} ({b.AvailableCopies} available)"
            })
            .ToListAsync();

        var readers = await _context.ApplicationUsers
            .Where(u => u.Role == UserRole.Reader)
            .OrderBy(u => u.FullName)
            .Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.FullName
            })
            .ToListAsync();

        ViewBag.Books = availableBooks;
        ViewBag.Readers = readers;
    }
}

