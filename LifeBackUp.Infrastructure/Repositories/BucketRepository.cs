using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using LifeBackUp.Core.Communication;
using LifeBackUp.Core.Interfaces;

namespace LifeBackUp.Infrastructure.Repositories
{

    public class BucketRepository : IBucketRepository
    {
        private readonly IAmazonS3 _s3ClientAmazonS3;

        public BucketRepository(IAmazonS3 s3ClientAmazonS3)
        {
            _s3ClientAmazonS3 = s3ClientAmazonS3;
        }

        public async Task<bool> DoesBucketExist(string bucketName)
        {
            return await _s3ClientAmazonS3.DoesS3BucketExistAsync(bucketName);
        }

        public async Task<CreateBucketResponse> CreateBucket(string bucketName)
        {
            var putBucketRequest = new PutBucketRequest()
            {
                BucketName = bucketName,
                UseClientRegion = true
            };

            var response = await _s3ClientAmazonS3.PutBucketAsync(putBucketRequest);

            return new CreateBucketResponse
            {
                BucketName = bucketName,
                RequestId = response.ResponseMetadata.RequestId
            };
        }

        public async Task<IEnumerable<ListS3BucketResponse>> ListBuckets()
        { 
            var response = await _s3ClientAmazonS3.ListBucketsAsync();

            return response.Buckets.Select(a => new ListS3BucketResponse
            {
                BucketName = a.BucketName,
                CreationDate = a.CreationDate
            });
        }

        public async Task DeleteBucket(string bucketName)
        {
            await _s3ClientAmazonS3.DeleteBucketAsync(bucketName);
        }
    }
}
