﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using MVVM_Util;

namespace EasyFramework
{
    public class MainWindowVM : BindableBase
    {
        public ResourceDictionary ViewResources;

        public MainWindowVM()
        {
            _forceViewRefresh = false;
            _invalidatesImplicitDataTemplateResources = false;
            _selectedViewModel = new ViewModelA(" Init");
        }

        public ViewModelBase SelectedViewModel
        {
            get => _selectedViewModel;
            set => SetProperty(ref _selectedViewModel, value);
        }
        private ViewModelBase _selectedViewModel;

        public bool ForceViewRefresh
        {
            get => _forceViewRefresh;
            set => SetProperty(ref _forceViewRefresh, value);
        }
        private bool _forceViewRefresh;

        public bool InvalidatesImplicitDataTemplateResources
        {
            get => _invalidatesImplicitDataTemplateResources;
            set
            {
                SetProperty(ref _invalidatesImplicitDataTemplateResources, value);
                ViewResources.InvalidatesImplicitDataTemplateResources = _invalidatesImplicitDataTemplateResources;
            }
        }
        private bool _invalidatesImplicitDataTemplateResources;

        public ICommand SwitchViewAndOrViewModelGoodPattern => VVMSequenceCommand(true);
        public ICommand SwitchViewAndOrViewModelBadPattern => VVMSequenceCommand(false);

        private void SwapView()
        {
            if (_currentView == typeof(View1))
                SetView(typeof(View2));
            else if (_currentView == typeof(View2))
                SetView(typeof(View1));
        }

        private void SwapViewModel()
        {
            if (SelectedViewModel.GetType() == typeof(ViewModelA))
                SetViewModel(typeof(ViewModelB), " - A -> B");
            else if (SelectedViewModel.GetType() == typeof(ViewModelB))
                SetViewModel(typeof(ViewModelA), " - B -> A");
        }

        private void SwapViewAndSetViewModel()
        {
            SwapView();
            SetViewModel(SelectedViewModel.GetType());
        }


        private void SwapViewModelAndSetView()
        {
            SwapViewModel();
            SetView(_currentView);
        }

        // This is a quick & fast hack: this should grab the current View from ViewResources.
        private Type _currentView = typeof(View1);
        private void SetView(Type VType)
        {
            if (VType == typeof(View1))
            {
                ViewBinder.BindView(ViewResources, typeof(View1), typeof(ViewModelBase));
                _currentView = typeof(View1);
            }
            else if (VType == typeof(View2))
            {
                ViewBinder.BindView(ViewResources, typeof(View2), typeof(ViewModelBase));
                _currentView = typeof(View2);
            }
        }

        private void SetViewModel(Type VMType, string VMString = null)
        {
            if (ForceViewRefresh)
                SelectedViewModel = null;

            if (VMString == null && VMType == typeof(ViewModelA))
                VMString = " - Set B: " + DateTime.Now.Ticks;
            if (VMString == null && VMType == typeof(ViewModelB))
                VMString = " - Set A: " + DateTime.Now.Ticks;

            if (VMType == typeof(ViewModelA)) SelectedViewModel = new ViewModelA(VMString);
            if (VMType == typeof(ViewModelB)) SelectedViewModel = new ViewModelB(VMString);
        }


        public ICommand SwitchView => new RelayCommand(x => SwapView());

        public ICommand SwitchViewModel => new RelayCommand(x => SwapViewModel());
        public ICommand SwitchViewAndSetViewModel => new RelayCommand(x => SwapViewAndSetViewModel());

        public ICommand SwitchViewModelAndSetView => new RelayCommand(x => SwapViewModelAndSetView());

        private ICommand VVMSequenceCommand(bool useBreakingPattern)
        {
            var CommandToExecute = 0;
            return new RelayCommand(x =>
            {
                CommandToExecute++;
                switch (CommandToExecute)
                {
                    case 1:
                        SwitchToCaseOne();
                        break;
                    case 2:
                        if (useBreakingPattern) SwitchToCaseTwo();
                        else SwitchToCaseTree();
                        break;
                    case 3:
                        if (useBreakingPattern) SwitchToCaseTree();
                        else SwitchToCaseTwo();
                        break;
                    case 4:
                        SwitchToCaseFour();
                        CommandToExecute = 0;
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            });

            void SwitchToCaseOne()
            {
                SetView(typeof(View1));
                SetViewModel(typeof(ViewModelA), " - 1");
            }
            void SwitchToCaseTwo()
            {
                SetView(typeof(View2));
                SetViewModel(typeof(ViewModelA), " - 2");
            }
            void SwitchToCaseTree()
            {
                SetView(typeof(View1));
                SetViewModel(typeof(ViewModelB), " - 3");
            }
            void SwitchToCaseFour()
            {
                SetView(typeof(View2));
                SetViewModel(typeof(ViewModelB), " - 4");
            }
        }
    }
}