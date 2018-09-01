using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KworkTelegramBot
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class WebViewWindow : Window
    {
        static WebViewWindow _inst;
        public static WebViewWindow Inst
        {
            get
            {
                if (_inst == null)
                    _inst = new WebViewWindow();
                return _inst;
            }
        }

        public ChromiumWebBrowser WV { get; private set; }

        bool keyDown ;

        public WebViewWindow()
        {
            InitializeComponent();

            try
            {
                Uri iconUri = new Uri(System.IO.Path.Combine(Environment.CurrentDirectory, "icon.jpg"), UriKind.Absolute);
                Icon = BitmapFrame.Create(iconUri);
            }
            catch { }

            var webView = new ChromiumWebBrowser();
            webView.BrowserSettings.ImageLoading = CefSharp.CefState.Disabled;
            webViewGrid.Children.Clear();
            webViewGrid.Children.Add(webView);
            webView.LoadingStateChanged+=(sender, args)=>
            {
                Dispatcher.Invoke(() =>
                {
                    urlTextBox.Text = webView.Address;
                });
            };
            webView.Address = "google.com";

            WV = webView;
        }

        private void Forward_Click(object sender, RoutedEventArgs e)
        {
            if (keyDown)
            {
                try
                {
                    WV.Address = urlTextBox.Text;
                }
                catch {
                }
            }
            if (WV.CanGoForward)
            {
                WV.ForwardCommand.Execute(null);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (WV.CanGoBack)
            {
                WV.BackCommand.Execute(null);
            }
        }

        private void urlTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            keyDown = true;

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Visibility = Visibility.Hidden;
        }


    }
}
