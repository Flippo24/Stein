﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TempManager.Configuration;
using TempManager.ViewModels;
using WpfBase.Commands;

namespace TempManager.Commands.MainViewModelCommands
{
    public class DeleteApplicationCommand
        : ViewModelCommand<MainViewModel>
    {
        public DeleteApplicationCommand(MainViewModel parent, object view = null) : base(parent, view) { }
        
        public override void Execute(MainViewModel viewModel, object view, object parameter)
        {
            var applicationToDelete = parameter as ApplicationViewModel;
            if (applicationToDelete == null)
                return;

            var setupToDelete = AppConfigurationService.CurrentConfiguration.Setups.FirstOrDefault(s => s.Path == applicationToDelete.Path);
            if (setupToDelete == null)
                return;

            AppConfigurationService.CurrentConfiguration.Setups.Remove(setupToDelete);
            if (!AppConfigurationService.SaveConfiguration())
                return;

            viewModel.Applications.Remove(applicationToDelete);
        }
    }
}
