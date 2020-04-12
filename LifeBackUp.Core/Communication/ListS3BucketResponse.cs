using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace LifeBackUp.Core.Communication
{
    public class ListS3BucketResponse
    {
        public string BucketName { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
