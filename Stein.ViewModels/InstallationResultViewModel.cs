﻿using System.Collections.ObjectModel;
using NKristek.Smaragd.ViewModels;
using Stein.Localizations;
using Stein.ViewModels.Types;

namespace Stein.ViewModels
{
    public sealed class InstallationResultViewModel
        : ViewModel
    {
        private string _installerName;
        
        public string InstallerName
        {
            get => _installerName;
            set => SetProperty(ref _installerName, value, out _);
        }
        
        private InstallationResultState _state;
        
        public InstallationResultState State
        {
            get => _state;
            set
            {
                if (!SetProperty(ref _state, value, out _))
                    return;

                SetLocalizedReason();
            }
        }

        private bool _isExceptionVisible;
        
        public bool IsExceptionVisible
        {
            get => _isExceptionVisible;
            set => SetProperty(ref _isExceptionVisible, value, out _);
        }

        private ExceptionViewModel _exception;
        
        public ExceptionViewModel Exception
        {
            get => _exception;
            set
            {
                if (!SetProperty(ref _exception, value, out _))
                    return;

                SetLocalizedReason();
            }
        }

        private void SetLocalizedReason()
        {
            if (Exception == null)
                return;

            var isReadOnly = Exception.IsReadOnly;
            Exception.IsReadOnly = false;
            switch (State)
            {
                case InstallationResultState.Success:
                    Exception.LocalizedReason = Strings.InstallationSuccess;
                    break;
                case InstallationResultState.Skipped:
                    Exception.LocalizedReason = Strings.InstallationSkipped;
                    break;
                case InstallationResultState.Cancelled:
                    Exception.LocalizedReason = Strings.InstallationCancelled;
                    break;
                case InstallationResultState.DownloadFailed:
                    Exception.LocalizedReason = Strings.InstallationDownloadFailed;
                    break;
                case InstallationResultState.InstallationFailed:
                    Exception.LocalizedReason = Strings.InstallationFailed;
                    break;
            }
            Exception.IsReadOnly = isReadOnly;
        }

        public ObservableCollection<string> InstallationLogFilePaths { get; } = new ObservableCollection<string>();
    }
}
