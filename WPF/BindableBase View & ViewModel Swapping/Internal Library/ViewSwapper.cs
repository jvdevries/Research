using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using static System.String;

namespace EasyFramework
{
    /// <summary>
    /// ViewSwapper (re)binds a View to a ViewModel through the DataTemplate part of ViewModel first XAML code.
    /// </summary>
    public static class ViewSwapper
    {
        public static void Swap(ResourceDictionary D, Type V, Type VM)
        {
            if (D == null) return;

            var DT = CreateTemplate(V, VM);

            if (DT == null) return;

            if (D.Contains(DT.DataTemplateKey ?? throw new InvalidOperationException()))
                // Cannot add an existing key.
                D.Remove(DT.DataTemplateKey);

            D.Add(DT.DataTemplateKey, DT);
        }

        private static DataTemplate CreateTemplate(Type VType, Type VMType)
        {
            if (VType == null || VMType == null)
                return null;

            const string xamlTemplate = "<DataTemplate DataType=\"{{x:Type vm:{0}}}\"><v:{1} /></DataTemplate>";
            var xaml = Format(xamlTemplate, VMType.Name, VType.Name);

            var context = new ParserContext { XamlTypeMapper = new XamlTypeMapper(new string[0]) };

            context.XamlTypeMapper.AddMappingProcessingInstruction("v", VType.Namespace, VType.Assembly.FullName);
            context.XamlTypeMapper.AddMappingProcessingInstruction("vm", VMType.Namespace, VMType.Assembly.FullName);

            context.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            context.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");
            context.XmlnsDictionary.Add("v", "v");
            context.XmlnsDictionary.Add("vm", "vm");

            var template = (DataTemplate)XamlReader.Parse(xaml, context);

            return template;
        }
    }
}