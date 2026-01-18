using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LyteLauncher.Core
{
    /// <summary>
    /// Interaction logic for RobloxPanel.xaml
    /// </summary>
    public partial class RobloxPanel : Page
    {
        public RobloxPanel()
        {
            InitializeComponent();

            var settings = UserSettings.Get();

            if (settings != null)
            {
                CurrentTheme.Content = settings.CurrentBootstrapPreset;
            }
        }

        private void BootstrapperThemesSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UserSettings.Save((settings) =>
            {
                var selected = (string)BootstrapperThemesSelect.SelectedValue;
                settings.CurrentBootstrapPreset = selected;
                CurrentTheme.Content = selected;
                return settings;
            });
        }
    }
}
