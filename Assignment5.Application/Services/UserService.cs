using Assignment5.Application.DTOs;
using Assignment5.Application.Interfaces.IRepositories;
using Assignment5.Application.Interfaces.IService;
using Assignment5.Domain.Models;
using Assignment6.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment5.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> AddUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("User data cannot be null");
            }
            var existingUser = await _userRepository.GetAllUsers();
            if (existingUser.Any(u => u.libraryCardNumber.Equals(user.libraryCardNumber, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"User with library card number {user.libraryCardNumber} already exists.");
            }
            return await _userRepository.AddUser(user);
        }

        public async Task<object> GetAllUsers(QueryObjectMember query)
        {
            var allusers = await _userRepository.GetAllUsers();
            var temp = allusers.AsQueryable();

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                string keywordLower = query.Keyword.ToLower();
                temp = temp.Where(b => b.firstName.ToLower().Contains(keywordLower) ||
                                       b.lastName.ToLower().Contains(keywordLower) ||
                                       b.position.ToLower().Contains(keywordLower) ||
                                       b.libraryCardNumber.ToLower().Contains(keywordLower) ||
                                       b.userId.ToString().Contains(query.Keyword));
            }

            if (query.UserId.HasValue)
                temp = temp.Where(b => b.userId == query.UserId.Value);

            if (!string.IsNullOrEmpty(query.FirstName))
                temp = temp.Where(b => b.firstName.ToLower().Contains(query.FirstName.ToLower()));

            if (!string.IsNullOrEmpty(query.LastName))
                temp = temp.Where(b => b.lastName.ToLower().Contains(query.LastName.ToLower()));

            if (!string.IsNullOrEmpty(query.FullName))
            {
                temp = temp.Where(b => (b.firstName + " " + b.lastName).ToLower().Contains(query.FullName.ToLower()));
            }

            if (!string.IsNullOrEmpty(query.Position))
                temp = temp.Where(b => b.position.ToLower().Contains(query.Position.ToLower()));

            if (!string.IsNullOrEmpty(query.LibraryCardNumber))
                temp = temp.Where(b => b.libraryCardNumber.ToLower().Contains(query.LibraryCardNumber.ToLower()));

            var total = temp.Count();

            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                switch (query.SortBy.ToLower())
                {
                    case "firstname":
                        temp = query.SortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase)
                            ? temp.OrderBy(s => s.firstName)
                            : temp.OrderByDescending(s => s.firstName);
                        break;
                    case "lastname":
                        temp = query.SortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase)
                            ? temp.OrderBy(s => s.lastName)
                            : temp.OrderByDescending(s => s.lastName);
                        break;
                    case "position":
                        temp = query.SortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase)
                            ? temp.OrderBy(s => s.position)
                            : temp.OrderByDescending(s => s.position);
                        break;
                    case "librarycardnumber":
                        temp = query.SortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase)
                            ? temp.OrderBy(s => s.libraryCardNumber)
                            : temp.OrderByDescending(s => s.libraryCardNumber);
                        break;
                    default:
                        temp = query.SortOrder.Equals("asc")
                            ? temp.OrderBy(s => s.userId)
                            : temp.OrderByDescending(s => s.userId);
                        break;
                }
            }

            var skipNumber = (query.PageNumber - 1) * query.PageSize;

            var userList = temp.Skip(skipNumber).Take(query.PageSize).ToList();

            return new { total = total, data = userList };
        }

        public async Task<IEnumerable<User>> GetAllUsersNoPages()
        {
            var users = await _userRepository.GetAllUsers();
            return users;
        }

        public async Task<User> GetUserById(int userId)
        {
            if (userId <= 0)
            {
                throw new Exception("User ID must be greater than zero.");
            }

            var existingUser = await _userRepository.GetUserById(userId);
            if (existingUser == null)
            {
                throw new Exception("User ID not found.");
            }
            return await _userRepository.GetUserById(userId);
        }

        public async Task<bool> UpdateUser(int userId, User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("User cannot be null.");
            }

            if (userId <= 0)
            {
                throw new Exception("User ID must be greater than zero.");
            }

            var existingUser = await _userRepository.GetUserById(userId);
            if (existingUser == null)
            {
                throw new Exception("User ID not found.");
            }

            existingUser.firstName = user.firstName;
            existingUser.lastName = user.lastName;
            existingUser.position = user.position;
            existingUser.libraryCardNumber = user.libraryCardNumber;
            existingUser.privilage = user.privilage;
            existingUser.notes = user.notes;

            return await _userRepository.UpdateUser(existingUser);
        }

        public async Task<bool> DeleteUser(int userId)
        {
            if (userId <= 0)
            {
                throw new Exception("User ID must be greater than zero.");
            }

            var existingUser = await _userRepository.GetUserById(userId);
            if (existingUser == null)
            {
                throw new Exception("User ID not found.");
            }

            return await _userRepository.DeleteUser(userId);
        }
    }
}
