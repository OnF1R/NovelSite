using Amazon.S3;
using Amazon.S3.Transfer;
using NovelSite.Services.S3.Interfaces;

namespace NovelSite.Services.S3.Implimentations
{
    public class FileService : IFileService
    {
        private const string S3Url = "https://2f58d602-2c33-481e-875b-700b4d4b3263.selstorage.ru";

        private IAmazonS3 _s3Client;
        private string _bucketName;

        public FileService(IAmazonS3 s3Client, IConfiguration configuration)
        {
            _s3Client = s3Client;
            _bucketName = configuration["AWS:BucketName"];
        }

        public async Task<string> UploadFileAsync(IFormFile file, string userId, string newFileName)
        {
            try
            {
                var fileTransferUtility = new TransferUtility(_s3Client);

                using (var newMemoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(newMemoryStream);

                    var uploadRequest = new TransferUtilityUploadRequest
                    {
                        InputStream = newMemoryStream,
                        Key = $"Users/{userId}/Avatar/{newFileName}",
                        BucketName = _bucketName,
                        ContentType = file.ContentType,
                        CannedACL = S3CannedACL.PublicRead // Доступ к файлу публичный
                    };

                    await fileTransferUtility.UploadAsync(uploadRequest);

                    return $"{S3Url}/Users/{userId}/Avatar/{newFileName}";
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
