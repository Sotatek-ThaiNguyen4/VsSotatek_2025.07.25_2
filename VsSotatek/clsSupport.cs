using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VsSotatek
{
    internal class clsSupport
    {
        /// <summary>
        /// SUPPORT FOLDER
        /// </summary>
        #region SUPPORT FOLDER
        const string directorySaveImgSRC = "\\SRC IMAGE";
        const string directorySaveImgNG = "\\NG IMAGE";
        const string directorySaveImgOK = "\\OK IMAGE";
        const string directorySaveImgWARP = "\\WARP IMAGE";

        public static bool MakeFolder()
        {
            bool IsSuccess = true; // kiểm tra điều kiện khi khởi tạo form nếu tạo folder bị lỗi
            try
            {
                string directoryDate = "\\" + DateTime.Now.ToString("yyyyMMdd");
                // Tạo folder 
                if (!Directory.Exists(clsDefine.PATH_SYSTEM))
                    Directory.CreateDirectory(clsDefine.PATH_SYSTEM);
                if (!Directory.Exists(clsDefine.PATH_LOG_ROOT_HISTORY_APP))
                    Directory.CreateDirectory(clsDefine.PATH_LOG_ROOT_HISTORY_APP);

                clsDefine.PATH_LOG_FOLDER_IMG_DATA_DATE = clsDefine.PATH_LOG_ROOT_IMAGE_DATA + directoryDate;
                clsDefine.PATH_LOG_FOLDER_IMG_DATA_SRC = clsDefine.PATH_LOG_ROOT_IMAGE_DATA + directoryDate + "\\" + directorySaveImgSRC;
                clsDefine.PATH_LOG_FOLDER_IMG_DATA_NG = clsDefine.PATH_LOG_ROOT_IMAGE_DATA + directoryDate + "\\" + directorySaveImgNG;
                clsDefine.PATH_LOG_FOLDER_IMG_DATA_OK = clsDefine.PATH_LOG_ROOT_IMAGE_DATA + directoryDate + "\\" + directorySaveImgOK;
                clsDefine.PATH_LOG_FOLDER_IMG_DATA_WARP = clsDefine.PATH_LOG_ROOT_IMAGE_DATA + directoryDate + "\\" + directorySaveImgWARP;
                if (!Directory.Exists(clsDefine.PATH_LOG_FOLDER_IMG_DATA_SRC))
                    Directory.CreateDirectory(clsDefine.PATH_LOG_FOLDER_IMG_DATA_SRC);
                if (!Directory.Exists(clsDefine.PATH_LOG_FOLDER_IMG_DATA_NG))
                    Directory.CreateDirectory(clsDefine.PATH_LOG_FOLDER_IMG_DATA_NG);
                if (!Directory.Exists(clsDefine.PATH_LOG_FOLDER_IMG_DATA_OK))
                    Directory.CreateDirectory(clsDefine.PATH_LOG_FOLDER_IMG_DATA_OK);
                if (!Directory.Exists(clsDefine.PATH_LOG_FOLDER_IMG_DATA_WARP))
                    Directory.CreateDirectory(clsDefine.PATH_LOG_FOLDER_IMG_DATA_WARP);
            }
            catch
            {
                IsSuccess = false; // Trả về là false nếu tạo folder lỗi
            }
            return IsSuccess;
        }
        #endregion FOLDER

        public static void SaveMatToCsv(Mat mat, string filename)
        {
            using (StreamWriter writer = new StreamWriter(filename))
            {
                for (int y = 0; y < mat.Rows; y++)
                {
                    string[] rowValues = new string[mat.Cols];
                    for (int x = 0; x < mat.Cols; x++)
                    {
                        rowValues[x] = mat.At<byte>(y, x).ToString(); // Giữ 2 số thập phân
                    }
                    writer.WriteLine(string.Join(",", rowValues));
                }
            }
        }

        public static void SaveArrayToCsv<T>(IEnumerable<T> arr, string filename)
        {
            using (StreamWriter writer = new StreamWriter(filename))
            {
                List<string> rowValues = new List<string>();
                foreach (var item in arr)
                {
                    if (item == null)
                    {
                        rowValues.Add("null");
                    }
                    else
                    {
                        rowValues.Add(item.ToString());
                    }
                }
                writer.WriteLine(string.Join(",", rowValues));
            }
        }

    }
}
