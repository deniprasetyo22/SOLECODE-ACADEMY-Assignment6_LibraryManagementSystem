using Assignment5.Application.DTOs;
using Assignment5.Application.Interfaces.IRepositories;
using Assignment5.Domain.Models;
using Assignment5.Persistence.Context;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment5.Persistence.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly LibraryContext _context;

        public BookRepository(LibraryContext context)
        {
            _context = context;
        }

        public async Task<Book> AddBook(Book book)
        {
            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task<IEnumerable<Book>> GetAllBooks()
        {
            return await _context.Books.Where(b => !b.status.Contains("Deleted")).ToListAsync();
        }

        public async Task<Book> GetBookById(int bookId)
        {
            return await _context.Books.FirstOrDefaultAsync(b => b.bookId == bookId && !b.status.Contains("Deleted"));
        }

        public async Task<bool> UpdateBook(Book book)
        {
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteBook(int bookId)
        {
            var deleteBook = await _context.Books.FindAsync(bookId);

            if (deleteBook == null)
            {
                return false;
            }

            _context.Books.Remove(deleteBook);
            await _context.SaveChangesAsync();
            return true;
        }



    }
}
