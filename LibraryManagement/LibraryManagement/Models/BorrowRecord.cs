using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagement.Models;

public class BorrowRecord
{
    public int Id { get; set; }

    public int BookId { get; set; }

    [Required(ErrorMessage = "Student name is required.")]
    [MaxLength(100, ErrorMessage = "Student name cannot exceed 100 characters.")]
    [Display(Name = "Student Name")]
    public string StudentName { get; set; } = string.Empty;

    [Display(Name = "Borrow Date")]
    [DataType(DataType.DateTime)]
    public DateTime BorrowDate { get; set; }

    [Display(Name = "Due Date")]
    [DataType(DataType.Date)]
    public DateTime DueDate { get; set; }

    [Display(Name = "Return Date")]
    [DataType(DataType.Date)]
    public DateTime? ReturnDate { get; set; }

    // Computed property — NOT stored in DB
    [NotMapped]
    public bool IsReturned => ReturnDate.HasValue;

    // Navigation property
    public Book? Book { get; set; }
}
