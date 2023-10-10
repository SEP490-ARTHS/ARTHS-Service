namespace ARTHS_Utility.Constants
{
    public static class OnlineOrderStatus
    {
        public const string Processing = "Processing";     // Chờ sử lý
        public const string HandedOver = "HandedOver";     // Đã bàn giao cho đơn vị vận chuyển
        public const string Transport = "Transport";       // Đang giao
        public const string Finished = "Finished";         // Đã giao
        public const string Canceled = "Canceled";         // Đã hủy
    }
}
