using System;
using System.Windows.Controls;

namespace SupportTool.Logger
{
    class TextBoxLogger : LoggerInterface
    {
        private LoggerInterface inner;
        private TextBox textBox;

        public TextBoxLogger(LoggerInterface inner, TextBox textBox)
        {
            this.inner = inner;
            this.textBox = textBox;
        }

        public void Clear()
        {
            inner.Clear();
        }

        public void Log(string message)
        {
            inner.Log(message);

            textBox.AppendText(message + Environment.NewLine);
            textBox.ScrollToEnd();
        }
    }
}
