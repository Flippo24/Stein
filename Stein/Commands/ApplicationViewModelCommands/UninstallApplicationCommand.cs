﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Stein.Services;
using Stein.ViewModels;
using WpfBase.Commands;

namespace Stein.Commands.ApplicationViewModelCommands
{
    public class UninstallApplicationCommand
        : AsyncViewModelCommand<ApplicationViewModel>
    {
        public UninstallApplicationCommand(ApplicationViewModel parent) : base(parent) { }

        public override bool CanExecute(ApplicationViewModel viewModel, object view, object parameter)
        {
            if (viewModel.SelectedInstallerBundle == null)
                return false;

            var mainWindowViewModel = viewModel.FirstParentOfType<MainWindowViewModel>();
            return mainWindowViewModel != null && mainWindowViewModel.CurrentInstallation == null && viewModel.SelectedInstallerBundle.Installers.Any(i => i.IsInstalled);
        }

        public override async Task ExecuteAsync(ApplicationViewModel viewModel, object view, object parameter)
        {
            var mainWindowViewModel = viewModel.FirstParentOfType<MainWindowViewModel>();
            mainWindowViewModel.CurrentInstallation = new InstallationViewModel(mainWindowViewModel)
            {
                Type = InstallationViewModel.InstallationType.Uninstall,
                InstallerCount = 0,
                CurrentIndex = 0
            };
            
            if (ViewModelService.ShowDialog(viewModel.SelectedInstallerBundle) == true)
            {
                var installersToInstall = viewModel.SelectedInstallerBundle.Installers.Where(i => i.IsEnabled && i.IsInstalled);
                mainWindowViewModel.CurrentInstallation.InstallerCount = installersToInstall.Count();

                var currentInstaller = 1;
                foreach (var installer in installersToInstall)
                {
                    mainWindowViewModel.CurrentInstallation.Name = installer.Name;
                    mainWindowViewModel.CurrentInstallation.CurrentIndex = currentInstaller;
                    
                    Debug.WriteLine("Uninstalling " + installer.Name);
                    await InstallService.UninstallAsync(installer, viewModel.EnableSilentInstallation);
                    currentInstaller++;
                }
            }
            
            mainWindowViewModel.CurrentInstallation = null;
            mainWindowViewModel.RefreshApplicationsCommand.Execute(null);
        }
    }
}
