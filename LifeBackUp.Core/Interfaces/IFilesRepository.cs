﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LifeBackUp.Core.Files;
using Microsoft.AspNetCore.Http;

namespace LifeBackUp.Core.Interfaces
{
    public interface IFilesRepository
    {
        Task<AddFileResponse> UploadFiles(string bucketName, IList<IFormFile> formFiles);
        Task<IEnumerable<ListFilesResponse>> ListFiles(string bucketName);
        Task DownloadFile(string bucketName, string fileName);
        Task<DeleteFileResponse> DeleteFile(string bucketName, string fileName);
        Task AddJsonObject(string bucketName, AddJsonObjectRequest request);

        Task<GetJsonObjectResponse> GetJsonObject(string bucketName, string fileName);
    }
}
