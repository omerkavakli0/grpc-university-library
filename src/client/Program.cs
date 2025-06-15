using System;
using System.Threading.Tasks;
using Grpc.Net.Client;
using University;

class Program
{
    static async Task Main(string[] args)
    {
        using var channel = GrpcChannel.ForAddress("http://localhost:5038");
        var bookServiceClient = new BookService.BookServiceClient(channel);

        // Create a new book
        var createBookResponse = await bookServiceClient.CreateBookAsync(new CreateBookRequest
        {
            Book = new Book
            {
                Id = Guid.NewGuid().ToString(),
                Title = "Olasiliksiz",
                Author = "Adam Fawer",
                Isbn = "978-3-16-148410-0",
                Publisher = "Ithaki Yayınları",
                PageCount = 712,
                Stock = 5
            }
        });
        Console.WriteLine($"Created Book: {createBookResponse.Book.Title}");

        // List all books
        var listResponse = await bookServiceClient.ListBooksAsync(new ListBooksRequest());
        foreach (var book in listResponse.Books)
        {
            Console.WriteLine($"Book: {book.Title} by {book.Author}");
        }

        // StudentService örnekleri
        var studentServiceClient = new StudentService.StudentServiceClient(channel);
        // Create a new student
        var createStudentResponse = await studentServiceClient.CreateStudentAsync(new CreateStudentRequest
        {
            Student = new Student
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Omer Kavakli",
                StudentNumber = "12345678",
                Email = "omerkavakli@example.com",
                IsActive = true
            }
        });
        Console.WriteLine($"Created Student: {createStudentResponse.Student.Name}");

        var createStudentResponse2 = await studentServiceClient.CreateStudentAsync(new CreateStudentRequest
        {
            Student = new Student
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Halil Ozkan",
                StudentNumber = "87654321",
                Email = "halilozkan@example.com",
                IsActive = false
            }
        });
        Console.WriteLine($"Created Student: {createStudentResponse2.Student.Name}");


        // List all students
        var studentListResponse = await studentServiceClient.ListStudentsAsync(new ListStudentsRequest());
        foreach (var student in studentListResponse.Students)
        {
            Console.WriteLine($"Student: {student.Name} ({student.StudentNumber})");
        }

        // LoanService örnekleri
        var loanServiceClient = new LoanService.LoanServiceClient(channel);
        // Borrow a book
        var borrowResponse = await loanServiceClient.BorrowBookAsync(new BorrowBookRequest
        {
            StudentId = createStudentResponse.Student.Id,
            BookId = createBookResponse.Book.Id,
        });
        Console.WriteLine($"Loan created: {borrowResponse.Loan.Id} - Status: {borrowResponse.Loan.Status}");

        // List all loans
        var loanListResponse = await loanServiceClient.ListLoansAsync(new ListLoansRequest());
        foreach (var loan in loanListResponse.Loans)
        {
            Console.WriteLine($"Loan: {loan.Id} | Student: {loan.StudentId} | Book: {loan.BookId} | Status: {loan.Status}");
        }

        // BookService: GetBook
        var getBookResponse = await bookServiceClient.GetBookAsync(new GetBookRequest { Id = createBookResponse.Book.Id });
        Console.WriteLine($"GetBook: {getBookResponse.Book.Title}");

        // BookService: UpdateBook
        var updateBookResponse = await bookServiceClient.UpdateBookAsync(new UpdateBookRequest
        {
            Book = new Book
            {
                Id = createBookResponse.Book.Id,
                Title = "Olasiliksiz (Güncel)",
                Author = "Adam Fawer",
                Isbn = createBookResponse.Book.Isbn,
                Publisher = createBookResponse.Book.Publisher,
                PageCount = 720,
                Stock = 4
            }
        });
        Console.WriteLine($"Updated Book: {updateBookResponse.Book.Title}");

        // BookService: DeleteBook
        var deleteBookResponse = await bookServiceClient.DeleteBookAsync(new DeleteBookRequest { Id = createBookResponse.Book.Id });
        Console.WriteLine($"Deleted Book Success: {deleteBookResponse.Success}");

        // StudentService: GetStudent
        var getStudentResponse = await studentServiceClient.GetStudentAsync(new GetStudentRequest { Id = createStudentResponse.Student.Id });
        Console.WriteLine($"GetStudent: {getStudentResponse.Student.Name}");

        // StudentService: UpdateStudent
        var updateStudentResponse = await studentServiceClient.UpdateStudentAsync(new UpdateStudentRequest
        {
            Student = new Student
            {
                Id = createStudentResponse.Student.Id,
                Name = "Omer Kavakli (Güncel)",
                StudentNumber = createStudentResponse.Student.StudentNumber,
                Email = createStudentResponse.Student.Email,
                IsActive = false
            }
        });
        Console.WriteLine($"Updated Student: {updateStudentResponse.Student.Name}");

        // StudentService: DeleteStudent
        var deleteStudentResponse = await studentServiceClient.DeleteStudentAsync(new DeleteStudentRequest { Id = createStudentResponse.Student.Id });
        Console.WriteLine($"Deleted Student Success: {deleteStudentResponse.Success}");

        // LoanService: GetLoan
        var getLoanResponse = await loanServiceClient.GetLoanAsync(new GetLoanRequest { Id = borrowResponse.Loan.Id });
        Console.WriteLine($"GetLoan: {getLoanResponse.Loan.Id} | Status: {getLoanResponse.Loan.Status}");

        // LoanService: ReturnBook
        var returnBookResponse = await loanServiceClient.ReturnBookAsync(new ReturnBookRequest { LoanId = borrowResponse.Loan.Id });
        Console.WriteLine($"Returned Loan: {returnBookResponse.Loan.Id} | Status: {returnBookResponse.Loan.Status}");
    }
}
