using System.Windows.Input;
using System.Windows.Media;
using MVVM_Lib;

namespace DPBindableBase
{
    public interface IColorPickerVM : MVVM_Lib.IDPBindableBase
    {
        SolidColorBrush SelectedColor { get; set; }
        ICommand StoreFavoriteCommand { get; }
        ICommand RestoreFavoriteCommand { get; }
        ICommand RemoveFavoriteCommand { get; }
    }
}