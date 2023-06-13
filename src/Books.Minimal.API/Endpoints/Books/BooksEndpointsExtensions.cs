namespace Books.Minimal.API.Endpoints.Books
{
    public static class BooksEndpointsExtensions
    {
        public static IEndpointRouteBuilder MapBookEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapCreateBook();
            app.MapGetAllBooks();
            app.MapGetBookById();
            app.MapUpdateBook();
            app.MapDeleteBook();

            return app;
        }
    }
}
