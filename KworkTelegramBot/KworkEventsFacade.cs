using CefSharp.Wpf;
using RollingOutTools.Common;
using RollingOutTools.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace KworkTelegramBot
{
    static class KworkEventsFacade
    {
        const string SavedMessagesKey = "kwork_saved";
        static Timer timer;

        public static void RestartChecking()
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
            }

            timer = new Timer(AppSettings.Inst.CheckDelayMinutes*60*1000);
            timer.Elapsed += (s,a) =>
            {
                Check();
            };
            timer.Start();
            Check();
        }

        public static void ClearSaved()
        {
            StorageHardDrive.Set(SavedMessagesKey, null);
        }

        static void Check()
        {            
            var task = Task.Run(async () =>
                  {
                      var newRecords = await GetNew(WebViewWindow.Inst.WV);
                      OnNewRecords?.Invoke(newRecords);
                  });
            task.ContinueWith((t) =>
            {
                if (task.Exception != null)
                    WindowLogger.Log(task.Exception.ToString());
            });
        }

        static async Task<List<KworkRecord>> GetNew(ChromiumWebBrowser wv)
        {
            var savedList = await StorageHardDrive.Get<List<KworkRecord>>(SavedMessagesKey) ?? new List<KworkRecord>() ;

            foreach (var item in savedList.ToArray())
            {
                if (DateTime.Now - item.ParseDate>TimeSpan.FromDays(5))
                {
                    savedList.Remove(item);
                }
            }

            var parsedList=await KworkParser.Parse(wv);

            //magic here
            var savedListClone = savedList.ToArray();
            foreach (var itemP in parsedList.ToArray())
            {
                bool breaked=false;
                foreach (var itemS in savedListClone)
                {
                    if (itemS.Title == itemP.Title)
                    {
                        parsedList.Remove(itemP);
                        breaked = true;
                        break;
                    }
                }

                if (!breaked)
                {
                    WindowLogger.Log("NEW record!!!");
                    WindowLogger.Log(itemP.ToJsonStr());
                    savedList.Add(itemP);
                }             
            }

            await StorageHardDrive.Set(SavedMessagesKey, savedList);

            return parsedList;
        }


        public static event Action<List<KworkRecord>> OnNewRecords;
    }
}
