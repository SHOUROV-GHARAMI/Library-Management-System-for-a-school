using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.ViewModels;

public class BorrowViewModel
{
    public int BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;

    [Required(ErrorMessage = "Student name is required.")]
    [MaxLength(100, ErrorMessage = "Student name cannot exceed 100 characters.")]
    [Display(Name = "Student Name")]
    public string StudentName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Due date is required.")]
    [Display(Name = "Due Date")]
    [DataType(DataType.Date)]
    public DateTime DueDate { get; set; } = DateTime.Today.AddDays(14);
}
