using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using ResourceBibleStudyXamarin.Model;
using ResourceBibleStudyXamarin.Widget;
using System;
using System.Collections.Generic;
using Exception = System.Exception;
using Fragment = Android.Support.V4.App.Fragment; 

namespace ResourceBibleStudyXamarin.Fragments
{
    public class DailyScriptureFragment : Fragment, View.IOnTouchListener
    {

        private const string TabPosition = "tab_position";


        private const string TAG = "DailyScriptureFragment";
        private TextView mDailyReadingTitle;
        private Book mDailyReadingBook;
        private TextView mDailyReadingContent;

        Activity mActivity;
        static Typeface mTypeface;
        private Button mPreviousButton;
        private Button mNextButton;
        private FloatingActionButton mDailyScripturesFloatingActionButton;
        private FloatingActionButton mDiscussionFloatingActionButton;

        static List<DailyScriptures> mDailyScriptures;

        DailyScriptures todaysReading;

        public static Bible mBible;
        public DailyScriptureFragment()
        {
            // Required empty public constructor
        }

        public static DailyScriptureFragment newInstance(string param1, string param2)
        {
            var fragment = new DailyScriptureFragment();

            return fragment;
        }


        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            if (Arguments != null)
            {

            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container,
                                 Bundle savedInstanceState)
        {
            // Inflate the layout for this fragment
            var dailyFragmentView = inflater.Inflate(Resource.Layout.fragment_daily_scripture, container, false);

            mActivity = Activity;
            mDailyScriptures = BibleHelper.GetDailyScriptures(mActivity);
            mBible = BibleHelper.GetBible(mActivity);

            mDailyReadingTitle = (TextView)dailyFragmentView.FindViewById(Resource.Id.daily_reading_book_title);
            mDailyReadingContent = (TextView)dailyFragmentView.FindViewById(Resource.Id.daily_reading_bible_content);
            mPreviousButton = (Button)dailyFragmentView.FindViewById(Resource.Id.btnPrevious);
            mNextButton = (Button)dailyFragmentView.FindViewById(Resource.Id.btnNext);

            var fab = dailyFragmentView.FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            //mDiscussionFloatingActionButton = (FloatingActionButton)dailyFragmentView.FindViewById(Resource.Id.discussion_fab);
            //mDiscussionFloatingActionButton.Click += (sender, args) =>
            //{
            //    StartActivity(new Intent(mActivity, typeof(MainActivity)));
            //};

            //mDailyScripturesFloatingActionButton = (FloatingActionButton)dailyFragmentView.FindViewById(Resource.Id.daily_fab);

            //mDailyScripturesFloatingActionButton.Click += (sender, args) =>
            //        {
            //            ShowCalender();
            //        };

            mNextButton.Click += (sender, args) =>
            {
                var dailyReadingText = mDailyReadingTitle.Text;
                if (dailyReadingText.ToLower().Contains("first"))
                {
                    LoadDailyReading(13);

                }
                else if (dailyReadingText.ToLower().Contains("second"))
                {
                    LoadDailyReading(18);
                }
                else
                {
                    LoadDailyReading(1);
                }

            };



            mPreviousButton.Click += (sender, arg) =>
            {

                var dailyReadingText = mDailyReadingTitle.Text;
                if (dailyReadingText.ToLower().Contains("first"))
                {
                    LoadDailyReading(18);

                }
                else if (dailyReadingText.ToLower().Contains("second"))
                {
                    LoadDailyReading(1);
                }
                else
                {
                    LoadDailyReading(13);
                }
            };

            var now = DateTime.Now;
            todaysReading = mDailyScriptures[(now.Day) - 1];

            var timeOfTheDay = now.Hour;
            try
            {
                LoadDailyReading(timeOfTheDay);
            }
            catch (Exception exception)
            {
                Toast.MakeText(mActivity, string.Format("Unable to load reading plan.. :{0}", exception.Message), ToastLength.Long);
            }


            return dailyFragmentView;
        }


