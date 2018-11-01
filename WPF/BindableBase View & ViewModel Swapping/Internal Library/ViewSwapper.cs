using System;
using System.ComponentModel;
using System.Diagnostics;
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
            Debug.Assert(D != null);
            Debug.Assert(V != null);
            Debug.Assert(VM != null);

            var DT = CreateTemplate(V, VM);

            Debug.Assert(DT != null);

            if (D.Contains(DT.DataTemplateKey ?? throw new InvalidOperationException()))
                // Cannot add an existing key.
                D.Remove(DT.DataTemplateKey);

            D.Add(DT.DataTemplateKey, DT);
        }

        private static DataTemplate CreateTemplate(Type VType, Type VMType)
        {
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