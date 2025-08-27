using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Shared.Controllers.Models
{
    [XmlRoot("ProgramMappings")]
    public class ProgramMappingsXml
    {
        [XmlArray("Profiles")]
        [XmlArrayItem("FileExtension")]
        // Niveleta
        public List<FileExtension> Profiles { get; set; }

        [XmlArray("Routes")]
        [XmlArrayItem("FileExtension")]
        // Trasa / Směrové řešení
        public List<FileExtension> Routes { get; set; }

        [XmlArray("Corridor")]
        [XmlArrayItem("FileExtension")]
        // Koridor
        public List<FileExtension> Corridor { get; set; }

        [XmlArray("Surveys")]
        [XmlArrayItem("FileExtension")]
        // Vytyčení
        public List<FileExtension> Surveys { get; set; }

        [XmlArray("CrossSections")]
        [XmlArrayItem("FileExtension")]
        // Příčné řezy
        public List<FileExtension> CrossSections { get; set; }

        [XmlArray("CombinedCrossSections")]
        [XmlArrayItem("FileExtension")]
        // Spojené příčné řezy
        public List<FileExtension> CombinedCrossSections { get; set; }

        [XmlArray("IFCReferences")]
        [XmlArrayItem("FileExtension")]
        // IFC Podklady
        public List<FileExtension> IFCReferences { get; set; }

        public class FileExtension
        {
            [XmlAttribute("Extension")]
            // Přípona souboru
            public string Extension { get; set; }

            [XmlAttribute("Type")]
            // Optional: Listing, Calculation, Result
            public string Type { get; set; }

            [XmlAttribute("XmlType")]
            // If file is has Xml base
            public bool XmlType { get; set; } = false;
        }
    }
}
