﻿using System;
using System.ComponentModel;
using System.Threading;
using NKristek.Smaragd.Attributes;
using NKristek.Smaragd.Commands;
using NKristek.Smaragd.ViewModels;
using Stein.Presentation;
using Stein.ViewModels.Types;

namespace Stein.ViewModels
{
    public sealed class InstallationViewModel
        : ViewModel, IDisposable
    {
        private readonly IProgressBarService _progressBarService;

        public InstallationViewModel(IProgressBarService progressBarService)
        {
            _progressBarService = progressBarService ?? throw new ArgumentNullException(nameof(progressBarService));
            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs? e)
        {
            if (e != null && e.PropertyName != nameof(Progress) && e.PropertyName != nameof(State))
                return;

            switch (State)
            {
                case InstallationState.Cancelled:
                    _progressBarService.SetState(ProgressBarState.Indeterminate);
                    break;
                case InstallationState.Preparing:
                case InstallationState.Install:
                    _progressBarService.SetState(ProgressBarState.Normal);
                    _progressBarService.SetProgress(Progress);
                    break;
                case InstallationState.Finished:
                    _progressBarService.SetState(ProgressBarState.None);
                    break;
            }
        }

        private string? _name;
        
        public string? Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private InstallationState _state;
        
        public InstallationState State
        {
            get => _state;
            set
            {
                if (!SetProperty(ref _state, value))
                    return;
                if (value == InstallationState.Cancelled)
                    CancellationTokenSource.Cancel();
            }
        }

        private InstallationOperation _currentOperation;
        
        public InstallationOperation CurrentOperation
        {
            get => _currentOperation;
            set => SetProperty(ref _currentOperation, value);
        }

        [PropertySource(nameof(CurrentOperation))]
        public bool IsInstalling => CurrentOperation != InstallationOperation.None;
        
        private int _installerCount;
        
        public int TotalInstallerFileCount
        {
            get => _installerCount;
            set => SetProperty(ref _installerCount, value);
        }

        private int _currentInstallerIndex;
        
        public int CurrentInstallerIndex
        {
            get => _currentInstallerIndex;
            set => SetProperty(ref _currentInstallerIndex, value);
        }

        private int _processedInstallerFileCount;

        public int ProcessedInstallerFileCount
        {
            get => _processedInstallerFileCount;
            set => SetProperty(ref _processedInstallerFileCount, value);
        }

        private double _downloadProgress;
        
        public double DownloadProgress
        {
            get => _downloadProgress;
            set => SetProperty(ref _downloadProgress, value);
        }
        
        [PropertySource(nameof(ProcessedInstallerFileCount), nameof(TotalInstallerFileCount))]
        public double InstallationProgress => (double)ProcessedInstallerFileCount / TotalInstallerFileCount;
        
        [PropertySource(nameof(DownloadProgress), nameof(InstallationProgress))]
        public double Progress => (DownloadProgress + InstallationProgress) / 2;
        
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        
        public CancellationTokenSource CancellationTokenSource
        {
            get => _cancellationTokenSource;
            set => SetProperty(ref _cancellationTokenSource, value);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            State = InstallationState.Finished;
            _cancellationTokenSource?.Dispose();
        }

        private IViewModelCommand<InstallationViewModel>? _cancelOperationCommand;

        [IsDirtyIgnored]
        [IsReadOnlyIgnored]
        public IViewModelCommand<InstallationViewModel>? CancelOperationCommand
        {
            get => _cancelOperationCommand;
            set
            {
                if (SetProperty(ref _cancelOperationCommand, value, out var oldValue))
                {
                    if (oldValue != null)
                        oldValue.Context = null;
                    if (value != null)
                        value.Context = this;
                }
            }
        }
    }
}
