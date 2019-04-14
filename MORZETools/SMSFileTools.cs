using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS
{
    public class SMSFileTools
    {
        static public string SMSPath
        {
            get
            {
                
                const string smsDir = "SMS-messenger\\";
                string path;
                path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                if (path[path.Length - 1] != '\\')
                    path += "\\";
                path += smsDir;
                if (Directory.Exists(path) == false)
                    Directory.CreateDirectory(path);
                return path;
            }
        }
    }
}
