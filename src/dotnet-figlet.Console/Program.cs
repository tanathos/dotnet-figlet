using Colorful;
using NDesk.Options;
using System;
using System.Collections.Generic;
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
            bool showHelp = false;
            bool showPreview = false;
            string font = "standard";
            string fileOutput = null;

            bool wroteToFile = false;

            var options = new OptionSet()
            {
                { "h|help|?", v => { showHelp = true; } },
                { "f|font=", v => { font = v; } },
                { "o|output=", filename => {
                    if (filename == null || (filename.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0))
                    {
                        Console.WriteLine("Invalid output file name");
                        Environment.Exit(0);
                    }

                    fileOutput = filename;
                } },
                { "p|preview", v => { showPreview = true; } }
            };

            var parsed = options.Parse(args);

            if (showHelp)
            {
                _showHelp();
                Environment.Exit(0);
            }

            if (showPreview)
            {
                _showPreview();
                Environment.Exit(0);
            }

#if DEBUG

            Console.WriteLine($"[Loading font]: dotnet_figlet.Console.Fonts.{font}.flf");
#endif

            var assembly = typeof(Program).GetTypeInfo().Assembly;
            Stream resource = assembly.GetManifestResourceStream($"dotnet_figlet.Console.Fonts.{font}.flf");

            if (resource == null)
            {
                Console.WriteLine($"Unknow font: {font}");
                Console.WriteLine("To see a list of available fonts use figlet -p");

                Environment.Exit(0);
            }

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

                List<string> tokens = new List<string>();
                foreach (string extraparam in extraparams)
                {
                    tokens.AddRange(extraparam.Split(' ').ToList());
                }

#if DEBUG

                Console.WriteLine($"[Input]: {message}");
                Console.WriteLine($"Console.WindowWidth: {Console.WindowWidth} Console.LargestWindowWidth: {Console.LargestWindowWidth}");

#endif

                int start = 0;
                int take = tokens.Count;

                bool needsToBeSplitted = true;

                List<string> lines = new List<string>();

                while (needsToBeSplitted)
                {
                    if (take <= 0)
                    {
                        needsToBeSplitted = false;

                        if (start < tokens.Count)
                            lines.Add(String.Join(" ", tokens.ToArray()));
                    }
                    else
                    {
                        string[] subSentence = tokens.Skip(start).Take(take).ToArray();

                        string line = String.Join(" ", subSentence);
                        bool subsentenceLengthOk = figlet.ToAscii(line).CharacterGeometry.GetLength(1) <= (Console.WindowWidth - 1);

                        if (subSentence.Length == 0)
                            needsToBeSplitted = false;
                        else if (subsentenceLengthOk)
                        {
                            lines.Add(line);

                            start = start + subSentence.Length;
                            take = tokens.Count - start;
                        }
                        else
                        {
                            take--;
                        }
                    }
                }

                TextWriter defaultOutput = Console.Out;
                FileStream stream = null;
                StreamWriter writer = null;

                if (fileOutput != null)
                {
                    try
                    {
                        stream = new FileStream(fileOutput, FileMode.OpenOrCreate, FileAccess.Write);
                        writer = new StreamWriter(stream);

                        wroteToFile = true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        Environment.Exit(0);
                    }

                    Console.SetOut(writer);
                }

                foreach (string line in lines)
                {
                    Console.WriteLine(figlet.ToAscii(String.Join(" ", line)).ToString());
                }

                if (wroteToFile)
                {
                    Console.SetOut(defaultOutput);

                    writer.Close();
                    stream.Close();
                }
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
    }
}
