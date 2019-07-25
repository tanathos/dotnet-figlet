using Figgle;
using System;

namespace dotnet_figlet.console
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(FiggleFonts.Alligator.Render("Hello World!"));
        }
    }
}
