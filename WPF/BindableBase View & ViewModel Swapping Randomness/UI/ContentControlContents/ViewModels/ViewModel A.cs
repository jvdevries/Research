using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using MVVM_Util;

namespace EasyFramework
{
    public class ViewModelA : BindableBase
    {
        public ViewModelA(string suffix)
        {
            Text = Text + Environment.NewLine + suffix;
        }

        public string Text { get; } = "VM A";
    }
}