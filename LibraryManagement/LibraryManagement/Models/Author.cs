using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models;

public class Author
{
    public int Id { get; set; }

    [Required(ErrorMessage = "First name is required.")]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    [Display(Name = "Date of Birth")]
    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    // Navigation property
    public ICollection<Book> Books { get; set; } = new List<Book>();

    // Computed helper — not stored in DB
    public string FullName => $"{FirstName} {LastName}";
}
