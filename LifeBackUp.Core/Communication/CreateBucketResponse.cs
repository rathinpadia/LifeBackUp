using System;
using System.Collections.Generic;
using System.Text;

namespace LifeBackUp.Core.Communication
{
    public class CreateBucketResponse
    {
        public string RequestId { get; set; }
        public string BucketName { get; set; }
    }
}
