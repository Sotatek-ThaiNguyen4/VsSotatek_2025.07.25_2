using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VsSotatek
{
    public class clsLog
    {
        public static void WriteLog_txt(string _pathFolderLog, string message)
        {
            try
            {
                string pathLog = _pathFolderLog + string.Format("\\{0}.txt", DateTime.Now.ToString("yyyy.MM.dd"));
                string[] text = new string[1];
                text[0] = message;
                File.AppendAllLines(pathLog, text);
            }
            catch
            { }
        }

        public static void WriteLog_CSV(string _path, string message)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(_path, true, System.Text.Encoding.UTF8))
                {
                    writer.WriteLine(message);
                }

            }
            catch
            { }
        }
    }
}
