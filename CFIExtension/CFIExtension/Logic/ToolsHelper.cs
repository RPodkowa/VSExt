using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;
using EnvDTE;
using System.IO;
using System.Diagnostics;
using Microsoft.VisualStudio.Shell.Interop;
using System.Globalization;

namespace CFIExtension.Logic
{
    public class ToolsHelper
    {
        private string solutionDir;
        private OutputWriter outputWriter;

        public ToolsHelper(Package package)
        {
            DTE dte = (DTE)((IServiceProvider)package).GetService(typeof(DTE));

            solutionDir = Path.GetDirectoryName(dte.Solution.FullName);
            outputWriter = new OutputWriter("CFI", new Guid("0F44E2D1-F5FA-4d2d-AB30-22BE8ECD9789"));
        }

        public async void CopyAsync()
        {
            bool ret = await TaksCopy();
        }

        private Task<bool> TaksCopy()
        {
            return Task<bool>.Factory.StartNew(() => Copy());
        }

        private bool Copy()
        {
            string toolsPath = solutionDir + @"\#Tools\";
            string program = toolsPath + "SCopy.exe";
            string arguments = string.Format("@\"release.copylist\" /SolutionDir:\"{0}\"", solutionDir);
            return StartProcess(program, arguments, toolsPath);
        }

        public async void RunAsync(string mode)
        {
            bool ret = await TaksRun(mode);
        }

        private Task<bool> TaksRun(string mode)
        {
            return Task<bool>.Factory.StartNew(() => Run(mode));
        }

        private bool Run(string mode)
        {
            var version = XmlSettingsReader.GetVersionInfo(solutionDir);

            string program = solutionDir + @"\Release\Amag.App.exe";
            string arguments = GetRunArguments(version, mode);
            return StartLongProcess(program, arguments);
        }

        public async void CopyRunAsync(string mode)
        {
            bool ret = await TaksCopyRun(mode);
        }

        private Task<bool> TaksCopyRun(string mode)
        {
            return Task<bool>.Factory.StartNew(() => CopyRun(mode));
        }

        private bool CopyRun(string mode)
        {
            return Copy() && Run(mode);
        }

        public async void UpdateAmagDataAsync()
        {
            bool ret = await TaksUpdateAmagData();
        }

        private Task<bool> TaksUpdateAmagData()
        {
            return Task<bool>.Factory.StartNew(() => UpdateAmagData());
        }

        private bool UpdateAmagData()
        {
            string toolsPath = solutionDir + @"\#Tools\";
            string program = toolsPath + "Update Amag.Data.bat";
            string arguments = solutionDir;
            return StartProcess(program, arguments, solutionDir, true, true);
        }
        public async void ENumsAsync()
        {
            bool ret = await TaksENums();
        }

        private Task<bool> TaksENums()
        {
            return Task<bool>.Factory.StartNew(() => ENums());
        }

        private bool ENums()
        {
            string path = solutionDir + @"\Amag\";
            string program = path + "ENums.exe";
            string arguments = "";
            return StartProcess(program, arguments);
        }

        private bool StartLongProcess(string fileName, string arguments, string workingDirectory = null)
        {
            return StartProcess(fileName, arguments, workingDirectory, false);
        }

        private bool StartProcess(string fileName, string arguments, string workingDirectory = null, bool waitForEnd = true, bool waitForEnter = false)
        {
            if (!File.Exists(fileName))
                return false;

            var stopWatch = new Stopwatch();

            outputWriter.EmptyLine();
            outputWriter.EmptyLine();

            outputWriter.WriteLine(fileName + " " + arguments);

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.FileName = fileName;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            if (waitForEnter) process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            if (!string.IsNullOrEmpty(workingDirectory)) process.StartInfo.WorkingDirectory = workingDirectory;

            stopWatch.Start();

            outputWriter.EmptyLine();
            outputWriter.WriteLine("Start");
            outputWriter.EmptyLine();

            process.Start();
                        
            while (waitForEnd && !process.StandardOutput.EndOfStream)
            {
                string line = process.StandardOutput.ReadLine();
                string timestamp = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
                outputWriter.Write(timestamp + ": ");
                outputWriter.WriteLine(line);

                if (waitForEnter && process.StandardOutput.Peek() < 0)
                  process.StandardInput.WriteLine();
            }

            stopWatch.Stop();

            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format(" [{0:00}:{1:00}:{2:00}.{3:00}]", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);

            outputWriter.EmptyLine();
            if (waitForEnd) outputWriter.WriteLine("Stop" + elapsedTime);
            else outputWriter.WriteLine("Started" + elapsedTime);
            return true;
        }

        private string GetRunArguments(Xml.Version version, string mode)
        {
            if (string.IsNullOrEmpty(version.DB.User)) version.DB.User = "axuser";
            if (string.IsNullOrEmpty(version.DB.Password)) version.DB.Password = "abaxpmags";

            List<string> aruments = new List<string>();
            aruments.Add(version.Launcher);
            aruments.Add("0");
            aruments.Add(AddDoubleQuotes(mode));
            aruments.Add(AddDoubleQuotes(""));
            aruments.Add(AddDoubleQuotes(version.DB.Name));
            aruments.Add(AddDoubleQuotes(version.DB.Host));
            aruments.Add(AddDoubleQuotes(version.DB.Name));
            aruments.Add(AddDoubleQuotes(version.DB.User));
            aruments.Add(AddDoubleQuotes(version.DB.Password));
            aruments.Add(AddDoubleQuotes(version.Vendo.User));
            aruments.Add(AddDoubleQuotes(version.Vendo.Password));
            aruments.Add("-pass_not_enc");
            aruments.Add("-ignorepatchcheck");

            return string.Join(" ", aruments.ToArray());
        }
        private string AddDoubleQuotes(string value)
        {
            return "\"" + value + "\"";
        }
    }
}
