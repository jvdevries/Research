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

        public ICommand SwitchViewAndOrViewModelGoodPattern => SwitchViewAndOrViewModelCommand(true);
        public ICommand SwitchViewAndOrViewModelBadPattern => SwitchViewAndOrViewModelCommand(false);

        private ICommand SwitchViewAndOrViewModelCommand(bool useBreakingPattern)
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
                if (ForceViewRefresh) CurrentUserControlVM = new ViewModelB("");
                CurrentUserControlVM = new ViewModelA(" - 1");
            }
            void SwitchToCaseTwo()
            {
                ViewSwapper.Swap(ViewResources, typeof(ViewB), typeof(ViewModelA));
                if (ForceViewRefresh) CurrentUserControlVM = new ViewModelB("");
                CurrentUserControlVM = new ViewModelA(" - 2");
            }
            void SwitchToCaseTree()
            {
                ViewSwapper.Swap(ViewResources, typeof(ViewA), typeof(ViewModelB));
                if (ForceViewRefresh) CurrentUserControlVM = new ViewModelA("");
                CurrentUserControlVM = new ViewModelB(" - 1");
            }
            void SwitchToCaseFour()
            {
                ViewSwapper.Swap(ViewResources, typeof(ViewB), typeof(ViewModelB));
                if (ForceViewRefresh) CurrentUserControlVM = new ViewModelA("");
                CurrentUserControlVM = new ViewModelB(" - 2");
            }
        }
    }
}