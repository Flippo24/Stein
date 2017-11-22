﻿using System;
using System.Xml.Serialization;

namespace Stein.ConfigurationTypes
{
    [Serializable]
    public class InstallerFile
    {
        [XmlElement("Name")]
        public string Name;

        [XmlElement("Path")]
        public string Path;

        [XmlElement("IsEnabled")]
        public bool IsEnabled;

        [XmlElement("Version", typeof(VersionXml))]
        public Version Version;

        [XmlElement("Culture")]
        public string Culture;

        [XmlElement("ProductCode")]
        public string ProductCode;

        [XmlElement("Created")]
        public DateTimeXml CreatedXml;

        [XmlIgnore]
        public DateTime Created
        {
            get
            {
                return CreatedXml.Date ?? DateTime.MinValue;
            }

            set
            {
                CreatedXml = new DateTimeXml(value);
            }
        }
    }
}