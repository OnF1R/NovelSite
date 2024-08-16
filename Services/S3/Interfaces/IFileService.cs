namespace NovelSite.Services.S3.Interfaces
{
    public interface IFileService
    {
        Task<string> UploadFileAsync(IFormFile file, string userId, string newFileName);
    }
}
