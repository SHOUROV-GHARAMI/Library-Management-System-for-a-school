using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Controllers;

public class BooksController : Controller
{
    private readonly LibraryDbContext _context;

    public BooksController(LibraryDbContext context)
    {
        _context = context;
    }

    // ── a. Index — list all books sorted by Title ──────────────────────────────
    public async Task<IActionResult> Index()
    {
        var books = await _context.Books
            .Include(b => b.Author)
            .OrderBy(b => b.Title)
            .Select(b => new BookIndexViewModel
            {
                Id              = b.Id,
                Title           = b.Title,
                ISBN            = b.ISBN,
                AuthorFullName  = b.Author!.FirstName + " " + b.Author.LastName,
                AvailableCopies = b.AvailableCopies,
                TotalCopies     = b.TotalCopies
            })
            .ToListAsync();

        return View(books);
    }

    // ── b. Details — book info + all borrow records ────────────────────────────
    public async Task<IActionResult> Details(int id)
    {
        var book = await _context.Books
            .Include(b => b.Author)
            .Include(b => b.BorrowRecords)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (book == null)
            return NotFound();

        return View(book);
    }

    // ── c. Create GET ──────────────────────────────────────────────────────────
    public async Task<IActionResult> Create()
    {
        await PopulateAuthorsDropdownAsync();
        return View();
    }

    // ── c. Create POST ─────────────────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Book book)
    {
        // Business rule: TotalCopies must be at least 1
        if (book.TotalCopies < 1)
            ModelState.AddModelError(nameof(book.TotalCopies),
                "Total Copies must be at least 1.");

        // Business rule: AvailableCopies must equal TotalCopies on creation
        if (book.AvailableCopies != book.TotalCopies)
            ModelState.AddModelError(nameof(book.AvailableCopies),
                "Available Copies must equal Total Copies when creating a new book.");

        // Check ISBN uniqueness manually for a friendly error
        bool isbnExists = await _context.Books
            .AnyAsync(b => b.ISBN == book.ISBN);
        if (isbnExists)
            ModelState.AddModelError(nameof(book.ISBN),
                "A book with this ISBN already exists.");

        if (ModelState.IsValid)
        {
            _context.Add(book);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Book \"{book.Title}\" was added successfully.";
            return RedirectToAction(nameof(Index));
        }

        await PopulateAuthorsDropdownAsync(book.AuthorId);
        return View(book);
    }

    // GET: Books/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null)
            return NotFound();

        await PopulateAuthorsDropdownAsync(book.AuthorId);
        return View(book);
    }

    // POST: Books/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Book book)
    {
        if (id != book.Id)
            return NotFound();

        if (book.TotalCopies < 1)
            ModelState.AddModelError(nameof(book.TotalCopies),
                "Total Copies must be at least 1.");

        if (book.AvailableCopies > book.TotalCopies)
            ModelState.AddModelError(nameof(book.AvailableCopies),
                "Available Copies cannot exceed Total Copies.");

        bool isbnExists = await _context.Books
            .AnyAsync(b => b.ISBN == book.ISBN && b.Id != id);
        if (isbnExists)
            ModelState.AddModelError(nameof(book.ISBN),
                "A book with this ISBN already exists.");

        if (ModelState.IsValid)
        {
            _context.Update(book);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Book \"{book.Title}\" was updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        await PopulateAuthorsDropdownAsync(book.AuthorId);
        return View(book);
    }

    // ── d. Borrow GET ──────────────────────────────────────────────────────────
    public async Task<IActionResult> Borrow(int bookId)
    {
        var book = await _context.Books.FindAsync(bookId);
        if (book == null)
            return NotFound();

        var vm = new BorrowViewModel
        {
            BookId    = book.Id,
            BookTitle = book.Title,
            DueDate   = DateTime.Today.AddDays(14)
        };

        return View(vm);
    }

    // ── d. Borrow POST ─────────────────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Borrow(BorrowViewModel vm)
    {
        var book = await _context.Books.FindAsync(vm.BookId);
        if (book == null)
            return NotFound();

        // Always restore BookTitle (not posted from the hidden field safely)
        vm.BookTitle = book.Title;

        if (!ModelState.IsValid)
            return View(vm);

        // Check availability
        if (book.AvailableCopies <= 0)
        {
            ModelState.AddModelError(string.Empty,
                "No copies available. Please check back later.");
            return View(vm);
        }

        // Create the borrow record
        var record = new BorrowRecord
        {
            BookId      = vm.BookId,
            StudentName = vm.StudentName,
            BorrowDate  = DateTime.Now,
            DueDate     = vm.DueDate
        };

        book.AvailableCopies--;

        _context.BorrowRecords.Add(record);
        await _context.SaveChangesAsync();

        TempData["Success"] =
            $"Book \"{book.Title}\" successfully borrowed by {vm.StudentName}.";

        return RedirectToAction(nameof(Details), new { id = vm.BookId });
    }

    // ── Private helper ─────────────────────────────────────────────────────────
    private async Task PopulateAuthorsDropdownAsync(int? selectedId = null)
    {
        var authors = await _context.Authors
            .OrderBy(a => a.LastName)
            .ThenBy(a => a.FirstName)
            .Select(a => new
            {
                a.Id,
                FullName = a.FirstName + " " + a.LastName
            })
            .ToListAsync();

        ViewBag.AuthorId = new SelectList(authors, "Id", "FullName", selectedId);
    }
}
