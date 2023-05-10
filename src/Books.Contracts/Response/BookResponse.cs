namespace Books.Contracts.Response
{
    public class BookResponse
    {
        public required Guid Id { get; init; }
        public required string Title { get; init; }
        public required string Overview { get; init; }
        public required int YearOfRelease { get; set; }
        public required IEnumerable<string> Genres { get; init; } = Enumerable.Empty<string>();
    }
}
