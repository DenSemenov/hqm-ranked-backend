using Amazon.Runtime;
using Amazon.S3.Model;
using Amazon.S3;
using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_backend.Services.Interfaces;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Serilog;
using hqm_ranked_backend.Common;
using hqm_ranked_backend.Helpers;
using Microsoft.AspNetCore.Http;

namespace hqm_ranked_backend.Services
{
    public class StorageService: IStorageService
    {
        private RankedDb _dbContext;
        private AmazonS3Client? _client;
        private string _S3Domain;
        private string _S3Bucket;
        private Guid _S3Id;
        public StorageService(RankedDb dbContext)
        {
            _dbContext = dbContext;

            var settings = _dbContext.Settings.FirstOrDefault();
            if (!String.IsNullOrEmpty(settings.S3Domain) && !String.IsNullOrEmpty(settings.S3Bucket) && !String.IsNullOrEmpty(settings.S3User) && !String.IsNullOrEmpty(settings.S3Key))
            {
                _S3Domain = settings.S3Domain;
                _S3Bucket = settings.S3Bucket;
                _S3Id = settings.Id;

                AmazonS3Config config = new AmazonS3Config()
                {
                    ServiceURL = string.Format("https://{0}", settings.S3Domain),
                    UseHttp = false
                };
                AWSCredentials creds = new BasicAWSCredentials(settings.S3User, settings.S3Key);
                _client = new AmazonS3Client(creds, config);
            }
        }

        public async Task<string> GetStorage()
        {
            var storageUrl = String.Empty;

            var setting = await _dbContext.Settings.FirstOrDefaultAsync();
            if (setting != null)
            {
                storageUrl = String.Format("https://{0}/{1}/{2}/", setting.S3Domain, setting.S3Bucket, setting.Id);
            }

            return storageUrl;
        }

        public async Task<List<string>> GetAllFileNames()
        {
            var fileNames = new List<string>();

            if (_client !=null)
            {
                var files = await _client.ListObjectsAsync(_S3Bucket, _S3Id.ToString());
                fileNames = files.S3Objects.Select(x => x.Key).ToList();
            }

            return fileNames;
        }

        public async Task<StorageType> UploadFile(string name, IFormFile file)
        {
            var result = StorageType.S3;
            if (_client != null)
            {
                try
                {
                    using (Stream fileToUpload = file.OpenReadStream())
                    {
                        var putObjectRequest = new PutObjectRequest();
                        putObjectRequest.BucketName = _S3Bucket;
                        putObjectRequest.Key = _S3Id.ToString() + "/" + name;
                        putObjectRequest.InputStream = fileToUpload;

                        await _client.PutObjectAsync(putObjectRequest);
                    }
                }
                catch (Exception ex)
                {
                    var log = LogHelper.GetErrorLog(ex.Message, ex.StackTrace);
                    Log.Error(log);
                }
            }

            return result;
        }

        public async Task<StorageType> UploadFileStream(string name, Stream file)
        {
            var result = StorageType.S3;
            if (_client != null)
            {
                try
                {
                    var putObjectRequest = new PutObjectRequest();
                    putObjectRequest.BucketName = _S3Bucket;
                    putObjectRequest.Key = _S3Id.ToString() + "/" + name;
                    putObjectRequest.InputStream = file;

                    await _client.PutObjectAsync(putObjectRequest);
                }
                catch (Exception ex)
                {
                    var log = LogHelper.GetErrorLog(ex.Message, ex.StackTrace);
                    Log.Error(log);
                }

            }

            return result;
        }

        public async Task<StorageType> UploadTextFile(string name, string text)
        {
            var result = StorageType.S3;
            if (_client != null)
            {
                try
                {
                    using (Stream fileToUpload = GenerateStreamFromString(text))
                    {
                        var putObjectRequest = new PutObjectRequest();
                        putObjectRequest.BucketName = _S3Bucket;
                        putObjectRequest.Key = _S3Id.ToString() + "/" + name;
                        putObjectRequest.InputStream = fileToUpload;

                        await _client.PutObjectAsync(putObjectRequest);
                    }
                }
                catch (Exception ex)
                {
                    var log = LogHelper.GetErrorLog(ex.Message, ex.StackTrace);
                    Log.Error(log);
                }

            }

            return result;
        }

        public async Task<string> LoadTextFile(string name)
        {
            var result = String.Empty;

            if (_client != null)
            {
                var file = await _client.GetObjectAsync(_S3Bucket, _S3Id.ToString() + "/" + name);

                using (StreamReader reader = new StreamReader(file.ResponseStream))
                {
                    result = reader.ReadToEnd();
                }
            }

            return result;
        }

        public async Task<string> RemoveFile(string name)
        {
            var result = String.Empty;

            if (_client != null)
            {
                await _client.DeleteObjectAsync(_S3Bucket, _S3Id.ToString() + "/" + name);
            }

            return result;
        }

        private Stream GenerateStreamFromString(string p)
        {
            var bytes = Encoding.UTF8.GetBytes(p);
            var strm = new MemoryStream();
            strm.Write(bytes, 0, bytes.Length);
            return strm;
        }
    }
}
