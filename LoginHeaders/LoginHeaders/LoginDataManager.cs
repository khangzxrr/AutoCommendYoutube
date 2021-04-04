using System.IO;

namespace LoginHeaders
{
    public class LoginDataManager
    {
        public string dataFile { get; set; }
        public string errorFile = "cookie_error.txt";
        private StreamWriter errorlog;
        public LoginDataManager(string dataFile)
        {
            this.dataFile = dataFile;

            File.Delete(errorFile);
            errorlog = File.AppendText(errorFile);
            errorlog.AutoFlush = true;
        }

        public void errorLogData(string cookieStr)
        {
            errorlog.WriteLine(cookieStr);
        }
        public string[] GetData()
        {
            return File.ReadAllLines(dataFile);
        }

        public void Close()
        {
            errorlog.Close();
        }
        
    }
}
