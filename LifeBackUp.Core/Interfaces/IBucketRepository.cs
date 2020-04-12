using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LifeBackUp.Core.Communication;
using LifeBackUp.Core.Files;

namespace LifeBackUp.Core.Interfaces
{
    public interface IBucketRepository
    {
        Task<bool> DoesBucketExist(string bucketName);
        Task<CreateBucketResponse> CreateBucket(string bucketName);
        Task<IEnumerable<ListS3BucketResponse>> ListBuckets();
        Task DeleteBucket(string bucketName);
        
    }
}
