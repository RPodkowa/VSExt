using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CFIExtension.Logic
{
    public class TextEditorHelper
    {
        private const string mainCodePart = "\\Amag\\";
        private const int standardFindOptions = (int)(vsFindOptions.vsFindOptionsNone);
        private DTE2 dte;

        public TextEditorHelper(Package package)
        {            
            dte = ((IServiceProvider)package).GetService(typeof(DTE)) as DTE2;     
        }

        public void CopySelectionAdress()
        {
            Clipboard.SetText(GetSelectionAdress());
        }

        private string GetSelectionAdress()
        {
            string ret = "";

            //File patch
            var fileName = dte.ActiveDocument.FullName;

            //Line number
            TextDocument doc = (TextDocument)(dte.ActiveDocument.Object("TextDocument"));
            var lnNumber = doc.Selection.CurrentLine;
            var sel = doc.Selection;
            //
            ret += string.Format("{0} (linia: {1})", fileName, lnNumber);
            return ret;
        }

        public void InsertGuidCPP()
        {
            TextDocument doc = (TextDocument)(dte.ActiveDocument.Object("TextDocument"));
            
            var guidString = GetGuidString();
            var guidName = GetGuidName(guidString);
            var guidImplementation = GetGuidImplementation(guidName, guidString);

            var cursor = FindLast(doc, "static const GUID");
            bool isFirst = false;
            if (cursor == null)
            {
                cursor = FindLast(doc, "#include");
                isFirst = false;
            }

            if (cursor == null)
                return;
                        
            //------------------------------------------------------------------
            //  Undo Context 
            var undoContext = dte.UndoContext;

            undoContext.Open("Insert GUID " + guidName);

            cursor.EndOfLine();

            cursor.Insert("\r\n");
            if (isFirst) cursor.Insert("\r\n//GUID\r\n");
            cursor.Insert(guidImplementation);

            doc.Selection.Text = guidName;

            undoContext.Close();
            //------------------------------------------------------------------
        }

        private List<EditPoint> FindAll(TextDocument doc, string toFind)
        {
            var matches = new List<EditPoint>();
            var cursor = doc.StartPoint.CreateEditPoint();
            EditPoint end = null;
            TextRanges dummy = null;

            while (cursor != null && cursor.FindPattern(toFind, standardFindOptions, ref end, ref dummy))
            {
                matches.Add(cursor.CreateEditPoint());
                cursor = end;
            }

            return matches;
        }

        private EditPoint FindLast(TextDocument doc, string toFind)
        {
            var matches = FindAll(doc, toFind);
            if (matches.Count <= 0) return null;

            EditPoint lastPoint = null;
            foreach (var m in matches)
            {
                if (lastPoint == null || m.Line > lastPoint.Line)
                    lastPoint = m;
            }

            return lastPoint;
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
            ret += GetProjectName();

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

        private string GetProjectName()
        {
            string projName = dte.ActiveDocument.FullName;
            var startIndex = projName.IndexOf(mainCodePart);
            startIndex += mainCodePart.Length;
            var stopIndex = projName.IndexOf("\\", startIndex);
            projName = projName.Substring(startIndex, stopIndex - startIndex);
            return projName;
        }
    }
}
