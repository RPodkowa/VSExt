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
            ret+=".20200310";
            //EndVersionBodyToReplace
            return ret;
        }

        
        private string GetDescription()
        {
            var desc = new List<string>();
            //BodyToReplace
            desc.Add("Functions:");
            desc.Add(" 1. Copy Release");
            desc.Add(" 2. Run Vendo in normal mode");
            desc.Add(" 3. Run Vendo in reader mode");
            desc.Add(" 4. Run Vendo in RCP mode");
            desc.Add(" 5. Copy Release and run Vendo in normal mode");
            desc.Add(" 6. Copy Release and run Vendo in reader mode");
            desc.Add(" 7. Copy Release and run Vendo in RCP mode");
            desc.Add(" 8. Run repository Vendo in normal mode");
            desc.Add(" 9. Run repository Vendo in reader mode");
            desc.Add(" 10. Run repository Vendo in RCP mode");
            desc.Add(" 11. Update Amag.Data");
            desc.Add(" 12. Update Enums");
            desc.Add(" 13. macros.xml");
            desc.Add(" 14. changes.lst");
            desc.Add(" 15. changes.lst (MRP)");
            desc.Add(" 16. facts.txt");
            desc.Add(" 17. Icons");
            desc.Add(" 18. About Extension");
            desc.Add(" 19. Generate Guid");
            desc.Add(" 20. Copy selection adress");
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
