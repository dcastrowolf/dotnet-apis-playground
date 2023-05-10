using Books.Application.Models;
using Books.Application.Repositories;
using FluentValidation;

namespace Books.Application.Validators
{
    public sealed class BookValidator : AbstractValidator<Book>
    {
        private readonly IBooksRepository _booksRepository;
        public BookValidator(IBooksRepository booksRepository)
        {
            _booksRepository = booksRepository;
            RuleFor(x => x.Id)
                .NotEmpty();

            RuleFor(x => x.Genres)
                .NotEmpty();

            RuleFor(x => x.Title)
                .NotEmpty();

            RuleFor(x => x.Overview)
                .NotEmpty();

            RuleFor(x => x.YearOfRelease)
                .LessThanOrEqualTo(DateTime.UtcNow.Year);
        }
    }
}
