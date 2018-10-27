using Android.Graphics;
using Java.IO;
using Java.Lang;
using Java.Nio.Channels;
using Math = System.Math;

namespace ResourceBibleStudyXamarin.Utils
{


    public class AndroidUtilities
    {

        public static float density = 1;
        public static int statusBarHeight = 0;
        public static Point displaySize = new Point();

        public AndroidUtilities()
        {
            density = App.GetInstance().Resources.DisplayMetrics.Density;
            CheckDisplaySize();

        }
      
    

    public static int dp(float value)
    {
        return (int)Math.Ceiling(density * value);
    }

    public static void RunOnUIThread(Runnable runnable)
    {
        RunOnUIThread(runnable, 0);
    }

    public static void RunOnUIThread(Runnable runnable, long delay)
    {
        if (delay == 0)
        {
            App.applicationHandler.Post(runnable);
        }
        else
        {
            App.applicationHandler.PostDelayed(runnable, delay);
        }
    }


        public static bool CopyFile(File sourceFile, File destFile)
        {
            if (!destFile.Exists())
            {
                destFile.CreateNewFile();
            }

            FileChannel source = null;
            FileChannel destination = null;
            try
            {
                source = new FileInputStream(sourceFile).Channel;
                destination = new FileOutputStream(destFile).Channel;
                destination.TransferFrom(source, 0, source.Size());
            }
            catch (Exception e)
            {
                //FileLog.e("tmessages", e);
                return false;
            }
            finally
            {
                if (source != null)
                {
                    source.Close();
                }

                if (destination != null)
                {
                    destination.Close();
                }

            }
            return false;
        }

        public static void CheckDisplaySize()
{
    try
    {
         
    }
    catch (Exception e)
    {
    }
}
 



}

}