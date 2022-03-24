using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Humate.RESTApi.Infrastructure.InternalService.File
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<FileStorageService> _logger;

        public FileStorageService(IConfiguration configuration,
            ILogger<FileStorageService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> StoreFileAsync(string fileName, Stream file, CancellationToken cancellationToken)
        {
            try
            {
                var path = Environment.CurrentDirectory + _configuration["GlobalParameter:StoragePath"];
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                path = $"{path}{Path.DirectorySeparatorChar}{fileName}";
                await using var fileStream = System.IO.File.Create(path);
                await file.CopyToAsync(fileStream, cancellationToken);
                _logger.LogInformation("{fileName} successfully stored in directory", fileName);
                return path;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while trying to store file");
                return "";
            }
        }

        public async Task<string> StoreInUserPathAsync(Guid userId, string fileName, byte[] fileContent,
            string newFolder = default, CancellationToken cancellationToken = default)
        {
            try
            {
                var path = newFolder == default ? Path.Combine(Environment.CurrentDirectory +_configuration["GlobalParameter:StoragePath"],
                    userId.ToString()) : Path.Combine(Environment.CurrentDirectory +_configuration["GlobalParameter:StoragePath"],
                    userId.ToString(), newFolder);
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                path = $"{path}{Path.DirectorySeparatorChar}{fileName}";
                await using var fileStream = System.IO.File.Create(path);
                await fileStream.WriteAsync(fileContent,0,fileContent.Length, cancellationToken);
                _logger.LogInformation("{fileName} successfully stored in directory", fileName);
                return path;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while trying to store file");
                return "";
            }
        }

        public Stream GetFile(string fullPath)
        {
            try
            {
                if (System.IO.File.Exists(fullPath))
                {
                    _logger.LogInformation("Return file stream from directory");
                    return System.IO.File.OpenRead(fullPath);
                }

                _logger.LogWarning("Failed to found file in directory");
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while trying to recover a file from directory");
                return null;
            }
        }
    }
}
