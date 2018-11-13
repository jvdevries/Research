using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace UWPAutoScroller
{
    public sealed partial class AutoScroller : UserControl
    {
        private IAutoScrollerViewModel ViewModel { get; set; }
        private bool _autoTextWidthControllerInitialValueUsed = false;
        private readonly float _autoTextWidthControllerInitialSize = 10000f;
        private readonly int _timeNeededPerCharacterReadInMsec = 35;

        public string AutoText
        {
            get
            {
                if (_autoTextWidthControllerInitialValueUsed)
                {
                    // The initial value was used, so a size check is needed.
                    AutoTextBlock.Text = ViewModel.AutoText;

                    var widthNeeded = GetAutoScrollerNecessaryWidth();
                    if (AutoTextWidthController.Width < widthNeeded)
                    {
                        AutoTextWidth = 0;
                        _autoTextWidthControllerInitialValueUsed = false;
                    }
                }
                return ViewModel.AutoText;
            }
        }
        public double AutoTextWidth
        {
            get => GetAutoScrollerNecessaryWidth();
            set => AutoTextWidthController.Width = GetAutoScrollerNecessaryWidth();
        }

        private readonly Compositor _compositor;

        public AutoScroller()
        {
            this.InitializeComponent();
            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            // Needed for Translation.X or it'll crash...
            ElementCompositionPreview.SetIsTranslationEnabled(AutoTextBlock, true);

            Window.Current.SizeChanged += (o, e) =>
            {
                Animate();
            };

            DataContextChanged += (o, e) =>
            {
                ViewModel = DataContext as IAutoScrollerViewModel;
            };
        }

        private void HorizontalAutoScrollerLoaded(object sender, RoutedEventArgs e)
        {
            Animate();
        }

        private void Animate()
        {
            // Get the width of the visible TextBox
            var renderWidth = PositionAndSizeDeterminer.RenderSize.Width;

            // Get the invisible width for the TextBox
            var desiredWidth = GetAutoScrollerNecessaryWidth();

            // Calculate the scroll distance
            var characters = AutoTextBlock.Text.Length;
            var scrollDistance = -1f * (desiredWidth - (float)renderWidth);

            // Do not scroll if not needed
            if (scrollDistance >= 0) return;

            // Calculate the duration based on time per character read
            var duration = characters * _timeNeededPerCharacterReadInMsec * 1.4; // Offset for the initial delay. 

            var animation = _compositor.CreateScalarKeyFrameAnimation();
            animation.InsertKeyFrame(0.0f, 0, _compositor.CreateLinearEasingFunction());
            animation.InsertKeyFrame(0.2f, 0, _compositor.CreateLinearEasingFunction());
            animation.InsertKeyFrame(0.8f, scrollDistance, _compositor.CreateLinearEasingFunction());
            animation.InsertKeyFrame(1.0f, scrollDistance, _compositor.CreateLinearEasingFunction());
            animation.IterationBehavior = AnimationIterationBehavior.Forever;
            animation.Duration = TimeSpan.FromMilliseconds(duration);

            ElementCompositionPreview.GetElementVisual(AutoTextBlock).StartAnimation("Translation.X", animation);
        }

        private float GetAutoScrollerNecessaryWidth()
        {
            if (AutoTextBlock.Text.Length == 0)
            {
                _autoTextWidthControllerInitialValueUsed = true;
                return _autoTextWidthControllerInitialSize;
            }

            AutoTextBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            return (float)AutoTextBlock.DesiredSize.Width;
        }
    }
}
