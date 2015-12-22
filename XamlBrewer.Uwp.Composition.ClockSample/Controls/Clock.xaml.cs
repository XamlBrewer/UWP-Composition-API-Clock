using Microsoft.UI.Composition.Toolkit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
//using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace XamlBrewer.Uwp.Controls
{
    public sealed partial class Clock : UserControl
    {
        private Compositor _compositor;
        private ContainerVisual _root;

        private SpriteVisual _hourhand;
        private SpriteVisual _minutehand;
        private SpriteVisual _secondhand;

        private CompositionScopedBatch _batch;

        private DispatcherTimer _timer = new DispatcherTimer();

        public Clock()
        {
            this.InitializeComponent();

            this.Loaded += Clock_Loaded;

            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
        }

        private void Clock_Loaded(object sender, RoutedEventArgs e)
        {
            _root = GetVisual(Container);
            _compositor = _root.Compositor;

            // Hour Ticks
            SpriteVisual tick;
            for (int i = 0; i < 12; i++)
            {
                tick = _compositor.CreateSpriteVisual();
                tick.Size = new Vector2(4.0f, 20.0f);
                tick.Brush = _compositor.CreateColorBrush(Colors.Silver);
                tick.Offset = new Vector3(98.0f, 0.0f, 0);
                tick.CenterPoint = new Vector3(2.0f, 100.0f, 0);
                tick.RotationAngleInDegrees = i * 30;
                _root.Children.InsertAtTop(tick);
            }

            // Hour Hand
            Uri localUri = new Uri("ms-appx:///Assets/hour_hand.png");
            var _imageFactory = CompositionImageFactory.CreateCompositionImageFactory(_compositor);
            CompositionImageOptions options = new CompositionImageOptions()
            {
                DecodeWidth = 32,
                DecodeHeight = 94,
            };

            var _image = _imageFactory.CreateImageFromUri(localUri, options);
            _hourhand = _compositor.CreateSpriteVisual();
            _hourhand.Size = new Vector2(32.0f, 94.0f);
            _hourhand.Brush = _compositor.CreateSurfaceBrush(_image.Surface);

            //_hourhand = _compositor.CreateSpriteVisual();
            //_hourhand.Size = new Vector2(4.0f, 100.0f);
            //_hourhand.Brush = _compositor.CreateColorBrush(Colors.Black);
            _hourhand.CenterPoint = new Vector3(16.0f, 80.0f, 0);
            _hourhand.Offset = new Vector3(84.0f, 20.0f, 0);
            _root.Children.InsertAtTop(_hourhand);

            // Minute Hand
            _minutehand = _compositor.CreateSpriteVisual();
            _minutehand.Size = new Vector2(4.0f, 120.0f);
            _minutehand.Brush = _compositor.CreateColorBrush(Colors.Black);
            _minutehand.CenterPoint = new Vector3(2.0f, 100.0f, 0);
            _minutehand.Offset = new Vector3(98.0f, 0.0f, 0);
            _root.Children.InsertAtTop(_minutehand);

            // Second Hand
            _secondhand = _compositor.CreateSpriteVisual();
            _secondhand.Size = new Vector2(2.0f, 120.0f);
            _secondhand.Brush = _compositor.CreateColorBrush(Colors.Red);
            _secondhand.CenterPoint = new Vector3(1.0f, 100.0f, 0);
            _secondhand.Offset = new Vector3(99.0f, 0.0f, 0);
            _root.Children.InsertAtTop(_secondhand);

            // Add XAML element.
            //var xaml = new Ellipse();
            //xaml.Height = 20;
            //xaml.Width = 20;
            //xaml.Fill = new SolidColorBrush(Colors.Red);
            //xaml.SetValue(Canvas.LeftProperty, 90);
            //xaml.SetValue(Canvas.TopProperty, 90);

            //Container.Children.Add(xaml);

            _timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            // V 1.0 - Incremental, no animation
            // _secondhand.RotationAngleInDegrees += 6.0f;

            // V2.0 - Incremental, animation
            //var animation = _compositor.CreateScalarKeyFrameAnimation();
            //animation.InsertExpressionKeyFrame(1.00f, "this.StartingValue + delta");
            //animation.SetScalarParameter("delta", 6.0f);
            //animation.Duration = TimeSpan.FromMilliseconds(100);
            //_secondhand.StartAnimation("RotationAngleInDegrees", animation);

            // V3.0 - To target, not animation
            //var now = DateTime.Now;
            //_secondhand.RotationAngleInDegrees = now.Second * 6;

            // V4.0 - To target, animation
            var now = DateTime.Now;

            _batch = _compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
            var animation = _compositor.CreateScalarKeyFrameAnimation();
            var seconds = (float)(int)now.TimeOfDay.TotalSeconds;
            animation.InsertKeyFrame(0.00f, seconds * 6);
            animation.InsertKeyFrame(1.00f, (seconds + 1) * 6);
            animation.Duration = TimeSpan.FromMilliseconds(900);
            _secondhand.StartAnimation("RotationAngleInDegrees", animation);
            _batch.End();
            _batch.Completed += Batch_Completed;
        }

        private void Batch_Completed(object sender, CompositionBatchCompletedEventArgs args)
        {
            _batch.Completed -= Batch_Completed;

            var now = DateTime.Now;
            _hourhand.RotationAngleInDegrees = (float)now.TimeOfDay.TotalHours * 30;
            _minutehand.RotationAngleInDegrees = now.Minute * 6;
        }

        private static ContainerVisual GetVisual(UIElement element)
        {
            var hostVisual = ElementCompositionPreview.GetElementVisual(element);
            ContainerVisual root = hostVisual.Compositor.CreateContainerVisual();
            ElementCompositionPreview.SetElementChildVisual(element, root);
            return root;
        }
    }
}
