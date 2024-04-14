using System.Collections.Generic;
using System.Threading.Tasks;
using WordCountApp.Models;

namespace WordCountApp.Services
{
    public interface IWordCountService
    {
        WordCountResult CountWords(string text);
        Task UploadToS3Async(WordCountResult wordCountResult, string fileNameWithoutExtension);
        Task<WordCountResult> RetrieveFromS3Async(string fileKey);
    }
}
