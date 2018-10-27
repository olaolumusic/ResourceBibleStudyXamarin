
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Widget;
using Java.Lang;
using ResourceBibleStudyXamarin.Fragments;
using Fragment = Android.App.Fragment;
using FragmentManager = Android.App.FragmentManager;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace ResourceBibleStudyXamarin
{
    [Activity(Label = "BibleActivity", MainLauncher = true)]
    public class BibleActivity : AppCompatActivity
    {

        private DrawerLayout mDrawerLayout;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_bible);

            mDrawerLayout = (DrawerLayout) FindViewById(Resource.Id.drawer_layout);

            var navigationView = (NavigationView) mDrawerLayout.FindViewById(Resource.Id.navigation_view);

            navigationView.NavigationItemSelected += (sender, args) =>
            {
                var menuItem = args.MenuItem;

                menuItem.SetChecked(true);
                mDrawerLayout.CloseDrawers();

                switch (menuItem.TitleFormatted.ToString().ToLower())
                {
                    case "discussion forum":

                        StartActivity(new Intent(ApplicationContext, typeof(MainActivity)));
                        break;

                    default:
                        Toast.MakeText(this, menuItem.TitleFormatted, ToastLength.Long);
                        break;

                }

            };

            var toolbar = (Toolbar) FindViewById(Resource.Id.toolbar);

            SetSupportActionBar(toolbar);
            var actionBar = SupportActionBar;
            actionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu);
            actionBar.SetDisplayHomeAsUpEnabled(true);

            var adapter = new BiblePagerAdapter(SupportFragmentManager);
            var viewPager = (ViewPager) FindViewById(Resource.Id.viewpager);
            viewPager.Adapter = adapter;

            var tabLayout = (TabLayout) FindViewById(Resource.Id.tablayout);
            tabLayout.SetupWithViewPager(viewPager);
        }


        public class BiblePagerAdapter : FragmentStatePagerAdapter
        {

            public BiblePagerAdapter(Android.Support.V4.App.FragmentManager fm) : base(fm)
            {

            }

            public override ICharSequence GetPageTitleFormatted(int position)
            {
                var title = "";
                switch (position)
                {
                    case 0:
                        title = "Study Plan";
                        break;
                    case 1:
                        title = "Bible";
                        break;
                }

                return new String(title);
            }


            public override Android.Support.V4.App.Fragment GetItem(int position)
            {
                Android.Support.V4.App.Fragment fragment = null;
                if (position == 0)
                {

                    fragment = new DailyScriptureFragment();
                }

                if (position == 1)
                {
                    //fragment = new BibleFragment();
                }

                return fragment;
            }

            public override int Count => 2;


        }

    }
}