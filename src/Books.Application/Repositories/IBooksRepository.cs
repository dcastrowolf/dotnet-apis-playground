﻿using Books.Application.Models;

namespace Books.Application.Repositories
{
    public interface IBooksRepository
    {
        Task<bool> CreateAsync(Book book, CancellationToken token = default);

        Task<Book?> GetByIdAsync(Guid id, CancellationToken token = default);

        Task<IEnumerable<Book>> GetAllAsync(GetAllBooksOptions options, CancellationToken token = default);

        Task<bool> UpdateAsync(Book book, CancellationToken token = default);

        Task<bool> DeleteAsync(Guid id, CancellationToken token = default);

        Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default);
    }
}
