using Books.Application.Database;
using Books.Application.Models;
using Dapper;

namespace Books.Application.Repositories
{
    public sealed class BooksRepository : IBooksRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public BooksRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<bool> CreateAsync(Book book)
        {
            using var conn = await _dbConnectionFactory.CreateConnectionAsync();
            using var transaction = conn.BeginTransaction();

            var result = await conn.ExecuteAsync(new CommandDefinition(
            """
                INSERT INTO books (id, title, overview, yearofrelease)
                VALUES (@Id, @Title, @Overview, @YearOfRelease)
            """, book
            ));

            if (result > 0)
            {
                foreach (var genre in book.Genres)
                {
                    await conn.ExecuteAsync("""
                        INSERT INTO genres (bookId, name)
                        VALUES (@BookId, @Name)
                    """, new { BookId = book.Id, Name = genre });
                }
            }

            transaction.Commit();

            return result > 0;
        }

        public async Task<Book?> GetByIdAsync(Guid id)
        {
            using var conn = await _dbConnectionFactory.CreateConnectionAsync();
            var book = await conn.QuerySingleOrDefaultAsync<Book>(new CommandDefinition("""
                SELECT * FROM books WHERE id = @id
            """, new { id }));

            if (book is null)
            {
                return null;
            }

            var genres = await conn.QueryAsync<string>(new CommandDefinition("""
                SELECT name FROM genres WHERE bookid = @id
            """, new { id }));

            foreach (var genre in genres)
            {
                book.Genres.Add(genre);
            }
            return book;
        }

        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            using var conn = await _dbConnectionFactory.CreateConnectionAsync();
            var result = await conn.QueryAsync(new CommandDefinition("""
                SELECT m.*, string_agg(g.name,',') as genres
                FROM books m left join genres g on m.id = g.bookid
                GROUP BY m.id
            """));

            return result.Select(x => new Book
            {
                Id = x.id,
                Title = x.title,
                Overview = x.overview,
                YearOfRelease = x.yearofrelease,
                Genres = Enumerable.ToList(x.genres.Split(','))
            });
        }

        public async Task<bool> UpdateAsync(Book book)
        {
            using var conn = await _dbConnectionFactory.CreateConnectionAsync();
            using var transaction = conn.BeginTransaction();

            await conn.ExecuteAsync(new CommandDefinition("""
                DELETE FROM genres WHERE bookid = @id
            """, new { id = book.Id }));

            foreach (var genre in book.Genres)
            {
                await conn.ExecuteAsync(new CommandDefinition("""
                INSERT INTO genres (bookId, name)
                values (@BookId, @Name)
            """, new { BookId = book.Id, Name = genre }));
            }

            var result = await conn.ExecuteAsync(new CommandDefinition("""
                UPDATE MOVIES SET overview = @Overview, title = @Title, yearofrelease = @YearOfRelease
                WHERE id = @Id
            """, book));

            transaction.Commit();

            return result > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            using var conn = await _dbConnectionFactory.CreateConnectionAsync();
            using var transaction = conn.BeginTransaction();

            await conn.ExecuteAsync(new CommandDefinition("""
                DELETE FROM genres WHERE bookid = @id
            """, new { id }));

            var result = await conn.ExecuteAsync(new CommandDefinition("""
                DELETE FROM books WHERE id = @id
            """, new { id }));

            transaction.Commit();

            return result > 0;
        }

        public async Task<bool> ExistsByIdAsync(Guid id)
        {
            using var conn = await _dbConnectionFactory.CreateConnectionAsync();
            return await conn.ExecuteScalarAsync<bool>(new CommandDefinition(
                """
                SELECT COUNT(1) FROM books WHERE id = @id
            """, new { id }
            ));
        }
    }
}
