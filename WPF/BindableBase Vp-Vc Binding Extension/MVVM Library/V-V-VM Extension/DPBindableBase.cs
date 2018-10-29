using System.Runtime.CompilerServices;

namespace MVVM_Lib
{
    public abstract class DPBindableBase : BindableBase
    {
        private DPVMBinding DPViewModelBindings;

        public void SetDPViewModelBindings(DPVMBinding binding)
            => DPViewModelBindings = binding;

        protected override bool SetProperty<T>(ref T member, T val, [CallerMemberName] string propertyName = null)
        {
            DPViewModelBindings.SetDP(propertyName, val);

            return base.SetProperty(ref member, val, propertyName);
        }
    }
}