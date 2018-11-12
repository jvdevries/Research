using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWPAutoScroller
{
    public class AutoScrollerViewModel : IAutoScrollerViewModel
    {
        public string AutoText => "This very long text is brought to you by " + nameof(IAutoScrollerViewModel) + " with ❤";
    }
}
