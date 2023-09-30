namespace ARTHS_Service.Interfaces
{
    public interface ICloudStorageService
    {
        Task<string> Upload(Guid id, string contentType, Stream stream);
        Task<string> Delete(Guid id);
    }
}
