﻿using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Collections.Generic;
using nkristek.MVVMBase.ViewModels;
using Stein.Localizations;
using Stein.Services;

namespace Stein.Views
{
    public class WpfDialogService : IDialogService
    {
        private Stack<Window> windowStack = new Stack<Window>();

        public WpfDialogService(Window root)
        {
            windowStack.Push(root);
        }

        public void ShowPopup(ViewModel contextViewModel)
        {
            var popup = CreateView<Popup>(contextViewModel);
            popup.IsOpen = true;
        }

        public bool? ShowDialog(DialogModel dialogViewModel)
        {
            var dialog = CreateView<Window>(dialogViewModel);
            dialog.Owner = windowStack.Peek();

            windowStack.Push(dialog);
            
            bool? result;
            try
            {
                result = dialog.ShowDialog();
            }
            finally
            {
                windowStack.Pop();
            }

            return result;
        }

        private TView CreateView<TView>(ViewModel contextViewModel) where TView : FrameworkElement
        {
            if (contextViewModel == null)
                throw new ArgumentNullException("contextViewModel");

            var resourceKey = contextViewModel.GetType();
            var view = Application.Current.TryFindResource(resourceKey);

            if (view == null)
                view = Application.Current.MainWindow.TryFindResource(resourceKey);

            if (view == null)
                throw new NotSupportedException(Strings.NoDialogExistsForDialogModel);

            if (!(view is TView))
                throw new Exception(String.Format(Strings.ViewHasWrongTypeX, resourceKey.Name, view.GetType().Name, typeof(TView).Name));

            var viewAsTargetType = (TView)view;
            viewAsTargetType.DataContext = contextViewModel;

            return viewAsTargetType;
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }
        
        public void ShowError(Exception exception)
        {
            var exceptionMessage = LogService.BuildExceptionMessage(exception);
            ShowMessage(exceptionMessage);
        }
    }
}
