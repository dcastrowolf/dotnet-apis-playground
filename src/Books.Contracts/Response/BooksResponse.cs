namespace Books.Contracts.Response
{
    public class BooksResponse
    {
        public required IEnumerable<BookResponse> Items { get; init; } = Enumerable.Empty<BookResponse>();
    }
}
