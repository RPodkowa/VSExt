using ButtonsReader.Xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace ButtonsReader
{
    public static class XmlButtonReader
    {
        public static List<string> GetDescription(string solutionDir)
        {            
            string xmlFile = solutionDir + @"CFIExtension\CFIPackage.vsct";

            // A FileStream is needed to read the XML document.
            FileStream fs = new FileStream(xmlFile, FileMode.Open);
            XmlReader reader = XmlReader.Create(fs);

            XmlSerializer serializer = new XmlSerializer(typeof(CommandTable));
            CommandTable commandTable = (CommandTable)serializer.Deserialize(reader);
            fs.Close();

            List<string> description = new List<string>();
            description.Add("Functions:");

            if (commandTable == null || commandTable.Commands == null)
                return description;

            var commands = commandTable.Commands;

            // Menu / toolbar
            int i = 0;
            foreach (var m in commands.Menus.Menu)
            {
                description.Add(string.Format(" {0}. {1}:", ++i, m.Type));
                int j = 0;
                foreach (var g in commands.Groups.Group)
                {
                    if (g.Parent.Id != m.Id) continue;
                    description.Add(string.Format("  {0}.{1}. {2}", i, ++j, g.Id));
                    int k = 0;
                    foreach (var b in commands.Buttons.Button)
                    {
                        if (b.Parent.Id != g.Id) continue;

                        var txt = b.Strings.ToolTipText;
                        if (string.IsNullOrEmpty(txt)) txt = b.Strings.MenuText;
                        if (string.IsNullOrEmpty(txt)) txt = b.Strings.CommandName;
                        if (string.IsNullOrEmpty(txt)) txt = b.Strings.ButtonText;

                        description.Add(string.Format("   {0}.{1}.{2} {3}", i, j, ++k, txt));
                    }
                }
            }

            //Context            
            {
                description.Add(string.Format(" {0}. {1}:", ++i, "ContextMenu:"));
                int j = 0;
                foreach (var g in commands.Groups.Group)
                {
                    if (g.Parent.Id != "IDM_VS_CTXT_CODEWIN") continue;
                    description.Add(string.Format("  {0}.{1}. {2}", i, ++j, g.Id));
                    int k = 0;
                    foreach (var b in commands.Buttons.Button)
                    {
                        if (b.Parent.Id != g.Id) continue;

                        var txt = b.Strings.ToolTipText;
                        if (string.IsNullOrEmpty(txt)) txt = b.Strings.MenuText;
                        if (string.IsNullOrEmpty(txt)) txt = b.Strings.CommandName;
                        if (string.IsNullOrEmpty(txt)) txt = b.Strings.ButtonText;

                        description.Add(string.Format("   {0}.{1}.{2} {3}", i, j, ++k, txt));
                    }
                }
            }

            return description;
        }
    }
}
