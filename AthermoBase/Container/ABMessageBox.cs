using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Button = System.Windows.Controls.Button;

namespace AthermoBase.Container
{
    public class ABMessageBox
    {
        public Grid NotificationOverlay { get; set; }

        public Grid MessageBoxControl { get; set; }
        public Border MessageBoxBorder { get; set; }
        public TextBlock MessageBoxTitle { get; set; }
        public TextBlock MessageBoxTextBlock { get; set; }
        public Button MessageBoxButton { get; set; }

        public ABMessageBox(Grid messageBoxControl, Border messageBoxBorder, TextBlock messageBoxTitle, TextBlock messageBoxTextBlock, Button messageBoxButton, Grid notificationOverlay)
        {
            MessageBoxControl = messageBoxControl;
            MessageBoxBorder = messageBoxBorder;
            MessageBoxTitle = messageBoxTitle;
            MessageBoxTextBlock = messageBoxTextBlock;
            MessageBoxButton = messageBoxButton;
            NotificationOverlay = notificationOverlay;
        }
        public void Show(string message, string title, string button, bool buttonVisibility, Color textColor, Color titleColor, Color boxColor, string colorMode, bool specialTextColor)
        {
            if (NotificationOverlay.Visibility == Visibility.Visible)
            {
                DoubleAnimation notifanimation = new DoubleAnimation
                {
                    From = 1,
                    To = 0,
                    Duration = TimeSpan.FromSeconds(0.3)
                };
                notifanimation.Completed += (sender, e) =>
                {
                    NotificationOverlay.Visibility = Visibility.Collapsed;
                    SetControl(message, title, button, buttonVisibility, textColor, titleColor, boxColor, colorMode, specialTextColor);
                };
                NotificationOverlay.BeginAnimation(UIElement.OpacityProperty, notifanimation);
            }
            else if (MessageBoxControl.Visibility == Visibility.Visible)
            {
                DoubleAnimation notifanimation = new DoubleAnimation
                {
                    From = 1,
                    To = 0,
                    Duration = TimeSpan.FromSeconds(0.3)
                };
                notifanimation.Completed += (sender, e) =>
                {
                    MessageBoxControl.Visibility = Visibility.Collapsed;
                    SetControl(message, title, button, buttonVisibility, textColor, titleColor, boxColor, colorMode, specialTextColor);
                };
                MessageBoxControl.BeginAnimation(UIElement.OpacityProperty, notifanimation);
            }
            else
            {
                SetControl(message, title, button, buttonVisibility, textColor, titleColor, boxColor, colorMode, specialTextColor);
            }
        }

        private void SetControl(string message, string title, string button, bool buttonVisibility, Color textColor, Color titleColor, Color boxColor, string colorMode, bool specialTextColor)
        {
            if (button == "OK")
            {
                MessageBoxButton.Content = "Ok";
            }
            else if (button == "CANCEL")
            {
                MessageBoxButton.Content = "Cancel";
            }
            else if (button == "ABORT")
            {
                MessageBoxButton.Content = "Abort";
            }
            else if (button == "CLOSE")
            {
                MessageBoxButton.Content = "Close";
            }
            else
            {
                MessageBoxButton.Content = "Ok";
            }
            MessageBoxTitle.Text = title;
            MessageBoxTextBlock.Text = message;
            MessageBoxButton.Visibility = buttonVisibility ? Visibility.Visible : Visibility.Collapsed;

            if (colorMode != null)
            {
                AppColorModeAPI appColor = new AppColorModeAPI();
                WindowsElementsAPI windowsElements = new WindowsElementsAPI();

                bool isDarkMode = colorMode == "Dark" ||
                                  (colorMode == "windowsMode" && windowsElements.styleMode(true)[1] == "Dark");

                SolidColorBrush backgroundBrush = new SolidColorBrush(isDarkMode ? appColor.darkMode : appColor.lightMode);
                SolidColorBrush textBrush = new SolidColorBrush(isDarkMode ? appColor.lightMode : appColor.darkMode);

                if (specialTextColor)
                {
                    MessageBoxBorder.Background = backgroundBrush;
                    MessageBoxTextBlock.Foreground = new SolidColorBrush(textColor);
                    MessageBoxTitle.Foreground = new SolidColorBrush(titleColor);
                }
                else
                {
                    MessageBoxBorder.Background = backgroundBrush;
                    MessageBoxTextBlock.Foreground = textBrush;
                    MessageBoxTitle.Foreground = textBrush;
                }
            }

            MessageBoxControl.Visibility = Visibility.Visible;

            DoubleAnimation animation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.3)
            };

            MessageBoxControl.BeginAnimation(UIElement.OpacityProperty, animation);
        }
    }
}
