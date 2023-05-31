using Books.Application.Database;
using Books.Application.Models;
using Dapper;

namespace Books.Application.Repositories;

public sealed class RatingRepository : IRatingRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public RatingRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<bool> RateBookAsync(Guid bookId, int rating, Guid userId, CancellationToken token = default)
    {
        using var conn = await _dbConnectionFactory.CreateConnectionAsync(token);
        var result = await conn.ExecuteAsync(new CommandDefinition("""
            INSERT INTO ratings(userid, bookid, rating) 
                VALUES (@userId, @bookId, @rating)
            ON CONFLICT (userid, bookid) DO UPDATE 
                SET rating = @rating
            """, new { userId, bookId, rating }, cancellationToken: token));

        return result > 0;

    }
    public Task<float?> GetRatingAsync(Guid bookId, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
    public Task<(float? Rating, int? UserRating)> GetRatingAsync(Guid bookId, Guid userId, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
    public Task<bool> DeleteRatingAsync(Guid bookId, Guid userId, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
    public Task<IEnumerable<BookRating>> GetRatingsForUserAsync(Guid userId, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}

