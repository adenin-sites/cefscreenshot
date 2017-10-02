using CefSharp;
using CefSharp.OffScreen;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CefScreenshot
{
    public class Program
    {
        private static ChromiumWebBrowser browser;
        private static Options options;

        public static void Main(string[] args)
        {
            options = new Options();
            if (!CommandLine.Parser.Default.ParseArguments(args, options))
            {
                //Show the help screen and return if option requirements are not met.
                return;
            }

            var settings = new CefSharp.CefSettings()
            {
                //By default CefSharp will use an in-memory cache, you need to specify a Cache Folder to persist data
                CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Cache")
            };

            //Perform dependency check to make sure all relevant resources are in our output directory.
            Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);

            // Create the offscreen Chromium browser.
            browser = new ChromiumWebBrowser(options.Url);

            // An event that is fired when the first page is finished loading.
            // This returns to us from another thread.
            browser.LoadingStateChanged += BrowserLoadingStateChanged;

            // We have to wait for something, otherwise the process will exit too soon.
            /* Jacob: After the basic prototype is done, this needs to be changed to a non-user interactive wait. Probably waiting for the screenshot to be done in some thread-safe manner */
            Console.ReadKey();

            // Clean up Chromium objects.  You need to call this in your application otherwise
            // you will get a crash when closing.
            Cef.Shutdown();
        }

        private static void BrowserLoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            // Check to see if loading is complete - this event is called twice, one when loading starts
            // second time when it's finished
            // (rather than an iframe within the main frame).
            if (!e.IsLoading)
            {
                // Remove the load event handler, because we only want one snapshot of the initial page.
                browser.LoadingStateChanged -= BrowserLoadingStateChanged;

                //Give the browser a little time to render
                /* Jacob: See if there is a way to wait for DomContentLoad event? */
                Thread.Sleep(500);
                // Wait for the screenshot to be taken.
                var task = browser.ScreenshotAsync();
                task.ContinueWith(x =>
                {

                    // Make a file to save it to
                    string screenshotPath;
                    if (options.OutputFile != "")
                    {
                        screenshotPath = options.OutputFile;
                    }
                    else
                    {
                        screenshotPath = Path.Combine(System.IO.Path.GetTempPath(), DateTime.Now.ToString("yyyyMMddHHmmss")+".png");
                    }

                    Console.WriteLine();
                    Console.WriteLine("Screenshot ready. Saving to {0}", screenshotPath);

                    // Save the Bitmap to the path.
                    // The image type is auto-detected via the ".png" extension.
                    task.Result.Save(screenshotPath);

                    // We no longer need the Bitmap.
                    // Dispose it to avoid keeping the memory alive.  Especially important in 32-bit applications.
                    task.Result.Dispose();

                    Console.WriteLine("Press any key to exit.");
                }, TaskScheduler.Default);
            }
        }
    }
}
