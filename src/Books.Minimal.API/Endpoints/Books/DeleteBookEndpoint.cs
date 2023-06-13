using Books.Application.Services;

namespace Books.Minimal.API.Endpoints.Books
{
    public static class DeleteBookEndpoint
    {
        public const string Name = "DeleteBook";

        public static IEndpointRouteBuilder MapDeleteBook(this IEndpointRouteBuilder app)
        {
            app.MapDelete("/api/v1/books/{id:guid}", async (
                Guid id,
                IBooksService booksService,
                CancellationToken token) =>
            {
                var deleted = await booksService.DeleteAsync(id, token);
                return !deleted ? Results.NotFound() : Results.NoContent();
            });

            return app;
        }

    }
}


