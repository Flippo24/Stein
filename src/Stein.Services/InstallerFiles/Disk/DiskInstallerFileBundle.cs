﻿using System;
using System.Collections.Generic;
using Stein.Common.InstallerFiles;

namespace Stein.Services.InstallerFiles.Disk
{
    public class DiskInstallerFileBundle
        : IInstallerFileBundle
    {
        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public DateTime Created { get; set; }

        /// <inheritdoc />
        public IEnumerable<IInstallerFile> InstallerFiles { get; set; }
    }
}
