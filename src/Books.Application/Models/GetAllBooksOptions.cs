namespace Books.Application.Models;

public sealed class GetAllBooksOptions
{
    public string? Title { get; set; }
    public int? YearOfRelease { get; set; }
    public Guid? UserId { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

