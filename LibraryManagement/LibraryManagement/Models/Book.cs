using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagement.Models;

public class Book
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [MaxLength(150, ErrorMessage = "Title cannot exceed 150 characters.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "ISBN is required.")]
    [StringLength(13, MinimumLength = 13, ErrorMessage = "ISBN must be exactly 13 characters.")]
    public string ISBN { get; set; } = string.Empty;

    [Display(Name = "Publication Year")]
    [Range(1000, 9999, ErrorMessage = "Please enter a valid year.")]
    public int PublicationYear { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Total Copies cannot be negative.")]
    [Display(Name = "Total Copies")]
    public int TotalCopies { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Available Copies cannot be negative.")]
    [Display(Name = "Available Copies")]
    public int AvailableCopies { get; set; }

    // Foreign Key
    [Display(Name = "Author")]
    public int AuthorId { get; set; }

    // Navigation properties
    public Author? Author { get; set; }
    public ICollection<BorrowRecord> BorrowRecords { get; set; } = new List<BorrowRecord>();
}
