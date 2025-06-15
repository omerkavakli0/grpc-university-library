using Grpc.Core;
using System.Collections.Concurrent;
using University;

namespace Server.Services
{
    public class BookServiceImpl : BookService.BookServiceBase
    {
        public static readonly ConcurrentDictionary<string, Book> Books = new();

        public override Task<ListBooksResponse> ListBooks(ListBooksRequest request, ServerCallContext context)
        {
            var response = new ListBooksResponse();
            response.Books.AddRange(Books.Values);
            return Task.FromResult(response);
        }

        public override Task<GetBookResponse> GetBook(GetBookRequest request, ServerCallContext context)
        {
            if (!Books.TryGetValue(request.Id, out var book))
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Book not found"));
            }
            return Task.FromResult(new GetBookResponse { Book = book });
        }

        public override Task<CreateBookResponse> CreateBook(CreateBookRequest request, ServerCallContext context)
        {
            var book = request.Book;
            if (string.IsNullOrWhiteSpace(book.Id))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Book Id is required"));
            }
            if (Books.ContainsKey(book.Id))
            {
                throw new RpcException(new Status(StatusCode.AlreadyExists, "Book already exists"));
            }
            Books[book.Id] = book;
            return Task.FromResult(new CreateBookResponse { Book = book });
        }

        public override Task<UpdateBookResponse> UpdateBook(UpdateBookRequest request, ServerCallContext context)
        {
            var book = request.Book;
            if (string.IsNullOrWhiteSpace(book.Id))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Book Id is required"));
            }
            if (!Books.ContainsKey(book.Id))
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Book not found"));
            }
            Books[book.Id] = book;
            return Task.FromResult(new UpdateBookResponse { Book = book });
        }

        public override Task<DeleteBookResponse> DeleteBook(DeleteBookRequest request, ServerCallContext context)
        {
            if (string.IsNullOrWhiteSpace(request.Id))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Book Id is required"));
            }
            var success = Books.TryRemove(request.Id, out _);
            if (!success)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Book not found"));
            }
            return Task.FromResult(new DeleteBookResponse { Success = success });
        }
    }
}
