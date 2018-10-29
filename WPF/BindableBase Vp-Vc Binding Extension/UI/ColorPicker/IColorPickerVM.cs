using System.Windows.Input;
using System.Windows.Media;
using MVVM_Lib;

namespace DPBindableBase
{
    public interface IColorPickerVM : IDPBindableBase
    {
        SolidColorBrush SelectedColor { get; set; }
        ICommand StoreFavorite { get; }
        ICommand RestoreFavorite { get; }
        ICommand RemoveFavorite { get; }
    }
}