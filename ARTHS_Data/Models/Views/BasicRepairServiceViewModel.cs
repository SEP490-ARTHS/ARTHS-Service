﻿namespace ARTHS_Data.Models.Views
{
    public class BasicRepairServiceViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public int Price { get; set; }
        public string Image { get; set; } = null!;
    }
}