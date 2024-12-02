using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment6.Application.DTOs
{
    public class QueryObjectMember
    {
        public int? UserId { get; set; } = null;
        public string? FullName { get; set; } = null;
        public string? FirstName { get; set; } = null;
        public string? LastName { get; set; } = null;

        public string? Position { get; set; } = null;
        public string? LibraryCardNumber { get; set; } = null;
        public string? Keyword { get; set; } = null;

        public string? SortBy { get; set; } = "userId";
        public string? SortOrder { get; set; } = "desc";
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
