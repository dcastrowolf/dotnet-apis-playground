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

        public async Task<bool> CreateAsync(Book book)
        {
            await _bookValidator.ValidateAndThrowAsync(book);
            return await _booksRepository.CreateAsync(book);
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            return _booksRepository.DeleteAsync(id);
        }

        public Task<IEnumerable<Book>> GetAllAsync()
        {
            return _booksRepository.GetAllAsync();
        }

        public Task<Book?> GetByIdAsync(Guid id)
        {
            return _booksRepository.GetByIdAsync(id);
        }

        public async Task<Book?> UpdateAsync(Book book)
        {
            await _bookValidator.ValidateAndThrowAsync(book);
            bool bookExists = await _booksRepository.ExistsByIdAsync(book.Id);
            if (!bookExists)
            {
                return null;
            }
            await _booksRepository.UpdateAsync(book);
            return book;
        }
    }
}
