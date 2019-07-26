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
            // Workaround for Colorful.Console issue https://github.com/tomakita/Colorful.Console/issues/16
            _resetDefaultColors();

            bool showHelp = false;
            string font = "standard";
            string color = "color";

            var options = new OptionSet()
            {
                { "h|help|?", v => { showHelp = true; } },
                { "f|font=", v => { font = v; } },
                { "c|color=", v => { color = v; } }
            };

            var parsed = options.Parse(args);

            Color renderColor = Color.FromName(color);
            if (!renderColor.IsKnownColor)
                renderColor = Console.ForegroundColor;

            Console.WriteLine($"[Loading font]: dotnet_figlet.Console.Fonts.{font}.flf");
            Console.WriteLine($"[Color]: {renderColor.Name}");

            var assembly = typeof(Program).GetTypeInfo().Assembly;
            Stream resource = assembly.GetManifestResourceStream($"dotnet_figlet.Console.Fonts.{font}.flf");

            FigletFont figletFont = FigletFont.Load(resource);
            Figlet figlet = new Figlet(figletFont);

            Console.WriteLine($"[Figlet font]: MaxLength={figletFont.MaxLength}, Height={figletFont.Height}, FullLayout={figletFont.FullLayout}, BaseLine={figletFont.BaseLine}");

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
                Console.WriteLine($"[Input]: {message}");
                Console.WriteLine($"{Console.WindowWidth} {Console.LargestWindowWidth}");

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

                foreach (string line in lines)
                {
                    Console.WriteLine( figlet.ToAscii(String.Join(" ", line)).ToString(), renderColor );
                }
            }

            if (showHelp)
            {
                _showHelp();
            }
            
            // TODO: calculate console width and rearrange the string tokens to newlines

            // Console.WriteLine(figlet.ToAscii(message).ToString(), renderColor);

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
        }

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
