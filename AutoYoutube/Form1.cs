// Decompiled with JetBrains decompiler
// Type: AutoYoutube.Form1
// Assembly: AutoYoutube, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00E87B93-1BD7-497A-BDD2-3A581C9826C5
// Assembly location: C:\Users\skyho\Downloads\Release-Copy\Release - Copy\AutoYoutube.exe

using AutoYoutube.Core.Extensions;
using LoginHeaders;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Forms;

namespace AutoYoutube
{
    public class Form1 : Form
    {
        private List<IWebDriver> webDrivers;
        private LoginDataManager loginDatas;
        private List<string> keys = new List<string>();
        private List<string> comments = new List<string>();
        private Button button1;
        private CheckBox checkBoxFilter;
        private TextBox numVideo;
        private Label label1;
        private CheckBox checkBox1;


        private TextBox delayField;
        private Label label2;
        private CheckBox multilineCheckbox;
        private ProgressBar progressBarDone;
        private Label totalCookies;
        private Label totalKeys;
        private Label totalComments;
       
        public Form1()
        {
            this.InitializeComponent();

            CloseAllChrome();

            numVideo.Text = ConfigurationManager.AppSettings.Get("default_video_count");
            this.webDrivers = new List<IWebDriver>();
            this.keys = this.readKeys();
            this.comments = this.readComments();

            this.loginDatas = new LoginDataManager("cookies.txt");


            this.totalCookies.Text = "Total cookies: " + loginDatas.GetData().Length.ToString();
            this.totalKeys.Text = "Total keys: " + keys.Count.ToString();
            this.totalComments.Text = "Total Comments: " + comments.Count.ToString();

            
        }

        private List<string> readKeys() => ((IEnumerable<string>)File.ReadAllLines("key.txt")).Where<string>((Func<string, bool>)(s => !string.IsNullOrEmpty(s) && !string.IsNullOrWhiteSpace(s))).ToList<string>();

        private List<string> readComments()
        {
            List<string> lines = File.ReadAllLines("comment.txt").ToList();

            if (!lines.Last().Contains("--"))
            {
                lines.Add("---");
            }

            if (multilineCheckbox.Checked)
            {
                List<string> comments = new List<string>();

                string comment = "";
                foreach(var line in lines)
                {
                    if (line.Contains("---"))
                    {
                        if (comment != "")
                        {
                            comments.Add(comment);
                            comment = "";
                        }

                    }
                    else
                    {
                        comment += line + "\n";
                    }
                }

                return comments;

            }

            return lines.ToList<string>();
        }

     
        private void button1_Click(object sender, EventArgs e)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerAsync();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {

            
            if (this.checkBox1.Checked)
            {
                this.runOneEmailOneKey();
            }
            else
            {
                this.runManyEmailManyKey();
            }
        }

      
        private async Task<ChromeDriver> createChrome()
        {
            ChromeDriver driver;
            while (true)
            {
                try
                {
                    CloseAllChrome();

                    var driverService = ChromeDriverService.CreateDefaultService();
                    driverService.HideCommandPromptWindow = true;
                    driverService.Port = new Random().Next(6569, 8100);

                    var chromeOptions = new ChromeOptions();

                    //chrome binary
                    //chromeOptions.BinaryLocation = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe";
                    //=====================
                    chromeOptions.AddArgument("--no-sandbox");

                    chromeOptions.AddExtension("ModHeader.crx");

                    
                    Thread.Sleep(1000);

                    driver = new ChromeDriver(driverService, chromeOptions, TimeSpan.FromSeconds(30));

                    var devtools = driver.GetDevToolsSession().GetVersionSpecificDomains<OpenQA.Selenium.DevTools.V89.DevToolsSessionDomains>();
                    

                    foreach (var window in driver.WindowHandles)
                    {
                        driver.SwitchTo().Window(window);
                        if (driver.WindowHandles.Count == 1)
                        {
                            break;
                        }
                        
                        driver.Close();
                    }

                    driver.Manage().Window.Maximize();


                    return driver;
                }


                catch (Exception ex)
                {
                    //if ()
                    //{
                    //    MessageBox.Show("ERROR DETECTED CANNOT RETRY: " + ex.Message);
                    //    Application.Exit();
                    //}
                    AutoClosingMessageBox.Show(ex.Message, "error when create webdriver", 2000);

                    driver = null;
                    continue;

                    //test another chrome location
                }
            }
            
        }

