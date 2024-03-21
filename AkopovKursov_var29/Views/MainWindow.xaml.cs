using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using AkopovKursov_var29.ViewModels;


namespace AkopovKursov_var29.Views
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Settings.SaveSettings();
        }

    }
}
