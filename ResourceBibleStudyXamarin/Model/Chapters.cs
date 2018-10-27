using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace ResourceBibleStudyXamarin.Model
{
    public class Chapters
    {
        public int ChapterId  { get; set; }
        public int BookId  { get; set; }
        public List<Verse> ChapterVerses  { get; set; }
    }
}