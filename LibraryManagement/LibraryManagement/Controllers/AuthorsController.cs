using LibraryManagement.Data;
using LibraryManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Controllers;

public class AuthorsController : Controller
{
    private readonly LibraryDbContext _context;

    public AuthorsController(LibraryDbContext context)
    {
        _context = context;
    }

    // List all authors
    public async Task<IActionResult> Index()
    {
        var authors = await _context.Authors
            .OrderBy(a => a.LastName)
            .ThenBy(a => a.FirstName)
            .ToListAsync();

        return View(authors);
    }

    // Details — show author + their books
    public async Task<IActionResult> Details(int id)
    {
        var author = await _context.Authors
            .Include(a => a.Books)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (author == null)
            return NotFound();

        return View(author);
    }

    // Create GET
    public IActionResult Create()
    {
        return View();
    }

    // Create POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Author author)
    {
        if (ModelState.IsValid)
        {
            _context.Add(author);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Author \"{author.FullName}\" added successfully.";
            return RedirectToAction(nameof(Index));
        }
        return View(author);
    }

    // GET: Authors/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var author = await _context.Authors.FindAsync(id);
        if (author == null)
            return NotFound();

        return View(author);
    }

    // POST: Authors/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Author author)
    {
        if (id != author.Id)
            return NotFound();

        if (ModelState.IsValid)
        {
            _context.Update(author);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Author \"{author.FullName}\" updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        return View(author);
    }
}
