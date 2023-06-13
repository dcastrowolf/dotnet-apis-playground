using Books.Application.Services;
using Books.Contracts.Request;

namespace Books.Minimal.API.Endpoints.Ratings
{
    public static class RateBookEndpoint
    {
        public const string Name = "RateBook";

        public static IEndpointRouteBuilder MapRateBook(this IEndpointRouteBuilder app)
        {
            app.MapPut("/api/v1/books/{bookId:Guid}/ratings", async (
                    Guid bookId,
                    RateBookRequest request,
                    IRatingService ratingService,
                    CancellationToken token) =>
            {
                //TODO: Get userId from token
                var userId = Guid.NewGuid();
                var result = await ratingService.RateBookAsync(bookId, request.Rating, userId, token);
                return result ? Results.Ok() : Results.NotFound();
            });
            return app;
        }
    }
}
