using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Threading.Tasks;
using LifeBackUp.Core.Files;
using LifeBackUp.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LifeBackUp.Api.Controllers
{
    [Route("api/files")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IFilesRepository _filesRepository;

        public FilesController(IFilesRepository filesRepository)
        {
            _filesRepository = filesRepository;
        }

        /// <summary>
        /// Add files to a bucket
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="formFiles"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{bucketName}/add")]
        public async Task<ActionResult<AddFileResponse>> AddFiles(string bucketName, IList<IFormFile> formFiles)
        {
            if (formFiles == null)
                return BadRequest("The request doesn't contain any files to be uploaded.");

            var response = await _filesRepository.UploadFiles(bucketName, formFiles);

            if (response == null)
                return BadRequest();

            return Ok(response);

        }

        /// <summary>
        /// List all the files from a bucket
        /// </summary>
        /// <param name="bucketName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{bucketName}/list")]
        public async Task<ActionResult<IEnumerable<ListFilesResponse>>> ListFiles(string bucketName)
        {
            var response = await _filesRepository.ListFiles(bucketName);

            return Ok(response);
        }

        /// <summary>
        /// Download file from a bucket
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{bucketName}/download/{fileName}")]
        public async Task<IActionResult> DownloadFile(string bucketName, string fileName)
        {
            await _filesRepository.DownloadFile(bucketName, fileName);

            return Ok();
        }

        /// <summary>
        /// Delete file from a bucket
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{bucketName}/delete/{fileName}")]
        public async Task<ActionResult<DeleteFileResponse>> DeleteFile(string bucketName, string fileName)
        {
            var response = await _filesRepository.DeleteFile(bucketName, fileName);

            return Ok(response);
        }

        /// <summary>
        /// Add json object to a bucket
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{bucketName}/addjsonobject")]
        public async Task<IActionResult> AddJsonObject(string bucketName, AddJsonObjectRequest request)
        {
            await _filesRepository.AddJsonObject(bucketName, request);

            return Ok();
        }

        /// <summary>
        /// Get json object from a bucket
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{bucketName}/getjsonobject")]
        public async Task<ActionResult<GetJsonObjectResponse>> GetJsonObject(string bucketName,string fileName)
        {
            var response = await _filesRepository.GetJsonObject(bucketName, fileName);

            return Ok(response);
        } 
    }
}