using System.Windows.Input;
using System.Windows.Media;
using MVVM_Lib;

namespace DPBindableBase
{
    public class ColorPickerVM : MVVM_Lib.DPBindableBase, IColorPickerVM
    {
        public SolidColorBrush SelectedColor
        {
            get => _SelectedColor;
            set => SetProperty(ref _SelectedColor, value, nameof(SelectedColor));
        }
        private SolidColorBrush _SelectedColor;

        public SolidColorBrush FavoriteColor
        {
            get => _FavoriteColor;
            set => SetProperty(ref _FavoriteColor, value, nameof(FavoriteColor));
        }
        private SolidColorBrush _FavoriteColor;

        public ICommand RestoreFavoriteCommand => new RelayCommand(x =>
        {
            SelectedColor = FavoriteColor;
        },
        x => FavoriteColor != null);

        public ICommand StoreFavoriteCommand => new RelayCommand(x =>
        {
            FavoriteColor = SelectedColor;
        },
        x => SelectedColor != null);

        public ICommand RemoveFavoriteCommand => new RelayCommand(x =>
        {
            FavoriteColor = null;
        });
    }
}