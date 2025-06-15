using Grpc.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

var app = builder.Build();

// Configure the HTTP request pipeline.
// app.MapGrpcService<GreeterService>();
app.MapGrpcService<Server.Services.BookServiceImpl>();
app.MapGrpcService<Server.Services.StudentServiceImpl>();
app.MapGrpcService<Server.Services.LoanServiceImpl>();
app.MapGrpcReflectionService();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Lifetime.ApplicationStarted.Register(() => {
    // BookService için örnek kayıt
    Server.Services.BookServiceImpl.Books.TryAdd("1000", new University.Book {
        Id = "1000",
        Title = "Olasiliksiz",
        Author = "Adam Fawer",
        Isbn = "978-3-16-148410-0",
        Publisher = "Ithaki Yayınları",
        PageCount = 712,
        Stock = 5
    });
    // StudentService için örnek kayıt
    Server.Services.StudentServiceImpl.Students.TryAdd("5000", new University.Student {
        Id = "5000",
        Name = "Omer Kavakli",
        StudentNumber = "12345678",
        Email = "omerkavakli@example.com",
        IsActive = true
    });
    // LoanService için örnek kayıt
    Server.Services.LoanServiceImpl.Loans.TryAdd("9000", new University.Loan {
        Id = "9000",
        StudentId = "5000",
        BookId = "1000",
        LoanDate = DateTime.UtcNow.ToString("yyyy-MM-dd"),
        ReturnDate = "",
        Status = University.LoanStatus.Ongoing
    });
});

app.Run();
