using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using ResourceBibleStudyXamarin.Model;
using ResourceBibleStudyXamarin.Widget;
using System;
using Android.Content.Res;
using Android.Widget;
using Java.Lang;
using Fragment = Android.Support.V4.App.Fragment;
using Orientation = Android.Content.Res.Orientation;

namespace ResourceBibleStudyXamarin.Fragments
{
    public class BibleFragment : Fragment, View.IOnTouchListener
    { 
        private DraggableGridView mDraggableGridView;

        static int GRID_ROWS = 0;

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

            ShowProgressDialog("Loading Fragment");

            mTypeface = Typeface.CreateFromAsset(Activity.Assets, "OpenSans-Light.ttf");
            mDraggableGridView = ((DraggableGridView)bibleFragmentView.FindViewById(Resource.Id.bibleDraggableView));

            var fab = bibleFragmentView.FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            Configuration config = Resources.Configuration;

            if (config.Orientation == Orientation.Landscape)
            {
                GRID_ROWS = 4;
            }
            else
            {
                GRID_ROWS = 3;
            }
            SetUpDraggableViewForBible();

            return bibleFragmentView;
        }
        private void SetUpDraggableViewForBible()
        {

            mDraggableGridView.RemoveAllViews();
            foreach (var book in mBible.Books)
            {

                var view = new ImageView(Activity);

                view.SetImageBitmap(GetThumb(book.BookName));
                mDraggableGridView.AddView(view);
            }

            
           // SetUpFloatingBarAction(0);
        }
        private static Bitmap GetThumb(string s)
        {
            var bmp = Bitmap.CreateBitmap(150, 150,   Bitmap.Config.Rgb565);
            var canvas = new Canvas(bmp);
            var paint = new Paint
            {
                Color = (Color.Rgb(_random.Next(128), _random.Next(128), _random.Next(128))),
                TextSize = 24,
                Flags = PaintFlags.AntiAlias
            };


            canvas.DrawRect(new Rect(0, 0, 150, 150), paint);
            paint.Color = Color.White;
            paint.TextAlign =Paint.Align.Center; 
            canvas.DrawText(s, 75, 75, paint);

            return bmp;
        }

    private static void FabOnClick(object sender, EventArgs eventArgs)
        {
            var view = (View)sender;
             
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        }
        private void ShowProgressDialog(string message)
        {
            if (mProgressDialog == null)
            {

                mProgressDialog = new ProgressDialog(Activity) {Indeterminate = true};
                mProgressDialog.SetMessage(message); 
            }

            mProgressDialog.Show();
        }
        public bool OnTouch(View v, MotionEvent e)
        {
            return true;
            //throw new NotImplementedException();
        }
    }
}