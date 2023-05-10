using Books.API.Mapping;
using Books.Application.Services;
using Books.Contracts.Request;
using Microsoft.AspNetCore.Mvc;

namespace Books.API.Controllers;

[ApiController]
[Route("/api/v1/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBooksService _booksService;
    public BooksController(IBooksService booksService)
    {
        _booksService = booksService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBookRequest request)
    {
        var book = request.MapToBook();
        await _booksService.CreateAsync(book);

        var response = book.MapToBookResponse();
        return CreatedAtAction(nameof(Get), new { id = response.Id }, response);
    }

    [HttpGet("{{id:guid}}")]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var book = await _booksService.GetByIdAsync(id);
        if (book is null)
        {
            return NotFound();
        }
        var response = book.MapToBookResponse();
        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var movies = await _booksService.GetAllAsync();
        var response = movies.MapToBooksResponse();
        return Ok(response);
    }

    [HttpPut("{{id:guid}}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateBookRequest request)
    {
        var book = request.MapToBook(id);
        var updated = await _booksService.UpdateAsync(book);
        if (updated is not null)
        {
            return NotFound();
        }
        var response = book.MapToBookResponse();
        return Ok(response);
    }

    [HttpDelete("{{id:guid}}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        bool deleted = await _booksService.DeleteAsync(id);
        return !deleted ? NotFound() : NoContent();
    }

}
