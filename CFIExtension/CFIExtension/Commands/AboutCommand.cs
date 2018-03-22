using System;
using System.ComponentModel.Design;
using System.Globalization;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace CFIExtension
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class AboutCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0112;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("a1cdc366-abd2-4e52-bcb4-ea939220d430");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        /// Initializes a new instance of the <see cref="AboutCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private AboutCommand(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(this.MenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static AboutCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new AboutCommand(package);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            //CFIPackage cfiPackage = this.package as CFIPackage;

            //string message = cfiPackage.OptionInteger.ToString();
            //string title = "AboutCommand";

            //// Show a message box to prove we were here
            //VsShellUtilities.ShowMessageBox(
            //    this.ServiceProvider,
            //    message,
            //    title,
            //    OLEMSGICON.OLEMSGICON_INFO,
            //    OLEMSGBUTTON.OLEMSGBUTTON_OK,
            //    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

            IVsOutputWindow outWindow = Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;

            // Use e.g. Tools -> Create GUID to make a stable, but unique GUID for your pane.
            // Also, in a real project, this should probably be a static constant, and not a local variable
            Guid customGuid = new Guid("0F44E2D1-F5FA-4d2d-AB30-22BE8ECD9789");
            string customTitle = "Custom Window Title";
            outWindow.CreatePane(ref customGuid, customTitle, 1, 1);
            
        
            IVsOutputWindowPane customPane;
            outWindow.GetPane(ref customGuid, out customPane);

            customPane.OutputString("Hello, Custom World!");
            customPane.Activate(); // Brings this pane into view
            
        }
    }
}
