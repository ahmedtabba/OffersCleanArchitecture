using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Application.Common.Interfaces.Assets;
using Offers.CleanArchitecture.Application.Common.Models.Assets;
using Microsoft.AspNetCore.Hosting;
using Microsoft.VisualBasic;

namespace Offers.CleanArchitecture.Infrastructure.Repositories.Assets;
public class LocalFileService : IFileService
{
    private readonly IWebHostEnvironment _webHostEnvironment;

    public LocalFileService(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<string> UploadFileAsync(FileDto file)
    {
        var filePath = string.Empty;

        var path = _webHostEnvironment.WebRootPath + "\\" + "Uploads\\";
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
        using (FileStream fileStream = System.IO.File.Create(path + uniqueFileName))
        {
            await file.Content.CopyToAsync(fileStream);
            fileStream.Flush();
            if (string.IsNullOrWhiteSpace(_webHostEnvironment.WebRootPath))
            {
                _webHostEnvironment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }
            // Construct the URL for the uploaded file
            filePath = $"/Uploads/{uniqueFileName}";
        }

        return filePath;
    }

    public Task DeleteFileAsync(string filePath)
    {
        //if (!Directory.Exists(filePath))
        ////System.IO.File.Exists(filePath)
        //{
        //    return false;
        //}
        var fullPathOfFileToDelete = _webHostEnvironment.WebRootPath + filePath;

        System.IO.File.Delete(fullPathOfFileToDelete);
        return Task.CompletedTask;
    }
}
