﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment5.Application.DTOs
{
    public class BookDto
    {
        public int Id { get; set; }
        public string? category { get; set; }
        public string? title { get; set; }
        public string? ISBN { get; set; }
        public string? author { get; set; }
        public string? publisher { get; set; }
        public string? description { get; set; }
        public string? location { get; set; }
        public int? totalBook { get; set; }
        public string? language { get; set; }
    }
}
