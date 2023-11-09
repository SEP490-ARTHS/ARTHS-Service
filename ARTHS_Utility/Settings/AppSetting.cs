namespace ARTHS_Utility.Settings
{
    public class AppSetting
    {
        //secret key for authentication
        public string SecretKey { get; set; } = null!;
        public string RefreshTokenSecret { get; set; } = null!;

        // firebase cloud storage
        public string Bucket { get; set; } = null!;
        public string Folder { get; set; } = null!;

        //VNpay
        public string MerchantId { get; set; } = null!;
        public string MerchantPassword { get; set; } = null!;
        public string VNPayUrl { get; set; } = null!;
        public string ReturnUrl { get; set; } = null!;

        //Zalopay
        public int AppId { get; set; }
        public string Key1 { get; set; } = null!;
        public string Key2 { get; set; } = null!;
    }
}
