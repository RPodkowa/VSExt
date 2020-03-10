using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.Shell;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;

namespace CFIExtension.Logic
{
    public class FileHelper
    {
        private string solutionDir;
        private string notepadPath;

        private FileHelper(Package package)
        {
            DTE dte = (DTE)((IServiceProvider)package).GetService(typeof(DTE));
            solutionDir = System.IO.Path.GetDirectoryName(dte.Solution.FullName);
            notepadPath = (package as CFIPackage).OptionNotepadPath;
        }

        public static void OpenNotepad(Package package, string fileName)
        {
            var fh = new FileHelper(package);
            fh.OpenNotepad(fileName);
        }

        private void OpenNotepad(string fileName)
        {
            System.Diagnostics.Process.Start(notepadPath, GetFileFullPath(fileName));
        }

        public static void GoToLocation(Package package, string location)
        {
            var fh = new FileHelper(package);
            fh.GoToLocation(location);
        }

        private void GoToLocation(string location)
        {
            System.Diagnostics.Process.Start(GetFileFullPath(location));
        }

        private string GetFileFullPath(string fileName)
        {
            string ret = solutionDir;
            switch (fileName)
            {
                case "macros.xml": ret += @"\Release\etykiety\macros.xml"; break;
                case "changes.lst": ret += @"\Amag\sql\changes.lst"; break;
                case "changes.lst(MRP)": ret += @"\Amag\sql\mrp\changes.lst"; break;
                case "facts.txt": ret += @"\Amag\Amag.App\Resources\facts.txt"; break;
                case "icons": ret += @"\Amag\Amag.UI.Core\Resources\Icons"; break;
                default:
                    return "";
            }

            return ret;
        }
    }
}
