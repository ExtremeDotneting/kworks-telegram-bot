using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KworkTelegramBot
{
    /// <summary>
    /// 
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Inst { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            Inst = this;
            try
            {
                Uri iconUri = new Uri(System.IO.Path.Combine(Environment.CurrentDirectory, "icon.jpg"), UriKind.Absolute);
                Icon = BitmapFrame.Create(iconUri);
            }
            catch { }

            WebViewWindow.Inst.Visibility = Visibility.Visible;
            WebViewWindow.Inst.Visibility = Visibility.Hidden;
            LoadSettings();
            TelegramBotHandler.Restart();
            KworkEventsFacade.OnNewRecords += (records) =>
            {
                WindowLogger.Log("Send to telegram");
                TelegramBotHandler.Send(records);                
            };
//#if RELEASE
            KworkEventsFacade.RestartChecking();
//#endif

            TrayIconHelper.InitTrayIcon(TrayIconHelper.MenuFromDict(new Dictionary<string, Action>()
            {
                {
                    "Show",
                    ()=>
                    {
                        Dispatcher.Invoke(()=>{
                            MainWindow.Inst.Visibility=Visibility.Visible;
                        });
                    }
                },
                {
                    "Exit",
                    ()=>
                    {
                        Application.Current.Shutdown();
                    }
                }
            }));

            TrayIconHelper.ShowNotification("","Kwork telegram bot launched successfully.");

            if (App.IsLaunchedWithSystem)
            {
                Task.Run(async () =>
                {
                    await Task.Delay(500);
                    Dispatcher.Invoke(() =>
                    {
                        this.Visibility = Visibility.Hidden;
                        WebViewWindow.Inst.Visibility = Visibility.Hidden;
                    });
                });
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            WebViewWindow.Inst.Visibility = Visibility.Visible;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            KworkEventsFacade.RestartChecking();
        }

        private void Button_ClearSaved(object sender, RoutedEventArgs e)
        {
            KworkEventsFacade.ClearSaved();
            WindowLogger.Log("Cleanr.");
        }        

        void LoadSettings()
        {
            var st=AppSettings.Inst;
            Dispatcher.Invoke(() =>
            {
                cbAutolaunch.IsChecked = st.AutoLaunch;
                tbTelega.Text = st.BotToken;
                delaySlider.Value = st.CheckDelayMinutes;
                tbTelegaChanel.Text = st.Chanel;
            });

        }

        void SaveSettings()
        {
            AppSettings.Inst.CheckDelayMinutes = (int)delaySlider.Value;
            AppSettings.Inst.BotToken = tbTelega.Text;

            try
            {
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                    "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run",
                    true
                    );
                Assembly curAssembly = Assembly.GetExecutingAssembly();
                if (cbAutolaunch.IsChecked == true)
                {
                    key.SetValue(curAssembly.GetName().Name, curAssembly.Location);
                }
                else
                {
                    key.DeleteValue(curAssembly.GetName().Name + " /winstart");
                }

                AppSettings.Inst.AutoLaunch = cbAutolaunch.IsChecked == true;
                
            }
            catch
            {
            }

            AppSettings.Inst.Chanel = tbTelegaChanel.Text;
            AppSettings.Save();
        }

        private void Button_Submit(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            TelegramBotHandler.Restart();
            KworkEventsFacade.RestartChecking();
            TrayIconHelper.ShowNotification("","Submited.");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
            WebViewWindow.Inst.Visibility = Visibility.Hidden;
            TrayIconHelper.ShowNotification("", "Still running here.");
        }


    }
}
