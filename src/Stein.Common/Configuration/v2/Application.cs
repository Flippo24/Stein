﻿using System;
using System.Xml.Serialization;
using Stein.Utility.XML;

namespace Stein.Common.Configuration.v2
{
    [Serializable]
    public class Application
    {
        [XmlAttribute]
        public Guid Id;

        [XmlElement]
        public CDataString Name;
        
        [XmlElement]
        public bool EnableSilentInstallation = true;
        
        [XmlElement]
        public bool DisableReboot = true;
        
        [XmlElement]
        public bool EnableInstallationLogging = true;
        
        [XmlElement]
        public bool AutomaticallyDeleteInstallationLogs = true;
        
        [XmlElement]
        public int KeepNewestInstallationLogs = 10;

        [XmlElement]
        public bool FilterDuplicateInstallers = true;

        [XmlElement]
        public InstallerFileBundleProviderConfiguration Configuration;
    }
}
