using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MVVM_Lib;

namespace DPBindableBase
{
    public partial class ColorPicker : UserControl
    {
        private IColorPickerVM ViewModel => (IColorPickerVM)Resources[nameof(ViewModel)];

        public ColorPicker()
        {
            InitializeComponent();

            var _DPVMBindings = new DPVMBinding(this, ViewModel); // Binds DP to VM
            ViewModel.SetDPViewModelBindings(_DPVMBindings); // Inform VM about our bindings

            // Type is needed to be able to use nameof().
            DependencyProperty DPSelectedColor = _DPVMBindings.CreateDPBinding(nameof(ViewModel.SelectedColor), nameof(DPSelectedColor));
            DependencyProperty DPStoreFavoriteCommand = _DPVMBindings.CreateDPBinding(nameof(ViewModel.StoreFavoriteCommand), nameof(DPStoreFavoriteCommand));
            DependencyProperty DPRestoreFavoriteCommand = _DPVMBindings.CreateDPBinding(nameof(ViewModel.RestoreFavoriteCommand), nameof(DPRestoreFavoriteCommand));
            DependencyProperty DPRemoveFavoriteCommand = _DPVMBindings.CreateDPBinding(nameof(ViewModel.RemoveFavoriteCommand), nameof(DPRemoveFavoriteCommand));
        }

        // Parameter for the View
        public Style ItemStyle { get; set; }

        // Parameter for the View
        public SolidColorBrush[] Colors { get; set; }
    }
}