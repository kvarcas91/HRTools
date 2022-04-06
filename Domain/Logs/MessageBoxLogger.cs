using Domain.Interfaces;
using System;

namespace Domain.Logs
{
    public class MessageBoxLogger : ILogger
    {

        private Action<string> _showMessage;

        public MessageBoxLogger(Action<string> messageBox)
        {
            _showMessage = messageBox;
        }

        public void Log(string message)
        {
            _showMessage(message);
        }
    }
}
