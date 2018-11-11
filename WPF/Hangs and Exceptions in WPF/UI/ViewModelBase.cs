using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Model;
using UI;

namespace VM
{
    public partial class ViewModelBase : BindableBase
    {
        public string LeftText
        {
            get => _leftText;
            set => SetProperty(ref _leftText, value);
        }
        private string _leftText = "Left";

        public string MiddleText
        {
            get => _middleText;
            set => SetProperty(ref _middleText, value);
        }
        private string _middleText = "Middle";

    }
}