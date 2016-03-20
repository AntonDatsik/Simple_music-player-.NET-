using Lab7.Model;
using System.Text;
using System.Windows.Media.Imaging;

namespace Lab7.ViewModel
{
    class InfoWindowViewModel
    {
        private Item item;
        public InfoWindowViewModel(Item item)
        {
            this.item = item;
        }

        public string Name
        {
            get
            {
                return item.Name;
            }
            set
            {
                item.Name = value;
            }
        }
        public string FileName
        {
            get
            {
                return item.FileName;
            }
        }
        public string Length
        {
            get
            {
                return item.Length;
            }
        }
        public string Perfomers
        {
            get
            {
                string[] performers = item.Perfomers;
                StringBuilder tempBuilder = new StringBuilder();
                for (int i = 0; i < performers.Length - 1; ++i)
                {
                    tempBuilder.Append(performers[i] + ", ");
                }
                tempBuilder.Append(performers[performers.Length - 1]);

                return tempBuilder.ToString();
            }
            set
            {
                item.Perfomers = value.Split(',');
            }
        }
        public string Album
        {
            get
            {
                return item.Album;
            }
            set
            {
                item.Album = value;
            }
        }
        public string Genres
        {
            get
            {
                StringBuilder tempBuilder = new StringBuilder();
                string[] genres = item.Genres;

                for (int i = 0; i < genres.Length - 1; ++i)
                {
                    tempBuilder.Append(genres[i] + ", ");
                }
                tempBuilder.Append(genres[genres.Length - 1]);

                return tempBuilder.ToString();
            }
            set
            {
                item.Genres = value.Split(',');
            }
        }

        public int Grade
        {
            get
            {
                return item.Grade;
            }
            set
            {
                if (value >= 0 && value <= 10)
                {
                    item.Grade = value;
                }
            }

        }

        public BitmapImage Image
        {
            get
            {
                return item.Image;
            }
        }
    }
}
