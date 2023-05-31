using Books.API.Mapping;
using Books.Application.Services;
using Books.Contracts.Request;
using Microsoft.AspNetCore.Mvc;

namespace Books.API.Controllers;

[ApiController]
[Route("/api/v1/books")]
public class BooksController : ControllerBase
{
    private readonly IBooksService _booksService;
    public BooksController(IBooksService booksService)
    {
        _booksService = booksService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBookRequest request, CancellationToken token)
    {
        var book = request.MapToBook();
        await _booksService.CreateAsync(book, token);

        var response = book.MapToBookResponse();
        return CreatedAtAction(nameof(Get), new { id = response.Id }, response);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken token)
    {
        var book = await _booksService.GetByIdAsync(id, token);
        if (book is null)
        {
            return NotFound();
        }
        var response = book.MapToBookResponse();
        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken token)
    {
        var movies = await _booksService.GetAllAsync(token);
        var response = movies.MapToBooksResponse();
        return Ok(response);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateBookRequest request, CancellationToken token)
    {
        var book = request.MapToBook(id);
        var updated = await _booksService.UpdateAsync(book, token);
        if (updated is null)
        {
            return NotFound();
        }
        var response = book.MapToBookResponse();
        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken token)
    {
        var deleted = await _booksService.DeleteAsync(id, token);
        return !deleted ? NotFound() : NoContent();
    }
}
