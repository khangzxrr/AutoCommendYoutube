using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNKClass
{
    class CookieManager
    {
        public string cookiesFile { get; set; }
        public string errorFile = "cookie_error.txt";
        private StreamWriter errorlog;
        public CookieManager(string cookiesFile)
        {
            this.cookiesFile = cookiesFile;

            File.Delete(errorFile);
            errorlog = File.AppendText(errorFile);
            errorlog.AutoFlush = true;
        }

        public void errorLogCookie(string cookieStr)
        {
            errorlog.WriteLine(cookieStr);
        }
        public string[] GetCookies()
        {
            return File.ReadAllLines(cookiesFile);
        }

        public void Close()
        {
            errorlog.Close();
        }
        
    }
}
