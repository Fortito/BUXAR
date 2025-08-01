using System.Runtime.CompilerServices;

namespace WebApplication6.Helper;

public static class FileHelper
{
    public static async Task<string> SaveFileAsync(this IFormFile file,string WebRootPath,string folder)
    {
        string filepath = Path.Combine(WebRootPath, folder).ToLower();
        if (!Directory.Exists(filepath))
        {
            Directory.CreateDirectory(filepath);
        }
        var path = $"/{folder}/" + Guid.NewGuid() + file.FileName;
        using FileStream fileStream = new(WebRootPath + path, FileMode.Create);
        await file.CopyToAsync(fileStream);
        return path;
    }
}
