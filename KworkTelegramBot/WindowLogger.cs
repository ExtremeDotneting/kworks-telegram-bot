using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KworkTelegramBot
{
    static class WindowLogger
    {
        static int linesCount = 0;
        public static void Log(string msg)
        {

            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    var tb = (Application.Current.MainWindow as MainWindow).LogsTextBox;

                    linesCount++;
                    if (linesCount > 300)
                    {
                        tb.Text = "";
                        linesCount = 0;
                    }
                    tb.Text += msg + "\n";
                }
                catch
                {

                }
            });

        }
    }
}
