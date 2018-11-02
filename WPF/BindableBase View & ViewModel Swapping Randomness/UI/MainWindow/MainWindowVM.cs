using System;
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
            _ForceViewRefresh = false;
            _InvalidatesImplicitDataTemplateResources = false;
            _CurrentUserControlVM = new ViewModelA(" Init");
        }

        public BindableBase CurrentUserControlVM
        {
            get => _CurrentUserControlVM;
            set => SetProperty(ref _CurrentUserControlVM, value);
        }
        private BindableBase _CurrentUserControlVM;

        public bool ForceViewRefresh
        {
            get => _ForceViewRefresh;
            set => SetProperty(ref _ForceViewRefresh, value);
        }
        private bool _ForceViewRefresh;

        public bool InvalidatesImplicitDataTemplateResources
        {
            get => _InvalidatesImplicitDataTemplateResources;
            set
            {
                SetProperty(ref _InvalidatesImplicitDataTemplateResources, value);
                ViewResources.InvalidatesImplicitDataTemplateResources = _InvalidatesImplicitDataTemplateResources;
            }
        }
        private bool _InvalidatesImplicitDataTemplateResources;

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
            if (CurrentUserControlVM.GetType() == typeof(ViewModelA))
                SetViewModel(typeof(ViewModelB), " - A -> B");
            else if (CurrentUserControlVM.GetType() == typeof(ViewModelB))
                SetViewModel(typeof(ViewModelA), " - B -> A");
        }

        private void SwapViewAndSetViewModel()
        {
            SwapView();
            SetViewModel(CurrentUserControlVM.GetType());
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
                ViewBinder.BindView(ViewResources, typeof(View1), CurrentUserControlVM.GetType());
                _currentView = typeof(View1);
            }
            else if (VType == typeof(View2))
            {
                ViewBinder.BindView(ViewResources, typeof(View2), CurrentUserControlVM.GetType());
                _currentView = typeof(View2);
            }
        }

        private void SetViewModel(Type VMType, string VMString = null)
        {
            if (ForceViewRefresh)
                CurrentUserControlVM = null;

            if (VMString == null && VMType == typeof(ViewModelA))
                VMString = " - Set B: " + DateTime.Now.Ticks;
            if (VMString == null && VMType == typeof(ViewModelB))
                VMString = " - Set A: " + DateTime.Now.Ticks;

            if (VMType == typeof(ViewModelA)) CurrentUserControlVM = new ViewModelA(VMString);
            if (VMType == typeof(ViewModelB)) CurrentUserControlVM = new ViewModelB(VMString);
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