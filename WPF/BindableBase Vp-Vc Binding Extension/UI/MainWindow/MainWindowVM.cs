using MVVM_Lib;
using System.Windows.Media;

namespace DPBindableBase
{
    public class MainWindowVM : BindableBase
    {
        public string FavoritesHistory
        {
            get => _FavoritesHistory;
            set => SetProperty(ref _FavoritesHistory, value, nameof(FavoritesHistory));
        }

        private string _FavoritesHistory;

        public void PullFavoritesHistory()
        {
            FavoritesHistory = SelectedColorsHistoryModel.Get();
        }
    }
}
