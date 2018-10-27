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
    public class Book
    {
        public int Id { get; set; }
        public string BookName { get; set; }
        public List<Chapters> BookChapter { get; set; }

    }
}