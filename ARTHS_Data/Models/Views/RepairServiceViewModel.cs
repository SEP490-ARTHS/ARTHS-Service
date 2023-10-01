﻿namespace ARTHS_Data.Models.Views
{
    public class RepairServiceViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public int Price { get; set; }
        public string ImageUrl { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime CreateAt { get; set; }
    }
}