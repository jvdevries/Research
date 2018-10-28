using System.Runtime.CompilerServices;

namespace MVVM_Util
{
    public abstract class DPBindableBase : BindableBase
    {
        public DPVMBinding DPViewModelBindings;

        protected override bool SetProperty<T>(ref T member, T val, [CallerMemberName] string propertyName = null)
        {
            DPViewModelBindings.SetDP(propertyName, val);

            return base.SetProperty(ref member, val, propertyName);
        }
    }
}