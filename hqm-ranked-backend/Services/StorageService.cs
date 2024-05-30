using Amazon.Runtime;
using Amazon.S3.Model;
using Amazon.S3;
using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_backend.Services.Interfaces;
using System.Text;
using Microsoft.EntityFrameworkCore;

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

        public async Task<bool> UploadFile(string name, IFormFile file)
        {
            if (_client != null)
            {
                using (Stream fileToUpload = file.OpenReadStream())
                {
                    var putObjectRequest = new PutObjectRequest();
                    putObjectRequest.BucketName = _S3Bucket;
                    putObjectRequest.Key = _S3Id.ToString()+"/"+name;
                    putObjectRequest.InputStream = fileToUpload;

                    await _client.PutObjectAsync(putObjectRequest);
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

            if (_client != null)
            {
                var putObjectRequest = new PutObjectRequest();
                putObjectRequest.BucketName = _S3Bucket;
                putObjectRequest.Key = _S3Id.ToString() + "/" + name;
                putObjectRequest.InputStream = file;

                await _client.PutObjectAsync(putObjectRequest);

                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> UploadTextFile(string name, string text)
        {
            if (_client != null)
            {
                using (Stream fileToUpload = GenerateStreamFromString(text))
                {
                    var putObjectRequest = new PutObjectRequest();
                    putObjectRequest.BucketName = _S3Bucket;
                    putObjectRequest.Key = _S3Id.ToString() + "/" + name;
                    putObjectRequest.InputStream = fileToUpload;

                    await _client.PutObjectAsync(putObjectRequest);
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

        private Stream GenerateStreamFromString(string p)
        {
            var bytes = Encoding.UTF8.GetBytes(p);
            var strm = new MemoryStream();
            strm.Write(bytes, 0, bytes.Length);
            return strm;
        }
    }
}
