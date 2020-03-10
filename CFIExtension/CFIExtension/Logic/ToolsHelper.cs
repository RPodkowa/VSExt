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
        
        private ToolsHelper(Package package)
        {
            serviceProvider = (IServiceProvider)package;
            DTE dte = (DTE)(serviceProvider).GetService(typeof(DTE));

            solutionDir = Path.GetDirectoryName(dte.Solution.FullName);
            outputWriter = new OutputWriter("CFI", new Guid("0F44E2D1-F5FA-4d2d-AB30-22BE8ECD9789"));
        }

        public static void CopyRelease(Package package)
        {
            var vh = new ToolsHelper(package);
            vh.CopyAsync();
        }

        private async void CopyAsync()
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

        public static void Run(Package package, string mode)
        {
            var vh = new ToolsHelper(package);
            vh.RunAsync(mode);
        }

        private async void RunAsync(string mode)
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

            string program = solutionDir + @"\Release\" + GetApplicationName(mode);
            string arguments = GetRunArguments(version, mode);
            return new ToolsHelperResult(StartLongProcess(program, arguments));
        }

        public static void CopyRun(Package package, string mode)
        {
            var vh = new ToolsHelper(package);
            vh.CopyRunAsync(mode);
        }

        private async void CopyRunAsync(string mode)
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

        public static void RunRepository(Package package, string mode)
        {
            var vh = new ToolsHelper(package);
            vh.RunRepositoryAsync(mode);
        }

        private async void RunRepositoryAsync(string mode)
        {
            var ret = await TaksRunRepository(mode);
            ret.ShowIfError(serviceProvider);
        }

        private Task<ToolsHelperResult> TaksRunRepository(string mode)
        {
            return Task<ToolsHelperResult>.Factory.StartNew(() => RunRepository(mode));
        }

        private ToolsHelperResult RunRepository(string mode)
        {
            var version = XmlSettingsReader.GetVersionInfo(solutionDir);

            string program = GetRepositoryExe(version, mode);
            if (string.IsNullOrEmpty(program))
                return new ToolsHelperResult(false, "Brak repozytorium!"); ;



            string arguments = GetRunArguments(version, mode);
            return new ToolsHelperResult(StartLongProcess(program, arguments));
        }

        public static void UpdateAmagData(Package package)
        {
            var vh = new ToolsHelper(package);
            vh.UpdateAmagDataAsync();
        }

        private async void UpdateAmagDataAsync()
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

            //DTE dte2 = (DTE)(serviceProvider).GetService(typeof(DTE));
            //var files = new List<string>();

            //List<string> returnValue = new List<string>();
            //foreach (Project project in dte2.Solution.Projects)
            //{
            //    files.Add(project.UniqueName);
            //    string Name = project.Name;
            //    string FileName = project.FileName;
            //    string Kind = project.Kind;
            //    string UniqueName = project.UniqueName;
            //    string ExtenderCATID = project.ExtenderCATID;
            //    string FullName = project.FullName;
            //    if (Name != "Logic") continue;


            //    returnValue.AddRange(GetAllProjectFiles(Name, project.ProjectItems));
            //}

            if (!StartProcess(program, arguments, solutionDir, true, true))
                return false;

            return AfterUpdateAmagData();
        }

        public static List<string> GetAllProjectFiles(string sufix, ProjectItems projectItems)
        {

            //dte2.Solution.AddFromFile(@"D:\Projects\Vendo\Master\Amag\Amag.Data\Config\Map\rp_test_tab.xml");

            List<string> returnValue = new List<string>();
            
            if (projectItems !=null)
            {
                foreach (ProjectItem projectItem in projectItems)
                {
                    var sufix2 = sufix+":" + projectItems.Count+ " ";
                    var emems = projectItems.Count;
                    var emems2 = projectItem.FileCount;
                    for (short i = 1; i <= projectItem.FileCount; i++)
                    {
                        string fileName = projectItem.FileNames[i];
                        if (fileName == null) continue;

                        fileName = sufix2 + fileName;
                        returnValue.Add(fileName);
                    }
                    
                    string piName = projectItem.Name;

                    if (piName == "Map")
                    {
                        projectItem.ProjectItems.AddFromFile(@"D:\Projects\Vendo\Master\Amag\Amag.Data\Config\Map\rp_test_tab.xml");
                        projectItem.Save();
                    }

                    returnValue.AddRange(GetAllProjectFiles(piName, projectItem.ProjectItems));

                    var sp = projectItem.SubProject;
                    if (sp!=null)
                    {
                        string Name = sp.Name;
                        string FileName = sp.FileName;
                        string Kind = sp.Kind;
                        string UniqueName = sp.UniqueName;
                        string ExtenderCATID = sp.ExtenderCATID;
                        string FullName = sp.FullName;

                        if (Name != "Amag.Data")
                            continue;
                        
                        returnValue.AddRange(GetAllProjectFiles(sufix, sp.ProjectItems));
                    }
                }
            }

            return returnValue;
        }

        private bool AfterUpdateAmagData()
        {
            var txt = outputWriter.GetText();

            //11:57:54.562: 	File saved: D:\Projects\Vendo\Master\#Tools\..\Amag\Amag.Data\Config\Map\rp_test_tab.xml
            var files = new List<string>();
            foreach (var line in txt)
            {
                if (line.Contains("File saved:"))
                {
                    var file = line;                                        
                    var startIndex = file.IndexOf("\\Amag\\");
                    file = solutionDir + file.Substring(startIndex);
                    files.Add(file);
                }
            }



            return true;
        }

        public static void ENums(Package package)
        {
            var vh = new ToolsHelper(package);
            vh.ENumsAsync();
        }

        private async void ENumsAsync()
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
            aruments.Add("-logs");
            aruments.Add("-safemode");

            return string.Join(" ", aruments.ToArray());
        }

        private string GetRepositoryExe(Xml.Version version, string mode)
        {
            if (string.IsNullOrEmpty(version.Repository))
                return null;

            return version.Repository + @"\"+ GetApplicationName(mode);
        }

        private string GetApplicationName(string mode)
        {
            string ret = "Amag.App.exe";
            if (mode == "rcp") ret = "VendoRCP.exe";
            return ret;
        }

        private string AddDoubleQuotes(string value)
        {
            return "\"" + value + "\"";
        }
    }
}
