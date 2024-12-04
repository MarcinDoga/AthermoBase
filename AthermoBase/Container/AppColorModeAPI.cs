using System.Drawing;
using System.Windows.Media;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;

namespace AthermoBase.Container
{
    internal class AppColorModeAPI
    {
        public AppColorModeAPI() { }

        public Color darkMenuTextColor = Color.FromRgb(64, 64, 64);
        public Color lightMenuTextColor = Colors.White;

        public Color darkBorder = (Color)ColorConverter.ConvertFromString("#707070");
        public Color lightBorder = Colors.WhiteSmoke;

        public Color darkMode = Color.FromRgb(48, 48, 48);
        public Color darkModeSettingsPanel = Color.FromRgb(64, 64, 64);

        public Color lightMode = Color.FromRgb(208, 208, 208);
        public Color lightModeSettingsPanel = Color.FromRgb(224, 224, 224);

        public Color darkStackPanel = Color.FromRgb(80, 80, 80);
        public Color lightStackPanel = Color.FromRgb(120, 120, 120);
    }
}
