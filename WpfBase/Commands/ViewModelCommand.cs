﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfBase.ViewModels;

namespace WpfBase.Commands
{
    public abstract class ViewModelCommand<TViewModel>
        : Command where TViewModel : ViewModel
    {
        public ViewModelCommand(TViewModel parent, object view = null)
        {
            Parent = parent;
            View = view;
        }

        private WeakReference<TViewModel> _Parent;

        private TViewModel Parent
        {
            get
            {
                if (_Parent != null && _Parent.TryGetTarget(out TViewModel parent))
                    return parent;
                return null;
            }

            set
            {
                _Parent = value != null ? new WeakReference<TViewModel>(value) : null;
            }
        }

        private WeakReference<object> _View;

        private object View
        {
            get
            {
                if (_View != null && _View.TryGetTarget(out object view))
                    return view;
                return Parent?.View;
            }

            set
            {
                _View = value != null ? new WeakReference<object>(value) : null;
            }
        }

        public virtual bool CanExecute(TViewModel viewModel, object view, object parameter)
        {
            return true;
        }

        public abstract void Execute(TViewModel viewModel, object view, object parameter);

        public Type AcceptedViewModelType
        {
            get
            {
                return typeof(TViewModel);
            }
        }

        public sealed override bool CanExecute(object parameter)
        {
            return CanExecute(Parent, View, parameter);
        }

        public sealed override void Execute(object parameter)
        {
            if (!CanExecute(Parent, View, parameter))
                throw new InvalidOperationException("canexecute");

            Execute(Parent, View, parameter);
        }
    }
}
