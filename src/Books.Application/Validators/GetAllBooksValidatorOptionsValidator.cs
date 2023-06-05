using Books.Application.Models;
using FluentValidation;

namespace Books.Application.Validators
{
    public sealed class GetAllBooksValidatorOptionsValidator : AbstractValidator<GetAllBooksOptions>
    {
        public GetAllBooksValidatorOptionsValidator()
        {
            RuleFor(x => x.YearOfRelease)
                .LessThanOrEqualTo(DateTime.UtcNow.Year);

            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 25)
                .WithMessage("You can get between 1 and 25 books per page");
        }
    }
}