        private static void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View)sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        }

        private void LoadDailyReading(int timeOfDay)
        {

            mDailyReadingTitle.Typeface = mTypeface;
            mDailyReadingContent.Typeface = mTypeface;

            if (timeOfDay < 12)
            {
                var bookTitle = string.Format("<strong>First Reading | {0} </strong>", todaysReading.FirstReading);

                mDailyReadingTitle.TextFormatted = Html.FromHtml(bookTitle);

                var firstReading = todaysReading.FirstReading.Split('.');

                foreach (var book in mBible.Books)
                {
                    if (!book.BookName.ToLower().Contains(firstReading[0].Trim().ToLower())) continue;
                    mDailyReadingBook = book;
                    break;
                }
                var bibleText = "";

                //--------So we get the chapters if it contains a to (-) string --------
                var chapterNumbers = new List<Integer>();

                if (firstReading[1].Trim().Contains("-"))
                {
                    var readingPlanChapter = firstReading[1].Split('-');

                    chapterNumbers.Add(Integer.ValueOf(readingPlanChapter[0].Trim()));
                    chapterNumbers.Add(Integer.ValueOf(readingPlanChapter[1].Trim()));
                }
                else
                {
                    chapterNumbers.Add(Integer.ValueOf(firstReading[1].Trim()));
                }
                //-------------------- That's all folks --------------------------------


                foreach (int chapter in chapterNumbers)
                {

                    var chapters = mDailyReadingBook.BookChapter[chapter - 1];

                    bibleText += $"<p><strong>Chapter. {chapters.ChapterId} </strong></p>";

                    foreach (var verse in chapters.ChapterVerses)
                    {
                        bibleText += $"{verse.Id}. {verse.VerseText} <br/>";
                    }
                }

                mDailyReadingContent.SetOnTouchListener(this);

                mDailyReadingContent.TextFormatted = Html.FromHtml(bibleText);
            }
            else if (timeOfDay < 17)
            {
                //fail safe should the section be empty..
                if (todaysReading.SecondReading == null || todaysReading.SecondReading.Equals("", StringComparison.CurrentCultureIgnoreCase))
                {

                    mDailyReadingTitle.TextFormatted = Html.FromHtml("<strong>Second Reading | None </strong>");
                    mDailyReadingContent.Text = GetString(Resource.String.app_name);

                }
                else
                {
                    string bookTitle;
                    bookTitle = string.Format("<strong>Second Reading | {0} </strong>",
                            todaysReading.SecondReading);

                    mDailyReadingTitle.TextFormatted = Html.FromHtml(bookTitle);
                    var secondReading = todaysReading.SecondReading.Split('.');

                    foreach (var book in mBible.Books)
                    {
                        if (book.BookName.ToLower().Contains(secondReading[0].Trim().ToLower()))
                        {
                            mDailyReadingBook = book;
                            break;
                        }
                    }
                    var bibleText = "";

                    //--------So we get the chapters if it contains a to (-) string --------
                    var chapterNumbers = new List<Integer>();
                    if (secondReading[1].Trim().Contains("-") && secondReading[1].Trim().Contains(":"))
                    {
                        var readingPlanChapter = secondReading[1].Split(':');
                        chapterNumbers.Add(Integer.ValueOf(readingPlanChapter[0].Trim()));
                    }
                    else
                    if (secondReading[1].Trim().Contains("-"))
                    {
                        var readingPlanChapter = secondReading[1].Split('-');
                        chapterNumbers.Add(Integer.ValueOf(readingPlanChapter[0].Trim()));
                        chapterNumbers.Add(Integer.ValueOf(readingPlanChapter[1].Trim()));
                    }
                    else
                    {
                        chapterNumbers.Add(Integer.ValueOf(secondReading[1].Trim()));
                    }
                    //-------------------- That's all folks --------------------------------


                    foreach (int chapter in chapterNumbers)
                    {

                        var chapters = mDailyReadingBook.BookChapter[chapter - 1];
                        bibleText += string.Format("<p><strong>Chapter. {0} </strong></p>", chapters.ChapterId);
                        foreach (var verse in chapters.ChapterVerses)
                        {
                            bibleText += string.Format("{0}. {1} <br/>", verse.Id, verse.VerseText);
                        }
                    }
                    mDailyReadingContent.TextFormatted = Html.FromHtml(bibleText);
                    mDailyReadingContent.SetOnTouchListener(this);
                }
            }
            else
            {

                if (todaysReading.SecondReading == null || todaysReading.SecondReading.Equals(""))
                {
                    mDailyReadingTitle.TextFormatted = (Html.FromHtml("<strong>Third Reading | None </strong>"));
                    mDailyReadingContent.Text = GetString(Resource.String.app_name);

                }
                else
                {
                    string bookTitle;

                    bookTitle = $"<strong>Third Reading | {todaysReading.ThirdReading} </strong>";

                    mDailyReadingTitle.TextFormatted = (Html.FromHtml(bookTitle));
                    var thirdReading = todaysReading.ThirdReading.Split('.');

                    foreach (var book in mBible.Books)
                    {
                        if (book.BookName.ToLower().Contains(thirdReading[0].Trim().ToLower()))
                        {

                            mDailyReadingBook = book;
                            break;
                        }
                    }
                    var bibleText = "";

                    //--------So we get the chapters if it contains a to (-) string --------
                    var chapterNumbers = new List<Integer>();

                    if (thirdReading[1].Trim().Contains("-"))
                    {
                        var readingPlanChapter = thirdReading[1].Split('-');
                        chapterNumbers.Add(Integer.ValueOf(readingPlanChapter[0].Trim()));
                        chapterNumbers.Add(Integer.ValueOf(readingPlanChapter[1].Trim()));
                    }
                    else
                    {
                        chapterNumbers.Add(Integer.ValueOf(thirdReading[1].Trim()));
                    }
                    //-------------------- That's all folks --------------------------------


                    foreach (int chapter in chapterNumbers)
                    {
                        Log.Debug(TAG, "Chapters to load" + (chapter - 1));
                        var chapters = mDailyReadingBook.BookChapter[chapter - 1];
                        bibleText += string.Format("<p><strong>Chapter. {0} </strong></p>", chapters.ChapterId);
                        try
                        {
                            foreach (var verse in chapters.ChapterVerses)
                            {
                                bibleText += string.Format("{0}. {1} <br/>", verse.Id, verse.VerseText);
                            }
                        }
                        catch (Exception ex)
                        {
                            // ex.printStackTrace();
                        }
                    }
                    mDailyReadingContent.TextFormatted = (Html.FromHtml(bibleText));
                }

            }
        }
        private void ShowCalender()
        {// Created a new Dialog

            var title = "Select A Date";
            var cancel = "Cancel";
            var ok = "Ok";

            var alertDialog = new AlertDialog.Builder(Activity);
            alertDialog.SetIcon(Resource.Drawable.logo);
            alertDialog.SetTitle(title);
            var factory = LayoutInflater.From(Activity);

            //View calenderView = factory.Inflate(Resource.Layout.resource_calender, null);
            //alertDialog.SetView(calenderView);


            //alertDialog.Show();

        }


        public bool OnTouch(View v, MotionEvent e)
        {
            return true;
            //throw new NotImplementedException();
        }
    }
}