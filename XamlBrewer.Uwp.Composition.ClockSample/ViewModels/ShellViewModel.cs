using XamlBrewer.Uwp.Composition.ClockSample;

namespace Mvvm
{
    class ShellViewModel : ViewModelBase
    {
        public ShellViewModel()
        {
            // Build the menu
            Menu.Add(new MenuItem() { Glyph = "\uE006", Text = "Modern", NavigationDestination = typeof(MainPage) });
            Menu.Add(new MenuItem() { Glyph = "\uE208", Text = "Classic", NavigationDestination = typeof(OtherPage) });
            Menu.Add(new MenuItem() { Glyph = "\uE11D", Text = "Silly", NavigationDestination = typeof(SillyPage) });
        }
    }
}
