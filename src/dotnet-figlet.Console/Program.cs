using Colorful;
using NDesk.Options;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using Console = Colorful.Console;

namespace dotnet_figlet.console
{
    class Program
    {
        static void Main(string[] args)
        {
            // Temp disabled, see below
            // _resetDefaultColors();

            bool showHelp = false;
            bool showPreview = false;
            string font = "standard";
            string color = "color";

            var options = new OptionSet()
            {
                { "h|help|?", v => { showHelp = true; } },
                { "f|font=", v => { font = v; } },
                { "c|color=", v => { color = v; } },
                { "p|preview", v => { showPreview = true; } }
            };

            var parsed = options.Parse(args);

            if (showHelp)
            {
                _showHelp();
            }

            if (showPreview)
            {
                _showPreview();
            }

            Color renderColor = Color.FromName(color);
            if (!renderColor.IsKnownColor)
                renderColor = Console.ForegroundColor;

#if DEBUG

            Console.WriteLine($"[Loading font]: dotnet_figlet.Console.Fonts.{font}.flf");
            Console.WriteLine($"[Color]: {renderColor.Name}");
#endif
            var assembly = typeof(Program).GetTypeInfo().Assembly;
            Stream resource = assembly.GetManifestResourceStream($"dotnet_figlet.Console.Fonts.{font}.flf");

            FigletFont figletFont = FigletFont.Load(resource);
            Figlet figlet = new Figlet(figletFont);

#if DEBUG

            Console.WriteLine($"[Figlet font]: MaxLength={figletFont.MaxLength}, Height={figletFont.Height}, FullLayout={figletFont.FullLayout}, BaseLine={figletFont.BaseLine}");

#endif
            List<string> extraparams;
            try
            {
                extraparams = options.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("Invalid option(s): ");
                Console.WriteLine(e.Message);
                return;
            }

            string message = "";

            if (extraparams.Count > 0)
            {
                message = String.Join(" ", extraparams.ToArray());

#if DEBUG

                Console.WriteLine($"[Input]: {message}");
                Console.WriteLine($"Console.WindowWidth: {Console.WindowWidth} Console.LargestWindowWidth: {Console.LargestWindowWidth}");

#endif

                int start = 0;
                int take = extraparams.Count;

                bool needsToBeSplitted = true;

                List<string> lines = new List<string>();

                while (needsToBeSplitted)
                {
                    if (take <= 0)
                    {
                        needsToBeSplitted = false;

                        if (start < extraparams.Count)
                            lines.Add(String.Join(" ", extraparams.ToArray()));
                    }
                    else
                    {
                        string[] subSentence = extraparams.Skip(start).Take(take).ToArray();

                        string line = String.Join(" ", subSentence);
                        bool subsentenceLengthOk = figlet.ToAscii(line).CharacterGeometry.GetLength(1) <= Console.WindowWidth;

                        if (subSentence.Length == 0)
                            needsToBeSplitted = false;
                        else if (subsentenceLengthOk)
                        {
                            lines.Add(line);

                            start = start + subSentence.Length;
                            take = extraparams.Count - start;
                        }
                        else
                        {
                            take--;
                        }
                    }
                }

                Console.ForegroundColor = renderColor;

                foreach (string line in lines)
                {
                    // Temp: by now I'll not use the Colorful.Console for the color itself as in Powershell there's a strange bug involving the background going to purple...
                    // Console.WriteLine( figlet.ToAscii(String.Join(" ", line)).ToString(), renderColor );
                    System.Console.WriteLine(figlet.ToAscii(String.Join(" ", line)).ToString());
                }

                Console.ResetColor();
            }

            Environment.Exit(0);
        }

        private static void _showHelp()
        {
            Console.WriteLine("Usage: figlet [options] <sentence to figlettize>");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("\t-h, --help\t\t\tShow this page");
            Console.WriteLine("\t-f, --font <font name>\t\tFiglet font to use for render, 'standard' is the default");
            Console.WriteLine("\t-c, --color <color name>\tColor to use");
            Console.WriteLine("\t-p, --preview\t\t\tShow a preview of all embedded fonts");
            Console.WriteLine();
        }

        private static void _showPreview()
        {
            var assembly = typeof(Program).GetTypeInfo().Assembly;
            string[] resourceNames = assembly.GetManifestResourceNames();

            List<string> fonts = resourceNames.Where(w => w.EndsWith(".flf")).ToList();

            foreach (string font in fonts)
            {
                string fontName = font.Replace("dotnet_figlet.Console.Fonts.", "");
                Stream resource = assembly.GetManifestResourceStream(font);
                FigletFont figletFont = FigletFont.Load(resource);
                Figlet figlet = new Figlet(figletFont);

                Console.WriteLine(fontName);
                Console.WriteLine(figlet.ToAscii("Lorem Ipsum"));
            }
        }

        /// <summary>
        /// Workaround for Colorful.Console issue https://github.com/tomakita/Colorful.Console/issues/16
        /// </summary>
        private static void _resetDefaultColors()
        {
            Console.Write("", Color.Navy);
            Console.Write("", Color.Green);
            Console.Write("", Color.Teal);
            Console.Write("", Color.Maroon);
            Console.Write("", Color.Purple);
            Console.Write("", Color.Olive);
            Console.Write("", Color.Silver);
            Console.Write("", Color.Gray);
            Console.Write("", Color.Blue);
            Console.Write("", Color.Lime);
            Console.Write("", Color.Cyan);
            Console.Write("", Color.Red);
            Console.Write("", Color.Fuchsia);
            Console.Write("", Color.Yellow);
        }
    }
}
