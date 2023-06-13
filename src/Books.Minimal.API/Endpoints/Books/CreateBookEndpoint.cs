using Books.Application.Services;
using Books.Contracts.Request;
using Books.Minimal.API.Mapping;

namespace Books.Minimal.API.Endpoints.Books
{
    public static class CreateBookEndpoint
    {
        public const string Name = "CreateBook";

        public static IEndpointRouteBuilder MapCreateBook(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/v1/books", async (
                CreateBookRequest request,
                IBooksService booksService,
                CancellationToken token) =>
            {
                var book = request.MapToBook();
                await booksService.CreateAsync(book, token);

                var response = book.MapToBookResponse();
                return TypedResults.CreatedAtRoute(response, "", new { id = response.Id });
            });
            return app;
        }
    }
}
