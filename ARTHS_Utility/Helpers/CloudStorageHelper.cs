using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;

namespace ARTHS_Utility.Helpers
{
    public class CloudStorageHelper
    {
        private static readonly StorageClient Storage;
        private static readonly UrlSigner UrlSigner;

        static CloudStorageHelper()
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            //thư mục gốc
            var projectRoot = Path.GetFullPath(Path.Combine(basePath, "..", "..", "..", ".."));
            string credentialPath = Path.Combine(projectRoot, "ARTHS_Utility", "Helpers", "CloudStorage", "arths-45678-firebase-adminsdk-plwhs-954089d6b7.json");
            //if (!File.Exists(credentialPath))
            //{
            //    throw new FileNotFoundException($"File not found at {credentialPath}");
            //}
            var credential = GoogleCredential.FromFile(credentialPath);

            // Storage
            Storage = StorageClient.Create(credential);

            // Url Signer
            UrlSigner = UrlSigner.FromCredential(credential);
        }

        public static StorageClient GetStorage()
        {
            return Storage;
        }

        // Generate signed cloud storage object url 
        public static string GenerateV4UploadSignedUrl(string bucketName, string objectName)
        {
            var options = UrlSigner.Options.FromDuration(TimeSpan.FromHours(24));

            var template = UrlSigner.RequestTemplate
                .FromBucket(bucketName)
                .WithObjectName(objectName)
                .WithHttpMethod(HttpMethod.Get);

            return UrlSigner.Sign(template, options);
        }
    }
}
