using Grpc.Core;
using System.Collections.Concurrent;
using University;

namespace Server.Services
{
    public class LoanServiceImpl : LoanService.LoanServiceBase
    {
        public static readonly ConcurrentDictionary<string, Loan> Loans = new();

        public override Task<ListLoansResponse> ListLoans(ListLoansRequest request, ServerCallContext context)
        {
            var response = new ListLoansResponse();
            response.Loans.AddRange(Loans.Values);
            return Task.FromResult(response);
        }

        public override Task<GetLoanResponse> GetLoan(GetLoanRequest request, ServerCallContext context)
        {
            if (string.IsNullOrWhiteSpace(request.Id))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Loan Id is required"));
            }
            if (!Loans.TryGetValue(request.Id, out var loan))
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Loan not found"));
            }
            return Task.FromResult(new GetLoanResponse { Loan = loan });
        }

        public override Task<BorrowBookResponse> BorrowBook(BorrowBookRequest request, ServerCallContext context)
        {
            if (string.IsNullOrWhiteSpace(request.StudentId) || string.IsNullOrWhiteSpace(request.BookId))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "StudentId and BookId are required"));
            }
            // Aynı öğrenci ve kitap için aktif ödünç var mı kontrolü
            var alreadyLoaned = Loans.Values.Any(l => l.StudentId == request.StudentId && l.BookId == request.BookId && l.Status == LoanStatus.Ongoing);
            if (alreadyLoaned)
            {
                throw new RpcException(new Status(StatusCode.AlreadyExists, "This book is already borrowed by the student and not yet returned."));
            }
            var loan = new Loan
            {
                Id = Guid.NewGuid().ToString(),
                StudentId = request.StudentId,
                BookId = request.BookId,
                LoanDate = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                ReturnDate = "",
                Status = LoanStatus.Ongoing
            };
            Loans[loan.Id] = loan;
            return Task.FromResult(new BorrowBookResponse { Loan = loan });
        }

        public override Task<ReturnBookResponse> ReturnBook(ReturnBookRequest request, ServerCallContext context)
        {
            if (string.IsNullOrWhiteSpace(request.LoanId))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "LoanId is required"));
            }
            if (!Loans.TryGetValue(request.LoanId, out var loan))
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Loan not found"));
            }
            if (loan.Status == LoanStatus.Returned)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Loan is already returned"));
            }
            loan.ReturnDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
            loan.Status = LoanStatus.Returned;
            Loans[loan.Id] = loan;
            return Task.FromResult(new ReturnBookResponse { Loan = loan });
        }
    }
}
