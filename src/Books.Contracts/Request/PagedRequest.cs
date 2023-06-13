namespace Books.Contracts.Request;

public class PagedRequest
{
    private const int DefaultPage = 1;
    private const int DefaultPageSize = 10;

    public required int Page { get; init; } = DefaultPage;

    public required int PageSize { get; init; } = DefaultPageSize;
}
