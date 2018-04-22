﻿using System;
using nkristek.MVVMBase.ViewModels;
using Stein.Localizations;

namespace Stein.ViewModels
{
    public class InstallationResultViewModel
        : ViewModel
    {
        /// <summary>
        /// The result of the finished installation
        /// </summary>
        [PropertySource(nameof(InstallCount), nameof(ReinstallCount), nameof(UninstallCount), nameof(FailedCount))]
        public string Result
        {
            get
            {
                var resultMessage = String.Format(Strings.DidInstallXPrograms, InstallCount, ReinstallCount, UninstallCount);
                if (FailedCount > 0)
                    resultMessage = String.Join("\n", resultMessage, String.Format(Strings.XInstallersFailed, FailedCount));
                return resultMessage;
            }
        }

        private int _installCount;
        /// <summary>
        /// How many installers were installed
        /// </summary>
        public int InstallCount
        {
            get => _installCount;
            set => SetProperty(ref _installCount, value);
        }

        private int _uninstallCount;
        /// <summary>
        /// How many installers were uninstalled
        /// </summary>
        public int UninstallCount
        {
            get => _uninstallCount;
            set => SetProperty(ref _uninstallCount, value);
        }

        private int _reinstallCount;
        /// <summary>
        /// How many installers were reinstalled
        /// </summary>
        public int ReinstallCount
        {
            get => _reinstallCount;
            set => SetProperty(ref _reinstallCount, value);
        }

        private int _failedCount;
        /// <summary>
        /// How many installers failed to process
        /// </summary>
        public int FailedCount
        {
            get => _failedCount;
            set => SetProperty(ref _failedCount, value);
        }
    }
}