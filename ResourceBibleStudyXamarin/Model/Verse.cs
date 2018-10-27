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
    public class Verse
    {
        public int Id { get; set; }
        public string VerseText { get; set; }
    }
}