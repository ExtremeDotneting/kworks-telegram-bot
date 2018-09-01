using CefSharp;
using CefSharp.Wpf;
using RollingOutTools.Common;
using RollingOutTools.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KworkTelegramBot
{
    static class KworkParser
    {
        public static async Task<List<KworkRecord>> Parse(ChromiumWebBrowser wv)
        {
            var res = new List<KworkRecord>();
            var strList = (await GetFromPage(wv)).Split(new string[] { "^^^" }, StringSplitOptions.None);
            for(int i = 0; i < strList.Length - 1; i++)
            {
                try
                {
                    var recordArr = strList[i].Split(new string[] { "###" }, StringSplitOptions.None);
                    var newRecord = new KworkRecord()
                    {
                        Title = recordArr[0],
                        Text = recordArr[1],
                        Info = recordArr[2], 
                        ParseDate=DateTime.Now
                    };
                    if (newRecord.Text.EndsWith("Скрыть"))
                    {
                        newRecord.Text=newRecord.Text.Remove(newRecord.Text.Length - 6);
                    }
                    newRecord.Text = newRecord.Text.Trim();
                    res.Add(newRecord);

                    WindowLogger.Log($"Parsed record *{newRecord.Title}*.");
                }
                catch(Exception ex)
                {
                    WindowLogger.Log("Error while parsing record.");
                    WindowLogger.Log(ex.ToString());
                }
            }
            return res;
        }

        /// <summary>
        /// Возвращает строку со значениями.
        /// </summary>
        /// <param name="wv"></param>
        /// <returns></returns>
        static Task<string> GetFromPage(ChromiumWebBrowser wv)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                wv.Address = "https://google.com";
            });
            var promise= new TaskCompletionSource<string>();      
            WindowLogger.Log("Start page inspection.");
            AttachOneRiseEventHandler(
                wv,
                () =>
                {
                    var script = File.ReadAllText("kwork_parse.js");
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Task<JavascriptResponse> jsResTask = wv.EvaluateScriptAsync(script);
                        jsResTask.ContinueWith((jsRes) =>
                        {
                            var res = jsRes.Result.Result.ToString();
                            WindowLogger.Log("Get page data as string.");
                            promise.SetResult(res);
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                wv.Address = "https://google.com";
                            });
                        });
                    });
                    
                }
                );
            Application.Current.Dispatcher.Invoke(() =>
            {
                wv.Address = "https://kwork.ru/projects";
            });           
            return promise.Task;
        }

        static void AttachOneRiseEventHandler(ChromiumWebBrowser wv,Action act)
        {
            EventHandler<FrameLoadEndEventArgs> resEvent = null;
            resEvent=new EventHandler<FrameLoadEndEventArgs>((sender, args) =>
            {
                try
                {
                    act();
                }
                finally
                {
                    wv.FrameLoadEnd -= resEvent;
                }
            });

            Application.Current.Dispatcher.Invoke(() =>
            {
                wv.FrameLoadEnd += resEvent;
            });
            
        }

        
    }
}
