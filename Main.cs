using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using HuajiTech.CoolQ;
using HuajiTech.CoolQ.Events;
using HuajiTech.CoolQ.Messaging;
using Newtonsoft.Json.Linq;

namespace Cat_Robot
{
    internal class Main : Plugin
    {
        public Main(IMessageEventSource messageEventSource)
        {
            messageEventSource.AddMessageReceivedEventHandler(OnMessageReceived);
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            string MesCon = e.Message.Content;

            if (MesCon.ToLower().StartsWith("cat "))
            {
                string[] Words = MesCon.Split(' ');
                string AllWord = MesCon.Substring(MesCon.IndexOf(Words[1]) + Words[1].Length).Trim();


                switch (Words[1].ToLower())
                {
                    case "list":
                        e.Reply(Resources.FunctionList);
                        break;
                    case "ver":
                    case "version":
                        e.Reply(Resources.VersionString);
                        break;
                    case "m":
                    case "music":
                        if (Words.Length >= 3)
                        {
                            string html = GetWebpageSourceCode($"http://music.163.com/api/search/pc?s={WebUtility.UrlEncode(AllWord)}&type=1", new WebClient());

                            if (html.IndexOf("songs") == -1)
                                e.Source.Send($"错误：\n没有叫 {AllWord} 的歌曲");
                            else
                            {
                                JToken FirstSong = JObject.Parse(html)["result"]["songs"][0];
                                Music FirstMusic = new Music { Platform = MusicPlatform.Netease, Id = Convert.ToInt64(FirstSong["id"]) };
                                e.Reply("http://music.163.com/song/media/outer/url?id=" + FirstSong["id"] + ".mp3");
                                e.Reply(FirstMusic);
                            }
                        }
                        break;
                    case "tl":
                    case "translate":
                        if (Words.Length >= 3)
                        {
                            string Chinese = Translate("zh", AllWord);
                            string Final = Chinese == AllWord ? Translate("en", AllWord) : Chinese;
                            e.Reply(Final);
                        }
                        break;
                    case "calc":
                        if (Words.Length >= 3)
                        {
                            string Prefix = "成功：\n";
                            string Result;
                            try {
                                Result = new DataTable().Compute(AllWord, "").ToString();
                            } catch (Exception Exp) {
                                Prefix = "失败：\n";
                                Result = Exp.Message;
                            }
                            if (string.IsNullOrEmpty(Result))
                                Prefix = "失败";

                            e.Reply(Prefix + Result);
                        }
                        break;
                }
            }
        }

        private static string Translate(string ToLanguage, string Text)
        {
            var Results = JArray.Parse(GetWebpageSourceCode($"https://translate.google.cn/translate_a/single?client=gtx&sl=auto&tl={ToLanguage}&dt=t&q={Text}", new WebClient()))[0];
            var Result = "";
            foreach (JToken subToken in Results)
                Result += subToken.First;

            return Result;
        }

        private static string GetWebpageSourceCode(string url, WebClient webC)
        {
            string strHTML;
            Stream myStream = webC.OpenRead(url);
            StreamReader sr = new StreamReader(myStream, Encoding.GetEncoding("utf-8"));
            strHTML = sr.ReadToEnd();
            myStream.Close();

            return strHTML;
        }
    }
}