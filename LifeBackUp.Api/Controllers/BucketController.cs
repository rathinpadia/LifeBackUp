using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LifeBackUp.Core.Communication;
using LifeBackUp.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LifeBackUp.Api.Controllers
{
    [Route("api/bucket")]
    [ApiController]
    public class BucketController : ControllerBase
    {
        private readonly IBucketRepository _bucketRepository;

        public BucketController(IBucketRepository bucketRepository)
        {
            _bucketRepository = bucketRepository;
        }

        /// <summary>
        /// Create a new AWS S3 Bucket
        /// </summary>
        /// <param name="bucketName"></param>
        /// <returns>Newly added S3 Bucket with unique id</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/bucket/lifebackupbucketrp
        ///
        /// </remarks>
        /// <response code="200">bucket is added successfully</response>
        [Produces("application/json")]
        [HttpPost]
        [Route("create/{bucketName}")]
        public async Task<ActionResult<CreateBucketResponse>> CreateS3Bucket([FromRoute] string bucketName)
        {
            var bucketExists = await _bucketRepository.DoesBucketExist(bucketName);
            if (bucketExists)
            {
                return BadRequest("S3 bucket already exists");
            }

            var result = await _bucketRepository.CreateBucket(bucketName);

            if (result == null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        /// <summary>
        /// Lists all AWS S3 Buckets
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("list")]
        public async Task<ActionResult<IEnumerable<ListS3BucketResponse>>> ListS3Buckets()
        {
            var result = await _bucketRepository.ListBuckets();

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Delete an AWS S3 Bucket
        /// </summary>
        /// <param name="bucketName"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("delete/{bucketName}")]
        public async Task<IActionResult> DeleteS3Bucket(string bucketName)
        {
            await _bucketRepository.DeleteBucket(bucketName);
            return Ok();
        }

    }
}