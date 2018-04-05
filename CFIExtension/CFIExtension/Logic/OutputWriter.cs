using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFIExtension.Logic
{
    public class OutputWriter
    {
        private IVsOutputWindowPane outputWindowPane;
        private string accLine;
        private List<string> outputLines;

        public OutputWriter(string paneName, Guid paneGuid)
        {
            IVsOutputWindow outWindow = Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;
            outWindow.CreatePane(ref paneGuid, paneName, 1, 1);

            outWindow.GetPane(ref paneGuid, out outputWindowPane);
            outputWindowPane.Activate();

            ClearText();
        }

        public void ClearText()
        {
            accLine = "";
            outputLines = new List<string>();
        }

        public List<string> GetText()
        {
            return outputLines;
        }

        public void EmptyLine()
        {
            WriteCommon("", true);
        }

        public void WriteLine(string txt)
        {
            WriteCommon(txt, true);
        }

        public void Write(string txt)
        {
            WriteCommon(txt, false);
        }

        private void WriteCommon(string txt, bool writeLine)
        {
            accLine += txt;
            if (writeLine)
            {
                outputLines.Add(accLine);
                accLine = "";
                txt += "\r\n";
            }
            outputWindowPane.OutputStringThreadSafe(txt);
            //outputWindowPane.Activate();
        }
    }
}
