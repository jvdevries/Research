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

        public static void Add(string toAdd)
        {
            if (MadeFavorites.Count == 3)
                MadeFavorites.RemoveAt(0);
            MadeFavorites.Add(toAdd);
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