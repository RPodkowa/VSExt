using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CFIExtension.Logic
{
    public class GuidGenerator
    {
        private const string GenerateString = "GENUUIDL";
        private const string NewLineString = "\r\n";
        private string fileName;
        private string mainCodePart;
        
        public GuidGenerator(string fileName)
        {
            this.fileName = fileName;
            mainCodePart = "\\Amag\\";
        }

        public void TryGenerate()
        {
            string text = File.ReadAllText(fileName);
            bool saveFile = false;

            while (HasStringToGenerate(text))
            {
                saveFile = true;
                text = GetNewFileText(text);
            }
                        
            if (saveFile) File.WriteAllText(fileName, text, Encoding.UTF8);            
        }

        private bool HasStringToGenerate(string txt)
        {
            return txt.Contains(GenerateString);
        }

        private string GetNewFileText(string txt)
        {
            bool createSection;
            var implIndex = GetImplementationPosition(txt, out createSection);
            var guidIndex = GetGuidPosition(txt);

            var guidString = GetGuidString();
            var guidName = GetGuidName(guidString);
            var guidImplementation = GetGuidImplementation(guidName, guidString);            

            return Replace(txt, implIndex, guidIndex, guidName, guidImplementation, createSection);
        }

        private string Replace(string txt, int implIndex, int guidIndex, string guidName, string guidImpl, bool createSection)
        {
            if (guidIndex < implIndex)
            {
                txt = InsertImplementationString(txt, implIndex, guidImpl, createSection);
                txt = ReplaceGuidString(txt, guidIndex, guidName);
            }
            else
            {
                txt = ReplaceGuidString(txt, guidIndex, guidName);
                txt = InsertImplementationString(txt, implIndex, guidImpl, createSection);
            }

            return txt;
        }

        private string InsertImplementationString(string txt, int implIndex, string guidImpl, bool createSection)
        {
            string implementationString = guidImpl + NewLineString;
            if (createSection)
                implementationString = NewLineString + "//GUID" + NewLineString + implementationString;

            txt = txt.Insert(implIndex, implementationString);
            return txt;
        }

        private string ReplaceGuidString(string txt, int guidIndex, string guidName)
        {
            txt = txt.Remove(guidIndex, GenerateString.Length);
            txt = txt.Insert(guidIndex, guidName);
            return txt;
        }

        private int GetImplementationPosition(string txt, out bool createSection)
        {
            createSection = false;

            int index = 0;
            // 1. Juz wystepuje 'static const GUID' - daje na koncu
            if (index <= 0) index = txt.LastIndexOf("static const GUID");

            // 2. Nie wystepuje 'static const GUID' - daje na koncu #include
            if (index <= 0)
            {
                index = txt.LastIndexOf("#include");
                createSection = true;
            }

            if (index != 0)
            {
                index = txt.IndexOf(NewLineString, index);
                index += NewLineString.Length;
            }

            return index;
        }

        private int GetGuidPosition(string txt)
        {
            int index = txt.IndexOf(GenerateString);
            return index;
        }

        private string GetGuidString()
        {
            Guid g; g = Guid.NewGuid();

            string ret = g.ToString("X");
            ret = ret.Replace(",", ", ");
            ret = ret.Replace("{", "{ ");
            ret = ret.Replace("}", " }");
            ret = ret.ToUpper();
            ret = ret.Replace("0X", "0x");

            return ret;
        }

        //static const GUID UUREPORTADDINSRADFD646823 = { 0xFD646823, 0x3EA9, 0x4923, { 0xAB, 0x76, 0x7B, 0x3E, 0xCA, 0x29, 0x74, 0xBD } };
        private string GetGuidName(string guid)
        {
            // UU
            string ret = "UU";

            // Projekt
            string projName = fileName;            
            var startIndex = projName.IndexOf(mainCodePart);
            startIndex += mainCodePart.Length;
            var stopIndex = projName.IndexOf("\\", startIndex);
            projName = projName.Substring(startIndex, stopIndex - startIndex);
            ret += projName;

            //User
            string userName = Environment.UserName;
            if (userName.Length >= 3) userName = userName.Substring(0, 3);
            ret += userName;

            // Guid
            string guidString = guid;
            guidString = guidString.Substring(4, 8);
            ret += guidString;

            ret = ret.ToUpper();
            return ret;
        }

        private string GetGuidImplementation(string guidName, string guid)
        {
            return string.Format("static const GUID {0} = {1};", guidName, guid);
        }
    }
}
