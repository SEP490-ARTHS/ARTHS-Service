using ARTHS_Utility.Helpers.Models;
using Newtonsoft.Json;
using System.Text;

namespace ARTHS_Utility.Helpers
{
    public static class GhnHelper
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private const string GHNBaseUrl = "https://dev-online-gateway.ghn.vn/shiip/public-api/v2/shipping-order/create";
        private const string Token = "ed4ee04c-749f-11ee-a6e6-e60958111f48";
        private const string ShopId = "190193";

        public static async Task<GhnCreateResponseModel?> CreateShippingOrderAsync(GhnCreateRequestModel requestData)
        {
            httpClient.DefaultRequestHeaders.Clear();

            // Set Token and ShopId in headers
            httpClient.DefaultRequestHeaders.Add("Token", Token);
            httpClient.DefaultRequestHeaders.Add("ShopId", ShopId);
            var json = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(GHNBaseUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseString);

                if (responseObject != null && responseObject.ContainsKey("data") && responseObject["data"] != null)
                {
                    string dataString = JsonConvert.SerializeObject(responseObject["data"]);
                    GhnCreateResponseModel? dataObject = JsonConvert.DeserializeObject<GhnCreateResponseModel>(dataString);
                    return dataObject;
                }
            }

            return null;
        }
    }
}
