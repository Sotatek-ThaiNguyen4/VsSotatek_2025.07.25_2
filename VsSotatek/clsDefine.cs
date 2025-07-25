using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VsSotatek
{
    public class clsDefine
    {
        public static string _VERSION_ = "Ver.0.0.1"; // Version
        public static string _DATE_MAKE_ = "Date.25.07.2025"; // ngày sửa

        public static string PATH_APP = "D:\\";
        public static string PATH_SYSTEM = "D:\\VISION SOTATEK\\System";
        public static string PATH_CONFIG = PATH_SYSTEM + "\\config.ini";
        public static string PATH_JOB = PATH_SYSTEM + "\\job.vs";

        //Thông số Vision
        public static double VI_CONTRAST = 2.5;//0.0-10.0 //Ngưỡng để làm trắng nền và tối Keo
        public static double VI_BRIGHTNESS = -45; //-255.0 -> 255.0 //Ngưỡng để làm trắng nền và tối Keo
        public static double VI_THRESHOLD = 230; //0 -> 255

        public static double VI_THRESHOLD_BU_KEO = 230; //Ngưỡng để tách lần đầu tiên bù bóng kéo (Ngưỡng phải lớn để lấy các điểm sáng)
        public static double VI_THRESHOLD_DETECT_EPOXY = 200; //Ngưỡng để tách Keo và nền FPCB (lấy độ sáng nền FPCB làm mốc - 20), nền trắng keo đen => nên ngưỡng lớn > 150 

        //Thông số blob bắt over keo
        public static double VI_CONTRAST_OVER = 2;//0.0-10.0 //Ngưỡng để làm trắng nền và tối tràn keo
        public static double VI_BRIGHTNESS_OVER = 0; //-255.0 -> 255.0 //Ngưỡng để làm trắng nền và tối tràn keo
        public static double VI_THRESHOLD_OVER = 230; //0 -> 255



        //Thông số Calibration
        public static double CAL_RESOLUTION = 0.0069; //
        public static int CAL_OFFSET_Y = 68; //

        //Thông số Master
        public static double MASTER_LENGHT_FPCB_MM = 5.4; //mm +- 0.2
        public static double MASTER_WIDTH_MM = 0.5; //mm < 0.5mm

        //Thống số Spect
        public static double SPECT_LENGHT_FPCB = 0.2; //mm


        //MODE
        public static bool M_RUN_SIMULATION_BUBBLE = false; //Bật True sẽ chạy simu check bọt khí dù OK hay NG
        public static bool M_CHECK_BUBBLE = false;
        public static bool M_GRAPHIC = true;

        //PATH
        public static string PATH_ROOT_LOG = "D:\\VISION SOTATEK"; // file Root lịch sử ảnh
        public static string PATH_LOG_ROOT_IMAGE_DATA = PATH_ROOT_LOG + "\\Image Data"; // Nơi lưu ảnh chụp của vision
        public static string PATH_LOG_ROOT_HISTORY_APP = PATH_ROOT_LOG + "\\Log"; // Nơi lưu log lịch sử hoạt động của APP

        public static string PATH_LOG_FOLDER_IMG_DATA_DATE;
        public static string PATH_LOG_FOLDER_IMG_DATA_SRC;
        public static string PATH_LOG_FOLDER_IMG_DATA_NG;
        public static string PATH_LOG_FOLDER_IMG_DATA_OK;
        public static string PATH_LOG_FOLDER_IMG_DATA_WARP;
        public static string PATH_MODEL_SVM;

        public static string[] HEAD_TILE_RESULT = new string[] { "Time", "Tact Time (ms)", "Nhãn", "File Name", "Result", "Align", "NG1-Ngắn(X)", "NG2-Rộng(Y)", "NG3-Mất kết nối", "NG4-Tràn keo", "NG5-Bọt khí",
                "FPCB lenght (pixel)", "FPCB lenght (mm)", "Resolution", "Epoxy lenght (pixel)", "Epoxy lenght (mm)", "Rate Lenght Epoxy/Fpcb", "Max epoxy width (pixel)",
                    "Max epoxy width (mm)"};

        public static void ReadDefine()
        {
            CAL_RESOLUTION = clsIni.ReadValue_double(PATH_CONFIG, "CAL", "CAL_RESOLUTION", CAL_RESOLUTION);
            CAL_OFFSET_Y = clsIni.ReadValue_int(PATH_CONFIG, "CAL", "CAL_OFFSET_Y", CAL_OFFSET_Y);
        }

        public static void WriteDefine()
        {
            clsIni.WriteValue(PATH_CONFIG, "CAL", "CAL_RESOLUTION", CAL_RESOLUTION.ToString());
            clsIni.WriteValue(PATH_CONFIG, "CAL", "CAL_OFFSET_Y", CAL_OFFSET_Y.ToString());
        }

    }
}
