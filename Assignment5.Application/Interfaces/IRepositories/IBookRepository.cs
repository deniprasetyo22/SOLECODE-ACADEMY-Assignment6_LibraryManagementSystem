using Assignment5.Application.DTOs;
using Assignment5.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment5.Application.Interfaces.IRepositories
{
    public interface IBookRepository
    {
        Task<Book> AddBook(Book book);
        Task<IEnumerable<Book>> GetAllBooks();
        Task<Book> GetBookById(int bookId);
        Task<bool> UpdateBook(Book book);
        Task<bool> DeleteBook(int bookId);
    }
}
