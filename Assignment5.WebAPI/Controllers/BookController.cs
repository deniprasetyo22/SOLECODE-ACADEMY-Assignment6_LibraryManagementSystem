using Asp.Versioning;
using Assignment5.Application.DTOs;
using Assignment5.Application.Interfaces.IService;
using Assignment5.Domain.Models;
using Assignment6.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Assignment5.WebAPI.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[Controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;
        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        /// <summary>
        /// Adds a new book to the system.
        /// </summary>
        /// <remarks>
        /// Ensure that the book data is not null and that the book details are valid.
        /// Validate that no book with the same ISBN or Title already exists.
        ///
        /// Sample request:
        ///
        ///     POST /api/Book
        ///     {
        ///         "category":"Education",
        ///         "title":"Biology",
        ///         "ISBN":"AAA11111",
        ///         "publisher":"Gramedia",
        ///         "description":"All about biology",
        ///         "location":"A1",
        ///         "price":30000,
        ///         "totalBook":5
        ///     }
        /// </remarks>
        /// <param name="book">The book to be added.</param>
        /// <returns>
        /// If the book is successfully added, returns a 200 OK response with a success message and the added book details.
        /// If a book with the same ISBN or Title already exists, returns a 400 Bad Request response with an error message.
        /// If the input data is invalid, returns a 400 Bad Request response with an error message.
        /// </returns>
        [Authorize(Roles = "Library Manager")]
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> AddBook([FromBody] Book book)
        {
            if (book == null)
            {
                return BadRequest("Invalid input data. Please check the book details.");
            }

            try
            {
                var addedBook = await _bookService.AddBook(book);
                return Ok(new { Message = "Book added successfully.", Book = addedBook });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves a list of all books in the system.
        /// </summary>
        /// <remarks>
        /// This endpoint retrieves all books available in the system.
        ///
        /// Sample request:
        ///
        ///     GET /api/Book
        /// </remarks>
        /// <returns>A list of books.</returns>
        [Authorize(Roles = "Library Manager, Librarian, Library User")]
        [HttpGet("noPages")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetAllBooksNoPages()
        {
            try
            {
                var books = await _bookService.GetAllBooksNoPages();
                return Ok(books);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves a book by its ID.
        /// </summary>
        /// <remarks>
        /// Ensure that the provided book ID is valid (greater than zero).
        ///
        /// Sample request:
        ///
        ///     GET /api/Book/5
        /// </remarks>
        /// <param name="bookId">The ID of the book to be retrieved.</param>
        /// <returns>Book details if found, otherwise an error message.</returns>
        [Authorize(Roles = "Library Manager, Librarian, Library User")]
        [HttpGet("{bookId}")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetBookById(int bookId)
        {
            if (bookId <= 0)
            {
                return BadRequest(new { message = "Book ID must be greater than zero." });
            }

            try
            {
                var book = await _bookService.GetBookById(bookId);
                if (book == null)
                {
                    return NotFound(new { message = $"Book with ID {bookId} not found." });
                }
                return Ok(book);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Updates an existing book by its ID.
        /// </summary>
        /// <remarks>
        /// Ensure that the book data is not null and that all required fields are provided.
        /// Validate that the book's name and ISBN are unique.
        ///
        /// Sample request:
        ///
        ///     PUT /api/Book/5
        ///     {
        ///        "Title": "Updated Book Title",
        ///        "Author": "Updated Author",
        ///        "PublicationYear": 1925,
        ///        "ISBN": "9780743273565"
        ///     }
        /// </remarks>
        /// <param name="bookId">The ID of the book to be updated.</param>
        /// <param name="book">The updated book details.</param>
        /// <returns>Success message if the book is updated successfully or an error message if validation fails.</returns>
        [Authorize(Roles = "Library Manager")]
        [HttpPut("{bookId}")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> UpdateBook(int bookId, [FromBody] Book book)
        {
            if (book == null)
            {
                return BadRequest(new { message = "Invalid input data. Please check the book details." });
            }

            try
            {
                var success = await _bookService.UpdateBook(bookId, book);
                if (!success)
                {
                    return BadRequest(new { message = "Failed to update the book. Please check the details." });
                }
                return Ok(new { message = "Book updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Deletes a book by its ID.
        /// </summary>
        /// <remarks>
        /// Ensure that the book ID provided is valid.
        ///
        /// Sample request:
        ///
        ///     DELETE /api/Book/5
        /// </remarks>
        /// <param name="bookId">The ID of the book to be deleted.</param>
        /// <returns>Success message if the book is deleted successfully or an error message if the book is not found.</returns>
        [Authorize(Roles = "Library Manager")]
        [HttpDelete("{bookId}")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> DeleteBook(int bookId)
        {
            try
            {
                var success = await _bookService.RemoveBook(bookId);
                if (!success)
                {
                    return NotFound(new { message = $"Book with ID {bookId} not found." });
                }
                return Ok(new { message = "Book deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Searches for books based on the provided search criteria and pagination.
        /// </summary>
        /// <remarks>
        /// This endpoint allows you to search for books by various criteria and paginate the results. Ensure the query parameters are provided correctly.
        ///
        /// Sample request:
        /// 
        ///     GET /api/Book/search
        /// </remarks>
        /// <param name="query">Search criteria including title, author, ISBN, and category. These parameters are optional.</param>
        /// <param name="pagination">Pagination details including page number and page size.</param>
        /// <returns>A list of books that match the search criteria, along with pagination details.</returns>
        [Authorize(Roles = "Library Manager, Librarian, Library User")]
        [HttpGet("search")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> SearchBooks([FromQuery] QueryObject query)
        {
            try
            {
                var result = await _bookService.SearchBooksAsync(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
