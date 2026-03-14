using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Data;

public class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) { }

    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<BorrowRecord> BorrowRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── Author ─────────────────────────────────────────────────────────────
        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.FirstName).IsRequired();
            entity.Property(a => a.LastName).IsRequired();
        });

        // ── Book ───────────────────────────────────────────────────────────────
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(b => b.Id);

            entity.Property(b => b.Title)
                  .IsRequired()
                  .HasMaxLength(150);

            entity.Property(b => b.ISBN)
                  .IsRequired()
                  .HasMaxLength(13);

            // ISBN must be unique
            entity.HasIndex(b => b.ISBN)
                  .IsUnique();

            // AvailableCopies cannot exceed TotalCopies (DB-level check constraint)
            entity.ToTable(t =>
                t.HasCheckConstraint(
                    "CK_Book_AvailableCopies_LessThanOrEqual_TotalCopies",
                    "[AvailableCopies] <= [TotalCopies]"
                ));

            // FK → Author
            entity.HasOne(b => b.Author)
                  .WithMany(a => a.Books)
                  .HasForeignKey(b => b.AuthorId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ── BorrowRecord ───────────────────────────────────────────────────────
        modelBuilder.Entity<BorrowRecord>(entity =>
        {
            entity.HasKey(br => br.Id);

            entity.Property(br => br.StudentName)
                  .IsRequired()
                  .HasMaxLength(100);

            // Default BorrowDate = current date/time (SQL Server)
            entity.Property(br => br.BorrowDate)
                  .HasDefaultValueSql("GETDATE()");

            // IsReturned is computed — ignore it
            entity.Ignore(br => br.IsReturned);

            // FK → Book
            entity.HasOne(br => br.Book)
                  .WithMany(b => b.BorrowRecords)
                  .HasForeignKey(br => br.BookId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
