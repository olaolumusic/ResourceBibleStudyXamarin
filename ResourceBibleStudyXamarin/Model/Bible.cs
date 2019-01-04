 
using System.Collections.Generic; 

namespace ResourceBibleStudyXamarin.Model
{
    public class Bible
    {
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Version { get; set; }
        public List<Book> Books { get; set; }
    }
}