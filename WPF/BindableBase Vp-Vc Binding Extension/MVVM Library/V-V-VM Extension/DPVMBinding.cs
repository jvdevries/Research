using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Reflection;

namespace MVVM_Lib
{
    public class DPVMBinding
    {
        private struct Binding
        {
            public string DPName;
            public DependencyProperty DP;
            public string VMPropertyName;
            public PropertyInfo VMProperty;
        }

        private readonly DependencyObject View;
        private readonly Type ViewType;
        private readonly dynamic ViewModel;
        private readonly List<Binding> BinderEntries;
        
        public DPVMBinding(DependencyObject View, dynamic ViewModel)
        {
            this.View = View;
            this.ViewModel = ViewModel;

            BinderEntries = new List<Binding>();

            ViewType = View.GetType();
        }

        public DependencyProperty CreateDPBinding(string ViewModelPropertyName, string DependencyPropertyName)
        {
            PropertyInfo VMPropertyInfo = ViewModel.GetType().GetProperty(ViewModelPropertyName);
            DependencyProperty DP = DependencyProperty.Register(DependencyPropertyName, VMPropertyInfo.PropertyType, ViewType, new FrameworkPropertyMetadata(GetVMPropertyValue(VMPropertyInfo), OnDPChanged));

            var BinderEntry = new Binding
            {
                DPName = DependencyPropertyName,
                DP = DP,
                VMPropertyName = ViewModelPropertyName,
                VMProperty = VMPropertyInfo
            };
            BinderEntries.Add(BinderEntry);

            return DP;
        }

        private object GetVMPropertyValue(PropertyInfo VMPropertyInfo)
            => VMPropertyInfo.GetValue(ViewModel, null);

        #region Notify ViewModel
        private void OnDPChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => OnDPChanged(d, e, e.NewValue);

        private void OnDPChanged<T>(DependencyObject d, DependencyPropertyChangedEventArgs e, T Value) where T : class
        {
            var oldValue = (T)e.OldValue;

            if (Value == oldValue)
                return;

            var property_changed__name = e.Property.ToString();

            Binding ChangedEntry =
                (from ViewModelBinderEntry in BinderEntries.AsEnumerable()
                 where ViewModelBinderEntry.DPName == property_changed__name
                 select ViewModelBinderEntry)
                 .FirstOrDefault();

            Debug.Assert(!ChangedEntry.Equals(default(Binding)));

            ChangedEntry.VMProperty.SetValue(ViewModel, Value);
        }

        // Overloads for when a Value-type is passed as argument.
        private void OnDPChanged(DependencyObject d, DependencyPropertyChangedEventArgs e, int Value)
            => OnDPChanged(d, e, (object)Value);
        private void OnDPChanged(DependencyObject d, DependencyPropertyChangedEventArgs e, double Value)
            => OnDPChanged(d, e, (object)Value);
        private void OnDPChanged(DependencyObject d, DependencyPropertyChangedEventArgs e, float Value)
            => OnDPChanged(d, e, (object)Value);
        #endregion

        #region Notify View
        public void SetDP<T>(string name, T value)
        {
            Binding ChangedEntry =
                (from ViewModelBinderEntry in BinderEntries.AsEnumerable()
                 where ViewModelBinderEntry.VMPropertyName == name
                 select ViewModelBinderEntry)
                 .FirstOrDefault();

            if (ChangedEntry.Equals(default(Binding))) return;

            View.SetValue(ChangedEntry.DP, value);
        }
        #endregion
    }
}