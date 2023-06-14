using Asp.Versioning;
using Books.API.Auth;
using Books.Application.Services;
using Books.Contracts.Request;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(AuthConstants.TrustedMemberPolicyName)]
    public async Task<IActionResult> GetUserRatings(CancellationToken token)
    {
        var userId = HttpContext.GetUserId();
        var ratings = await _ratingService.GetRatingsForUserAsync(userId.Value!, token);
        // var ratingsResponse = ratings.MapToResponse();
        return Ok(ratings);

    }

    [Route("/api/v1/books/{bookId:guid}/ratings")]
    [HttpPut]
    [Authorize(AuthConstants.TrustedMemberPolicyName)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RateBook([FromRoute] Guid bookId, [FromBody] RateBookRequest request, CancellationToken token)
    {
        var userId = HttpContext.GetUserId();
        var result = await _ratingService.RateBookAsync(bookId, request.Rating, userId.Value!, token);
        return result ? Ok() : NotFound();
    }

    [Route("/api/v1/books/{bookId:guid}/ratings")]
    [HttpDelete]
    [Authorize(AuthConstants.AdminUserPolicyName)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid bookId, CancellationToken token)
    {
        var userId = HttpContext.GetUserId();
        var result = await _ratingService.DeleteRatingAsync(bookId, userId.Value!, token);
        return result ? Ok() : NotFound();
    }
}
