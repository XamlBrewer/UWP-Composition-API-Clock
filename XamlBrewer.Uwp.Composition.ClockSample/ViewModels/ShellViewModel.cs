using Windows.UI.Xaml.Controls;
using XamlBrewer.Uwp.Composition.ClockSample;

namespace Mvvm
{
    class ShellViewModel : ViewModelBase
    {
        public ShellViewModel()
        {
            // Build the menu
            Menu.Add(new MenuItem() { Glyph = Symbol.Clock, Text = "Modern", NavigationDestination = typeof(MainPage) });
            Menu.Add(new MenuItem() { Glyph = Symbol.OutlineStar, Text = "Classic", NavigationDestination = typeof(OtherPage) });
            Menu.Add(new MenuItem() { Glyph = Symbol.Emoji, Text = "Silly", NavigationDestination = typeof(SillyPage) });
        }
    }
}
