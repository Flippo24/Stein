﻿using System;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using NKristek.Smaragd.Attributes;
using NKristek.Smaragd.Commands;
using Stein.Presentation;
using Stein.Services.InstallService;
using Stein.ViewModels.Commands.MainWindowViewModelCommands;
using Stein.ViewModels.Extensions;
using Stein.ViewModels.Services;
using Stein.ViewModels.Types;

namespace Stein.ViewModels.Commands.ApplicationViewModelCommands
{
    public sealed class InstallApplicationCommand
        : AsyncViewModelCommand<ApplicationViewModel>
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IDialogService _dialogService;

        private readonly IViewModelService _viewModelService;

        private readonly IInstallService _installService;

        public InstallApplicationCommand(IDialogService dialogService, IViewModelService viewModelService, IInstallService installService)
        {
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _viewModelService = viewModelService ?? throw new ArgumentNullException(nameof(viewModelService));
            _installService = installService ?? throw new ArgumentNullException(nameof(installService));
        }
        
        [CanExecuteSource(nameof(ApplicationViewModel.Parent), nameof(ApplicationViewModel.SelectedInstallerBundle))]
        protected override bool CanExecute(ApplicationViewModel viewModel, object parameter)
        {
            if (!(viewModel.Parent is MainWindowViewModel mainWindowViewModel) || mainWindowViewModel.CurrentInstallation != null)
                return false;

            return viewModel.SelectedInstallerBundle != null && viewModel.SelectedInstallerBundle.Installers.Any();
        }

        protected override async Task ExecuteAsync(ApplicationViewModel viewModel, object parameter)
        {
            if (!(viewModel.Parent is MainWindowViewModel mainWindowViewModel))
                return;

            var installers = viewModel.SelectedInstallerBundle.Installers;
            mainWindowViewModel.CurrentInstallation = _viewModelService.CreateViewModel<InstallationViewModel>(mainWindowViewModel);
            mainWindowViewModel.CurrentInstallation.Name = viewModel.Name;
            mainWindowViewModel.CurrentInstallation.TotalInstallerFileCount = installers.Count;

            InstallationResultDialogModel installationResult;
            try
            {
                installationResult = await ViewModelInstallService.Install(
                    _viewModelService,
                    _installService,
                    mainWindowViewModel.CurrentInstallation,
                    installers,
                    viewModel.EnableSilentInstallation, 
                    viewModel.DisableReboot, 
                    viewModel.EnableInstallationLogging, 
                    viewModel.AutomaticallyDeleteInstallationLogs, 
                    viewModel.KeepNewestInstallationLogs,
                    viewModel.FilterDuplicateInstallers);
            }
            finally
            {
                mainWindowViewModel.CurrentInstallation.State = InstallationState.Finished;
                mainWindowViewModel.CurrentInstallation = null;
            }

            Task refreshTask = null;
            var refreshCommand = mainWindowViewModel.GetCommand<MainWindowViewModel, RefreshApplicationsCommand>();
            if (refreshCommand != null)
                refreshTask = refreshCommand.ExecuteAsync(null);

            if (installationResult != null)
                _dialogService.ShowDialog(installationResult);
            else
                Log.Warn("No installation result to show.");

            if (refreshTask != null)
                await refreshTask;
        }
    }
}
