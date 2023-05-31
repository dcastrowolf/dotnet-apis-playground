using Books.Application.Services;
using Books.Contracts.Request;
using Microsoft.AspNetCore.Mvc;

namespace Books.API.Controllers;

[ApiController]
[Route("/api/v1/ratings")]
public class RatingsController : ControllerBase
{
    private readonly IRatingService _ratingService;

    public RatingsController(IRatingService ratingService)
    {
        _ratingService = ratingService;
    }

    [HttpGet]
    public Task<IActionResult> GetUserRatings(CancellationToken token)
    {
        throw new NotImplementedException();
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
    public Task<IActionResult> Delete([FromRoute] Guid bookId, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}
