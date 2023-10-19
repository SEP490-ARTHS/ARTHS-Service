namespace ARTHS_Utility.Constants
{
    public static class OnlineOrderStatus
    {
        public const string Processing = "Chờ xử lý";     // Chờ sử lý
        public const string HandedOver = "HandedOver";     // Đã bàn giao cho đơn vị vận chuyển
        public const string Transport = "Đang giao";       // Đang giao
        public const string Finished = "Hoàn thành";         // Đã giao
        public const string Canceled = "Đã hủy";         // Đã hủy
        public const string Paid = "Đã thanh toán";                 // Đã thanh toán
    }
}
