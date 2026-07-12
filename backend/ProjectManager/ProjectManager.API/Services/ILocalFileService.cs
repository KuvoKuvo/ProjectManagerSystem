namespace ProjectManager.API.Services
{
    public interface ILocalFileService
    {
        Task<string> SaveFileAsync(IFormFile file);
    }
}
