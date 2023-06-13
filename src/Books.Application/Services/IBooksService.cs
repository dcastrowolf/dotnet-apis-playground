using Books.Application.Models;

namespace Books.Application.Services
{
    public interface IBooksService
    {
        Task<bool> CreateAsync(Book book, CancellationToken token = default);

        Task<Book?> GetByIdAsync(Guid id, CancellationToken token = default);

        Task<IEnumerable<Book>> GetAllAsync(GetAllBooksOptions options, CancellationToken token = default);

        Task<Book?> UpdateAsync(Book book, CancellationToken token = default);

        Task<bool> DeleteAsync(Guid id, CancellationToken token = default);
    }
}
