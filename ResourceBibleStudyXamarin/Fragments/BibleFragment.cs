using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using ResourceBibleStudyXamarin.Model;
using ResourceBibleStudyXamarin.Widget;
using System;
using Fragment = Android.Support.V4.App.Fragment;

namespace ResourceBibleStudyXamarin.Fragments
{
    public class BibleFragment : Fragment, View.IOnTouchListener
    {
        RecyclerView mRecyclerView;
        private DraggableGridView mDraggableGridView;


        private const string TAG = "BibleFragment"; 

        static Typeface mTypeface;

        private static Random _random = new Random();

        private FloatingActionButton mBibleFloatingActionButton;

        private static Chapters mSelectedChapter;
        private static Book mSelectedBook;
        public static Bible mBible;

        private ProgressDialog mProgressDialog;


        public BibleFragment()
        {
        }

        public static BibleFragment NewInstance(string param1, string param2)
        {
            var fragment = new BibleFragment();

            return fragment;
        }


        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            if (Arguments != null)
            {

            } 
            mBible = BibleHelper.GetBible(Activity);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container,
                                 Bundle savedInstanceState)
        {
            // Inflate the layout for this fragment
            var bibleFragmentView = inflater.Inflate(Resource.Layout.fragment_bible, container, false);  
             
            return bibleFragmentView;
        }


        private static void FabOnClick(object sender, EventArgs eventArgs)
        {
            var view = (View)sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        } 

        public bool OnTouch(View v, MotionEvent e)
        {
            return true;
            //throw new NotImplementedException();
        }
    }
}