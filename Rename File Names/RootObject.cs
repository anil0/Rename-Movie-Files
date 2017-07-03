using System.Collections.Generic;
using System.Security.Policy;

namespace Rename_File_Names
{
    public class RootObject
    {
        public string rating { get; set; }
        public string description { get; set; }
        public string title { get; set; }
        public Url url { get; set; }
        public Poster poster { get; set; }
        public string release_date { get; set; }
        public string content_rating { get; set; }
        public string original_title { get; set; }
        public List<string> writers { get; set; }
        public string imdb_id { get; set; }
        public List<Cast> cast { get; set; }
        public string length { get; set; }
        public string rating_count { get; set; }
        public string storyline { get; set; }
        public List<object> stars { get; set; }
        public string year { get; set; }
        public List<string> genre { get; set; }
        public List<object> trailer { get; set; }
    }
}