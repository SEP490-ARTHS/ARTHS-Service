﻿namespace ARTHS_Data.Models.Requests.Filters
{
    public class MotobikeProductFilterModel
    {
        public string? Name { get; set; }
        public string? RepairService { get; set; }
        public string? Category { get; set; }
        public string? VehiclesName { get; set; }
        public Guid? DiscountId { get; set; }
    }
}
