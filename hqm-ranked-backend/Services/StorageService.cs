using Amazon.Runtime;
using Amazon.S3.Model;
using Amazon.S3;
using hqm_ranked_backend.Hubs;
using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_backend.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.IO;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace hqm_ranked_backend.Services
{
    public class StorageService: IStorageService
    {
        private RankedDb _dbContext;
        private IWebHostEnvironment _hostingEnvironment;
        public StorageService(RankedDb dbContext, IWebHostEnvironment hostingEnvironment)
        {
            _dbContext = dbContext;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task UploadFile(string name, IFormFile file)
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

                    var response = await client.PutObjectAsync(putObjectRequest);
                }
            }
        }

        public async Task UploadTextFile(string name, string text)
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

                    var response = await client.PutObjectAsync(putObjectRequest);
                }
            }
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
