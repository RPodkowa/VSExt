using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButtonsReader
{
    public static class StringWriter
    {
        public static void WriteDescription(string solutionDir, List<string> description)
        {
            string dialogFile = solutionDir + @"CFIExtension\GUI\AboutDialog.cs";

            string text = File.ReadAllText(dialogFile);

            string _BodyToReplace = "//BodyToReplace";
            string _EndBodyToReplace = "//EndBodyToReplace";
            string NewLineString = "\r\n";

            if (!text.Contains(_BodyToReplace) || !text.Contains(_EndBodyToReplace))
                return;

            int startPos = text.IndexOf(_BodyToReplace);
            int stopPos = text.IndexOf(_EndBodyToReplace);
            stopPos += _EndBodyToReplace.Length;

            text = text.Remove(startPos, stopPos - startPos);

            var newBody = new List<string>();

            newBody.Add(_BodyToReplace);

            foreach (var d in description)
            {
                newBody.Add(string.Format("            desc.Add({0});", AddDoubleQuotes(d)));
            }

            newBody.Add("            " + _EndBodyToReplace);

            var newString = string.Join(NewLineString, newBody.ToArray());
            text = text.Insert(startPos, newString);

            File.WriteAllText(dialogFile, text, Encoding.UTF8);
        }

        public static void WriteVersion(string solutionDir)
        {
            string dialogFile = solutionDir + @"CFIExtension\GUI\AboutDialog.cs";

            string text = File.ReadAllText(dialogFile);

            string _BodyToReplace = "//VersionBodyToReplace";
            string _EndBodyToReplace = "//EndVersionBodyToReplace";
            string NewLineString = "\r\n";

            if (!text.Contains(_BodyToReplace) || !text.Contains(_EndBodyToReplace))
                return;

            int startPos = text.IndexOf(_BodyToReplace);
            int stopPos = text.IndexOf(_EndBodyToReplace);
            stopPos += _EndBodyToReplace.Length;

            text = text.Remove(startPos, stopPos - startPos);

            var newBody = new List<string>();

            newBody.Add(_BodyToReplace);
            newBody.Add(string.Format("            ret+={0};", AddDoubleQuotes(DateTime.Now.ToString(".yyyyMMdd"))));
            newBody.Add("            " + _EndBodyToReplace);

            var newString = string.Join(NewLineString, newBody.ToArray());
            text = text.Insert(startPos, newString);

            File.WriteAllText(dialogFile, text, Encoding.UTF8);
        }

        private static string AddDoubleQuotes(string value)
        {
            return "\"" + value + "\"";
        }
    }
}
