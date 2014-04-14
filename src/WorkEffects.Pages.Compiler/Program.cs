using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkEffects.Pages.Engine;

namespace WorkEffects.Pages.Compiler
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = ConfigurationManager.AppSettings;
            var contentPath = settings["contentPath"];
            var outputPath = settings["outputPath"];

            var generator = new SiteGenerator(contentPath, outputPath) {EmitMessage = Console.WriteLine};
            generator.Run();

            Console.WriteLine("\r\nDone!");
            Console.ReadKey();
        }
    }
}
