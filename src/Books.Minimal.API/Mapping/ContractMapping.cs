using Books.Application.Models;
using Books.Contracts.Request;
using Books.Contracts.Response;

namespace Books.Minimal.API.Mapping
{
    public static class ContractMapping
    {
        public static Book MapToBook(this CreateBookRequest request)
        {
            return new Book
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Overview = request.Overview,
                YearOfRelease = request.YearOfRelease,
                Genres = request.Genres.ToList()
            };
        }

        public static Book MapToBook(this UpdateBookRequest request, Guid id)
        {
            return new Book
            {
                Id = id,
                Title = request.Title,
                Overview = request.Overview,
                YearOfRelease = request.YearOfRelease,
                Genres = request.Genres.ToList()
            };
        }

        public static BookResponse MapToBookResponse(this Book book)
        {
            return new BookResponse
            {
                Id = book.Id,
                Title = book.Title,
                Overview = book.Overview,
                YearOfRelease = book.YearOfRelease,
                Genres = book.Genres.ToList()
            };
        }

        public static BooksResponse MapToBooksResponse(this IEnumerable<Book> books)
        {
            return new BooksResponse
            {
                Items = books.Select(MapToBookResponse)
            };
        }

        public static GetAllBooksOptions MapToOptions(this GetAllBooksRequest request)
        {
            return new GetAllBooksOptions
            {
                Title = request.Title,
                YearOfRelease = request.YearOfRelease,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}
