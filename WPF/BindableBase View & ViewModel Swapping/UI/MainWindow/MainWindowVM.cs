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
            _CurrentUserControlVM = new ViewModelA("");
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
            if (currentView == 0 && currentViewModel == 0)
                ViewSwapper.Swap(ViewResources, typeof(ViewB), CurrentUserControlVM.GetType());
            if (currentView == 0 && currentViewModel == 1)
                ViewSwapper.Swap(ViewResources, typeof(ViewB), CurrentUserControlVM.GetType());
            if (currentView == 1 && currentViewModel == 0)
                ViewSwapper.Swap(ViewResources, typeof(ViewA), CurrentUserControlVM.GetType());
            if (currentView == 1 && currentViewModel == 1)
                ViewSwapper.Swap(ViewResources, typeof(ViewA), CurrentUserControlVM.GetType());

            currentView++;

            if (currentView == 2)
                currentView = 0;
        }

        private int currentView = 0;
        private void SetView(int to) // Should be enum
        {
            if (currentView == 0 && currentViewModel == 0)
                ViewSwapper.Swap(ViewResources, typeof(ViewB), CurrentUserControlVM.GetType());
            if (currentView == 0 && currentViewModel == 1)
                ViewSwapper.Swap(ViewResources, typeof(ViewB), CurrentUserControlVM.GetType());
            if (currentView == 1 && currentViewModel == 0)
                ViewSwapper.Swap(ViewResources, typeof(ViewA), CurrentUserControlVM.GetType());
            if (currentView == 1 && currentViewModel == 1)
                ViewSwapper.Swap(ViewResources, typeof(ViewA), CurrentUserControlVM.GetType());
        }

        private void SetViewModel(int to, string VMString = null) // Should be enum
        {
            if (ForceViewRefresh)
                CurrentUserControlVM = null;

            if (VMString == null && to == 0)
                VMString = " - SetViewModel B";
            if (VMString == null && to == 1)
                VMString = " - SetViewModel A";

            if (to == 0)
                CurrentUserControlVM = new ViewModelB(VMString);
            else
                CurrentUserControlVM = new ViewModelA(VMString);
        }

        private void SwapViewModel()
        {
            if (ForceViewRefresh)
                CurrentUserControlVM = null;

            if (currentViewModel == 0)
            {
                CurrentUserControlVM = new ViewModelB(" - S-VMB");
                currentViewModel = 1;
            }
            else
            {
                CurrentUserControlVM = new ViewModelA(" - S-VMA");
                currentViewModel = 0;
            }
        }

        public ICommand SwitchView => new RelayCommand(x => SwapView());

        public ICommand SwitchViewModel => new RelayCommand(x => SwapViewModel());

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
                ViewSwapper.Swap(ViewResources, typeof(ViewA), typeof(ViewModelA));
                if (ForceViewRefresh) CurrentUserControlVM = null;
                CurrentUserControlVM = new ViewModelA(" - 1");
            }
            void SwitchToCaseTwo()
            {
                ViewSwapper.Swap(ViewResources, typeof(ViewB), typeof(ViewModelA));
                if (ForceViewRefresh) CurrentUserControlVM = null;
                CurrentUserControlVM = new ViewModelA(" - 2");
            }
            void SwitchToCaseTree()
            {
                ViewSwapper.Swap(ViewResources, typeof(ViewA), typeof(ViewModelB));
                if (ForceViewRefresh) CurrentUserControlVM = null;
                CurrentUserControlVM = new ViewModelB(" - 3");
            }
            void SwitchToCaseFour()
            {
                ViewSwapper.Swap(ViewResources, typeof(ViewB), typeof(ViewModelB));
                if (ForceViewRefresh) CurrentUserControlVM = null;
                CurrentUserControlVM = new ViewModelB(" - 4");
            }
        }
    }
}