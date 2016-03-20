using System;

namespace Lab7.Model
{
    internal class Player : IDisposable
    {
        public MyObservableCollection<Item> PlayList = new MyObservableCollection<Item>();
        public MyObservableCollection<PlayerThread> PlayNowList = new MyObservableCollection<PlayerThread>();
        
        public void Dispose()
        {
            if (PlayList != null)
            {
                PlayList.Clear();
                PlayList = null;
            }

            if (PlayNowList == null) return;
            foreach (var thread in PlayNowList)
                thread.Dispose();
            PlayNowList.Clear();
            PlayNowList = null;
        }
    }
}
