using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;

namespace Lab7.Model
{
    class MyLocalization
    {
        public static List<CultureInfo> Languages { get; } = new List<CultureInfo>();

        static MyLocalization()
        {
            Languages.Clear();
            Languages.Add(new CultureInfo("en-US"));
            Languages.Add(new CultureInfo("ru-RU"));
        }

        public static event EventHandler LanguageChanged;

        public static CultureInfo Language
        {
            get
            {
                return Thread.CurrentThread.CurrentUICulture;
            }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                if (value == Thread.CurrentThread.CurrentUICulture) return;

                //1. Меняем язык приложения:
                Thread.CurrentThread.CurrentUICulture = value;

                //2. Создаём ResourceDictionary для новой культуры
                ResourceDictionary dict = new ResourceDictionary();
                switch (value.Name)
                {
                    case "ru-RU":
                        dict.Source = new Uri("resources/Localization/lang.ru-RU.xaml", UriKind.Relative);
                        break;
                    default:
                        dict.Source = new Uri("resources/Localization/lang.xaml", UriKind.Relative);
                        break;
                }

                //3. Находим старую ResourceDictionary и удаляем его и добавляем новую ResourceDictionary
                ResourceDictionary oldDict = (from d in Application.Current.Resources.MergedDictionaries
                                              where d.Source != null && d.Source.OriginalString.StartsWith("resources/Localization/lang.")
                                              select d).First();
                if (oldDict != null)
                {
                    int ind = Application.Current.Resources.MergedDictionaries.IndexOf(oldDict);
                    Application.Current.Resources.MergedDictionaries.Remove(oldDict);
                    Application.Current.Resources.MergedDictionaries.Insert(ind, dict);
                }
                else
                {
                    Application.Current.Resources.MergedDictionaries.Add(dict);
                }

                //4. Вызываем евент для оповещения всех окон.
                LanguageChanged?.Invoke(Application.Current, EventArgs.Empty);
            }
        }
    }
}
