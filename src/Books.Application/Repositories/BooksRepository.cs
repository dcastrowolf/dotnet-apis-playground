﻿using Books.Application.Database;
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

        public async Task<bool> CreateAsync(Book book, CancellationToken token = default)
        {
            using var conn = await _dbConnectionFactory.CreateConnectionAsync(token);
            using var transaction = conn.BeginTransaction();

            var result = await conn.ExecuteAsync(new CommandDefinition(
            """
                INSERT INTO books (id, title, overview, yearofrelease)
                VALUES (@Id, @Title, @Overview, @YearOfRelease)
            """, book, cancellationToken: token
            ));

            if (result > 0)
            {
                foreach (var genre in book.Genres)
                {
                    await conn.ExecuteAsync(new CommandDefinition("""
                        INSERT INTO genres (bookId, name)
                        VALUES (@BookId, @Name)
                    """, new { BookId = book.Id, Name = genre }, cancellationToken: token));
                }
            }

            transaction.Commit();

            return result > 0;
        }

        public async Task<Book?> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            using var conn = await _dbConnectionFactory.CreateConnectionAsync(token);
            var book = await conn.QuerySingleOrDefaultAsync<Book>(new CommandDefinition("""
                SELECT * FROM books WHERE id = @id
            """, new { id }, cancellationToken: token));

            if (book is null)
            {
                return null;
            }

            var genres = await conn.QueryAsync<string>(new CommandDefinition("""
                SELECT name FROM genres WHERE bookid = @id
            """, new { id }, cancellationToken: token));

            foreach (var genre in genres)
            {
                book.Genres.Add(genre);
            }
            return book;
        }

        public async Task<IEnumerable<Book>> GetAllAsync(GetAllBooksOptions options, CancellationToken token = default)
        {
            using var conn = await _dbConnectionFactory.CreateConnectionAsync(token);

            var result = await conn.QueryAsync(new CommandDefinition("""
                SELECT b.*, string_agg(g.name,',') as genres
                    round(avg(r.rating), 1) as rating,
                    myr.rating as userrating
                FROM books b 
                    LEFT JOIN genres g on b.id = g.bookid
                    LEFT JOIN ratings r on b.id = g.bookid
                    LEFT JOIN ratings myr on b.id = g.bookid
                        AND myr.userid = @userId
                WHERE (@title is null OR b.title LIKE ('%' || @title || '%'))
                    AND (@yearofrelease is null OR b.yearofrelease = @yearofrelease)
                GROUP BY b.id, userrating
                LIMIT @pageSize
                OFFSET @pageOffset
            """,
            new
            {
                userId = options.UserId,
                title = options.Title,
                yearofrelease = options.YearOfRelease,
                pageSize = options.PageSize,
                pageOffset = (options.Page - 1) * options.PageSize
            },
            cancellationToken: token));

            return result.Select(x => new Book
            {
                Id = x.id,
                Title = x.title,
                Overview = x.overview,
                YearOfRelease = x.yearofrelease,
                Genres = Enumerable.ToList(x.genres.Split(','))
            });
        }

        public async Task<bool> UpdateAsync(Book book, CancellationToken token = default)
        {
            using var conn = await _dbConnectionFactory.CreateConnectionAsync(token);
            using var transaction = conn.BeginTransaction();

            await conn.ExecuteAsync(new CommandDefinition("""
                DELETE FROM genres WHERE bookid = @id
            """, new { id = book.Id }, cancellationToken: token));

            foreach (var genre in book.Genres)
            {
                await conn.ExecuteAsync(new CommandDefinition("""
                INSERT INTO genres (bookId, name)
                values (@BookId, @Name)
            """, new { BookId = book.Id, Name = genre }, cancellationToken: token));
            }

            var result = await conn.ExecuteAsync(new CommandDefinition("""
                UPDATE books SET overview = @Overview, title = @Title, yearofrelease = @YearOfRelease
                WHERE id = @Id
            """, book, cancellationToken: token));

            transaction.Commit();

            return result > 0;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken token = default)
        {
            using var conn = await _dbConnectionFactory.CreateConnectionAsync(token);
            using var transaction = conn.BeginTransaction();

            await conn.ExecuteAsync(new CommandDefinition("""
                DELETE FROM genres WHERE bookid = @id
            """, new { id }, cancellationToken: token));

            var result = await conn.ExecuteAsync(new CommandDefinition("""
                DELETE FROM books WHERE id = @id
            """, new { id }, cancellationToken: token));

            transaction.Commit();

            return result > 0;
        }

        public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default)
        {
            using var conn = await _dbConnectionFactory.CreateConnectionAsync(token);
            return await conn.ExecuteScalarAsync<bool>(new CommandDefinition(
                """
                SELECT COUNT(1) FROM books WHERE id = @id
            """, new { id }, cancellationToken: token
            ));
        }
    }
}
