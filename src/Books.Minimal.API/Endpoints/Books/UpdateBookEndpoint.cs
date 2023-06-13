using Books.Application.Services;
using Books.Contracts.Request;
using Books.Minimal.API.Mapping;

namespace Books.Minimal.API.Endpoints.Books
{
    public static class UpdateBookEndpoint
    {
        public const string Name = "UpdateBook";

        public static IEndpointRouteBuilder MapUpdateBook(this IEndpointRouteBuilder app)
        {
            app.MapPut("/api/v1/books/{id:guid}", async (
                Guid id,
                UpdateBookRequest request,
                IBooksService booksService,
                CancellationToken token) =>
            {
                var book = request.MapToBook(id);
                var updated = await booksService.UpdateAsync(book, token);
                if (updated is null)
                {
                    return Results.NotFound();
                }
                var response = book.MapToBookResponse();
                return TypedResults.Ok(response);
            });

            return app;
        }
    }
}


