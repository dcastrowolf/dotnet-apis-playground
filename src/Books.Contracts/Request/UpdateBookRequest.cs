namespace Books.Contracts.Request
{
    public class UpdateBookRequest
    {
        public required string Title { get; init; }
        public required string Overview { get; init; }
        public required int YearOfRelease { get; init; }
        public required IEnumerable<string> Genres { get; init; } = Enumerable.Empty<string>();
    }
}
