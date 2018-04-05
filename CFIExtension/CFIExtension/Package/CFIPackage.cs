using System;
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using System.IO;

namespace CFIExtension
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(CFIPackage.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof(OptionPageGrid), "CFI", "General", 0, 0, true)]
    public sealed class CFIPackage : Package
    {
        /// <summary>
        /// CFIPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "9f3b370f-1249-48ab-8882-016fd64a59e9";

        /// <summary>
        /// Initializes a new instance of the <see cref="CFIPackage"/> class.
        /// </summary>
        public CFIPackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }

        private OptionPageGrid getOptionPageGrid()
        {
            return (OptionPageGrid)GetDialogPage(typeof(OptionPageGrid));
        }

        public string OptionNotepadPath
        {
            get { return getOptionPageGrid().NotepadPath; }
        }

        public string OptionVendoMasterName
        {
            get { return getOptionPageGrid().VendoMasterName; }
        }

        public string OptionToolsName
        {
            get { return getOptionPageGrid().ToolsName; }
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            CopyReleaseCommand.Initialize(this);
            RunNormalCommand.Initialize(this);
            RunReaderCommand.Initialize(this);
            RunRcpCommand.Initialize(this);
            OpenMacrosXmlCommand.Initialize(this);
            OpenFileChangesLstCommand.Initialize(this);
            OpenFileMrpChangesLstCommand.Initialize(this);
            OpenFileFactsCommand.Initialize(this);
            CopyRunNormalCommand.Initialize(this);
            CopyRunReaderCommand.Initialize(this);
            CopyRunRcpCommand.Initialize(this);
            UpdateAmagDataCommand.Initialize(this);
            AboutCommand.Initialize(this);
            GuidGenerateCommand.Initialize(this);
            UpdateEnumsCommand.Initialize(this);
            GoToIconsCommand.Initialize(this);
            RunFromPeposNormalCommand.Initialize(this);
            RunFromPeposReaderCommand.Initialize(this);
            CopySelectionAdressCommand.Initialize(this);
        }

        #endregion
    }

    public class OptionPageGrid : DialogPage
    {        
        private string notepadPath = @"C:\Windows\System32\notepad.exe";
        private string vendoMasterName = "Master";     
        private string toolsName = "#Tools";
        
        [Category("Paths")]
        [DisplayName("Notepad")]
        [Description("Sciezka do notatnika")]
        public string NotepadPath
        {
            get { return notepadPath; }
            set { notepadPath = value; }
        }

        [Category("Directories")]
        [DisplayName("Vendo master")]
        [Description("Nazwa katalogu z glownym projektem")]
        public string VendoMasterName
        {
            get { return vendoMasterName; }
            set { vendoMasterName = value; }
        }
        
        [Category("Directories")]
        [DisplayName("Tools")]
        [Description("Nazwa katalogu z narzedziami")]
        public string ToolsName
        {
            get { return toolsName; }
            set { toolsName = value; }
        }
    }
}
