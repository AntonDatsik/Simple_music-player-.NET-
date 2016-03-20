using System;
using System.Collections;
using System.Collections.Generic;
using Lab7.Model;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Lab7.ViewModel
{
    class MainViewModel
    {

        public static Player player = new Player();
        private static readonly List<MenuItem> MenuItems = new List<MenuItem>(); 

        #region Constructor
        public MainViewModel()
        {
            ClickCommandAdd =   new Command(arg => ClickMethodAdd());
            ClickCommandDelete = new Command(arg => ClickMethodDelete());
            ClickCommandPlay = new Command(arg => ClickMethodPlay());
            ClickCommandStop = new Command(arg => ClickMethodStop());
            ClickCommandPause = new Command(arg => ClickMethodPause());
            ClickCommandResume = new Command(arg => ClickMethodResume());
            ClickCommandGetInfo = new Command(arg => ClickMethodGetInfo());

            MyLocalization.LanguageChanged += LanguageChanged;
            CultureInfo currLang = MyLocalization.Language;

            //Заполняем меню смены языка:
            foreach (var lang in MyLocalization.Languages)
            {
                MenuItem menuLang = new MenuItem();
                menuLang.Header = lang.DisplayName;
                menuLang.Tag = lang;
                menuLang.IsChecked = lang.Equals(currLang);
                menuLang.Click += ChangeLanguageClick;
                MenuItems.Add(menuLang);
            }
        }
        #endregion

        #region Properties

        public List<MenuItem> MenuLanguageItems => MenuItems;

        public int SelectedItemInPlayList
        {
            private get;
            set;
        }

        public int SelectedItemInPlayingNowList
        {
            private get;
            set;
        }

        public Item ItemInPlayList
        {
            get
            {
                if (SelectedItemInPlayList >= 0 && PlayList.Count > 0)
                    return PlayList.ElementAt(SelectedItemInPlayList);
                else
                    return null;
            }
            
        }

        public MyObservableCollection<Item> PlayList => player.PlayList;

        public MyObservableCollection<PlayerThread> PlayingNowList => player.PlayNowList;

        #endregion

        #region Commands
        public ICommand ClickCommandAdd { get; set; }
        public ICommand ClickCommandDelete { get; set; }
        public ICommand ClickCommandPlay { get; set; }
        public ICommand ClickCommandStop { get; set; }
        public ICommand ClickCommandPause { get; set; }
        public ICommand ClickCommandResume { get; set; }
        public ICommand ClickCommandGetInfo { get; set; }
        #endregion

        #region Methods

        public void ClickMethodAdd()
        {
            player.PlayList.Add(ItemManager.AddItem());
        }

        public void ClickMethodDelete()
        {
            if (SelectedItemInPlayList >= 0 && PlayList.Count > 0)
                player.PlayList.RemoveAt(SelectedItemInPlayList);
        }
        

        public void ClickMethodPlay()
        {
            if (SelectedItemInPlayList < 0 || PlayList.Count <= 0) return;
            PlayerThread playerThread = new PlayerThread(player.PlayList.ElementAt(SelectedItemInPlayList), player);
            playerThread.Start();
            player.PlayNowList.Add(playerThread);
        }

        public void ClickMethodStop()
        {
            if (SelectedItemInPlayingNowList >= 0 && PlayingNowList.Count > 0)
            {
                player.PlayNowList.ElementAt(SelectedItemInPlayingNowList).Interrupt();
                player.PlayNowList.RemoveAt(SelectedItemInPlayingNowList);
            }
        }

        public void ClickMethodPause()
        {
            if (SelectedItemInPlayingNowList >= 0 && PlayingNowList.Count > 0)
            {
                player.PlayNowList.ElementAt(SelectedItemInPlayingNowList).Pause();
            }
        }

        public void ClickMethodResume()
        {
            if (SelectedItemInPlayingNowList >= 0 && PlayingNowList.Count > 0)
            {
                player.PlayNowList.ElementAt(SelectedItemInPlayingNowList).Play();
            }
        }

        public void ClickMethodGetInfo()
        {
            if (SelectedItemInPlayList >= 0 && PlayList.Count > 0)
            {
                App.CreateInfoWindow(PlayList.ElementAt(SelectedItemInPlayList));
            }
        }

        private void LanguageChanged(object sender, EventArgs e)
        {
            CultureInfo currLang = MyLocalization.Language;

            //Отмечаем нужный пункт смены языка как выбранный язык
            foreach (MenuItem i in MenuItems)
            {
                CultureInfo ci = i.Tag as CultureInfo;
                i.IsChecked = ci != null && ci.Equals(currLang);
            }
        }

        private void ChangeLanguageClick(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            if (mi != null)
            {
                CultureInfo lang = mi.Tag as CultureInfo;
                if (lang != null)
                {
                    MyLocalization.Language = lang;
                }
            }
        }
        #endregion
    }
}
