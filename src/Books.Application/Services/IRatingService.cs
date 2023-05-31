using Books.Application.Models;

namespace Books.Application.Services
{
    public interface IRatingService
    {
        Task<bool> RateBookAsync(Guid bookId, int rating, Guid userId, CancellationToken token = default);

        Task<bool> DeleteRatingAsync(Guid bookId, Guid userId, CancellationToken token = default);

        Task<IEnumerable<BookRating>> GetRatingsForUserAsync(Guid userId, CancellationToken token = default);
    }
}

