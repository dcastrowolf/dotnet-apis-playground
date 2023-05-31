using Books.Application.Models;
using Books.Application.Repositories;
using FluentValidation;
using FluentValidation.Results;

namespace Books.Application.Services
{
    public sealed class RatingService : IRatingService
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly IBooksRepository _booksRepository;

        public RatingService(IRatingRepository ratingRepository, IBooksRepository booksRepository)
        {
            _ratingRepository = ratingRepository;
            _booksRepository = booksRepository;
        }

        public Task<bool> DeleteRatingAsync(Guid bookId, Guid userId, CancellationToken token = default)
        {
            return _ratingRepository.DeleteRatingAsync(bookId, userId, token);
        }

        public Task<IEnumerable<BookRating>> GetRatingsForUserAsync(Guid userId, CancellationToken token = default)
        {
            return _ratingRepository.GetRatingsForUserAsync(userId, token);
        }

        public async Task<bool> RateBookAsync(Guid bookId, int rating, Guid userId, CancellationToken token = default)
        {
            if (rating is <= 0 or > 5)
            {
                throw new ValidationException(
                    new[] {
                        new ValidationFailure
                        {
                            PropertyName = "Rating",
                            ErrorMessage = "Rating must be between 1 and 5",
                        }
                    });
            }

            var bookExists = await _booksRepository.ExistsByIdAsync(bookId, token);

            return bookExists && await _ratingRepository.RateBookAsync(bookId, rating, userId, token);
        }
    }
}


