﻿using System;
using System.Linq;
using NKristek.Smaragd.Commands;
using NKristek.Smaragd.ViewModels;

namespace Stein.ViewModels.Extensions
{
    public static class ViewModelExtensions
    {
        /// <summary>
        /// Get the command with the specified type out of the <see cref="IViewModel.Commands"/> dictionary of the given <see cref="IViewModel"/>.
        /// </summary>
        /// <typeparam name="TViewModel">Type of the given <see cref="IViewModel"/>.</typeparam>
        /// <typeparam name="TViewModelCommand">Type of the command to get.</typeparam>
        /// <param name="viewModel">The <see cref="IViewModel"/> of which the command should be retrieved.</param>
        /// <returns>Either the command or null if the <see cref="IViewModel.Commands"/> dictionary does not contain a command with the specified <typeparamref name="TViewModelCommand"/> type.</returns>
        public static TViewModelCommand GetCommand<TViewModel, TViewModelCommand>(this TViewModel viewModel) 
            where TViewModel : class, IViewModel 
            where TViewModelCommand : class, IViewModelCommand<TViewModel>
        {
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));
            
            var commandName = typeof(TViewModelCommand).FullName;
            if (!String.IsNullOrEmpty(commandName) 
                && viewModel.Commands.TryGetValue(commandName, out var command) 
                && command is TViewModelCommand tCommand)
                return tCommand;
            return viewModel.Commands.OfType<TViewModelCommand>().FirstOrDefault();
        }
    }
}