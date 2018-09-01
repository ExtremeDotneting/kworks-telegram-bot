using RollingOutTools.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KworkTelegramBot
{
    class AppSettings
    {
        public static AppSettings _inst;
        public static AppSettings Inst
        {
            get
            {
                if (_inst == null)
                {
                    _inst = StorageHardDrive.Get<AppSettings>("app_settings").Result ?? new AppSettings();
                }
                return _inst;
            }
        }

        public bool AutoLaunch { get; set; }
        public string BotToken { get; set; }
        public string Chanel { get; set; }
        public int CheckDelayMinutes { get; set; } = 15;

        public static void Save()
        {
            StorageHardDrive.Set("app_settings",Inst);
        }
    }
}
