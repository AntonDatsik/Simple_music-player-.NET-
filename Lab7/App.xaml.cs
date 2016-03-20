using Lab7.Model;
using Lab7.View;
using Lab7.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Threading;

namespace Lab7
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        

        public App()
        {
            var window = new MainWindow
            {
                DataContext = new MainViewModel()
            };

            window.Show();
        }

        public static void CreateInfoWindow(Item item)
        {
            var window = new AdditionalInfoWindow
            {
                DataContext = new InfoWindowViewModel(item)
            };
            window.Show();
        }

        
    }
}
