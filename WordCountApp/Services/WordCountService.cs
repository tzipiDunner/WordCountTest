using Amazon.Runtime.Internal.Transform;
using Amazon.S3.Model;
using Amazon.S3;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Text;
using WordCountApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace WordCountApp.Services
{
    public class WordCountService : IWordCountService
    {
        private readonly AmazonS3Client _s3Client;
        private readonly string _bucketName;
        private readonly string _keyPrefix;

        public WordCountService(IConfiguration configuration)
        {
            var awsRegion = configuration["AWS:Region"];
            _s3Client = new AmazonS3Client(Amazon.RegionEndpoint.GetBySystemName(awsRegion));
            _bucketName = configuration["AWS:BucketName"];
            _keyPrefix = configuration["AWS:KeyPrefix"];
        }

        /// <summary>
        /// Counts the occurrences of each word in the provided text.
        /// </summary>
        /// <param name="text">The text to analyze.</param>
        /// <returns>A WordCountResult containing the total number of words and the frequency of each word.</returns>
        public WordCountResult CountWords(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return new WordCountResult { TotalWords = 0, WordCounts = new Dictionary<string, int>() };

            var wordCounts = new Dictionary<string, int>();
            var words = Regex.Split(text, @"\W+").Where(w => !string.IsNullOrEmpty(w));

            foreach (var word in words)
            {
                var lowerWord = word.ToLowerInvariant();
                if (wordCounts.ContainsKey(lowerWord))
                {
                    wordCounts[lowerWord]++;
                }
                else
                {
                    wordCounts.Add(lowerWord, 1);
                }
            }

            return new WordCountResult { TotalWords = words.Count(), WordCounts = wordCounts };
        }

        /// <summary>
        /// Asynchronously uploads a WordCountResult to AWS S3 as a JSON file.
        /// </summary>
        /// <param name="wordCountResult">The result object to upload.</param>
        /// <param name="fileNameWithoutExtension">The base name of the file to create, without extension.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task UploadToS3Async(WordCountResult wordCountResult, string fileNameWithoutExtension)
        {
            var json = JsonConvert.SerializeObject(wordCountResult);
            var key = $"{_keyPrefix}{fileNameWithoutExtension}.json";
            try
            {
                string wordCountResultJson = JsonConvert.SerializeObject(wordCountResult);

                var putRequest = new PutObjectRequest
                {
                    BucketName = _bucketName,
                    Key = key,
                    ContentBody = wordCountResultJson,
                    ContentType = "application/json"
                };

                var response = await _s3Client.PutObjectAsync(putRequest);
                Console.WriteLine($"File uploaded successfully to {_bucketName}/{key}");
            }
            catch (AmazonS3Exception ex)
            {
                Console.WriteLine($"Error encountered on server. Message:'{ex.Message}' when writing an object");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unknown error encountered on server. Message:'{ex.Message}'");
            }
        }

        /// <summary>
        /// Retrieves a WordCountResult stored in AWS S3 as a JSON file.
        /// </summary>
        /// <param name="fileKey">The key of the file in the S3 bucket.</param>
        /// <returns>The deserialized WordCountResult object.</returns>
        public async Task<WordCountResult> RetrieveFromS3Async(string fileKey)
        {
            try
            {
                var getRequest = new GetObjectRequest
                {
                    BucketName = _bucketName,
                    Key = fileKey
                };

                using (var response = await _s3Client.GetObjectAsync(getRequest))
                using (var reader = new StreamReader(response.ResponseStream))
                {
                    string file = await reader.ReadToEndAsync();
                    return JsonConvert.DeserializeObject<WordCountResult>(file);
                }
            }
            catch (AmazonS3Exception ex)
            {
                Console.WriteLine($"Error encountered on server when reading from S3. Message:'{ex.Message}'");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unknown error encountered on server. Message:'{ex.Message}'");
                throw;
            }
        }
    }
}
