using Colorful;
using NDesk.Options;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using Console = Colorful.Console;

namespace dotnet_figlet.console
{
    class Program
    {
        static void Main(string[] args)
        {
            bool showHelp = false;
            string font = "standard";

            var options = new OptionSet()
            {
                { "h|help|?", v => { showHelp = true; } },
                { "f|font=", v => { font = v; } }
            };

            var parsed = options.Parse(args);

            var assembly = typeof(Program).GetTypeInfo().Assembly;

            Console.WriteLine($"[Loading font]: dotnet_figlet.Console.Fonts.{font}.flf");

            Stream resource = assembly.GetManifestResourceStream($"dotnet_figlet.Console.Fonts.{font}.flf");

            FigletFont figletFont = FigletFont.Load(resource);
            Figlet figlet = new Figlet(figletFont);

            List<string> extra;
            try
            {
                extra = options.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("subclass: ");
                Console.WriteLine(e.Message);
                return;
            }

            string message = "";

            if (extra.Count > 0)
            {
                message = string.Join(" ", extra.ToArray());
                Console.WriteLine($"[Input]: {message}");
            }

            if (showHelp)
            {
                _showHelp();
            }

            Console.WriteLine(figlet.ToAscii(message).ToString(), Color.Blue);

            // Console.WriteLine(FiggleFonts.Alligator.Render(message));
        }

        private static void _showHelp()
        {
            Console.WriteLine("Usage: figlet <sentence to figlettize>");
        }
    }
}
