using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButtonsReader
{
    class Program
    {
        static void Main(string[] args)
        {
            var solutionDir = "";
            if (args.Length == 0)
            {
                solutionDir = @"D:\Projects\VendoHelper\VSExt\CFIExtension\";
                //Console.WriteLine("Please enter a solution dir");
                //return;
            }
            else
            {
                solutionDir = args[0];
            }

            var desc = XmlButtonReader.GetDescription(solutionDir);
            StringWriter.WriteDescription(solutionDir, desc);
            StringWriter.WriteVersion(solutionDir);
        }
    }
}
