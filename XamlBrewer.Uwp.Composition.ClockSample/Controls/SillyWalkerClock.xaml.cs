using Microsoft.UI.Composition.Toolkit;
using System;
using System.Numerics;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace XamlBrewer.Uwp.Controls
{
    public sealed partial class SillyWalkerClock : UserControl
    {
        private Compositor _compositor;
        private ContainerVisual _root;

        private SpriteVisual _background;
        private SpriteVisual _hourhand;
        private SpriteVisual _minutehand;
        private SpriteVisual _foreground;

        private DispatcherTimer _timer = new DispatcherTimer();

        public SillyWalkerClock()
        {
            this.InitializeComponent();

            this.Loaded += Clock_Loaded;

            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
        }

        public Uri FaceImage { get; set; } = new Uri("ms-appx:///Assets/Silly_Face.png");
        public Uri HourHandImage { get; set; } = new Uri("ms-appx:///Assets/Silly_Hour.png");
        public Uri MinuteHandImage { get; set; } = new Uri("ms-appx:///Assets/Silly_Minute.png");
        public Uri FrontImage { get; set; } = new Uri("ms-appx:///Assets/Silly_Front.png");

        private void Clock_Loaded(object sender, RoutedEventArgs e)
        {
            _root = GetVisual(Container);
            _compositor = _root.Compositor;

            // Background
            _background = _compositor.CreateSpriteVisual();
            _background.Size = new Vector2(800.0f, 800.0f);
            var _imageFactory = CompositionImageFactory.CreateCompositionImageFactory(_compositor);
            CompositionImageOptions options = new CompositionImageOptions()
            {
                DecodeWidth = 740,
                DecodeHeight = 740,
            };
            var _image = _imageFactory.CreateImageFromUri(FaceImage, options);
            _background.Brush = _compositor.CreateSurfaceBrush(_image.Surface);
            _root.Children.InsertAtTop(_background);

            // Hour Hand
            options = new CompositionImageOptions()
            {
                DecodeWidth = 740,
                DecodeHeight = 740,
            };

            _hourhand = _compositor.CreateSpriteVisual();
            _hourhand.Size = new Vector2(800.0f, 800.0f);
            _image = _imageFactory.CreateImageFromUri(HourHandImage, options);
            _hourhand.Brush = _compositor.CreateSurfaceBrush(_image.Surface);
            _hourhand.Offset = new Vector3(0.0f, 0.0f, 0);
            _hourhand.CenterPoint = new Vector3(200.0f, 400.0f, 0);
            _root.Children.InsertAtTop(_hourhand);

            // Minute Hand
            options = new CompositionImageOptions()
            {
                DecodeWidth = 740,
                DecodeHeight = 740,
            };
            _image = _imageFactory.CreateImageFromUri(MinuteHandImage, options);
            _minutehand = _compositor.CreateSpriteVisual();
            _minutehand.Size = new Vector2(800.0f, 800.0f);
            _minutehand.Brush = _compositor.CreateSurfaceBrush(_image.Surface);
            _minutehand.Offset = new Vector3(0.0f, 0.0f, 0);
            _minutehand.CenterPoint = new Vector3(200.0f, 400.0f, 0);
            _root.Children.InsertAtTop(_minutehand);

            // Foreground
            _foreground = _compositor.CreateSpriteVisual();
            _foreground.Size = new Vector2(800.0f, 800.0f);
            options = new CompositionImageOptions()
            {
                DecodeWidth = 740,
                DecodeHeight = 740,
            };
            _image = _imageFactory.CreateImageFromUri(FrontImage, options);
            _foreground.Brush = _compositor.CreateSurfaceBrush(_image.Surface);
            _root.Children.InsertAtTop(_foreground);

            SetHoursAndMinutes();

            _timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            SetHoursAndMinutes();
        }

        private void SetHoursAndMinutes()
        {
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
