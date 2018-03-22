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
    [ProvideOptionPage(typeof(OptionPageGrid), "My Category", "My Grid Page", 0, 0, true)]
    [ProvideOptionPage(typeof(OptionPageCustom), "My Category", "My Custom Page", 0, 0, true)]
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

        public int OptionInteger
        {
            get
            {
                OptionPageGrid page = (OptionPageGrid)GetDialogPage(typeof(OptionPageGrid));
                return page.OptionInteger;
            }
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
            CFIExtension.Commands.OpenFileChangesLstCommand.Initialize(this);
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
        }

        #endregion
    }

    public class OptionPageGrid : DialogPage
    {
        private int optionInt = 256;

        [Category("My Category")]
        [DisplayName("My Integer Option")]
        [Description("My integer option")]
        public int OptionInteger
        {
            get { return optionInt; }
            set { optionInt = value; }
        }
    }

    [Guid("00000000-0000-0000-0000-000000000000")]
    public class OptionPageCustom : DialogPage
    {
        private string optionValue = "alpha";

        public string OptionString
        {
            get { return optionValue; }
            set { optionValue = value; }
        }
    }

}
