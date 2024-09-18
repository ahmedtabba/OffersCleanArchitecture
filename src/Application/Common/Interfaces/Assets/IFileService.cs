using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Application.Common.Models.Assets;

namespace Offers.CleanArchitecture.Application.Common.Interfaces.Assets;
public interface IFileService
{
    // method to uplaod logo or post and return the path of it, it is general interface used to deal with any type of file
    // but we use it here to deal with photos
    Task<string> UploadFileAsync(FileDto file);
    // method to delete old logo if user upload new logo in updating process
    Task DeleteFileAsync(string filePath);
}

