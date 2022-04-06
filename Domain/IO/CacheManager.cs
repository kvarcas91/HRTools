using Domain.Types;
using System;
using System.Timers;

namespace Domain.IO
{
    public static class CacheManager
    {
        public static string MoveFrom { get; set; }
        public static string MoveTo { get; set; }
        public static CacheState CacheState;
        private static Timer _timer;
        private static Action<bool> _loader;

        public delegate bool PostLoad();

        public static void Initialize(int intervalInSeconds)
        {
            _timer = new Timer
            {
                Interval = intervalInSeconds * 1000
            };
            _timer.Elapsed += Update;
            _timer.AutoReset = true;
            _timer.Enabled = true;

            Update(null, null);
            
        }

        public static void Initialize(int intervalInSeconds, Action<bool> loader)
        {
            _loader = loader;
            Initialize(intervalInSeconds);
        }

        public static void ResetTimer()
        {
            if (_timer is null) return;

            _timer.Stop();
            Update(null, null);
            _timer.Start();
        }

        private static async void Update(object source, ElapsedEventArgs e)
        {
            CacheState = CacheState.InProgress;
            var fromFileInfo = FileHelper.GetLastWriteTime(MoveFrom);
            var toFileInfo = FileHelper.GetLastWriteTime(MoveTo);
            if (fromFileInfo.Equals(toFileInfo))
            {
                CacheState = CacheState.Stable;
                return;
            }

            if (_loader != null) _loader.Invoke(true);
            await FileHelper.CopyAndReplaceFileAsync(MoveFrom, MoveTo);
            
            if (_loader != null) _loader.Invoke(false);
            CacheState = CacheState.Stable;
        }
    }
}
