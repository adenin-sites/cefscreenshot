using CommandLine;
using CommandLine.Text;
using System;
using System.IO;
using System.Text;

namespace CefScreenshot
{
    class Options
    {
        [Option('u', "url", Required = true, HelpText = "Url of page to screenshot")]
        public string Url { get; set; }

        [Option('o', "outputFile", DefaultValue = "", HelpText = "The path of the output file")]
        public string OutputFile { get; set; }

        private string _cacheLocation;
        [Option("cacheLocation", DefaultValue = "", HelpText = "Location for the CEF cache, if not set it defaults to <local application data>\\CefSharp\\Cache")]
        public string cacheLocation
        {
            get
            {
                if (_cacheLocation == "")
                   return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Cache");
                return _cacheLocation;
            }
            set
            {
                _cacheLocation = value;
            }
        }

        [HelpOption]
        public string GetUsage()
        {
            var help = new HelpText
            {
                Heading = new HeadingInfo("CefScreenshot", "v0.2"),
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true
            };
            help.AddPreOptionsLine("Usage: CefScreenshot - u < url > [-o <path>]");
            help.AddOptions(this);
            return help;
        }
    }
}
