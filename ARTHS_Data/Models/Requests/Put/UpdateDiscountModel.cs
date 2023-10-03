﻿using Microsoft.AspNetCore.Http;

namespace ARTHS_Data.Models.Requests.Put
{
    public class UpdateDiscountModel
    {
        public string? Title { get; set; }
        public int? DiscountAmount { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public IFormFile? Image { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
    }
}