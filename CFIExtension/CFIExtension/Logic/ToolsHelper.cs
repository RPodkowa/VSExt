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
        private class ToolsHelperResult
        {
            public string message;
            public bool result;

            public ToolsHelperResult(bool ret)
            {
                message = ret ? "" : "Nieznany błąd operacji!";
                result = ret;
            }

            public ToolsHelperResult(bool result, string message)
            {
                this.result = result;
                this.message = message;
            }

            public void Show(IServiceProvider serviceProvider)
            {
                ShowCommon(serviceProvider, false);
            }

            public void ShowIfError(IServiceProvider serviceProvider)
            {
                ShowCommon(serviceProvider, true);
            }

            private void ShowCommon(IServiceProvider serviceProvider, bool ifError)
            {
                if (ifError && result)
                    return;

                OLEMSGICON icon = OLEMSGICON.OLEMSGICON_INFO;
                if (!result) icon = OLEMSGICON.OLEMSGICON_CRITICAL;

                VsShellUtilities.ShowMessageBox(serviceProvider, message, "", icon, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            }
        }

        private string solutionDir;
        private OutputWriter outputWriter;
        private IServiceProvider serviceProvider;
        
        public ToolsHelper(Package package)
        {
            serviceProvider = (IServiceProvider)package;
            DTE dte = (DTE)(serviceProvider).GetService(typeof(DTE));

            solutionDir = Path.GetDirectoryName(dte.Solution.FullName);
            outputWriter = new OutputWriter("CFI", new Guid("0F44E2D1-F5FA-4d2d-AB30-22BE8ECD9789"));
        }
        
        public async void CopyAsync()
        {
            var ret = await TaksCopy();

            ret.Show(serviceProvider);
        }

        private Task<ToolsHelperResult> TaksCopy()
        {
            return Task<ToolsHelperResult>.Factory.StartNew(() => Copy());
        }

        private ToolsHelperResult Copy()
        {
            string toolsPath = solutionDir + @"\#Tools\";
            string program = toolsPath + "SCopy.exe";
            string arguments = string.Format("@\"release.copylist\" /SolutionDir:\"{0}\"", solutionDir);
            if (!StartProcess(program, arguments, toolsPath))
                return new ToolsHelperResult(false);

            return GetCopyResult();
        }

        private ToolsHelperResult GetCopyResult()
        {
            var txt = outputWriter.GetText();

            //10:13:34.106: Copied: 0, Skipped: 288, Errors: 1
            string status = "";
            foreach (var line in txt)
            {
                if (line.Contains("Copied:") && line.Contains("Skipped:") && line.Contains("Errors:"))
                    status = line;
            }

            if (string.IsNullOrEmpty(status))
                return new ToolsHelperResult(false, "Brak podsumowania operacji!");

            status = status.Replace(",", "");
            List<string> result = status.Split(' ').ToList();
            var i = result.FindIndex(x => x == "Errors:") + 1;
            if (i >= result.Count)
                return new ToolsHelperResult(false, string.Format("Brak informacji o błędach! ({0})", status));

            var errors = int.Parse(result[i]);
            if (errors > 0)
                return new ToolsHelperResult(false, string.Format("Wystąpiły błędy przy kopiowaniu! ({0})", errors));

            return new ToolsHelperResult(true, status);
        }

        public async void RunAsync(string mode)
        {
            var ret = await TaksRun(mode);
            ret.ShowIfError(serviceProvider);
        }

        private Task<ToolsHelperResult> TaksRun(string mode)
        {
            return Task<ToolsHelperResult>.Factory.StartNew(() => Run(mode));
        }

        private ToolsHelperResult Run(string mode)
        {
            var version = XmlSettingsReader.GetVersionInfo(solutionDir);

            string program = solutionDir + @"\Release\Amag.App.exe";
            string arguments = GetRunArguments(version, mode);
            return new ToolsHelperResult(StartLongProcess(program, arguments));
        }

        public async void CopyRunAsync(string mode)
        {
            var ret = await TaksCopyRun(mode);
            ret.ShowIfError(serviceProvider);
        }

        private Task<ToolsHelperResult> TaksCopyRun(string mode)
        {
            return Task<ToolsHelperResult>.Factory.StartNew(() => CopyRun(mode));
        }

        private ToolsHelperResult CopyRun(string mode)
        {
            var ret = Copy();
            if (!ret.result) return ret;
            
            return Run(mode);
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
