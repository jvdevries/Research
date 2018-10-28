﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MVVM_Util;

namespace DPBindableBase
{
    public partial class ColorPicker : UserControl
    {
        private ColorPickerVM ViewModel => (ColorPickerVM)Resources[nameof(ViewModel)];

        public readonly DependencyProperty DPSelectedColor, DPStoreFavoriteCommand, DPRestoreFavoriteCommand;

        public ColorPicker()
        {
            InitializeComponent();

            var _DPVMBindings = new DPVMBinding(this, ViewModel); // Binds DP to VM
            ViewModel.DPViewModelBindings = _DPVMBindings; // Inform VM about our bindings

            DPSelectedColor = _DPVMBindings.CreateDPBinding(nameof(ViewModel.SelectedColor), nameof(DPSelectedColor));
            DPStoreFavoriteCommand = _DPVMBindings.CreateDPBinding(nameof(ViewModel.StoreFavoriteCommand), nameof(DPStoreFavoriteCommand));
            DPRestoreFavoriteCommand = _DPVMBindings.CreateDPBinding(nameof(ViewModel.RestoreFavoriteCommand), nameof(DPRestoreFavoriteCommand));
        }

        // Parameters for the View
        public Style ItemStyle { get; set; }

        public SolidColorBrush[] Colors { get; set; }
    }
}