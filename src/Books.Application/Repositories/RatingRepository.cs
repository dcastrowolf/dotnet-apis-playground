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

    public async Task<float?> GetRatingAsync(Guid bookId, CancellationToken token = default)
    {
        using var conn = await _dbConnectionFactory.CreateConnectionAsync(token);
        return await conn.QuerySingleOrDefaultAsync<float?>(new CommandDefinition("""
            SELECT round(avg(r.rating), 1) FROM ratings r
            WHERE bookid = @bookId
            """, new { bookId }, cancellationToken: token));
    }

    public async Task<(float? Rating, int? UserRating)> GetRatingAsync(Guid bookId, Guid userId, CancellationToken token = default)
    {
        using var conn = await _dbConnectionFactory.CreateConnectionAsync(token);
        return await conn.QuerySingleOrDefaultAsync<(float?, int?)>(new CommandDefinition("""
            SELECT round(avg(rating), 1), 
                   (SELECT rating 
                    FROM ratings 
                    WHERE bookid = @bookId 
                      AND userid = @userId
                    LIMIT 1) 
            FROM ratings
            WHERE bookid = @bookId
            """, new { bookId, userId }, cancellationToken: token));
    }

    public async Task<bool> DeleteRatingAsync(Guid bookId, Guid userId, CancellationToken token = default)
    {
        using var conn = await _dbConnectionFactory.CreateConnectionAsync(token);
        var result = await conn.ExecuteAsync(new CommandDefinition("""
            DELETE FROM ratings
            WHERE bookid = @bookId
                AND userid = @userId
            """, new { userId, bookId }, cancellationToken: token));

        return result > 0;
    }

    public async Task<IEnumerable<BookRating>> GetRatingsForUserAsync(Guid userId, CancellationToken token = default)
    {
        using var conn = await _dbConnectionFactory.CreateConnectionAsync(token);
        return await conn.QueryAsync<BookRating>(new CommandDefinition("""
            SELECT r.rating, r.bookid, b.slug
            FROM ratings r
            INNER JOIN books b on r.bookid = b.id
            WHERE userid = @userId
            """, new { userId }, cancellationToken: token));
    }
}

