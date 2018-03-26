using CFIExtension.Logic.Xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace CFIExtension.Logic
{
    public static class XmlSettingsReader
    {
        public static Xml.Version GetVersionInfo(string solutionDir)
        {
            string vendoVersion = Path.GetFileName(solutionDir);
            string xmlFile = Path.GetDirectoryName(solutionDir) + "\\versions.xml";
            
            // A FileStream is needed to read the XML document.
            FileStream fs = new FileStream(xmlFile, FileMode.Open);
            XmlReader reader = XmlReader.Create(fs);
                        
            XmlSerializer serializer = new XmlSerializer(typeof(Versions));
            Versions versions = (Versions)serializer.Deserialize(reader);
            fs.Close();

            if (versions == null || versions.Version == null)
                return null;

            foreach (var v in versions.Version)
            {
                if (v.Name == vendoVersion)
                    return v;
            }

            return null;
        }
    }
}
