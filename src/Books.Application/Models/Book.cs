namespace Books.Application.Models
{
    public sealed class Book
    {
        public required Guid Id { get; init; }

        public required string Title { get; init; }

        public required string Overview { get; init; }

        public required int YearOfRelease { get; set; }

        public float? Rating { get; set; }

        public int? UserRating { get; set; }

        public required List<string> Genres { get; init; } = new();
    }
}
