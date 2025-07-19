using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Albayan.Services
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile imageFile, string subfolder);
        void DeleteFile(string imagePath);
    }
}
