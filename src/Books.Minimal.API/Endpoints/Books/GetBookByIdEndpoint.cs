using Books.Application.Services;
using Books.Minimal.API.Mapping;

namespace Books.Minimal.API.Endpoints.Books
{
    public static class GetBookByIdEndpoint
    {
        public const string Name = "GetBookById";

        public static IEndpointRouteBuilder MapGetBookById(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/v1/books/{id:guid}", async (
                Guid id,
                IBooksService booksService,
                CancellationToken token) =>
            {
                var book = await booksService.GetByIdAsync(id, token);
                if (book is null)
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

