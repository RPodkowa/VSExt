using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ButtonsReader.Xml
{
    [XmlRoot(ElementName = "Extern", Namespace = "http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable")]
    public class Extern
    {
        [XmlAttribute(AttributeName = "href")]
        public string Href { get; set; }
    }

    [XmlRoot(ElementName = "Parent", Namespace = "http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable")]
    public class Parent
    {
        [XmlAttribute(AttributeName = "guid")]
        public string Guid { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "Strings", Namespace = "http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable")]
    public class Strings
    {
        [XmlElement(ElementName = "ButtonText", Namespace = "http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable")]
        public string ButtonText { get; set; }
        [XmlElement(ElementName = "CommandName", Namespace = "http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable")]
        public string CommandName { get; set; }
        [XmlElement(ElementName = "ToolTipText", Namespace = "http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable")]
        public string ToolTipText { get; set; }
        [XmlElement(ElementName = "MenuText", Namespace = "http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable")]
        public string MenuText { get; set; }
    }

    [XmlRoot(ElementName = "Menu", Namespace = "http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable")]
    public class Menu
    {
        [XmlElement(ElementName = "Parent", Namespace = "http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable")]
        public Parent Parent { get; set; }
        [XmlElement(ElementName = "Strings", Namespace = "http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable")]
        public Strings Strings { get; set; }
        [XmlAttribute(AttributeName = "guid")]
        public string Guid { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "priority")]
        public string Priority { get; set; }
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlElement(ElementName = "CommandFlag", Namespace = "http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable")]
        public string CommandFlag { get; set; }
    }

    [XmlRoot(ElementName = "Menus", Namespace = "http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable")]
    public class Menus
    {
        [XmlElement(ElementName = "Menu", Namespace = "http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable")]
        public List<Menu> Menu { get; set; }
    }

    [XmlRoot(ElementName = "Group", Namespace = "http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable")]
    public class Group
    {
        [XmlElement(ElementName = "Parent", Namespace = "http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable")]
        public Parent Parent { get; set; }
        [XmlAttribute(AttributeName = "guid")]
        public string Guid { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "priority")]
        public string Priority { get; set; }
    }

    [XmlRoot(ElementName = "Groups", Namespace = "http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable")]
    public class Groups
    {
        [XmlElement(ElementName = "Group", Namespace = "http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable")]
        public List<Group> Group { get; set; }
    }

    [XmlRoot(ElementName = "Icon", Namespace = "http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable")]
    public class Icon
    {
        [XmlAttribute(AttributeName = "guid")]
        public string Guid { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "Button", Namespace = "http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable")]
    public class Button
    {
        [XmlElement(ElementName = "Parent", Namespace = "http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable")]
        public Parent Parent { get; set; }
        [XmlElement(ElementName = "Icon", Namespace = "http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable")]
        public Icon Icon { get; set; }
        [XmlElement(ElementName = "Strings", Namespace = "http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable")]
        public Strings Strings { get; set; }
        [XmlAttribute(AttributeName = "guid")]
        public string Guid { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "priority")]
        public string Priority { get; set; }
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlElement(ElementName = "CommandFlag", Namespace = "http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable")]
        public string CommandFlag { get; set; }
    }

    [XmlRoot(ElementName = "Buttons", Namespace = "http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable")]
    public class Buttons
    {
        [XmlElement(ElementName = "Button", Namespace = "http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable")]
        public List<Button> Button { get; set; }
    }

    [XmlRoot(ElementName = "Bitmap", Namespace = "http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable")]
    public class Bitmap
    {
        [XmlAttribute(AttributeName = "guid")]
        public string Guid { get; set; }
        [XmlAttribute(AttributeName = "href")]
        public string Href { get; set; }
        [XmlAttribute(AttributeName = "usedList")]
        public string UsedList { get; set; }
    }

    [XmlRoot(ElementName = "Bitmaps", Namespace = "http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable")]
    public class Bitmaps
    {
        [XmlElement(ElementName = "Bitmap", Namespace = "http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable")]
        public List<Bitmap> Bitmap { get; set; }
    }

    [XmlRoot(ElementName = "Commands", Namespace = "http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable")]
    public class Commands
    {
        [XmlElement(ElementName = "Menus", Namespace = "http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable")]
        public Menus Menus { get; set; }
        [XmlElement(ElementName = "Groups", Namespace = "http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable")]
        public Groups Groups { get; set; }
        [XmlElement(ElementName = "Buttons", Namespace = "http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable")]
        public Buttons Buttons { get; set; }
        [XmlElement(ElementName = "Bitmaps", Namespace = "http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable")]
        public Bitmaps Bitmaps { get; set; }
        [XmlAttribute(AttributeName = "package")]
        public string Package { get; set; }
    }

    [XmlRoot(ElementName = "GuidSymbol", Namespace = "http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable")]
    public class GuidSymbol
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
        [XmlElement(ElementName = "IDSymbol", Namespace = "http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable")]
        public List<IDSymbol> IDSymbol { get; set; }
    }

    [XmlRoot(ElementName = "IDSymbol", Namespace = "http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable")]
    public class IDSymbol
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "Symbols", Namespace = "http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable")]
    public class Symbols
    {
        [XmlElement(ElementName = "GuidSymbol", Namespace = "http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable")]
        public List<GuidSymbol> GuidSymbol { get; set; }
    }

    [XmlRoot(ElementName = "CommandTable", Namespace = "http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable")]
    public class CommandTable
    {
        [XmlElement(ElementName = "Extern", Namespace = "http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable")]
        public List<Extern> Extern { get; set; }
        [XmlElement(ElementName = "Commands", Namespace = "http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable")]
        public Commands Commands { get; set; }
        [XmlElement(ElementName = "Symbols", Namespace = "http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable")]
        public Symbols Symbols { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlAttribute(AttributeName = "xs", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xs { get; set; }
    }
}
