namespace Books.Application.Models;

public sealed class BookRating
{
    public required Guid BookId { get; init; }
    public required int Rating { get; init; }
}
