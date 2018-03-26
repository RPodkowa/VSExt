using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace CFIExtension.Logic.Xml
{
    [XmlRoot(ElementName = "DB")]
    public class DB
    {
        [XmlElement(ElementName = "Host")]
        public string Host { get; set; }
        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "User")]
        public string User { get; set; }
        [XmlElement(ElementName = "Password")]
        public string Password { get; set; }
    }

    [XmlRoot(ElementName = "Vendo")]
    public class Vendo
    {
        [XmlElement(ElementName = "User")]
        public string User { get; set; }
        [XmlElement(ElementName = "Password")]
        public string Password { get; set; }
    }

    [XmlRoot(ElementName = "Version")]
    public class Version
    {
        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "Launcher")]
        public string Launcher { get; set; }
        [XmlElement(ElementName = "Repository")]
        public string Repository { get; set; }
        [XmlElement(ElementName = "DB")]
        public DB DB { get; set; }
        [XmlElement(ElementName = "Vendo")]
        public Vendo Vendo { get; set; }
    }

    [XmlRoot(ElementName = "Versions")]
    public class Versions
    {
        [XmlElement(ElementName = "Version")]
        public List<Version> Version { get; set; }
    }
}
