using Books.Application.Models;

namespace Books.Application.Services
{
    public interface IBooksService
    {
        Task<bool> CreateAsync(Book book);

        Task<Book?> GetByIdAsync(Guid id);

        Task<IEnumerable<Book>> GetAllAsync();

        Task<Book?> UpdateAsync(Book book);

        Task<bool> DeleteAsync(Guid id);
    }
}
