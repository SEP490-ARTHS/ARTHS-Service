using Newtonsoft.Json;

namespace ARTHS_Utility.Helpers.Models
{
    public class GhnWebHookResponse
    {
        public int? CODAmount { get; set; } // Tiền thu hộ
        public DateTime? CODTransferDate { get; set; } // Ngày chuyển tiền thu hộ
        public string? ClientOrderCode { get; set; }// Mã đơn hàng riêng của Khách hàng
        public int? ConvertedWeight { get; set; } // Khối lượng quy đổi
        public string? Description { get; set; } // Mô tả
        public FeeDetail? Fee { get; set; } // Phí
        public int? Height { get; set; } // Chiều cao
        public bool? IsPartialReturn { get; set; } // Đơn hàng giao 1 phần
        public int? Length { get; set; } // Chiều dài
        public string? OrderCode { get; set; } // Mã vận đơn
        public string? PartialReturnCode { get; set; } // Mã vận đơn giao 1 phần
        public int? PaymentType { get; set; } // Mã người thanh toán phí dịch vụ
        public string? Reason { get; set; } // Lý do
        public string? ReasonCode { get; set; } // Mã lý do
        public int? ShopID { get; set; } // Mã cửa hàng
        public string? Status { get; set; } // Trạng thái đơn hàng
        public DateTime? Time { get; set; } // Thời gian
        public decimal? TotalFee { get; set; } // Tổng tiền phí dịch vụ
        public string? Type { get; set; } // Loại
        public string? Warehouse { get; set; } // Bưu cục
        public int? Weight { get; set; } // Cân nặng
        public int? Width { get; set; } // Chiều rộng
    }

    public class FeeDetail
    {
        public int? CODFailedFee { get; set; } // Số tiền cần thu khi giao thất bại
        public int? CODFee { get; set; } // Phí thu COD
        public int? Coupon { get; set; } // Giá trị khuyến mãi
        public int? DeliverRemoteAreasFee { get; set; } // Phí giao hàng vùng sâu vùng xa
        public int? DocumentReturn { get; set; } // Phí thu chứng từ
        public int? DoubleCheck { get; set; } // Phí đồng kiểm
        public int? Insurance { get; set; } // Phí khai giá hàng hóa
        public int? MainService { get; set; } // Phí dịch vụ
        public int? PickRemoteAreasFee { get; set; } // Phí lấy hàng vùng sâu vùng xa
        public int? R2S { get; set; } // Phí giao lại hàng
        public int? Return { get; set; } // Phí hoàn hàng
        public int? StationDO { get; set; } // Phí gửi hàng tại bưu cục
        public int? StationPU { get; set; } // Phí lấy hàng tại bưu cục
        public int? Total { get; set; } // Tổng phí
    }
}
