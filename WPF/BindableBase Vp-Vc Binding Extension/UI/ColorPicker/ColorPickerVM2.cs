using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using MVVM_Lib;

namespace DPBindableBase
{
    public class ColorPickerVM2 : MVVM_Lib.DPBindableBase, IColorPickerVM
    {
        public SolidColorBrush[] Colors
        {
            get => _Colors;
            set => SetProperty(ref _Colors, value, nameof(Colors));
        }
        private SolidColorBrush[] _Colors = 
        {
            new SolidColorBrush(System.Windows.Media.Colors.Purple),
            new SolidColorBrush(System.Windows.Media.Colors.Yellow),
            new SolidColorBrush(System.Windows.Media.Colors.Orange)
        };
        
        public SolidColorBrush SelectedColor
        {
            get => _SelectedColor ?? (_SelectedColor = Colors.FirstOrDefault());
            set
            {
                SetProperty(ref _SelectedColor, value, nameof(SelectedColor));
                SelectedColorsHistoryModel.Add(_SelectedColor);
            }
        }
        private SolidColorBrush _SelectedColor;

        public SolidColorBrush FavoriteColor
        {
            get => _FavoriteColor;
            set => SetProperty(ref _FavoriteColor, value, nameof(FavoriteColor));
        }

        private SolidColorBrush _FavoriteColor;

        public ICommand RestoreFavorite => new RelayCommand(x =>
        {
            SelectedColor = FavoriteColor;
        },
        x => FavoriteColor != null);

        public ICommand StoreFavorite => new RelayCommand(x =>
        {
            FavoriteColor = SelectedColor;
        },
        x => SelectedColor != null);

        public ICommand RemoveFavorite => new RelayCommand(x =>
        {
            FavoriteColor = null;
        });
    }
}