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

namespace ResourceBibleStudyXamarin
{

    public class App : Application
    {

        static App Instance;
        public static volatile Handler applicationHandler = null;


        public override void OnCreate()
        {
            base.OnCreate();

            Instance = this;

            applicationHandler = new Handler(Instance.MainLooper);

            //NativeLoader.initNativeLibs(App.getInstance());

        }

        public static App GetInstance()
        {
            return Instance;
        }
    }

}