using Dapper;

namespace Books.Application.Database
{
    public sealed class DbInitializer
    {
        private readonly IDbConnectionFactory _connectionFactory;
        public DbInitializer(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task InitializeAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();

            await connection.ExecuteAsync("""
                CREATE TABLE IF NOT EXISTS books (
                    id UUID primary key,
                    title TEXT not null,
                    overview TEXT not null,
                    yearofrelease INTEGER not null);
            """);

            await connection.ExecuteAsync("""
                CREATE TABLE IF NOT EXISTS genres (
                    bookId UUID REFERENCES books(id),
                    name TEXT NOT NULL);
            """);
        }

    }
}
