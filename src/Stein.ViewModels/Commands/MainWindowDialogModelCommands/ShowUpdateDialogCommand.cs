﻿using System;
using NKristek.Smaragd.Attributes;
using NKristek.Smaragd.Commands;
using Stein.Presentation;

namespace Stein.ViewModels.Commands.MainWindowDialogModelCommands
{
    public class ShowUpdateDialogCommand
        : ViewModelCommand<MainWindowDialogModel>
    {
        private readonly IDialogService _dialogService;

        public ShowUpdateDialogCommand(IDialogService dialogService)
        {
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        }

        /// <inheritdoc />
        [CanExecuteSource(nameof(MainWindowDialogModel.AvailableUpdate))]
        protected override bool CanExecute(MainWindowDialogModel viewModel, object parameter)
        {
            return viewModel.AvailableUpdate != null;
        }

        /// <inheritdoc />
        protected override void Execute(MainWindowDialogModel viewModel, object parameter)
        {
            _dialogService.ShowDialog(viewModel.AvailableUpdate);
        }
    }
}
