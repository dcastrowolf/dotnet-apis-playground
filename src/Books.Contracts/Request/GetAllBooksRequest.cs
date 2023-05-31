namespace Books.Contracts.Request;

public sealed class GetAllBooksRequest : PagedRequest
{
    public string? Title { get; init; }

    public int? YearOfRelease { get; init; }
}
