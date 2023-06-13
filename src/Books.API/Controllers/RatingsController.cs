using Asp.Versioning;
using Books.Application.Services;
using Books.Contracts.Request;
using Microsoft.AspNetCore.Mvc;

namespace Books.API.Controllers;

[ApiController]
[Route("/api/v{version:apiVersion}/ratings")]
[ApiVersion("1.0")]
public class RatingsController : ControllerBase
{
    private readonly IRatingService _ratingService;

    public RatingsController(IRatingService ratingService)
    {
        _ratingService = ratingService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserRatings(CancellationToken token)
    {
        //TODO: Get userId from token
        var userId = Guid.NewGuid();
        var ratings = await _ratingService.GetRatingsForUserAsync(userId, token);
        // var ratingsResponse = ratings.MapToResponse();
        return Ok(ratings);

    }

    [Route("/api/v1/books/{bookId:guid}/ratings")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RateBook([FromRoute] Guid bookId, [FromBody] RateBookRequest request, CancellationToken token)
    {
        //TODO: Get userId from token
        var userId = Guid.NewGuid();
        var result = await _ratingService.RateBookAsync(bookId, request.Rating, userId, token);
        return result ? Ok() : NotFound();
    }

    [Route("/api/v1/books/{bookId:guid}/ratings")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid bookId, CancellationToken token)
    {
        //TODO: Get userId from token
        var userId = Guid.NewGuid();
        var result = await _ratingService.DeleteRatingAsync(bookId, userId, token);
        return result ? Ok() : NotFound();

    }
}
