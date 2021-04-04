using LoginHeaders;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace LoginHeaders
{
    public class HeaderModifier
    {
        public String loginData { get; set; }
        public LoginHeadersData loginHeadersData { get; set; }

        public HeaderModifier(string loginHeaderData)
        {
            this.loginData = loginHeaderData;
            this.loginHeadersData = LoginHeadersData.Deserialize(loginData);
        }

        public String GenerateStudioToken()
        {
            long unixSec = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 100;

            int sapidKeyPos = loginData.IndexOf("SAPISID=");
            int endedKeyPos = loginData.IndexOf(";", sapidKeyPos);
            string SAPISID = loginData.Substring(sapidKeyPos + 8, endedKeyPos - sapidKeyPos - 8);

            //time sapid origin
            SAPISID = $"{unixSec} {SAPISID} https://studio.youtube.com";

            var data = Encoding.ASCII.GetBytes(SAPISID);
            var hashData = new SHA1Managed().ComputeHash(data);

            var stringBuilder = new StringBuilder();
            foreach (var hashByte in hashData)
            {
                stringBuilder.AppendFormat("{0:x2}", hashByte);
            }

            stringBuilder.Insert(0, "SAPISIDHASH " + unixSec.ToString() + "_");

            return stringBuilder.ToString();
        }

        public void GenerateOAuth2Token(IWebDriver webDriver, bool isStudioToken = false)
        {
            var extensionUrl = "chrome-extension://idgpnmonknjnojddfkpgkljpfnnfcklj/_generated_background_page.html";
            webDriver.Url = extensionUrl;
            IJavaScriptExecutor javaScriptExecutor = (IJavaScriptExecutor)webDriver;
            

            var headersStr = "";
            foreach(var key in loginHeadersData.headers.Keys)
            {
                var currentHeadersValue = loginHeadersData.headers[key];

                if (isStudioToken && key.Equals("Authorization"))
                {
                    var studioHashToken = GenerateStudioToken();
                    currentHeadersValue = studioHashToken;
                }
                headersStr += $@"{{ enabled: true, name: '{key}', value: '{currentHeadersValue}', comment: ''}},";
            }

            headersStr = headersStr.Remove(headersStr.Length - 1, 1); //remove "," at the end

            javaScriptExecutor.ExecuteScript($@"localStorage.setItem('profiles', JSON.stringify([{{  title: 'oauth2', hideComment: true, appendMode: '', 
             headers: [{headersStr}],                          
             respHeaders:[],
             filters:[]
          }}]));");

            webDriver.Navigate().Refresh();

        }
    }
}
