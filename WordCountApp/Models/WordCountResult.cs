namespace WordCountApp.Models
{
    public class WordCountResult
    {
        public int TotalWords { get; set; }
        public Dictionary<string, int> WordCounts { get; set; }
    }
}
