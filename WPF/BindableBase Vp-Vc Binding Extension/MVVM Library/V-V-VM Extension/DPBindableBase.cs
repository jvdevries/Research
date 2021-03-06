﻿using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace MVVM_Lib
{
    public abstract class DPBindableBase : BindableBase, IDPBindableBase
    {
        public DPVMBinding DPViewModelBindings;

        public void SetDPViewModelBindings(DPVMBinding binding)
        {
            Debug.Assert(binding != null);
            DPViewModelBindings = binding;
        }

        protected override bool SetProperty<T>(ref T member, T val, [CallerMemberName] string propertyName = null)
        {
            DPViewModelBindings.OnVMChanged(propertyName, val);

            return base.SetProperty(ref member, val, propertyName);
        }
    }
}