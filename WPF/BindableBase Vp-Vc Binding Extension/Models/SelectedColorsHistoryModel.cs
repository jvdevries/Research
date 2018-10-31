using System;
using System.Collections.Generic;
using System.Text;
using MVVM_Lib;
using System.Windows.Media;

namespace DPBindableBase
{
    public static class SelectedColorsHistoryModel
    {
        private static readonly List<string> MadeFavorites = new List<string>();

        public static void Add(SolidColorBrush selectedColor)
        {
            if (MadeFavorites.Count == 3)
                MadeFavorites.RemoveAt(0);
            MadeFavorites.Add(getColorName(selectedColor));
        }

        private static string getColorName(SolidColorBrush selectedColor)
        {
            if (selectedColor.Color.Equals(new SolidColorBrush(Colors.Black).Color))
                return "Black";
            if (selectedColor.Color.Equals(new SolidColorBrush(Colors.White).Color))
                return "White";
            if (selectedColor.Color.Equals(new SolidColorBrush(Colors.Red).Color))
                return "Red";
            if (selectedColor.Color.Equals(new SolidColorBrush(Colors.Green).Color))
                return "Green";
            if (selectedColor.Color.Equals(new SolidColorBrush(Colors.Blue).Color))
                return "Blue";
            if (selectedColor.Color.Equals(new SolidColorBrush(Colors.Gold).Color))
                return "Gold";

            return "Unknown";
        }

        public static string Get()
        {
            var output = new StringBuilder();
            for (var i = MadeFavorites.Count - 1; i >= 0; i--)
                output.AppendLine(MadeFavorites[i]);
            return output.ToString();
        }
    }
}