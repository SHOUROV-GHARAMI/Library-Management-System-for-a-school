using LibraryManagement.Data;
using LibraryManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace LibraryManagement.Controllers;

public class HomeController : Controller
{
    private readonly LibraryDbContext _context;

    public HomeController(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.TotalBooks    = await _context.Books.CountAsync();
        ViewBag.TotalAuthors  = await _context.Authors.CountAsync();
        ViewBag.TotalBorrowed = await _context.BorrowRecords
                                    .CountAsync(br => br.ReturnDate == null);
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}
