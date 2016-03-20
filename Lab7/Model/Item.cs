using System.IO;
using System.Windows.Media.Imaging;

namespace Lab7.Model
{
    public class Item
    {
        private static int lastId = 0;
        private string name;
        private string length;
        private string[] perfomers;
        private string album;
        private string[] genres;

        public string Name
        {
            get
            {
                return name ?? "Unknown";
            }
            set
            {
                name = value;
            }
        }
        public string FileName { get; }

        public string Length
        {
            get
            {
                string res = length ?? "Unknown";
                return res;
            }
            set
            {
                length = value;
            }
        }
        public string[] Perfomers
        {
            get
            {
                string[] res = perfomers ?? new string[1] { "Unknown" };
                return res;
            }
            set
            {
                perfomers = value;
            }
        }
        public string Album
        {
            get
            {
                string res = album ?? "Unknown";
                return res;
            }
            set
            {
                album = value;
            }
        }
        public string[] Genres
        {
            get
            {
                string[] res = genres ?? new string[1] { "Unknown" };
                return res;
            }
            set
            {
                genres = value;
            }
        }

        public BitmapImage Image { get; }

        public int Grade
        {
            get; set;
        }
         
        public int Id { get; }

        public Item(string fileName)
        {
            this.FileName = fileName;

            //get tags from audioFile using TagLib
            var audioFile = TagLib.File.Create(fileName);
            name = audioFile.Tag.Title;
            length = audioFile.Properties.Duration.ToString("mm\\:ss");
            perfomers = audioFile.Tag.Performers;
            album = audioFile.Tag.Album;
            genres = audioFile.Tag.Genres;

            if (audioFile.Tag.Pictures.Length >= 1)
            {
                var bin = audioFile.Tag.Pictures[0].Data.Data;
                Image = new BitmapImage();
                Image.BeginInit();
                Image.CacheOption = BitmapCacheOption.OnLoad;
                Image.StreamSource = new MemoryStream(bin);
                Image.EndInit();
            }

            Id = ++lastId;
        }

        public override string ToString()
        {
            return $"{Id,-10} {name}";
        }
    }
}
