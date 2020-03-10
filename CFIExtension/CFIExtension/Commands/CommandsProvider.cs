using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using CFIExtension.Logic;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace CFIExtension
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class CommandsProvider
    {
        #region Main Commands
        /// <summary>
        /// Main Commands
        /// </summary>
        private const int CopyReleaseCommandId = 0x0100;
        private const int RunNormalCommandId = 0x0101;
        private const int RunReaderCommandId = 0x0102;
        private const int RunRcpCommandId = 0x0103;
        private const int CopyRunNormalCommandId = 0x0104;
        private const int CopyRunReaderCommandId = 0x0105;
        private const int CopyRunRcpCommandId = 0x0106;
        private const int UpdateAmagDataCommandId = 0x0107;
        private const int OpenFileMacrosXmlCommandId = 0x0108;
        private const int OpenFileChangesLstCommandId = 0x0109;
        private const int OpenFileMrpChangesLstCommandId = 0x0110;
        private const int OpenFileFactsCommandId = 0x0111;
        private const int AboutCommandId = 0x0112;
        private const int GoToIconsCommandId = 0x0113;
        private const int UpdateEnumsCommandCommandId = 0x0114;
        private const int RunRepositoryNormalCommandId = 0x0115;
        private const int RunRepositoryReaderCommandId = 0x0116;
        private const int RunRepositoryRcpCommandId = 0x0117;

        public static readonly List<int> MainCommands = new List<int>(
            new int[]
            {
                CopyReleaseCommandId,
                RunNormalCommandId, RunReaderCommandId, RunRcpCommandId,
                CopyRunNormalCommandId, CopyRunReaderCommandId, CopyRunRcpCommandId,
                UpdateAmagDataCommandId,
                OpenFileMacrosXmlCommandId, OpenFileChangesLstCommandId, OpenFileMrpChangesLstCommandId, OpenFileFactsCommandId,
                AboutCommandId,
                GoToIconsCommandId,
                UpdateEnumsCommandCommandId,
                RunRepositoryNormalCommandId, RunRepositoryReaderCommandId, RunRepositoryRcpCommandId
            }
        );

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid MainCommandSet = new Guid("a1cdc366-abd2-4e52-bcb4-ea939220d430");
        #endregion
        #region Context Commands
        /// <summary>
        /// Context Commands
        /// </summary>
        private const int GuidGenerateCommandId = 0x0100;
        private const int CopySelectionAdressCommandId = 0x0101;        

        public static readonly List<int> ContextCommands = new List<int>(
            new int[]
            {
                GuidGenerateCommandId,
                CopySelectionAdressCommandId
            }
        );

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid ContextCommandSet = new Guid("e4c4c8e6-dbe3-40f1-9d4e-cd8ed664f6d7");
        #endregion


        private const int cmdidMyAnchorCommand = 0x4002;
        private const int cmdidMyDynamicStartCommand = 0x4003;

        private readonly Package package;

        private CommandsProvider(Package package)
        {
            this.package = package ?? throw new ArgumentNullException("package");

            OleMenuCommandService commandService = ((IServiceProvider)package).GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                foreach (var commandId in MainCommands)
                {
                    var menuCommandID = new CommandID(MainCommandSet, commandId);
                    var menuItem = new MenuCommand(MenuItemCallback, menuCommandID);
                    commandService.AddCommand(menuItem);
                }

                foreach (var commandId in ContextCommands)
                {
                    var menuCommandID = new CommandID(ContextCommandSet, commandId);
                    var menuItem = new MenuCommand(MenuItemCallback, menuCommandID);
                    commandService.AddCommand(menuItem);
                }

                // Add the DynamicItemMenuCommand for the expansion of the root item into N items at run time.   
                CommandID dynamicItemRootId = new CommandID(MainCommandSet, cmdidMyDynamicStartCommand);
                DynamicItemMenuCommand dynamicMenuCommand = new DynamicItemMenuCommand(dynamicItemRootId, IsValidDynamicItem, OnInvokedDynamicItem, OnBeforeQueryStatusDynamicItem);
                commandService.AddCommand(dynamicMenuCommand);

            }
        }

        private bool IsValidDynamicItem(int commandId)
        {
            // The match is valid if the command ID is >= the id of our root dynamic start item   
            // and the command ID minus the ID of our root dynamic start item  
            // is less than or equal to the number of projects in the solution.  
            return (commandId >= cmdidMyDynamicStartCommand) && ((commandId - cmdidMyDynamicStartCommand) < 6);
        }

        private void OnInvokedDynamicItem(object sender, EventArgs args)
        {
            OleMenuCommand invokedCommand = (OleMenuCommand)sender;
            // If the command is already checked, we don’t need to do anything  

            var i = invokedCommand.MatchedCommandId;

            if (i == 16388) FileHelper.OpenNotepad(package, "changes.lst");
            if (i == 16389) FileHelper.OpenNotepad(package, "changes.lst(MRP)");
        }

        private void OnBeforeQueryStatusDynamicItem(object sender, EventArgs args)
        {
            OleMenuCommand matchedCommand = (OleMenuCommand)sender;
            matchedCommand.Enabled = true;
            matchedCommand.Visible = true;

            // Find out whether the command ID is 0, which is the ID of the root item.  
            // If it is the root item, it matches the constructed DynamicItemMenuCommand,  
            // and IsValidDynamicItem won't be called.  
            bool isRootItem = (matchedCommand.MatchedCommandId == 0);

            var i = matchedCommand.MatchedCommandId;

            var txt = "cmd_" + i;
            if (i == 16388) txt = "Dchanges.lst";
            if (i == 16389)
            {
                matchedCommand.Enabled = false;
                txt = "Dchanges.lst(MRP)";
            }

            matchedCommand.Text = txt;
        }

        public static CommandsProvider Instance
        {
            get;
            private set;
        }

        public static void Initialize(Package package)
        {
            Instance = new CommandsProvider(package);
        }

        private void MenuItemCallback(object sender, EventArgs e)
        {
            ExecuteCommand(sender as MenuCommand);
        }

        private void ExecuteCommand(MenuCommand menuCommand)
        {
            if (menuCommand == null) return;

            var commandID = menuCommand.CommandID;

            if (commandID.Guid == MainCommandSet)
            {
                switch (commandID.ID)
                {
                    case CopyReleaseCommandId: ToolsHelper.CopyRelease(package); break;
                    //case CopyReleaseCommandId: TextEditorHelper.AddReferencesToPatchFileCommandId(package); break;
                    case RunNormalCommandId: ToolsHelper.Run(package, "start"); break;
                    case RunReaderCommandId: ToolsHelper.Run(package, "reader"); break;
                    case RunRcpCommandId: ToolsHelper.Run(package, "rcp"); break;
                    case CopyRunNormalCommandId: ToolsHelper.CopyRun(package, "start"); break;
                    case CopyRunReaderCommandId: ToolsHelper.CopyRun(package, "reader"); break;
                    case CopyRunRcpCommandId: ToolsHelper.CopyRun(package, "rcp"); break;
                    case UpdateAmagDataCommandId: ToolsHelper.UpdateAmagData(package); break;
                    case OpenFileMacrosXmlCommandId: FileHelper.OpenNotepad(package, "macros.xml"); break;
                    case OpenFileChangesLstCommandId: FileHelper.OpenNotepad(package, "changes.lst"); break;
                    case OpenFileMrpChangesLstCommandId: FileHelper.OpenNotepad(package, "changes.lst(MRP)"); break;
                    case OpenFileFactsCommandId: FileHelper.OpenNotepad(package, "facts.txt"); break;
                    case AboutCommandId: AboutDialog.ShowAboutDialog(); break;
                    case GoToIconsCommandId: FileHelper.GoToLocation(package, "icons"); break;
                    case UpdateEnumsCommandCommandId: ToolsHelper.ENums(package); break;
                    case RunRepositoryNormalCommandId: ToolsHelper.RunRepository(package, "start"); break;
                    case RunRepositoryReaderCommandId: ToolsHelper.RunRepository(package, "reader"); break;
                    case RunRepositoryRcpCommandId: ToolsHelper.RunRepository(package, "rcp"); break;
                }
            }

            if (commandID.Guid == ContextCommandSet)
            {
                switch (commandID.ID)
                {
                    case GuidGenerateCommandId: TextEditorHelper.InsertGuidCPP(package); break;
                    case CopySelectionAdressCommandId: TextEditorHelper.CopySelectionAdress(package); break;
                }
            }
        }
    }

    class DynamicItemMenuCommand : OleMenuCommand
    {
        private Predicate<int> matches;

        public DynamicItemMenuCommand(CommandID rootId, Predicate<int> matches, EventHandler invokeHandler, EventHandler beforeQueryStatusHandler)
            : base(invokeHandler, null /*changeHandler*/, beforeQueryStatusHandler, rootId)
        {
            if (matches == null)
            {
                throw new ArgumentNullException("matches");
            }

            this.matches = matches;
        }
        public override bool DynamicItemMatch(int cmdId)
        {
            // Call the supplied predicate to test whether the given cmdId is a match.  
            // If it is, store the command id in MatchedCommandid   
            // for use by any BeforeQueryStatus handlers, and then return that it is a match.  
            // Otherwise clear any previously stored matched cmdId and return that it is not a match.  
            if (this.matches(cmdId))
            {
                this.MatchedCommandId = cmdId;
                return true;
            }

            this.MatchedCommandId = 0;
            return false;
        }
    }
}
