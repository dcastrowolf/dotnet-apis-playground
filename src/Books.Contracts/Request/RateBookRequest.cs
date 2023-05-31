namespace Books.Contracts.Request;

public sealed class RateBookRequest
{
    public required int Rating { get; init; }
}
