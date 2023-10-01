﻿using Microsoft.AspNetCore.Http;

namespace ARTHS_Data.Models.Requests.Post
{
    public class CreateRepairServiceModel
    {
        public string Name { get; set; } = null!;
        public int Price { get; set; }
        public string Description { get; set; } = null!;
        public IFormFile Image { get; set; } = null!;
    }
}