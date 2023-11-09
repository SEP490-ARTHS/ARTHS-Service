using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARTHS_Utility.Helpers.Models
{
    public class GhnCreateRequestModel
    {
        [JsonProperty("payment_type_id")]
        public int PaymentTypeId { get; set; } = 2;
        [JsonProperty("note")]
        public string Note { get; set; } = null!;
        [JsonProperty("required_note")]
        public string RequiredNote { get; set; } = "CHOTHUHANG";
        [JsonProperty("return_phone")]
        public string ReturnPhone { get; set; } = "0969920894";
        [JsonProperty("return_address")]
        public string ReturnAddress { get; set; } = "445, khu pho 8";
        [JsonProperty("return_district_id")]
        public string? ReturnDistrictId { get; set; } = null;
        [JsonProperty("return_ward_code")]
        public string? ReturnWardCode { get; set; } = null;
        [JsonProperty("client_order_code")]
        public string ClientOrderCode { get; set; } = null!;
        [JsonProperty("from_name")]
        public string FromName { get; set; } = "Thanh Huy Motobike";
        [JsonProperty("from_phone")]
        public string FromPhone { get; set; } = "0969920894";
        [JsonProperty("from_address")]
        public string FromAddress { get; set; } = "445, khu phố 8";
        [JsonProperty("from_ward_name")]
        public string FromWardName { get; set; } = "Tương Bình Hiệp";
        [JsonProperty("from_district_name")]
        public string FromDistrictName { get; set; } = "Thủ Dầu Một";
        [JsonProperty("from_province_name")]
        public string FromProvinceName { get; set; } = "Bình Dương";
        [JsonProperty("to_name")]
        public string ToName { get; set; } = null!;
        [JsonProperty("to_phone")]
        public string ToPhone { get; set; } = null!;
        [JsonProperty("to_address")]
        public string ToAddress { get; set; } = null!;
        [JsonProperty("to_ward_name")]
        public string ToWardName { get; set; } = null!;
        [JsonProperty("to_district_name")]
        public string ToDistrictName { get; set; } = null!;
        [JsonProperty("to_province_name")]
        public string ToProvinceName { get; set; } = null!;
        [JsonProperty("cod_amount")]
        public int? CODAmount { get; set; }
        [JsonProperty("content")]
        public string Content { get; set; } = null!;
        [JsonProperty("weight")]
        public int Weight { get; set; }//
        [JsonProperty("length")]
        public int Length { get; set; }//
        [JsonProperty("width")]
        public int Width { get; set; }//
        [JsonProperty("height")]
        public int Height { get; set; }//
        [JsonProperty("cod_failed_amount")]
        public int? CODFailedAmount { get; set; } = 2000;
        [JsonProperty("pick_station_id")]
        public int? PickStationId { get; set; }
        [JsonProperty("deliver_station_id")]
        public string? DeliverStationId { get; set; }
        [JsonProperty("insurance_value")]
        public int InsuranceValue { get; set; } //giá trị đơn hàng mất sẽ đền
        [JsonProperty("service_id")]
        public int? ServiceId { get; set; }
        [JsonProperty("service_type_id")]
        public int ServiceTypeId { get; set; } = 2;//
        [JsonProperty("coupon")]
        public string? Coupon { get; set; } = null;
        [JsonProperty("pickup_time")]
        public int? PickupTime { get; set; }
        [JsonProperty("pick_shift")]
        public int[] PickShift { get; set; } = {2};
        [JsonProperty("items")]
        public List<Item> Items { get; set; } = new List<Item>();
    }

    public class Item
    {
        [JsonProperty("name")]
        public string Name { get; set; } = null!;

        [JsonProperty("code")]
        public string Code { get; set; } = null!;

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("price")]
        public int? Price { get; set; }

        [JsonProperty("length")]
        public int? Length { get; set; }

        [JsonProperty("width")]
        public int? Width { get; set; }

        [JsonProperty("weight")]
        public int? Weight { get; set; }

        [JsonProperty("height")]
        public int? Height { get; set; }
    }
}
