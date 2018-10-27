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
    public class DailyScriptures
    {
        public long Id { get; set; }
        public string FirstReading { get; set; }
        public string SecondReading { get; set; }
        public string ThirdReading { get; set; }
        public string BookVersion { get; set; }
        public int DayOfTheYear { get; set; }
    }
}