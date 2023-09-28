using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARTHS_Service.Interfaces
{
    public interface ICloudStorageService
    {
        Task<string> Upload(Guid id, string contentType, Stream stream);
        Task<string> Delete(Guid id);
    }
}
