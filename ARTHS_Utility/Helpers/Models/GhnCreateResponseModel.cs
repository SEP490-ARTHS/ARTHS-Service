using Newtonsoft.Json;

namespace ARTHS_Utility.Helpers.Models
{
    public class GhnCreateResponseModel
    {
        [JsonProperty("order_code")]
        public string? OrderCode { get; set; }

        [JsonProperty("sort_code")]
        public string? SortCode { get; set; }

        [JsonProperty("trans_type")]
        public string? TransType { get; set; }

        [JsonProperty("ward_encode")]
        public string? WardEncode { get; set; }

        [JsonProperty("district_encode")]
        public string? DistrictEncode { get; set; }

        [JsonProperty("fee")]
        public Fee? Fee { get; set; }

        [JsonProperty("total_fee")]
        public string? TotalFee { get; set; }

        [JsonProperty("expected_delivery_time")]
        public DateTime ExpectedDeliveryTime { get; set; }
    }
    public class Fee
    {
        [JsonProperty("main_service")]
        public int MainService { get; set; }

        [JsonProperty("insurance")]
        public int Insurance { get; set; }

        [JsonProperty("station_do")]
        public int StationDo { get; set; }

        [JsonProperty("station_pu")]
        public int StationPu { get; set; }

        [JsonProperty("return")]
        public int Return { get; set; }

        [JsonProperty("r2s")]
        public int R2S { get; set; }

        [JsonProperty("coupon")]
        public int Coupon { get; set; }

        [JsonProperty("cod_failed_fee")]
        public int CodFailedFee { get; set; }
    }
}