        private void runOneEmailOneKey(int triedTime = 0)
        {
            //int count = this.cookies.Count;
            var chromeDriver = createChrome().GetAwaiter().GetResult();

            int KeyIndex = 0;
            while (true)
            {
                foreach (string loginData in loginDatas.GetData())
                {

                    try
                    {
                        HeaderModifier headerModifier = new HeaderModifier(loginData);
                        
                        int delayDuration = int.Parse(delayField.Text);

                        KeyIndex++;
                        if (KeyIndex == keys.Count)
                        {
                            KeyIndex = 0;
                        }

                        List<string> urls = chromeDriver.GetVideoUrls(keys[KeyIndex], getVideoFilters(), int.Parse(numVideo.Text));
                        if (urls == null) continue;

                        headerModifier.GenerateOAuth2Token(chromeDriver);

                        foreach (var url in urls)
                        {
                            try
                            {
                                chromeDriver.LoadVideoAndComment(url, delayDuration, comments);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                                Console.WriteLine(ex.StackTrace);
                                if (ex.Message.Contains("time"))
                                {
                                    chromeDriver = createChrome().GetAwaiter().GetResult();
                                    headerModifier.GenerateOAuth2Token(chromeDriver);
                                }
                            }

                        }

                       

                        triedTime = 0;
                    }

                    catch (Exception ex)
                    {
                        Console.WriteLine("Error at running one email: " + ex.Message + " " + ex.StackTrace);
                        chromeDriver = createChrome().GetAwaiter().GetResult() ;
                        triedTime++;
                    }

                    if (triedTime == 3)
                    {
                        break;
                    }
                }


                // this.labelCountCookies.Text = count--.ToString();
            }



        }

   
        private void KillProcesses(Process[] processes)
        {
            foreach (Process pro in processes)
            {
                try
                {
                    pro.Kill();
                }
                catch { }

            }
        }
        private void CloseAllChrome()
        {
            var processes = Process.GetProcessesByName("chromedriver");
            KillProcesses(processes);

            processes = Process.GetProcessesByName("conhost");
            KillProcesses(processes);

            foreach (Process process in Process.GetProcessesByName("chrome"))
            {
                if (process.MainWindowHandle == IntPtr.Zero) // some have no UI
                    continue;

                AutomationElement element = AutomationElement.FromHandle(process.MainWindowHandle);
                if (element != null)
                {
                    ((WindowPattern)element.GetCurrentPattern(WindowPattern.Pattern)).Close();
                }
            }
        }

