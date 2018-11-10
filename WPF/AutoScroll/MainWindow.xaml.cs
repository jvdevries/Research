using System;
using System.Text;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CollectionManager
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Prevent abusive CPU usage by limiting the FPS.
            Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(Timeline),
                new FrameworkPropertyMetadata { DefaultValue = 15 });

            Loaded += (o, i) => { Animate(); };
            SizeChanged += (o, i) => { Animate(); };
        }
        private static void VerticalScroller(TextBox tb, FrameworkElement parent)
        {
            var heightTextBox = new FormattedText(tb.Text, System.Globalization.CultureInfo.CurrentCulture, 
                FlowDirection.LeftToRight, new Typeface(tb.FontFamily.Source), tb.FontSize, tb.Foreground).Height;
            var endPos = parent.ActualHeight - tb.LineCount * heightTextBox;

            AutoScroll(tb, endPos, 0, endPos, 0, 0);
        }

        private static void HorizontalScroller(TextBox tb, FrameworkElement parent)
        {
            var textWidthOfTextBox = new FormattedText(tb.Text, System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight, new Typeface(tb.FontFamily.Source), tb.FontSize, tb.Foreground)
                .WidthIncludingTrailingWhitespace;
            var endPos = parent.ActualWidth - textWidthOfTextBox;

            AutoScroll(tb, endPos, endPos, 0, 0, 0);
        }

        private static void AutoScroll(TextBox textBox, double endPosition, double left, double top, double right, double bottom)
        {
            if (endPosition < 0)
            {
                // The Thickness is increased to make the Text move accross the Canvas.
                // ToDo: A margin should be used based upon the letters on the canvas...
                var thickAnimation = new ThicknessAnimation
                {
                    From = new Thickness(0, 0, 0, 0),
                    To = new Thickness(left, top, right, bottom),
                    RepeatBehavior = RepeatBehavior.Forever,
                    AutoReverse = true,
                    Duration = calculateDuration(textBox)
                };
                var myStoryboard = new Storyboard { Name = textBox.Name };
                myStoryboard.Children.Add(thickAnimation);
                Storyboard.SetTarget(thickAnimation, textBox);
                Storyboard.SetTargetProperty(thickAnimation, new PropertyPath(PaddingProperty));
                myStoryboard.Begin();
            }
            else
            {
                // cancel previous effects
                textBox.BeginAnimation(PaddingProperty, null);
            }
        }

        private const double milliSecondPerChar = 30;

        private static Duration calculateDuration(TextBox textBox)
        {
            var miliSeconds = (textBox.Text.Length * milliSecondPerChar);
            return new Duration(TimeSpan.FromMilliseconds(miliSeconds));
        }

        private void Animate()
        {
            // A quick hack for the example: animate Canvas & DockPanel elements of AnimatedWindowGrid.
            foreach (var element in FindVisualChildren<TextBox>(AnimatedWindowGrid))
            {
                var parent = VisualTreeHelper.GetParent(element);
                if (parent.GetType() == typeof(Canvas)) HorizontalScroller(element, parent as FrameworkElement);
                if (parent.GetType() == typeof(DockPanel)) VerticalScroller(element, parent as FrameworkElement);
            }
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            // A quick hack for the example: find the children.
            if (depObj == null) yield break;
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);
                if (child is T variable)
                    yield return variable;

                foreach (var childOfChild in FindVisualChildren<T>(child))
                    yield return childOfChild;
            }
        }
    }
}