using Android.App;
using Java.Lang;
using Newtonsoft.Json;
using ResourceBibleStudyXamarin.Model;
using System.Collections.Generic;
using System.IO;

namespace ResourceBibleStudyXamarin.Widget
{

    public class BibleHelper
    {
        private static Bible _bible;
        private static List<DailyScriptures> _dailyScriptures;
        private static Activity _activity;


        public static Bible GetBible(Activity activity)
        {
            _activity = activity;
            if (_bible != null && _bible.Books.Count > 0) return _bible;

            using (var isReader = new StreamReader(_activity.Assets.Open("msg.txt")))
            {
                _bible = JsonConvert.DeserializeObject<Bible>(isReader.ReadToEnd());
            }

            return _bible;
        }

        public static List<DailyScriptures> GetDailyScriptures(Activity activity)
        {
            _activity = activity;
            if (_dailyScriptures != null && _dailyScriptures.Count > 0) return _dailyScriptures;
            try
            {
                using (var isReader = new StreamReader(_activity.Assets.Open("dailyscriptures.txt")))
                {
                    _dailyScriptures = JsonConvert.DeserializeObject<List<DailyScriptures>>(isReader.ReadToEnd());
                }
            }
            catch (Exception exception)
            {
                exception.PrintStackTrace();
            }

            return _dailyScriptures;
        }


    }
}