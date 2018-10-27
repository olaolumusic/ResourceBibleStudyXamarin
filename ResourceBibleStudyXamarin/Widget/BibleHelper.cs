using Android.App;
using Java.IO;
using ResourceBibleStudyXamarin.Model;
using System.Collections.Generic;
using System.IO;

namespace ResourceBibleStudyXamarin.Widget
{

    public class BibleHelper
    {
        static Bible mBible;
        static List<DailyScriptures> mDailyScriptures;
        static Activity mActivity;

        public static Bible GetBible(Activity activity)
        {
            mActivity = activity;
            if (mBible != null && mBible.Books.Count > 0) return mBible;

            string content;
            var assets = mActivity.Assets;
            using (var reader = new StreamReader(assets.Open("msg.txt")))
            {

            }

            return mBible;
        }

        public static List<DailyScriptures> GetDailyScriptures(Activity activity)
        {
            mActivity = activity;
            if (mDailyScriptures != null && mDailyScriptures.Count > 0) return mDailyScriptures;

            using (var isReader = mActivity.Assets.Open("dailyscriptures.txt"))
            {
                var reader = new BufferedReader(new InputStreamReader(isReader));
                //Newton Json 
            }

            return mDailyScriptures;
        }


    }
}