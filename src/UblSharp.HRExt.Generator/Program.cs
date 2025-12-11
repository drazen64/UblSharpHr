using System;
using System.IO;
using System.Xml.Schema;
using UblSharp.Generator;

namespace UblSharp.HRExt.Generator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var generator = new UblGenerator();

            generator.Generate(
                new UblGeneratorOptions()
                {
                    XsdBasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"maindoc\"),
                    OutputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\UblSharp.HRExt"),
                    Namespace = "UblSharp.HRExt",
                    ValidationHandler = ValidationHandler,
                    GenerateCommonFiles = false
                });

            Console.WriteLine("Done.");
        }

        private static void ValidationHandler(object sender, ValidationEventArgs e)
        {
            Console.WriteLine($"{e.Severity}: {e.Message}");
            if (e.Severity == XmlSeverityType.Error)
            {
                throw e.Exception;
            }
        }
    }
}
