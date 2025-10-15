using LibraryManagement.Web.Data;
using LibraryManagement.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace LibraryManagement.Web.Controllers;

public class BooksController : Controller
{
    private readonly LibraryContext _context;

    public BooksController(LibraryContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? search)
    {
        var query = _context.Books.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(b => b.Title.Contains(search) || b.Author.Contains(search));
        }

        var books = await query.OrderBy(b => b.Title).ToListAsync();
        ViewBag.Search = search;
        return View(books);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = await _context.Books
            .Include(b => b.Loans)
            .ThenInclude(l => l.ApplicationUser)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    public IActionResult Create()
    {
        return View(new Book { AvailableCopies = 1, TotalCopies = 1 });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Title,Author,Category,Isbn,TotalCopies,AvailableCopies,CoverUrl,Description")] Book book)
    {
        if (book.AvailableCopies > book.TotalCopies)
        {
            ModelState.AddModelError(nameof(Book.AvailableCopies), "Available copies cannot exceed total copies.");
        }

        if (ModelState.IsValid)
        {
            if (book.AvailableCopies == 0)
            {
                book.AvailableCopies = book.TotalCopies;
            }

            _context.Add(book);
            await _context.SaveChangesAsync();
            TempData["Message"] = $"Book '{book.Title}' has been added.";
            return RedirectToAction(nameof(Index));
        }
        return View(book);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = await _context.Books.FindAsync(id);
        if (book == null)
        {
            return NotFound();
        }
        return View(book);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Author,Category,Isbn,TotalCopies,AvailableCopies,CoverUrl,Description")] Book book)
    {
        if (id != book.Id)
        {
            return NotFound();
        }

        if (book.AvailableCopies > book.TotalCopies)
        {
            ModelState.AddModelError(nameof(Book.AvailableCopies), "Available copies cannot exceed total copies.");
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(book);
                await _context.SaveChangesAsync();
                TempData["Message"] = $"Book '{book.Title}' updated.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await BookExists(book.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(book);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = await _context.Books.FirstOrDefaultAsync(m => m.Id == id);
        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book != null)
        {
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            TempData["Message"] = "Book removed from catalog.";
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task<bool> BookExists(int id)
    {
        return await _context.Books.AnyAsync(e => e.Id == id);
    }
}
