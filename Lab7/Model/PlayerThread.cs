using System;
using System.Threading;
using System.Windows.Media;

namespace Lab7.Model
{
    internal class PlayerThread : IDisposable
    {        
        private bool _isPlay;
        private bool _isStop;
        private bool _isInterrupt;
        private readonly AutoResetEvent _ar = new AutoResetEvent(false);
        private readonly Player _player;
        private MediaPlayer _mediaPlayer;
        
        public Item PlayingItem { get; }

        public PlayerThread(Item item, Player player)
        {
            PlayingItem = item;
            _player = player;
        }
        
        public void Pause()
        {
            _isStop = true;
            _isPlay = false;
            _ar.Set();
        }

        public void Play()
        {
            _isStop = false;
            _isPlay = true;
            _ar.Set();
        }

        public void Start()
        {
            Play();
            ThreadPool.QueueUserWorkItem(Run);
        }


        public void Interrupt()
        {
            _isInterrupt = true;
            _ar.Set();
        }

        void m_MediaEnded(object sender, EventArgs e) => Interrupt();

        private void Run(object state)
        {
            _mediaPlayer = new MediaPlayer();
            _mediaPlayer.Open(new Uri(PlayingItem.FileName));
            _mediaPlayer.MediaEnded += m_MediaEnded;
            while (true)
            {
                if (_isInterrupt) break;

                _ar.WaitOne();
                if (_isPlay)
                {
                    _mediaPlayer.Play();
                    _isPlay = false;
                }

                if (!_isStop) continue;
                _mediaPlayer.Pause();
                _isStop = false;
            }
            Dispose();
        }

        public override string ToString()
        {
            return PlayingItem.Name;
        }

        
        
        public void Dispose()
        {
            _ar.Dispose();
            if (_mediaPlayer != null)
            {
                _mediaPlayer.Stop();
                _mediaPlayer.Close();
                _mediaPlayer = null;
            }
            _player.PlayNowList.Remove(this);
        }
    }
}



//Thread, ThreadPool, Blocking, Semaphore, Task, Asynqawait, TPL
