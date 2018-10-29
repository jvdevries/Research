using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MVVM_Lib;

namespace DPBindableBase
{
    public partial class ColorPicker : UserControl
    {
        // The ColorPicker View requires access to the ViewModel for V -> VM syncing.
        // An interface is used to show that the ViewModel is easily replacable.
        // "IColorPicker: IDPBindableBase" declares the ViewModel Properties and their accessors.
        private IColorPickerVM ViewModel => (IColorPickerVM)Resources[nameof(ViewModel)];

        public ColorPicker()
        {
            InitializeComponent();

            // BoilerPlate code: initialises the Binder to View and ViewModel.
            var binder = new DPVMBinding(this, ViewModel);

            // BoilerPlate code: set the Binder in the ViewModel for VM -> V syncing.
            // Note: "ColorPickerVM: DPBindableBase, IColorPickerVM" has no code for this.
            ViewModel.SetDPViewModelBindings(binder);

            // For Blog layout purposes, these are pre-defined here to minimize the line-length.
            // Note: nameof() requires a Type definition to work, so var cannot be used.
            DependencyProperty DPSelectedColor, DPStoreFavorite, DPRestoreFavorite, DPRemoveFavorite;

            // Setup binding points for the Parent's View, and sync them to the VM Properties.
            DPSelectedColor = binder.CreateDPBinding(nameof(ViewModel.SelectedColor), nameof(DPSelectedColor));
            DPStoreFavorite = binder.CreateDPBinding(nameof(ViewModel.StoreFavorite), nameof(DPStoreFavorite));
            DPRestoreFavorite = binder.CreateDPBinding(nameof(ViewModel.RestoreFavorite), nameof(DPRestoreFavorite));
            DPRemoveFavorite = binder.CreateDPBinding(nameof(ViewModel.RemoveFavorite), nameof(DPRemoveFavorite));
        }

        // This allows the Parent's View to pass an ItemStyle to the ColorPicker items.
        public Style ItemStyle { get; set; }

        // This allows the Parent's View to pass an the Colors for the ColorPicker.
        public SolidColorBrush[] Colors { get; set; }
    }
}