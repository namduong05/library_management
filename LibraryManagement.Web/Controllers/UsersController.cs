using LibraryManagement.Web.Data;
using LibraryManagement.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace LibraryManagement.Web.Controllers;

public class UsersController : Controller
{
    private readonly LibraryContext _context;

    public UsersController(LibraryContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? role)
    {
        var query = _context.ApplicationUsers.AsQueryable();
        if (!string.IsNullOrWhiteSpace(role) && Enum.TryParse<UserRole>(role, out var parsedRole))
        {
            query = query.Where(u => u.Role == parsedRole);
        }

        ViewBag.Role = role;
        ViewBag.RoleOptions = Enum.GetValues<UserRole>()
            .Select(r => new SelectListItem(r.ToString(), r.ToString()))
            .ToList();

        var users = await query.OrderBy(u => u.FullName).ToListAsync();
        return View(users);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _context.ApplicationUsers
            .Include(u => u.Loans)
            .ThenInclude(l => l.Book)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    public IActionResult Create()
    {
        ViewBag.RoleOptions = Enum.GetValues<UserRole>()
            .Select(r => new SelectListItem(r.ToString(), r.ToString()))
            .ToList();
        return View(new ApplicationUser { RegisteredAt = DateTime.UtcNow });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("FullName,Email,PhoneNumber,Address,Role,RegisteredAt")] ApplicationUser applicationUser)
    {
        if (await _context.ApplicationUsers.AnyAsync(u => u.Email == applicationUser.Email))
        {
            ModelState.AddModelError(nameof(ApplicationUser.Email), "Email already exists in the system.");
        }

        if (ModelState.IsValid)
        {
            _context.Add(applicationUser);
            await _context.SaveChangesAsync();
            TempData["Message"] = $"User '{applicationUser.FullName}' created.";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.RoleOptions = Enum.GetValues<UserRole>()
            .Select(r => new SelectListItem(r.ToString(), r.ToString()))
            .ToList();
        return View(applicationUser);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var applicationUser = await _context.ApplicationUsers.FindAsync(id);
        if (applicationUser == null)
        {
            return NotFound();
        }

        ViewBag.RoleOptions = Enum.GetValues<UserRole>()
            .Select(r => new SelectListItem(r.ToString(), r.ToString()))
            .ToList();
        return View(applicationUser);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Email,PhoneNumber,Address,Role,RegisteredAt")] ApplicationUser applicationUser)
    {
        if (id != applicationUser.Id)
        {
            return NotFound();
        }

        if (await _context.ApplicationUsers.AnyAsync(u => u.Email == applicationUser.Email && u.Id != applicationUser.Id))
        {
            ModelState.AddModelError(nameof(ApplicationUser.Email), "Email already exists in the system.");
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(applicationUser);
                await _context.SaveChangesAsync();
                TempData["Message"] = $"User '{applicationUser.FullName}' updated.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ApplicationUserExists(applicationUser.Id))
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

        ViewBag.RoleOptions = Enum.GetValues<UserRole>()
            .Select(r => new SelectListItem(r.ToString(), r.ToString()))
            .ToList();
        return View(applicationUser);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var applicationUser = await _context.ApplicationUsers
            .FirstOrDefaultAsync(m => m.Id == id);
        if (applicationUser == null)
        {
            return NotFound();
        }

        return View(applicationUser);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var applicationUser = await _context.ApplicationUsers.FindAsync(id);
        if (applicationUser != null)
        {
            _context.ApplicationUsers.Remove(applicationUser);
            await _context.SaveChangesAsync();
            TempData["Message"] = "User removed.";
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task<bool> ApplicationUserExists(int id)
    {
        return await _context.ApplicationUsers.AnyAsync(e => e.Id == id);
    }
}
