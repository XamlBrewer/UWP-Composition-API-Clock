using Microsoft.UI.Composition.Toolkit;
using System;
using System.Numerics;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace XamlBrewer.Uwp.Controls
{
    public sealed partial class ImageClock : UserControl
    {
        private Compositor _compositor;
        private ContainerVisual _root;

        private SpriteVisual _background;
        private SpriteVisual _hourhand;
        private SpriteVisual _minutehand;
        private SpriteVisual _secondhand;

        private CompositionScopedBatch _batch;

        private DispatcherTimer _timer = new DispatcherTimer();

        public ImageClock()
        {
            this.InitializeComponent();

            this.Loaded += Clock_Loaded;

            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
        }

        public Uri FaceImage { get; set; } = new Uri("ms-appx:///Assets/roman_face.jpg");
        public Uri HourHandImage { get; set; } = new Uri("ms-appx:///Assets/hour_hand.png");
        public Uri MinuteHandImage { get; set; } = new Uri("ms-appx:///Assets/minute_hand.png");

        private void Clock_Loaded(object sender, RoutedEventArgs e)
        {
            _root = Container.GetVisual();
            _compositor = _root.Compositor;

            // Background
            _background = _compositor.CreateSpriteVisual();
            _background.Size = new Vector2(200.0f, 200.0f);
            var _imageFactory = CompositionImageFactory.CreateCompositionImageFactory(_compositor);
            CompositionImageOptions options = new CompositionImageOptions()
            {
                DecodeWidth = 400,
                DecodeHeight = 400,
            };
            var _image = _imageFactory.CreateImageFromUri(FaceImage, options);
            _background.Brush = _compositor.CreateSurfaceBrush(_image.Surface);
            _root.Children.InsertAtTop(_background);

            // Hour Hand
            options = new CompositionImageOptions()
            {
                DecodeWidth = 72,
                DecodeHeight = 240,
            };

            _hourhand = _compositor.CreateSpriteVisual();
            _hourhand.Size = new Vector2(24.0f, 80.0f);
            _image = _imageFactory.CreateImageFromUri(HourHandImage, options);
            _hourhand.Brush = _compositor.CreateSurfaceBrush(_image.Surface);
            _hourhand.CenterPoint = new Vector3(12.0f, 70.0f, 0);
            _hourhand.Offset = new Vector3(88.0f, 30.0f, 0);
            _root.Children.InsertAtTop(_hourhand);

            // Minute Hand
            options = new CompositionImageOptions()
            {
                DecodeWidth = 48,
                DecodeHeight = 270,
            };
            _image = _imageFactory.CreateImageFromUri(MinuteHandImage, options);
            _minutehand = _compositor.CreateSpriteVisual();
            _minutehand.Size = new Vector2(16.0f, 90.0f);
            _minutehand.Brush = _compositor.CreateSurfaceBrush(_image.Surface);
            _minutehand.CenterPoint = new Vector3(8.0f, 85.0f, 0);
            _minutehand.Offset = new Vector3(92.0f, 15.0f, 0);
            _root.Children.InsertAtTop(_minutehand);

            SetHoursAndMinutes();

            // Second Hand
            _secondhand = _compositor.CreateSpriteVisual();
            _secondhand.Size = new Vector2(1.0f, 90.0f);
            _secondhand.Brush = _compositor.CreateColorBrush(Colors.Red);
            _secondhand.CenterPoint = new Vector3(0.5f, 90.0f, 0);
            _secondhand.Offset = new Vector3(99.5f, 10.0f, 0);
            _root.Children.InsertAtTop(_secondhand);
            _secondhand.RotationAngleInDegrees = (float)(int)DateTime.Now.TimeOfDay.TotalSeconds * 6;

            _timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
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

            SetHoursAndMinutes();
        }

        private void SetHoursAndMinutes()
        {
            var now = DateTime.Now;
            _hourhand.RotationAngleInDegrees = (float)now.TimeOfDay.TotalHours * 30;
            _minutehand.RotationAngleInDegrees = now.Minute * 6;
        }
    }
}
