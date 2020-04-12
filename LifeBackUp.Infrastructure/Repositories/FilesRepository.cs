using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using LifeBackUp.Core.Files;
using LifeBackUp.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace LifeBackUp.Infrastructure.Repositories
{
    public class FilesRepository : IFilesRepository
    {
        private readonly IAmazonS3 _s3ClientAmazonS3;

        public FilesRepository(IAmazonS3 s3ClientAmazonS3)
        {
            _s3ClientAmazonS3 = s3ClientAmazonS3;
        }

        public async Task<AddFileResponse> UploadFiles(string bucketName, IList<IFormFile> formFiles)
        {
            var response = new List<string>();
            foreach (var file in formFiles)
            {
                var uploadRequest = new TransferUtilityUploadRequest
                {
                    InputStream = file.OpenReadStream(),
                    Key = file.FileName,
                    BucketName = bucketName,
                    CannedACL = S3CannedACL.NoACL
                };

                using (var fileTransferUtility = new TransferUtility(_s3ClientAmazonS3))
                {
                    await fileTransferUtility.UploadAsync(uploadRequest);
                }

                var expiryURLRequest = new GetPreSignedUrlRequest
                {
                    BucketName = bucketName,
                    Key = file.FileName,
                    Expires = DateTime.Now.AddDays(1)
                };

                var url = _s3ClientAmazonS3.GetPreSignedURL(expiryURLRequest);

                response.Add(url);
            }

            return new AddFileResponse
            {
                PreSignedUrl = response
            };
        }

        public async Task<IEnumerable<ListFilesResponse>> ListFiles(string bucketName)
        {
            var responses = await _s3ClientAmazonS3.ListObjectsAsync(bucketName);

            return responses.S3Objects.Select(a=> new ListFilesResponse
            {
                BucketName = a.BucketName,
                Key = a.Key,
                Owner = a.Owner.DisplayName,
                Size = a.Size
            });
        }

        public async Task DownloadFile(string bucketName, string fileName)
        {
            var pathAndFileName = $@"c:\S3Temp\\{fileName}";
            var downloadRequest = new TransferUtilityDownloadRequest
            {
                BucketName = bucketName,
                Key = fileName,
                FilePath = pathAndFileName
            };

            using (var transferUtility = new TransferUtility(_s3ClientAmazonS3))
            {
                await transferUtility.DownloadAsync(downloadRequest);
            }
        }

        public async Task<DeleteFileResponse> DeleteFile(string bucketName, string fileName)
        {
            var multiObjectDeleteRequest = new DeleteObjectsRequest
            {
                BucketName = bucketName
            };
            multiObjectDeleteRequest.AddKey(fileName);

            var response = await _s3ClientAmazonS3.DeleteObjectsAsync(multiObjectDeleteRequest);

            return new DeleteFileResponse
            {
                NumberOfDeletedObjects = response.DeletedObjects.Count()
            };
        }

        public async Task AddJsonObject(string bucketName, AddJsonObjectRequest request)
        {
            var createdOnUtc = DateTime.UtcNow;
            var s3Key = $"{createdOnUtc:yyyy}/{createdOnUtc:MM}/{createdOnUtc:yy}/{request.Id}";

            var putObjectRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = s3Key,
                ContentBody = JsonConvert.SerializeObject(request)
            };

            await _s3ClientAmazonS3.PutObjectAsync(putObjectRequest);
        }

        public async Task<GetJsonObjectResponse> GetJsonObject(string bucketName, string fileName)
        {
            var request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = fileName
            };

            var response = await _s3ClientAmazonS3.GetObjectAsync(request);

            using (var reader = new StreamReader(response.ResponseStream))
            {
                var contents = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<GetJsonObjectResponse>(contents);
            }
        }
    }
}
