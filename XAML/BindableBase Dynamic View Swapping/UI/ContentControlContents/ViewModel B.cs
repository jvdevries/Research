using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using MVVM_Util;

namespace EasyFramework
{
    public class ViewModelB : BindableBase
    {
        public ViewModelB(string suffix)
        {
            _text = _text + suffix;
        }

        public string text => _text;
        private string _text = "ViewModel B";
    }
}