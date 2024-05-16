using Amazon.Runtime;
using Amazon.S3.Model;
using Amazon.S3;
using hqm_ranked_backend.Hubs;
using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_backend.Services.Interfaces;
using System.Text;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Collections;

namespace hqm_ranked_backend.Services
{
    public class StorageService: IStorageService
    {
        private RankedDb _dbContext;
        public StorageService(RankedDb dbContext)
        {
            _dbContext = dbContext;
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
            var settings = _dbContext.Settings.FirstOrDefault();

            if (!String.IsNullOrEmpty(settings.S3Domain) && !String.IsNullOrEmpty(settings.S3Bucket) && !String.IsNullOrEmpty(settings.S3User) && !String.IsNullOrEmpty(settings.S3Key))
            {
                AmazonS3Config config = new AmazonS3Config()
                {
                    ServiceURL = string.Format("https://{0}", settings.S3Domain),
                    UseHttp = false,
                };
                AWSCredentials creds = new BasicAWSCredentials(settings.S3User, settings.S3Key);
                AmazonS3Client client = new AmazonS3Client(creds, config);
                var files = await client.ListObjectsAsync(settings.S3Bucket, settings.Id.ToString());
                fileNames = files.S3Objects.Select(x => x.Key).ToList();
            }

            return fileNames;
        }

        public async Task<bool> UploadFile(string name, IFormFile file)
        {
            var settings = _dbContext.Settings.FirstOrDefault();

            if (!String.IsNullOrEmpty(settings.S3Domain) && !String.IsNullOrEmpty(settings.S3Bucket) && !String.IsNullOrEmpty(settings.S3User) && !String.IsNullOrEmpty(settings.S3Key))
            {
                AmazonS3Config config = new AmazonS3Config()
                {
                    ServiceURL = string.Format("https://{0}", settings.S3Domain),
                    UseHttp = false,
                };
                AWSCredentials creds = new BasicAWSCredentials(settings.S3User, settings.S3Key);
                AmazonS3Client client = new AmazonS3Client(creds, config);

                using (Stream fileToUpload = file.OpenReadStream())
                {
                    var putObjectRequest = new PutObjectRequest();
                    putObjectRequest.BucketName = settings.S3Bucket;
                    putObjectRequest.Key = settings.Id.ToString()+"/"+name;
                    putObjectRequest.InputStream = fileToUpload;

                    await client.PutObjectAsync(putObjectRequest);
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> UploadFileStream(string name, Stream file)
        {
            var settings = _dbContext.Settings.FirstOrDefault();

            if (!String.IsNullOrEmpty(settings.S3Domain) && !String.IsNullOrEmpty(settings.S3Bucket) && !String.IsNullOrEmpty(settings.S3User) && !String.IsNullOrEmpty(settings.S3Key))
            {
                AmazonS3Config config = new AmazonS3Config()
                {
                    ServiceURL = string.Format("https://{0}", settings.S3Domain),
                    UseHttp = false,
                };
                AWSCredentials creds = new BasicAWSCredentials(settings.S3User, settings.S3Key);
                AmazonS3Client client = new AmazonS3Client(creds, config);

                var putObjectRequest = new PutObjectRequest();
                putObjectRequest.BucketName = settings.S3Bucket;
                putObjectRequest.Key = settings.Id.ToString() + "/" + name;
                putObjectRequest.InputStream = file;

                await client.PutObjectAsync(putObjectRequest);

                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> UploadTextFile(string name, string text)
        {
            var settings = _dbContext.Settings.FirstOrDefault();

            if (!String.IsNullOrEmpty(settings.S3Domain) && !String.IsNullOrEmpty(settings.S3Bucket) && !String.IsNullOrEmpty(settings.S3User) && !String.IsNullOrEmpty(settings.S3Key))
            {
                AmazonS3Config config = new AmazonS3Config()
                {
                    ServiceURL = string.Format("https://{0}", settings.S3Domain),
                    UseHttp = false,
                };
                AWSCredentials creds = new BasicAWSCredentials(settings.S3User, settings.S3Key);
                AmazonS3Client client = new AmazonS3Client(creds, config);

                using (Stream fileToUpload = GenerateStreamFromString(text))
                {
                    var putObjectRequest = new PutObjectRequest();
                    putObjectRequest.BucketName = settings.S3Bucket;
                    putObjectRequest.Key = settings.Id.ToString() + "/" + name;
                    putObjectRequest.InputStream = fileToUpload;

                    await client.PutObjectAsync(putObjectRequest);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<string> LoadTextFile(string name)
        {
            var result = String.Empty;

            var settings = _dbContext.Settings.FirstOrDefault();

            if (!String.IsNullOrEmpty(settings.S3Domain) && !String.IsNullOrEmpty(settings.S3Bucket) && !String.IsNullOrEmpty(settings.S3User) && !String.IsNullOrEmpty(settings.S3Key))
            {
                AmazonS3Config config = new AmazonS3Config()
                {
                    ServiceURL = string.Format("https://{0}", settings.S3Domain),
                    UseHttp = false,
                };
                AWSCredentials creds = new BasicAWSCredentials(settings.S3User, settings.S3Key);
                AmazonS3Client client = new AmazonS3Client(creds, config);

                var file = await client.GetObjectAsync(settings.S3Bucket, settings.Id.ToString() + "/" + name);

                using (StreamReader reader = new StreamReader(file.ResponseStream))
                {
                    result = reader.ReadToEnd();
                }

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
