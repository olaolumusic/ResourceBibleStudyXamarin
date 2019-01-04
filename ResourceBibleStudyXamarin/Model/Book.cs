using System.Collections.Generic;

namespace ResourceBibleStudyXamarin.Model
{
    public class Book
    {
        public int Id { get; set; }
        public string BookName { get; set; }
        public List<Chapters> BookChapter { get; set; }

    }
}