using Books.Application.Services;
using Books.Contracts.Request;
using Books.Minimal.API.Mapping;

namespace Books.Minimal.API.Endpoints.Books
{
    public static class GetAllBooksEndpoint
    {
        public const string Name = "GetAllBooks";

        public static IEndpointRouteBuilder MapGetAllBooks(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/v1/books", async (
                [AsParameters] GetAllBooksRequest request,
                IBooksService booksService,
                CancellationToken token) =>
            {
                var options = request.MapToOptions();
                var movies = await booksService.GetAllAsync(options, token);
                var response = movies.MapToBooksResponse();
                return TypedResults.Ok(response);
            });

            return app;
        }
    }
}


