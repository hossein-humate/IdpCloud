using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Humate.RESTApi.Infrastructure.InternalService.File
{
    public interface IFileStorageService
    {
        Task<string> StoreFileAsync(string fileName, Stream file, CancellationToken cancellationToken);
        Stream GetFile(string fullPath);

        Task<string> StoreInUserPathAsync(Guid userId, string fileName, byte[] fileContent,
            string newFolder = default, CancellationToken cancellationToken = default);
    }
}
