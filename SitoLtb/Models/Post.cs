﻿
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using SitoLtb.Data;

namespace SitoLtb.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public string? ApplicationUserId { get; set; }

        public string? Image { get; set; }

        public string Url { get; set; }

        public string? Categoria { get; set; }
        public DateTime DateTimeCreated { get; set; } = DateTime.Now;
        public ApplicationUser? ApplicationUser { get; set; }

    }
}
