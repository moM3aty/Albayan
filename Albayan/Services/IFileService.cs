using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Albayan.Services
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile imageFile, string subfolder);
        Task<string> SaveBase64FileAsync(string base64String, string subfolder);
        void DeleteFile(string imagePath);
    }
}
