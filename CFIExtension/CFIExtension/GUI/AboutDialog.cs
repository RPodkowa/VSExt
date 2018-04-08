using CFIExtension.Logic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CFIExtension
{
    partial class AboutDialog : Form
    {
        public static void ShowAboutDialog()
        {
            var ad = new AboutDialog();
            ad.ShowDialog();
        }
        private AboutDialog()
        {
            InitializeComponent();
            this.Text = String.Format("About {0}", AssemblyTitle);
            this.labelProductName.Text = AssemblyProduct;
            this.labelVersion.Text = GetVersion();
            this.labelCopyright.Text = AssemblyCopyright;
            this.labelCompanyName.Text = AssemblyCompany;
            this.textBoxDescription.Text = GetDescription();
        }

        #region Assembly Attribute Accessors

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        private string GetVersion()
        {
            var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            string ret = String.Format("Version {0}", assemblyVersion);
            //VersionBodyToReplace
            ret+=".20180408";
            //EndVersionBodyToReplace
            return ret;
        }

        
        private string GetDescription()
        {
            var desc = new List<string>();
            //BodyToReplace
            desc.Add("Functions:");
            desc.Add(" 1. Menu:");
            desc.Add("  1.1. CopyGroup");
            desc.Add("   1.1.1 Copy Release");
            desc.Add("  1.2. RunGroup");
            desc.Add("   1.2.1 Run Vendo in normal mode");
            desc.Add("   1.2.2 Run Vendo in reader mode");
            desc.Add("   1.2.3 Run Vendo in RCP mode");
            desc.Add("  1.3. CopyRunGroup");
            desc.Add("   1.3.1 Copy Release and run Vendo in normal mode");
            desc.Add("   1.3.2 Copy Release and run Vendo in reader mode");
            desc.Add("   1.3.3 Copy Release and run Vendo in RCP mode");
            desc.Add("  1.4. UpdateAmagDataGroup");
            desc.Add("   1.4.1 Update Amag.Data");
            desc.Add("  1.5. UpdateEnumsGroup");
            desc.Add("   1.5.1 Update Enums");
            desc.Add("  1.6. OpenFileGroup");
            desc.Add("  1.7. GoToLocationGroup");
            desc.Add("   1.7.1 Go to icons location");
            desc.Add("  1.8. AboutGroup");
            desc.Add("   1.8.1 About Extension");
            desc.Add(" 2. Toolbar:");
            desc.Add("  2.1. ToolbarCopyGroup");
            desc.Add("  2.2. ToolbarRunGroup");
            desc.Add("  2.3. ToolbarCopyRunGroup");
            desc.Add("  2.4. ToolbarUpdateAmagDataGroup");
            desc.Add(" 3. Menu:");
            desc.Add("  3.1. SubMenuOpenFileGroup");
            desc.Add("   3.1.1 Open macros.xml");
            desc.Add("   3.1.2 Open changes.lst");
            desc.Add("   3.1.3 Open changes.lst (MRP)");
            desc.Add("   3.1.4 Open facts.txt");
            desc.Add(" 4. ContextMenu::");
            desc.Add("  4.1. ContextMenuGroup");
            desc.Add("   4.1.1 Generate Guid");
            desc.Add("   4.1.2 Copy selection adress");
            //EndBodyToReplace
            return string.Join("\r\n", desc.ToArray());
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion
    }
}
