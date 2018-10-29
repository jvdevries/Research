using System;
using System.Collections.Generic;
using System.Text;
using MVVM_Lib;
using System.Windows.Media;

namespace DPBindableBase
{
    public static class FavoritesHistoryModel
    {
        private static readonly List<string> madeFavorites = new List<string>();

        public static void Add(string toAdd)
        {
            if (madeFavorites.Count == 3)
                madeFavorites.RemoveAt(0);
            madeFavorites.Add(toAdd);
        }

        public static string Get()
        {
            var output = new StringBuilder();
            for (var i = madeFavorites.Count - 1; i >= 0; i--)
                output.AppendLine(madeFavorites[i]);
            return output.ToString();
        }
    }
}