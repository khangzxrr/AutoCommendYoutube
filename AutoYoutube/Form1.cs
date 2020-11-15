// Decompiled with JetBrains decompiler
// Type: AutoYoutube.Form1
// Assembly: AutoYoutube, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00E87B93-1BD7-497A-BDD2-3A581C9826C5
// Assembly location: C:\Users\skyho\Downloads\Release-Copy\Release - Copy\AutoYoutube.exe

using AutoYoutube.Core.Extensions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using VNKClass;
using WebDriver;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace AutoYoutube
{
    public class Form1 : Form
    {
        private List<IWebDriver> webDrivers;
        private CookieManager cookies;
        private List<string> keys = new List<string>();
        private List<string> comments = new List<string>();
        private IContainer components;
        private Button button1;
        private Label labelCountCookies;
        private Label labelCountKeys;
        private Label labelCountComments;
        private CheckBox checkBoxFilter;
        private TextBox numVideo;
        private Label label1;
        private CheckBox checkBox1;

        private int KeyIndex = 0;

        ChromeOptions chromeOptions;
        private TextBox delayField;
        private Label label2;
        ChromeDriverService driverService;

        public Form1()
        {



            this.InitializeComponent();
            this.webDrivers = new List<IWebDriver>();
            this.keys = this.readKeys();
            this.comments = this.readComments();

            this.cookies = new CookieManager("cookie.txt");
            this.labelCountCookies.Text = cookies.GetCookies().Length.ToString();
            this.labelCountKeys.Text = this.keys.Count.ToString();
            this.labelCountComments.Text = this.comments.Count.ToString();
        }

        private List<string> readKeys() => ((IEnumerable<string>)File.ReadAllLines("key.txt")).Where<string>((Func<string, bool>)(s => !string.IsNullOrEmpty(s) && !string.IsNullOrWhiteSpace(s))).ToList<string>();

        private List<string> readComments() => ((IEnumerable<string>)File.ReadAllLines("comment.txt")).Where<string>((Func<string, bool>)(s => !string.IsNullOrEmpty(s) && !string.IsNullOrWhiteSpace(s))).ToList<string>();


        private void button1_Click(object sender, EventArgs e)
        {
            new DriverManager().SetUpDriver(new ChromeConfig());

            driverService = ChromeDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;


            chromeOptions = new ChromeOptions();
            chromeOptions.AddExtension("ModHeader.crx");
            KeyIndex = 0; //checkpoint keyindex

            if (this.checkBox1.Checked)
            {
                this.runOneEmailOneKey();
            }
            else
            {
                this.runManyEmailManyKey();
            }
                
        }

        private bool TestLogged(IWebDriver driver, string cookieStr)
        {
            driver.Url = "https://youtube.com/";
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);

            bool logged;
            try
            {
                driver.FindElement(By.CssSelector("#buttons > ytd-button-renderer"));
                logged = false;
            }
            catch (Exception)
            {
                Console.WriteLine("Logged!");
                logged = true;
                //if the exception is throw, then logged
            }

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);
            if (!logged)
            {
                cookies.errorLogCookie(cookieStr);
            }

            return logged;
        }
        private void runOneEmailOneKey()
        {
            //int count = this.cookies.Count;
            IWebDriver chrome = null;

            foreach (string cookieStr in cookies.GetCookies())
            {

                try
                {
                    chrome = (IWebDriver)new ChromeDriver(driverService, chromeOptions);

                    HeaderModifier.GenerateOAuth2Token(chrome, cookieStr);
                    if (!TestLogged(chrome, cookieStr)) continue;

                    int delayDuration = int.Parse(delayField.Text);
                    chrome.GetVideoAndComment(this.keys[KeyIndex++], this.comments, delayDuration, checkBoxFilter.Checked ? "CAMSBAgCEAE%253D" : "CAASBAgBEAE%253D", int.Parse(numVideo.Text));

                    if (KeyIndex == keys.Count)
                    {
                        KeyIndex = 0;
                    }

                    chrome.Quit();
                }

                catch (Exception ex)
                {
                    if (ex.Message.Contains("remote"))
                    {
                        chrome = (IWebDriver)new ChromeDriver(driverService, chromeOptions);
                    }
                    else
                    {
                        Console.WriteLine("Error at running one email: " + ex.Message + " " + ex.StackTrace);
                    }
                    
                }

                // this.labelCountCookies.Text = count--.ToString();
            }


        }

        private void runManyEmailManyKey()
        {
            // int count = this.cookies.Count;
            foreach (string cookieStr in cookies.GetCookies())
            {
                IWebDriver chrome = (IWebDriver)new ChromeDriver(driverService, chromeOptions);
                HeaderModifier.GenerateOAuth2Token(chrome, cookieStr);

                if (!TestLogged(chrome, cookieStr)) continue;

                foreach (string key in this.keys)
                {
                    try
                    {
                        int delayDuration = int.Parse(delayField.Text);
                        chrome.GetVideoAndComment(key, this.comments, delayDuration, checkBoxFilter.Checked ? "CAMSBAgCEAE%253D" : "CAASBAgBEAE%253D", int.Parse(numVideo.Text));
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("remote"))
                        {
                            chrome = (IWebDriver)new ChromeDriver(driverService, chromeOptions);
                        }
                        else
                        {
                            Console.WriteLine("Error at running many email: " + ex.Message + " " + ex.StackTrace);
                        }
                    }
                }

                chrome.Quit();
                //this.labelCountCookies.Text = count--.ToString();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.webDrivers.Clear();
            cookies.Close();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
                this.components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.labelCountCookies = new System.Windows.Forms.Label();
            this.labelCountKeys = new System.Windows.Forms.Label();
            this.labelCountComments = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBoxFilter = new System.Windows.Forms.CheckBox();
            this.numVideo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.delayField = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(43, 99);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(341, 55);
            this.button1.TabIndex = 0;
            this.button1.Text = "Run";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // labelCountCookies
            // 
            this.labelCountCookies.AutoSize = true;
            this.labelCountCookies.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCountCookies.ForeColor = System.Drawing.SystemColors.Highlight;
            this.labelCountCookies.Location = new System.Drawing.Point(38, 49);
            this.labelCountCookies.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelCountCookies.Name = "labelCountCookies";
            this.labelCountCookies.Size = new System.Drawing.Size(39, 29);
            this.labelCountCookies.TabIndex = 1;
            this.labelCountCookies.Text = "11";
            // 
            // labelCountKeys
            // 
            this.labelCountKeys.AutoSize = true;
            this.labelCountKeys.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCountKeys.ForeColor = System.Drawing.SystemColors.Highlight;
            this.labelCountKeys.Location = new System.Drawing.Point(208, 49);
            this.labelCountKeys.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelCountKeys.Name = "labelCountKeys";
            this.labelCountKeys.Size = new System.Drawing.Size(39, 29);
            this.labelCountKeys.TabIndex = 2;
            this.labelCountKeys.Text = "11";
            // 
            // labelCountComments
            // 
            this.labelCountComments.AutoSize = true;
            this.labelCountComments.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCountComments.ForeColor = System.Drawing.SystemColors.Highlight;
            this.labelCountComments.Location = new System.Drawing.Point(345, 49);
            this.labelCountComments.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelCountComments.Name = "labelCountComments";
            this.labelCountComments.Size = new System.Drawing.Size(39, 29);
            this.labelCountComments.TabIndex = 2;
            this.labelCountComments.Text = "11";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Location = new System.Drawing.Point(43, 170);
            this.checkBox1.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(113, 21);
            this.checkBox1.TabIndex = 3;
            this.checkBox1.Text = "1 email 1 key";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // checkBoxFilter
            // 
            this.checkBoxFilter.AutoSize = true;
            this.checkBoxFilter.Location = new System.Drawing.Point(43, 206);
            this.checkBoxFilter.Name = "checkBoxFilter";
            this.checkBoxFilter.Size = new System.Drawing.Size(115, 21);
            this.checkBoxFilter.TabIndex = 4;
            this.checkBoxFilter.Text = "\"Today\" Filter";
            this.checkBoxFilter.UseVisualStyleBackColor = true;
            // 
            // numVideo
            // 
            this.numVideo.Location = new System.Drawing.Point(335, 171);
            this.numVideo.Name = "numVideo";
            this.numVideo.Size = new System.Drawing.Size(49, 22);
            this.numVideo.TabIndex = 5;
            this.numVideo.Text = "30";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(210, 171);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 17);
            this.label1.TabIndex = 6;
            this.label1.Text = "Number of videos";
            // 
            // delayField
            // 
            this.delayField.Location = new System.Drawing.Point(335, 207);
            this.delayField.Name = "delayField";
            this.delayField.Size = new System.Drawing.Size(76, 22);
            this.delayField.TabIndex = 7;
            this.delayField.Text = "3000";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(257, 210);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 17);
            this.label2.TabIndex = 8;
            this.label2.Text = "Delay(ms)";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(426, 255);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.delayField);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numVideo);
            this.Controls.Add(this.checkBoxFilter);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.labelCountComments);
            this.Controls.Add(this.labelCountKeys);
            this.Controls.Add(this.labelCountCookies);
            this.Controls.Add(this.button1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "formname";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
