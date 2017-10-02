using CommandLine;
using CommandLine.Text;
using System.Text;

namespace CefScreenshot
{
    class Options
    {
        [Option('u', "url", Required = true, HelpText = "Url of page to screenshot")]
        public string Url { get; set; }

        [Option('o', "outputFile", DefaultValue = "", HelpText = "The path of the output file")]
        public string OutputFile { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var help = new HelpText
            {
                Heading = new HeadingInfo("CefScreenshot", "v0.1"),
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true
            };
            help.AddPreOptionsLine("Usage: CefScreenshot - u < url > [-o <path>]");
            help.AddOptions(this);
            return help;
        }
    }
}
