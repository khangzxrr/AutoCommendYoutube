using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WebDriver
{
    public class HeaderModifier
    {
        public static String YOUTUBE_TOKEN { get; set; }
        public static String STUDIO_TOKEN { get; set; }

        public static String Cookies { get; set; }

        public static String GenerateToken(string SAPISID, bool youtubeToken = true)
        {
            long unixSec = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 100;

            //time sapid origin
            SAPISID = $"{unixSec} {SAPISID} {(youtubeToken ? "https://www.youtube.com" : "https://studio.youtube.com")}";

            var data = Encoding.ASCII.GetBytes(SAPISID);
            var hashData = new SHA1Managed().ComputeHash(data);

            var stringBuilder = new StringBuilder();
            foreach (var hashByte in hashData)
            {
                stringBuilder.AppendFormat("{0:x2}", hashByte);
            }

            stringBuilder.Insert(0, "SAPISIDHASH " + unixSec.ToString() + "_");


            if (youtubeToken)
            {
                YOUTUBE_TOKEN = stringBuilder.ToString();
            }
            else
            {
                STUDIO_TOKEN = stringBuilder.ToString();
            }

            return stringBuilder.ToString();
        }

        public static void GenerateOAuth2Token(IWebDriver webDriver, string cookieStr, bool youtubeToken = true)
        {
            webDriver.Url = "chrome-extension://idgpnmonknjnojddfkpgkljpfnnfcklj/_generated_background_page.html";

            IJavaScriptExecutor javaScriptExecutor = (IJavaScriptExecutor)webDriver;

            Cookies = cookieStr;

            int sapidKeyPos = cookieStr.IndexOf("SAPISID=");
            int endedKeyPos = cookieStr.IndexOf(";", sapidKeyPos);
            string SAPISID = cookieStr.Substring(sapidKeyPos + 8, endedKeyPos - sapidKeyPos - 8);

            javaScriptExecutor.ExecuteScript($@"localStorage.setItem('profiles', JSON.stringify([{{  title: 'oauth2', hideComment: true, appendMode: '', 
             headers: [
                {{ enabled: true, name: 'Cookie', value: '{cookieStr}', comment: ''}},
               {{ enabled: true, name: 'authorization', value: '{GenerateToken(SAPISID, youtubeToken)}', comment: ''}}
             ],                          
             respHeaders:[],
             filters:[]
          }}]));");

            webDriver.Navigate().Refresh();
        }
    }
}
