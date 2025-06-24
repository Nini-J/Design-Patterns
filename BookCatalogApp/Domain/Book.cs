﻿using System;
namespace BookCatalogApp.Domain
{
    public class Book
    {
        
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Genre { get; set; } = string.Empty;
    }
}