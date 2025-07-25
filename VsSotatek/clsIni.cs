using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VsSotatek
{
    class clsIni
    {
        /// <summary>
        /// Yêu cầu class Log4net để lưu log
        /// </summary>

        [DllImport("kernel32")]
        static extern long WritePrivateProfileString(string strSection, string strKey, string strValue, string strFilePath);

        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(string strSection, string strKey, string strDefault, StringBuilder retVal, int iSize, string strFilePath);

        const int defaultSize = 255;
        static StringBuilder sbBuffer = new StringBuilder();
        const string strNull = "N/A";


        public static void WriteValue(string filePath, string section, string key, string value) // ghi giá trị vào file ini
        {
            try
            {
                WritePrivateProfileString(section, key, value, filePath);
            }
            catch (Exception ex)
            {
                //ClsLog.LOG.Error("Write value ini error: " + ex.ToString());
            }
        }

        public static string ReadValue_str(string filePath, string section, string key, string defaultValue) // Đọc giá trị string
        {
            sbBuffer.Clear();
            try
            {
                GetPrivateProfileString(section, key, strNull, sbBuffer, defaultSize, filePath);
                if (sbBuffer.ToString() == strNull) // Kiểm tra nếu chuỗi trả về == default (N/A) thì trả về giá trị mặc định
                {
                    //ClsLog.LOG.Error(string.Format("Can't read value {0} of {1}",key,section));
                    return defaultValue;
                }
                else
                {
                    return sbBuffer.ToString();
                }
            }
            catch (Exception ex)
            {
                //ClsLog.LOG.Error(string.Format("Read value {0} of {1} error: {2}",key,section,ex.ToString()));
                return defaultValue;
            }
        }

        public static int ReadValue_int(string filePath, string section, string key, int defaultValue) // Đọc giá trị int
        {
            sbBuffer.Clear();
            try
            {
                int value;
                GetPrivateProfileString(section, key, strNull, sbBuffer, defaultSize, filePath);
                if (sbBuffer.ToString() == strNull) // Kiểm tra nếu chuỗi trả về == default (N/A) thì trả về giá trị mặc định
                {
                    //ClsLog.LOG.Error(string.Format("Can't read value {0} of {1}",key,section));
                    return defaultValue;
                }
                else
                {
                    if (int.TryParse(sbBuffer.ToString(), out value))
                        return value;
                    else
                    {
                        //ClsLog.LOG.Error(string.Format("Convert value {0} of {1} to int error - read value = {2}",key,section, sbBuffer.ToString()));
                        return defaultValue;
                    }
                }
            }
            catch (Exception ex)
            {
                //ClsLog.LOG.Error(string.Format("Read value {0} of {1} error: {2}",key,section,ex.ToString()));
                return defaultValue;
            }
        }

        public static double ReadValue_double(string filePath, string section, string key, double defaultValue) // Đọc giá trị double
        {
            sbBuffer.Clear();
            try
            {
                double value;
                GetPrivateProfileString(section, key, strNull, sbBuffer, defaultSize, filePath);
                if (sbBuffer.ToString() == strNull) // Kiểm tra nếu chuỗi trả về == default (N/A) thì trả về giá trị mặc định
                {
                    //ClsLog.LOG.Error(string.Format("Can't read value {0} of {1}",key,section));
                    return defaultValue;
                }
                else
                {
                    if (double.TryParse(sbBuffer.ToString(), out value))
                        return value;
                    else
                    {
                        //ClsLog.LOG.Error(string.Format("Convert value {0} of {1} to double error, read value = {2}",key,section, sbBuffer.ToString()));
                        return defaultValue;
                    }
                }
            }
            catch (Exception ex)
            {
                //ClsLog.LOG.Error(string.Format("Read value {0} of {1} error: {2}",key,section,ex.ToString()));
                return defaultValue;
            }
        }
    }
}
