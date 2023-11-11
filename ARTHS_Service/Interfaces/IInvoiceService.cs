namespace ARTHS_Service.Interfaces
{
    public interface IInvoiceService
    {
        Task<string> GenerateInvoice(string orderId, string folderPath);
    }
}
