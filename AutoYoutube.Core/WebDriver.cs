﻿// Decompiled with JetBrains decompiler
// Type: AutoYoutube.Core.Extensions.WebDriver
// Assembly: AutoYoutube.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A4224557-9BE3-41E4-AE0B-A16A163F0DE0
// Assembly location: C:\Users\skyho\Downloads\Release-Copy\Release - Copy\AutoYoutube.Core.dll

using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Threading;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace AutoYoutube.Core.Extensions
{
    public static class WebDriver
    {
        private static Random rnd = new Random();

        private static IWebElement WebDriverWait(
          this IWebDriver webDriver,
          By by,
          int timeOut = 10)
        {
            
            return new WebDriverWait(webDriver, TimeSpan.FromSeconds((double)timeOut)).Until<IWebElement>(ExpectedConditions.ElementToBeClickable(by));
        }


        public static List<string> GetVideoUrls(
             this IWebDriver webDriver,
            string key,
            string filter = "CAASBAgBEAE%253D",
            int count = 30
           )
        {
            try
            {
                var searchUrl = $"https://www.youtube.com/results?search_query=" + key + $"&sp={filter}";


                webDriver.ExecuteJavaScript($"window.location.href = '{searchUrl}'");

                long num1 = 0;
                long num2;

                List<String> urls = new List<string>();
                do
                {

                    webDriver.ExecuteJavaScript("document.querySelectorAll(\"#dismissable>div>#buttons>ytd-toggle-button-renderer\").forEach(e => e.parentNode.parentNode.innerHTML = null); document.querySelectorAll(\"#dismissable>div>#badges>.badge-style-type-live-now\").forEach(e => e.parentNode.parentNode.innerHTML = null);");

                    var thumbnailElements = webDriver.FindElements(By.CssSelector("a#video-title"));

                    urls.Clear();
                    foreach (var thumbnail in thumbnailElements)
                    {
                        string videoUrl = thumbnail.GetAttribute("href");
                        urls.Add(videoUrl);
                    }

                    Console.WriteLine($"Got {urls.Count} videos");

                    if (urls.Count >= count)
                    {
                        break;
                    }

                    num2 = num1;
                    num1 = webDriver.ExecuteJavaScript<long>("return document.documentElement.scrollTop = document.documentElement.scrollHeight;");


                    Thread.Sleep(2000);
                }
                while (num1 > num2);



                urls = urls.GetRange(0, (count > urls.Count) ? urls.Count : count);

                return urls;
            }
            catch(Exception ex)
            {
                Console.WriteLine("Catched exception when reading video urls: " + ex.Message);
            }

            return null;
        }

        public static void LoadVideoAndComment(
             this IWebDriver webDriver,
             string url,
             int delayDuration,
             List<string> comments
            )
        {
            webDriver.ExecuteJavaScript($"window.location.href = '{url}'");


            webDriver.ExecuteJavaScript("window.scrollTo(0,document.body.scrollHeight);");

           
            if (webDriver.WebDriverWait(By.Id(nameof(comments))) != null)
            {
                Thread.Sleep(delayDuration);

                webDriver.ExecuteJavaScript("document.getElementById('comments').scrollIntoView();");
                try
                {
                    webDriver.WebDriverWait(By.Id("simplebox-placeholder"), 5).Click();
                    webDriver.WebDriverWait(By.Id("contenteditable-root")).SendKeys("1");

                    var parsingComment = comments[WebDriver.rnd.Next(comments.Count)].Replace("\n", "\\n")
                                            .Replace("'", "\\'");

                    var comment = $@"document.getElementById('contenteditable-root').innerHTML = '{parsingComment}'";
                    Console.WriteLine(comment);
                    webDriver.ExecuteJavaScript(comment);

                    webDriver.WebDriverWait(By.Id("submit-button")).Click();

                    Thread.Sleep(1500);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error at WebDriver: " + ex.Message + " " + ex.StackTrace);
                }
            }
        }

        /*
        public static void GetVideoAndComment(
          this IWebDriver webDriver,
          string key,
          List<string> comments,
          string filter = "CAASBAgBEAE%253D",
          int count = 30)
        {
            try
            {
               
            }
            catch
            {
            }
        }
        */


        public static void DeleteAllCookies(this IWebDriver webDriver)
        {
            try
            {
                webDriver.Manage().Cookies.DeleteAllCookies();
            }
            catch
            {
            }
        }

        public static bool IsClosed(this IWebDriver webDriver)
        {
            try
            {
                return string.IsNullOrEmpty(webDriver.Title);
            }
            catch
            {
                return true;
            }
        }
    }
}
