using LibraryManagement.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Web.Data;

public class DataSeeder
{
    private readonly LibraryContext _context;

    public DataSeeder(LibraryContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        if (!await _context.Books.AnyAsync())
        {
            var books = new List<Book>
            {
                new()
                {
                    Title = "Clean Code",
                    Author = "Robert C. Martin",
                    Category = "Software Engineering",
                    Isbn = "9780132350884",
                    TotalCopies = 6,
                    AvailableCopies = 6,
                    CoverUrl = "https://images-na.ssl-images-amazon.com/images/I/41jEbK-jG+L.jpg",
                    Description = "A handbook of agile software craftsmanship."
                },
                new()
                {
                    Title = "The Pragmatic Programmer",
                    Author = "Andrew Hunt, David Thomas",
                    Category = "Software Engineering",
                    Isbn = "9780201616224",
                    TotalCopies = 4,
                    AvailableCopies = 4,
                    CoverUrl = "https://images-na.ssl-images-amazon.com/images/I/41uPjEenkFL._SX380_BO1,204,203,200_.jpg",
                    Description = "Journey to mastery for modern programmers."
                },
                new()
                {
                    Title = "Atomic Habits",
                    Author = "James Clear",
                    Category = "Self-help",
                    Isbn = "9780735211292",
                    TotalCopies = 8,
                    AvailableCopies = 8,
                    CoverUrl = "https://images-na.ssl-images-amazon.com/images/I/513Y5o-DYtL.jpg",
                    Description = "Tiny changes lead to remarkable results."
                }
            };
            _context.Books.AddRange(books);
        }

        if (!await _context.ApplicationUsers.AnyAsync())
        {
            var users = new List<ApplicationUser>
            {
                new()
                {
                    FullName = "Lan Nguyen",
                    Email = "lan.nguyen@example.com",
                    PhoneNumber = "+84 912345678",
                    Role = UserRole.Librarian,
                    Address = "123 Nguyen Trai, Ha Noi"
                },
                new()
                {
                    FullName = "Minh Tran",
                    Email = "minh.tran@example.com",
                    PhoneNumber = "+84 934567890",
                    Role = UserRole.Librarian,
                    Address = "45 Hai Ba Trung, Ha Noi"
                },
                new()
                {
                    FullName = "Quang Le",
                    Email = "quang.le@example.com",
                    PhoneNumber = "+84 987654321",
                    Role = UserRole.Reader,
                    Address = "78 Vo Thi Sau, Ho Chi Minh"
                },
                new()
                {
                    FullName = "Thu Hoang",
                    Email = "thu.hoang@example.com",
                    PhoneNumber = "+84 923456789",
                    Role = UserRole.Reader,
                    Address = "56 Dien Bien Phu, Da Nang"
                }
            };
            _context.ApplicationUsers.AddRange(users);
        }

        await _context.SaveChangesAsync();

        if (!await _context.Loans.AnyAsync())
        {
            var firstBook = await _context.Books.FirstAsync();
            var firstReader = await _context.ApplicationUsers.FirstAsync(u => u.Role == UserRole.Reader);

            var loan = new Loan
            {
                BookId = firstBook.Id,
                ApplicationUserId = firstReader.Id,
                BorrowedAt = DateTime.UtcNow.AddDays(-3),
                DueAt = DateTime.UtcNow.AddDays(11),
                Status = LoanStatus.Borrowed
            };

            firstBook.AvailableCopies = Math.Max(0, firstBook.AvailableCopies - 1);

            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();
        }
    }
}

