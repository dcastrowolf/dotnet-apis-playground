using Books.Application.Models;
using Books.Application.Repositories;
using FluentValidation;

namespace Books.Application.Services
{
    public sealed class BooksService : IBooksService
    {
        private readonly IBooksRepository _booksRepository;
        private readonly IValidator<Book> _bookValidator;

        public BooksService(IBooksRepository booksRepository, IValidator<Book> bookValidator)
        {
            _booksRepository = booksRepository;
            _bookValidator = bookValidator;
        }

        public async Task<bool> CreateAsync(Book book, CancellationToken token = default)
        {
            await _bookValidator.ValidateAndThrowAsync(book, token);
            return await _booksRepository.CreateAsync(book, token);
        }

        public Task<bool> DeleteAsync(Guid id, CancellationToken token = default)
        {
            return _booksRepository.DeleteAsync(id, token);
        }

        public Task<IEnumerable<Book>> GetAllAsync(CancellationToken token = default)
        {
            return _booksRepository.GetAllAsync(token);
        }

        public Task<Book?> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            return _booksRepository.GetByIdAsync(id, token);
        }

        public async Task<Book?> UpdateAsync(Book book, CancellationToken token = default)
        {
            await _bookValidator.ValidateAndThrowAsync(book, token);
            var bookExists = await _booksRepository.ExistsByIdAsync(book.Id, token);
            if (!bookExists)
            {
                return null;
            }
            await _booksRepository.UpdateAsync(book, token);
            return book;
        }
    }
}
