using Books.Minimal.API.Endpoints.Books;

namespace Books.Minimal.API.Endpoints
{
    public static class EndpointsExtensions
    {
        public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder app)
        {
            // TODO: Add the enpoints of books and ratings when finished
            app.MapBookEndpoints();
            return app;
        }
    }
}
