using Amazon.Runtime;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3;
using Amazon;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Meme.Hub.Site.Models;

namespace Meme.Hub.Site.Services
{
    public class S3StorageService : IStorageService
    {
        private readonly string? _bucketName;
        private readonly string? _accessKey;
        private readonly string? _secretKey;
        private readonly ILogger<S3StorageService> _logger;

        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.EUWest2;
        public S3StorageService(IOptions<S3Settings> settings, ILogger<S3StorageService> logger)
        {
            _bucketName = settings.Value.BucketName;
            _accessKey = settings.Value.AccessKey;
            _secretKey = settings.Value.SecretKey;
            _logger = logger;
        }

        public async Task<bool> RemoveAsync(string tokenAddr, string fileName)
        {
            try
            {
                // Set up your AWS credentials
                var credentials = new BasicAWSCredentials(_accessKey, _secretKey);
                using var s3Client = new AmazonS3Client(credentials, bucketRegion);

                var keyName = CreatePath(tokenAddr, fileName);

                var deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = _bucketName,
                    Key = keyName
                };

                var deleteObjectResponse = await s3Client.DeleteObjectAsync(deleteObjectRequest);

                return deleteObjectResponse.HttpStatusCode == System.Net.HttpStatusCode.NoContent;
            }
            catch (AmazonS3Exception e)
            {
                _logger.LogError(
                        "Error encountered ***. Message:'{0}' when writing an object"
                        , e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(
                    "Unknown encountered on server. Message:'{0}' when writing an object"
                    , e.Message);
            }
            return false;
        }

        public async Task<string> UploadAsync(string tokenAddr, string fileName, Stream stream)
        {
            try
            {
                using (stream)
                {
                    // Set up your AWS credentials
                    var credentials = new BasicAWSCredentials(_accessKey, _secretKey);
                    using var s3Client = new AmazonS3Client(credentials, bucketRegion);
                    var fileTransferUtility = new TransferUtility(s3Client);

                    var key = CreatePath(tokenAddr, fileName);
                    await fileTransferUtility.UploadAsync(stream, _bucketName, key);
                    return CreateS3Url(bucketRegion, key);
                }
            }
            catch (AmazonS3Exception e)
            {
                _logger.LogError(
                        "Error encountered ***. Message:'{0}' when writing an object"
                        , e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(
                    "Unknown encountered on server. Message:'{0}' when writing an object"
                    , e.Message);
            }

            return null!;
        }

        private string CreateS3Url(RegionEndpoint bucketRegion, string key)
        {
            var region = bucketRegion.SystemName;

            return $"https://{_bucketName}.s3.{region}.amazonaws.com/{key}";
        }

        private static string CreatePath(string tokenAddr, string filename)
        {
            return $"memetokenasset/{tokenAddr}/{filename}";
        }
    }
}