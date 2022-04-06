using Domain.Interfaces;
using Domain.Types;
using System;

namespace Domain.Logs
{
    public class ToastLogger : ILogger
    {

        private Action<(string, NotificationType)> _action;

        public ToastLogger(Action<(string, NotificationType)> action)
        {
            _action = action;
        }

        public void Log(string message)
        {
            _action.Invoke((message, NotificationType.Error));
        }
    }
}