        private string getVideoFilters()
        {
            return checkBoxFilter.Checked ? "CAMSBAgCEAE%253D" : "CAASBAgBEAE%253D";
        }
        private void runManyEmailManyKey(int triedTime = 0)
        {
            // int count = this.cookies.Count;
            var chromeDriver = createChrome().GetAwaiter().GetResult();

            int numVideoToGet = int.Parse(numVideo.Text);
            int currentKeyIndex = 0;
            int delayDuration = int.Parse(delayField.Text);
            while (true)
            {

                //looping until you get video urls.
                List<string> videoUrls = chromeDriver.GetVideoUrls(keys[currentKeyIndex], getVideoFilters(), numVideoToGet);


                // 1 2 3 4 5 videos
                // 1 2 3     accounts

                // 0 1 2     videos
                // 0 1 2 3 4 accounts

                //so, solve it please?


                int maxOfVideosOrAccounts = Math.Max(videoUrls.Count, loginDatas.GetData().Length);

                int accountIndex = 0;
                for (int i = 0; i < maxOfVideosOrAccounts; i++)
                {
                    if (i >= videoUrls.Count)
                    {
                        i = 0; //reset video index, if ran out of videos but still accounts left.
                    }
                    if (accountIndex >= loginDatas.GetData().Length)
                    {
                        accountIndex = 0; //back to first index again, if videos is not ended but we ran out of accounts
                    }


                    try
                    {
                        //LOGIN =====
                        HeaderModifier headerModifier = new HeaderModifier(loginDatas.GetData()[accountIndex]);
                        headerModifier.GenerateOAuth2Token(chromeDriver);
                        //===========

                        chromeDriver.LoadVideoAndComment(videoUrls[i], delayDuration, comments);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error at running many email: " + ex.Message + " " + ex.StackTrace);
                    }

                    accountIndex++;
                }

                currentKeyIndex++;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseAllChrome();
            this.webDrivers.Clear();
            loginDatas.Close();
            Environment.Exit(0);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBoxFilter = new System.Windows.Forms.CheckBox();
            this.numVideo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.delayField = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.multilineCheckbox = new System.Windows.Forms.CheckBox();
            this.progressBarDone = new System.Windows.Forms.ProgressBar();
            this.totalCookies = new System.Windows.Forms.Label();
            this.totalKeys = new System.Windows.Forms.Label();
            this.totalComments = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(47, 130);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(341, 55);
            this.button1.TabIndex = 0;
            this.button1.Text = "Run";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Location = new System.Drawing.Point(44, 206);
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
            this.checkBoxFilter.Location = new System.Drawing.Point(44, 232);
            this.checkBoxFilter.Name = "checkBoxFilter";
            this.checkBoxFilter.Size = new System.Drawing.Size(115, 21);
            this.checkBoxFilter.TabIndex = 4;
            this.checkBoxFilter.Text = "\"Today\" Filter";
            this.checkBoxFilter.UseVisualStyleBackColor = true;
            // 
            // numVideo
            // 
            this.numVideo.Location = new System.Drawing.Point(336, 207);
            this.numVideo.Name = "numVideo";
            this.numVideo.Size = new System.Drawing.Size(49, 22);
            this.numVideo.TabIndex = 5;
            this.numVideo.Text = "10";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(211, 207);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 17);
            this.label1.TabIndex = 6;
            this.label1.Text = "Number of videos";
            // 
            // delayField
            // 
            this.delayField.Location = new System.Drawing.Point(336, 243);
            this.delayField.Name = "delayField";
            this.delayField.Size = new System.Drawing.Size(76, 22);
            this.delayField.TabIndex = 7;
            this.delayField.Text = "3000";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(258, 246);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 17);
            this.label2.TabIndex = 8;
            this.label2.Text = "Delay(ms)";
            // 
            // multilineCheckbox
            // 
            this.multilineCheckbox.AutoSize = true;
            this.multilineCheckbox.Location = new System.Drawing.Point(44, 259);
            this.multilineCheckbox.Name = "multilineCheckbox";
            this.multilineCheckbox.Size = new System.Drawing.Size(153, 21);
            this.multilineCheckbox.TabIndex = 9;
            this.multilineCheckbox.Text = "Multi line comments";
            this.multilineCheckbox.UseVisualStyleBackColor = true;
            this.multilineCheckbox.CheckedChanged += new System.EventHandler(this.reReadComments);
            // 
            // progressBarDone
            // 
            this.progressBarDone.Location = new System.Drawing.Point(43, 13);
            this.progressBarDone.Name = "progressBarDone";
            this.progressBarDone.Size = new System.Drawing.Size(341, 23);
            this.progressBarDone.TabIndex = 10;
            // 
            // totalCookies
            // 
            this.totalCookies.AutoSize = true;
            this.totalCookies.Location = new System.Drawing.Point(44, 61);
            this.totalCookies.Name = "totalCookies";
            this.totalCookies.Size = new System.Drawing.Size(96, 17);
            this.totalCookies.TabIndex = 11;
            this.totalCookies.Text = "Total cookies:";
            this.totalCookies.Click += new System.EventHandler(this.totalCookies_Click);
            // 
            // totalKeys
            // 
            this.totalKeys.AutoSize = true;
            this.totalKeys.Location = new System.Drawing.Point(44, 81);
            this.totalKeys.Name = "totalKeys";
            this.totalKeys.Size = new System.Drawing.Size(77, 17);
            this.totalKeys.TabIndex = 12;
            this.totalKeys.Text = "Total keys:";
            this.totalKeys.Click += new System.EventHandler(this.totalKey_Click);
            // 
            // totalComments
            // 
            this.totalComments.AutoSize = true;
            this.totalComments.Location = new System.Drawing.Point(45, 105);
            this.totalComments.Name = "totalComments";
            this.totalComments.Size = new System.Drawing.Size(112, 17);
            this.totalComments.TabIndex = 13;
            this.totalComments.Text = "Total comments:";
            this.totalComments.Click += new System.EventHandler(this.label3_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(426, 302);
            this.Controls.Add(this.totalComments);
            this.Controls.Add(this.totalKeys);
            this.Controls.Add(this.totalCookies);
            this.Controls.Add(this.progressBarDone);
            this.Controls.Add(this.multilineCheckbox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.delayField);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numVideo);
            this.Controls.Add(this.checkBoxFilter);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.button1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "formname";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void totalKey_Click(object sender, EventArgs e)
        {

        }

        private void totalCookies_Click(object sender, EventArgs e)
        {

        }

        private void reReadComments(object sender, EventArgs e)
        {
            comments = readComments();
            totalComments.Text = "Total comments: " + comments.Count.ToString();
        }
    }
}
