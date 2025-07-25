using OpenCvSharp.ML;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using ViSolution.Object;
using static OpenCvSharp.ConnectedComponents;
using System.Diagnostics;
using System.IO;
using ViSolution;
using System.Threading;
using OpenCvSharp.Features2D;
using System.Windows.Media.Animation;

namespace VsSotatek
{
    public class clsResultVision
    {
        public double Resolution { get { return clsDefine.CAL_RESOLUTION; } } // resolution = mm/pixel
        public double Lenght_Fpcb_pixel { get; set; }
        public double Lenght_Fpcb_mm { get { return Math.Round(Resolution * Lenght_Fpcb_pixel, 2); } }
        public double Lenght_Glue_pixel { get; set; } // chiều dài vùng keo dán
        public double Lenght_Glue_mm { get { return Math.Round(Resolution * Lenght_Glue_pixel, 2); } }
        public double Rate_LenghtEpoxy2lenghtFpcb { get { return Math.Round(Lenght_Glue_pixel / Lenght_Fpcb_pixel, 2); } }
        public double Width_Max_Glue_pixel { get; set; }
        public double Width_Max_Epoxy_mm { get { return Math.Round(Resolution * Width_Max_Glue_pixel, 2); } }
        public double Area_Glue { get; set; }
        public double Area_Total { get; set; }

        public string Check_Result { get; set; } //"OK", "NG"
        public bool Check_Align { get { return (Lenght_Fpcb_mm > 2.2 && Lenght_Fpcb_mm < 5.6); } } //true == OK //Vì Resolution k chuẩn nên chưa đặt limit được
        public bool Check_Lenght { get { return Rate_LenghtEpoxy2lenghtFpcb > 0.8; } } //true == OK

        public bool Check_Width
        {
            get
            {
                if (Check_Lenght == false)
                {
                    return true;
                }
                else
                {
                    return (Width_Max_Epoxy_mm > 0.1 && Width_Max_Epoxy_mm <= 0.5);
                }
            }
        } //true == OK

        private bool check_continuous = true;
        public bool Check_Continuous
        {
            get
            {
                if (Check_Lenght == false || Check_Width == false)
                {
                    return true;
                }
                else
                {
                    return check_continuous;
                }
            }

            set
            {
                check_continuous = value;
            }
        } //true == OK

        private bool check_over = true;
        public bool Check_over
        {
            get
            {
                if (Check_Lenght == false || Check_Width == false || Check_Continuous == false)
                {
                    return true;
                }
                else
                {
                    return check_over;
                }
            }

            set
            {
                check_over = value;
            }
        } //true == OK

        private bool check_airBubbles = true;
        public bool Check_airBubbles
        {
            get
            {
                if (Check_Lenght == false || Check_Width == false || Check_Continuous == false || Check_over == false)
                {
                    return true;
                }
                else
                {
                    return check_airBubbles;
                }
            }

            set
            {
                check_airBubbles = value;
            }
        } //true == OK   


        public List<string> NG_CODE
        {
            get
            {
                List<string> Code = new List<string>();
                if (!Check_Align) Code.Add("0: Align NG");
                if (!Check_Lenght) Code.Add("1: Không đủ chièu dài");
                if (!Check_Width) Code.Add("2: Không đạt chiều rộng");
                if (!Check_Continuous) Code.Add("3: Mất kết nối");
                if (!Check_over) Code.Add("4: Lỗi tràn keo");
                if (!Check_airBubbles) Code.Add("5: Lỗi bọt khí");
                return Code;
            }
        }


        public string[] DATA_VISION
        {
            get
            {
                string[] Log = new string[] { tmp_Time.ToString(), TactTime.ToString(), Label, FileName, Check_Result, Check_Align.ToString(), Check_Lenght.ToString(), Check_Width.ToString(),
                Check_Continuous.ToString(),  Check_over.ToString(), Check_airBubbles.ToString(), Math.Round(Lenght_Fpcb_pixel,4).ToString(), Math.Round(Lenght_Fpcb_mm,4).ToString(),
                Math.Round(Resolution,4).ToString(), Math.Round(Lenght_Glue_pixel,4).ToString(), Math.Round(Lenght_Glue_mm,4).ToString(), Math.Round(Rate_LenghtEpoxy2lenghtFpcb,4).ToString(),
                Math.Round(Width_Max_Glue_pixel,4).ToString(), Math.Round(Width_Max_Epoxy_mm,4).ToString()};
                return Log;
            }
        }

        public string LOG_VISION
        {
            get
            {
                string tmpLog = "";
                foreach (var item in DATA_VISION)
                {
                    tmpLog += item + ",";
                }
                return tmpLog;
            }
        }

        public string[] LOG_RESULT
        {
            get
            {
                if (NG_CODE.Count > 0)
                {
                    string[] tmpLog = new string[] { tmp_Time.ToString(), TactTime.ToString(), Check_Result,NG_CODE[0],
                        Math.Round(Lenght_Fpcb_mm,4).ToString(), Math.Round(Lenght_Glue_mm,4).ToString(), Math.Round(Width_Max_Epoxy_mm,4).ToString()};
                    return tmpLog;
                }
                else
                {
                    string[] tmpLog = new string[] { tmp_Time.ToString(), TactTime.ToString(), Check_Result,"OK",
                        Math.Round(Lenght_Fpcb_mm,4).ToString(), Math.Round(Lenght_Glue_mm,4).ToString(), Math.Round(Width_Max_Epoxy_mm,4).ToString()};
                    return tmpLog;
                }
            }
        }

        //tmp biến tạm
        public string Label = "_";
        public string FileName = "_";
        public string tmp_Time;
        public long TactTime;
    }

    class CODE_RESULT
    {
        public static string OK = "OK";
        public static string NG = "NG";
        public static string DETECT_EPOXY_NG = "DETECT_EPOXY_NG";
        public static string RUN_FAIL = "RUN_FAIL";
    }

    public class clsVision
    {
        public bool IsReady { set; get; }

        public string Folder_Log_Vision;
        //Toltal
        public clsResultVision RESULT_VISION;
        public string OutputResult { set; get; }
        public string OutputLog { set; get; }

        public Dictionary<string, Mat> dic_ImgMat = new Dictionary<string, Mat>();

        public Mat Img_Input = new Mat();
        public Mat Img_Align = new Mat();
        public Mat Img_OutputResult = new Mat();

        SVM svm;


        public clsVision()
        {
            LoadJob();
        }

        public bool LoadModel()
        {
            try
            {
                svm = SVM.Load(clsDefine.PATH_MODEL_SVM);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool LoadJob()
        {
            try
            {
                string _pathData = clsDefine.PATH_APP;
                clsDefine.ReadDefine();
                if (clsDefine.M_CHECK_BUBBLE)
                {
                    IsReady = LoadModel();
                }
                else
                {
                    IsReady = true;
                }
            }
            catch (Exception ex)
            {
                IsReady = false;
            }
            return IsReady;
        }

        Stopwatch TT1 = new Stopwatch(); // Total Time
        public void Run(Mat _imgInput, VsPoint _pt1, VsPoint _pt2, string _pathImg = "")
        {
            // clear data cũ
            this.OutputResult = CODE_RESULT.OK;
            Img_Input = new Mat();
            Img_OutputResult = null; //ảnh Result
            dic_ImgMat.Clear();
            GC.Collect();
            // Khởi tạo data mới
            RESULT_VISION = new clsResultVision();
            //Khởi tạo lưu ảnh gốc
            RESULT_VISION.tmp_Time = DateTime.Now.ToString("HH.mm.ss.fff");
            Folder_Log_Vision = string.Format("{0}\\{1}", clsDefine.PATH_LOG_FOLDER_IMG_DATA_SRC, RESULT_VISION.tmp_Time);
            if (!Directory.Exists(Folder_Log_Vision)) Directory.CreateDirectory(Folder_Log_Vision);
            //lấy thông tin ảnh
            if (_pathImg != "")
            {
                try
                {
                    string directoryPath = Path.GetDirectoryName(_pathImg);
                    string targetFolderName = new DirectoryInfo(directoryPath).Name;
                    RESULT_VISION.Label = targetFolderName;
                    RESULT_VISION.FileName = Path.GetFileName(_pathImg);
                }
                catch (Exception) { }
            }

            //STEP 1: grab image và Warp Image
            Img_Input = WarpImage(_imgInput, _pt1, _pt2);
            if (Img_Input.Empty()) { OutputResult = "IMAGE NG"; } //Kiểm tra ảnh đầu vào != null
            else
            {
                // Step 2: Run 
                TT1.Restart();
                try
                {
                    if (Img_Input.Channels() > 1)
                        Cv2.CvtColor(Img_Input, Img_Input, ColorConversionCodes.RGB2GRAY);

                    dic_ImgMat.Add("Img_Input", Img_Input);
                    RESULT_VISION.Lenght_Fpcb_pixel = Img_Input.Width;  // Kích thước pixel FPCB
                    // STEP2.1 XỬ LÝ ẢNH => BÙ VÙNG BÓNG KEO
                    /*
                      1. thực hiện Close  => làm nhoè ảnh
                      2. Threshold  => lấy vùng trắng (vùng thật trắng > 200)
                           => tập ảnh mới gửi cho vì nền cũng trắng nên bù cả vùng đen của nền //? chưa xử lý được
                            => chỗ này sẽ phải xử lý kiểu khác
                      3. Sau đó And với maskGlueMax để ra được ảnh chỉ có phần bóng keo màu đen còn nền màu trắng 
                      4. Đưa mask vào ảnh input để bù vùng trắng bóng keo thành màu đen
                        ***** vì nền đang trắng k bù được keo nên để xửl ý sau tạm thời bỏ bù keo
                     */

                    OpenCvSharp.Rect roi = new OpenCvSharp.Rect(0, clsDefine.CAL_OFFSET_Y, Img_Input.Width, Img_Input.Height - clsDefine.CAL_OFFSET_Y);
                    Mat outImgCrop = new Mat(Img_Input, roi);
                    Mat kernel_close = Cv2.GetStructuringElement(MorphShapes.Ellipse, new OpenCvSharp.Size(7, 7));
                    //Mat closedGlue = new Mat(); // Thực hiện phép Close
                    //Cv2.MorphologyEx(outImgCrop, closedGlue, MorphTypes.Close, kernel_close);
                    //dic_ImgMat.Add("closedGlue", closedGlue);
                    //Mat imgGlueBin = new Mat();
                    //Cv2.Threshold(closedGlue, imgGlueBin, 230, 255, ThresholdTypes.Binary);
                    //dic_ImgMat.Add("imgGlueBin", imgGlueBin);
                    //Mat maskGlueMax = Mat.Zeros(imgGlueBin.Height, imgGlueBin.Width, MatType.CV_8UC1);
                    //int pixelMaxWidth = (int)(0.49 / RESULT_VISION.Resolution);
                    //Rect rectGlueMax = new Rect(0, 0, maskGlueMax.Width, pixelMaxWidth);
                    //Cv2.Rectangle(maskGlueMax, rectGlueMax, new Scalar(255), -1);
                    //dic_ImgMat.Add("maskGlueMax", maskGlueMax);
                    //Mat mask = new Mat();
                    //Cv2.BitwiseAnd(imgGlueBin, maskGlueMax, mask);
                    //Cv2.BitwiseNot(mask, mask);
                    //dic_ImgMat.Add("mask", mask);
                    //Mat outImageInput = new Mat();
                    //Cv2.BitwiseAnd(outImgCrop, mask, outImageInput);
                    //dic_ImgMat.Add("outImageInput", outImageInput);

                    //STEP2.2 ===========> xử lý bắt blob keo
                    /*
                     //? phần này mai có thể thêm đoạn để kiểm tra kíhc thước keo có bất thường đoạn to đoạn nhỏ bằng cách kiểm tra từng đoạn nếu kíhc thước các đoạn k gần bằng nhau thì cũng tính NG
                     */
                    Mat outImageInput = outImgCrop.Clone();
                    Mat outImgConvert = new Mat();
                    outImageInput.ConvertTo(outImgConvert, outImgCrop.Type(), clsDefine.VI_CONTRAST, clsDefine.VI_BRIGHTNESS);
                    dic_ImgMat.Add("outImgConvert", outImgConvert);
                    Mat invertedImage_1 = new Mat();
                    Cv2.BitwiseNot(outImgConvert, invertedImage_1);
                    Mat closedImage = new Mat(); // Thực hiện phép Close
                    Cv2.MorphologyEx(invertedImage_1, closedImage, MorphTypes.Close, kernel_close);
                    Mat outImgClose = new Mat();
                    Cv2.BitwiseNot(closedImage, outImgClose);
                    dic_ImgMat.Add("outImgClose", outImgClose);
                    Mat outImgThreshold = new Mat();
                    Cv2.Threshold(outImgClose, outImgThreshold, clsDefine.VI_THRESHOLD_DETECT_EPOXY, 255, ThresholdTypes.Binary);
                    Mat invertedImage_2 = new Mat();
                    Cv2.BitwiseNot(outImgThreshold, invertedImage_2);
                    dic_ImgMat.Add("outImgThreshold", invertedImage_2);

                    Mat imgShiftX = ShifX(invertedImage_2);
                    Cv2.MorphologyEx(imgShiftX, imgShiftX, MorphTypes.Close, kernel_close);
                    dic_ImgMat.Add("imgShiftX", imgShiftX);

                    Mat imgShiftY = ShifY(invertedImage_2);
                    Cv2.MorphologyEx(imgShiftY, imgShiftY, MorphTypes.Close, kernel_close);
                    dic_ImgMat.Add("imgShiftY", imgShiftY);

                    Mat imgShift = imgShiftX.BitwiseAnd(imgShiftY);
                    Cv2.MorphologyEx(imgShift, imgShift, MorphTypes.Close, kernel_close);
                    dic_ImgMat.Add("imgShift", imgShift);

                    List<List<Rect>> lstRectBubble = new List<List<Rect>>(); // chứa các vùng bubble nhỏ
                    Mat ConnectImage = new Mat(); //ảnh này dùng để sử dụng cho bước check disconnect
                    Mat blobImage = new Mat();
                    Rect blob = clsToolBlob.Blob(imgShift, outImgConvert, out lstRectBubble, out ConnectImage);
                    if (blob != null)
                    {
                        ConnectImage = new Mat(ConnectImage, blob);
                        ConnectImage = ConnectImage.BitwiseNot();
                        blobImage = new Mat(outImgThreshold, blob);
                        dic_ImgMat.Add("blobImage", blobImage);

                        //Kiểm tra Epoxy có dàn đều (k bị chỗ lớn chỗ bé) lấy spect > 50% chiều dài Fpcb
                        Rect rect_Bottom = new Rect(0, blobImage.Height/2, blobImage.Width, blobImage.Height / 2);
                        Mat blobImage_Bottom = new Mat(blobImage, rect_Bottom);
                        double RateWidth = clsToolBlob.Blob_Bottom(blobImage_Bottom);
                        if (RateWidth < RESULT_VISION.Lenght_Fpcb_pixel/2) //Kiểm tra tỉ lệ chiều dài Epoxy
                        {
                            RESULT_VISION.Lenght_Glue_pixel = RateWidth;
                        }
                        else
                        {
                            RESULT_VISION.Lenght_Glue_pixel = blobImage.Width;
                        }
                  
                        RESULT_VISION.Width_Max_Glue_pixel = blobImage.Height;
                        List<bool> arrContinuous = new List<bool>();
                        List<Rect> arrBubbles = new List<Rect>();
                        List<Rect> lstRectOverEpoxy = new List<Rect>();
                        if ((RESULT_VISION.Check_Lenght == true && RESULT_VISION.Check_Width == true) || clsDefine.M_RUN_SIMULATION_BUBBLE)
                        {
                            //============ check mất kết nối
                            bool checkContinute = true;
                            //arrContinuous = CheckContinuous(ConnectImage, (int)clsDefine.VI_THRESHOLD, ref checkContinute);
                            arrContinuous = CheckConnect(ConnectImage, ref checkContinute);
                            RESULT_VISION.Check_Continuous = checkContinute;

                            //============ check lỗi tràn keo (keo rơi trên FPCB)
                            if (RESULT_VISION.Check_Continuous == true)
                            {
                                int offsetMargin = 5;
                                int pixelWidth_EpoxyMax = (int)(0.5 / clsDefine.CAL_RESOLUTION); //tiêu chuẩn keo dưới 0.5 nên lấy vùng kiểm tra tràn keo > 0.5 mm
                                Rect RectFPCB = new Rect(0, clsDefine.CAL_OFFSET_Y + pixelWidth_EpoxyMax + offsetMargin, Img_Input.Width, Img_Input.Height - (clsDefine.CAL_OFFSET_Y + pixelWidth_EpoxyMax + offsetMargin));
                                Mat FpcbImage = new Mat(Img_Input, RectFPCB);
                                dic_ImgMat.Add("FpcbImage", FpcbImage);
                                lstRectOverEpoxy = clsToolBlob.BlobFPCB(FpcbImage);
                                if (lstRectOverEpoxy.Count > 0)
                                {
                                    RESULT_VISION.Check_over = false;
                                    for (int i = 0; i < lstRectOverEpoxy.Count; i++)
                                    {
                                        int Y = lstRectOverEpoxy[i].Y + clsDefine.CAL_OFFSET_Y + pixelWidth_EpoxyMax + offsetMargin;
                                        Rect RectNew = new Rect(lstRectOverEpoxy[i].X, Y, lstRectOverEpoxy[i].Width, lstRectOverEpoxy[i].Height);
                                        lstRectOverEpoxy[i] = RectNew;
                                    }
                                }
                                else
                                {
                                    RESULT_VISION.Check_over = true;
                                }
                            }

                            //============ check lỗi bọt khí
                            if ((RESULT_VISION.Check_over == true && clsDefine.M_CHECK_BUBBLE) || clsDefine.M_RUN_SIMULATION_BUBBLE)
                            {
                                int countBubble = 0;
                                foreach (var NhomRectBubble in lstRectBubble)
                                {
                                    //Cắt theo đúng kích thước của bubble
                                    int minX = NhomRectBubble.Min(r => r.X);
                                    int minY = 0;
                                    //int minY = NhomRectBubble.Min(r => r.Y) - 10;
                                    if (minX >= 10) minX -= 10;
                                    //if (minY >= 10) minY -= 10;
                                    int Width = NhomRectBubble.Max(r => r.Right) - minX;
                                    int Height = NhomRectBubble.Max(r => r.Bottom);
                                    if (minX + Width < invertedImage_2.Width - 10) Width += 10;
                                    if (minY + Height <= invertedImage_2.Height - 10) Height += 10;
                                    if (minY < 20)
                                    {
                                        Rect RectBubble = new Rect(minX, minY + clsDefine.CAL_OFFSET_Y, Width, Height);
                                        arrBubbles.Add(RectBubble);
                                        Mat imgBubble = new Mat(Img_Input, RectBubble);
                                        dic_ImgMat.Add("imgBubble_" + countBubble, imgBubble);
                                        countBubble++;
                                        if (AICheck(imgBubble) == false)
                                        {
                                            RESULT_VISION.Check_airBubbles = false;
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (clsDefine.M_GRAPHIC)
                        {
                            Img_OutputResult = new Mat();
                            Cv2.CvtColor(Img_Input, Img_OutputResult, ColorConversionCodes.GRAY2RGB);
                            Rect RectWarp = new Rect(blob.X, blob.Y + clsDefine.CAL_OFFSET_Y, blob.Width, blob.Height);
                            Cv2.Rectangle(Img_OutputResult, RectWarp, Scalar.Blue, 2);
                            //Graphic mất kết nối
                            bool modeGraphic_Continute = true;
                            if (RESULT_VISION.Check_Continuous == false && modeGraphic_Continute)
                            {
                                bool invertContinuous = false;
                                for (int i = 0; i < arrContinuous.Count; i++)
                                {
                                    if (arrContinuous[i] == false && invertContinuous == false)
                                    {
                                        invertContinuous = true;
                                        Cv2.Line(Img_OutputResult, new OpenCvSharp.Point(blob.X + i, blob.Y + clsDefine.CAL_OFFSET_Y), new OpenCvSharp.Point(blob.X + i, blob.Y + clsDefine.CAL_OFFSET_Y + blob.Height - 1), Scalar.Red, 1);
                                    }
                                    else if (arrContinuous[i] == true && invertContinuous == true)
                                    {
                                        invertContinuous = false;
                                        Cv2.Line(Img_OutputResult, new OpenCvSharp.Point(blob.X + i, blob.Y + clsDefine.CAL_OFFSET_Y), new OpenCvSharp.Point(blob.X + i, blob.Y + clsDefine.CAL_OFFSET_Y + blob.Height - 1), Scalar.Green, 1);
                                    }
                                }
                            }
                            //Graphic Over Epoxy
                            bool modeGraphic_Over = true;
                            if (RESULT_VISION.Check_over == false && modeGraphic_Over)
                            {
                                foreach (var rectOver in lstRectOverEpoxy)
                                {
                                    Cv2.Rectangle(Img_OutputResult, rectOver, Scalar.Red);
                                }
                            }
                            //Graphic crop bubble
                            bool modeGraphic_Bubble = false;
                            if (modeGraphic_Bubble == true)
                            {
                                int countBubble = 0;
                                int CountMau = 0;
                                Scalar[] nhomMau = new Scalar[] { Scalar.Red, Scalar.Blue, Scalar.Yellow, Scalar.YellowGreen, Scalar.Green, Scalar.Orange, Scalar.Turquoise, Scalar.Violet, Scalar.BlueViolet, Scalar.Tomato };
                                int caseCrop = 1;
                                if (caseCrop == 1)
                                {
                                    foreach (var NhomRectBubble in lstRectBubble)
                                    {
                                        foreach (var rectBubble in NhomRectBubble)
                                        {
                                            Rect rectBubbleOrigin = new Rect(rectBubble.X, rectBubble.Y + clsDefine.CAL_OFFSET_Y, rectBubble.Width, rectBubble.Height);
                                            Cv2.Rectangle(Img_OutputResult, rectBubbleOrigin, nhomMau[CountMau]);
                                            countBubble++;
                                        }
                                        CountMau++;
                                        if (CountMau > nhomMau.Length) CountMau = 0;
                                    }
                                }
                                else if (caseCrop == 2)
                                {
                                    foreach (var rectBubble in arrBubbles)
                                    {
                                        Cv2.Rectangle(Img_OutputResult, rectBubble, nhomMau[CountMau]);
                                        CountMau++;
                                        if (CountMau > nhomMau.Length) CountMau = 0;
                                    }
                                }

                            }
                        }
                        else
                        {
                            Img_OutputResult = new Mat();
                            Cv2.CvtColor(Img_Input, Img_OutputResult, ColorConversionCodes.GRAY2RGB);
                            Rect RectWarp = new Rect(blob.X, blob.Y + clsDefine.CAL_OFFSET_Y, blob.Width, blob.Height);
                            Cv2.Rectangle(Img_OutputResult, RectWarp, Scalar.Blue, 2);
                        }
                    }
                    else
                    {
                        OutputResult = CODE_RESULT.DETECT_EPOXY_NG;
                    }
                }
                catch (Exception ex)
                {
                    OutputResult = CODE_RESULT.RUN_FAIL;
                    Console.WriteLine("clsVisionCv2", "Run()", ex.ToString());
                }
                TT1.Stop();
            }
            // STEP 3: Trả kết quả ảnh - lưu log hình ảnh
            if (RESULT_VISION.NG_CODE.Count < 1 && OutputResult == CODE_RESULT.OK)
            {
                RESULT_VISION.Check_Result = CODE_RESULT.OK;
            }
            else
            {
                OutputResult = CODE_RESULT.NG;
                RESULT_VISION.Check_Result = CODE_RESULT.NG;
            }

            if (Img_OutputResult == null) //Kiểm tra nếu trường hợp NG không có ảnh
            {
                Img_OutputResult = Img_Input;
            }

            RESULT_VISION.TactTime = TT1.ElapsedMilliseconds;
            LogVision(clsDefine.PATH_LOG_FOLDER_IMG_DATA_DATE, RESULT_VISION.LOG_VISION);
            PhanLoaiImage(_pathImg, RESULT_VISION);
        }

        List<bool> CheckConnect(Mat _imgsrc, ref bool _isContinuous)
        {   
            //clsVision.ShowImage("_imgsrc", _imgsrc, true, 1);
            _isContinuous = true;
            var indexer = _imgsrc.GetGenericIndexer<byte>(); //đọc về giá trị của mỗi điểm ảnh
            int spectThreshold = (int)(_imgsrc.Height * 0.2); //50% độ rộng của Epoxy Max
            int spectUp = (int)(_imgsrc.Height * 0.5); //50% độ rộng của Epoxy Max
            int[] idxEpoxy = new int[_imgsrc.Width];
            for (int x = 0; x < _imgsrc.Width; x++)
            {
                Mat colMat = _imgsrc.Col(x);
                byte[] columnData = new byte[_imgsrc.Height];
                _imgsrc.Col(x).GetArray(out columnData); // Chuyển cột thành mảng byte
                idxEpoxy[x] = Array.FindLastIndex(columnData, v => v == 0); // Tìm y lớn nhất có pixel = 0
            }

            int Peak = 0;
            bool UppTrent = false;
            int countUppTrent = 0; //Count Down và up để tránh điểm noise => tạo delay
            int countDownTrent = 0;
            int SpectDownUpTrend = (int)(_imgsrc.Width * 0.02); //down và upp phải > _imgsrc.Width*0.03 (3%) thì mới tính là thay đổi trend //k cần lấy mm vì resolution khi calib k chuẩn
            //int Trough = 0;
            bool DownTrent = false;
            bool[] boolArray = Enumerable.Repeat(true, idxEpoxy.Length).ToArray();
            int start = (int)(idxEpoxy.Length * 0.05); // loại bỏ 5% đầu và đuôi của Keo
            int end = idxEpoxy.Length - start;
            for (int i = start; i < end; i++)
            {
                if (idxEpoxy[i] >= spectThreshold)
                {
                    countDownTrent = 0;
                    if (idxEpoxy[i] >= Peak)
                    {
                        countUppTrent++;
                        Peak = idxEpoxy[i];
                        UppTrent = true;
                        if (DownTrent == true && countUppTrent >= SpectDownUpTrend) //Nếu upp trend mà giữ > 0.02mm thì sẽ bj disconnect
                        {
                            _isContinuous = false; ////////? kết quả NG
                            DownTrent = false;
                        }
                    }
                }
                else
                {
                    if (countDownTrent < SpectDownUpTrend)
                    {
                        countDownTrent++;
                    }
                    else
                    {
                        if (UppTrent == true)
                        {
                            countUppTrent = 0;
                            boolArray[i] = false;
                            if (idxEpoxy[i] < Peak)
                            {
                                Peak = idxEpoxy[i];
                            }
                            else
                            {
                                DownTrent = true;
                            }
                        }
                    }
                }
            }
            clsSupport.SaveArrayToCsv(boolArray, "D:\\Array.csv");
            return boolArray.ToList();
        }

        public void LogVision(string _path, string _result)
        {
            if (!File.Exists(_path + "\\DATA LOG.csv"))
            {
                string tmp = "";
                foreach (var item in clsDefine.HEAD_TILE_RESULT)
                {
                    tmp += item + ",";
                }
                clsLog.WriteLog_CSV(_path + "\\DATA LOG.csv", tmp);
            }

            clsLog.WriteLog_CSV(_path + "\\DATA LOG.csv", _result);
        }

        public void PhanLoaiImage(string _pathImage, clsResultVision _result)
        {
            string fileName = Path.GetFileName(_pathImage);
            string folder = "";

            if (_result.Check_Result == "OK")
            {
                folder = clsDefine.PATH_LOG_FOLDER_IMG_DATA_WARP + "\\OK";
            }
            else if (_result.Check_Lenght == false)
            {
                folder = clsDefine.PATH_LOG_FOLDER_IMG_DATA_WARP + "\\NG1_LENGHT";
            }
            else if (_result.Check_Width == false)
            {
                folder = clsDefine.PATH_LOG_FOLDER_IMG_DATA_WARP + "\\NG2_WIDTH";
            }
            else if (_result.Check_Continuous == false)
            {
                folder = clsDefine.PATH_LOG_FOLDER_IMG_DATA_WARP + "\\NG3_CONTINUOUS";
            }
            else if (_result.Check_over == false)
            {
                folder = clsDefine.PATH_LOG_FOLDER_IMG_DATA_WARP + "\\NG4_OVER";
            }
            else if (_result.Check_airBubbles == false)
            {
                folder = clsDefine.PATH_LOG_FOLDER_IMG_DATA_WARP + "\\NG5_BUBBLE";
            }
            else
            {
                folder = clsDefine.PATH_LOG_FOLDER_IMG_DATA_WARP + "\\NG_NO_LABEL";
            }

            if (Directory.Exists(folder) == false) ;
            {
                Directory.CreateDirectory(folder);
            }


            if (File.Exists(folder + "\\" + fileName) == false)
            {
                File.Copy(_pathImage, folder + "\\" + fileName);
            }

            Thread.Sleep(100);
        }

        #region CHECK SVM
        public string[] LABEL_SVM = new string[] { "OK", "NG" };
        public bool AICheck(Mat img)
        {
            try
            {
                if (svm == null) return false;
                int result = Predict_SVM(img);
                return LABEL_SVM[result] == "OK";
            }
            catch (Exception)
            {
                return false;
            }
        }

        private int Predict_SVM(Mat _imgAI)
        {
            int SIZE_W = 68; //36
            int SIZE_H = 128; //60
            Mat tmp = new Mat();
            Cv2.Resize(_imgAI.Clone(), tmp, new OpenCvSharp.Size(SIZE_W, SIZE_H));
            var mean = Cv2.Mean(tmp).Val0;
            Cv2.Threshold(tmp, tmp, mean, 255, ThresholdTypes.Binary);
            Cv2.Canny(tmp, tmp, mean, mean + 20);
            //trích đặc trưng
            float[] data_feture;
            HOGDescriptor hog = new HOGDescriptor();
            data_feture = hog.Compute(tmp);
            Mat testData = Mat.FromPixelData(1, data_feture.Length, MatType.CV_32F, data_feture);
            Mat rs = new Mat();
            svm.Predict(testData, rs);
            var rLb = (int)rs.At<float>(0, 0);
            return rLb;
        }
        #endregion

        public Mat WarpImage(Mat _img, VsPoint _pt1, VsPoint _pt2)
        {
            Mat ImgWarp = new Mat();
            try
            {
                //Warp Image
                var A = (x: _pt2.X, y: _pt2.Y);
                var B = (x: _pt1.X, y: _pt1.Y);
                var result = FindParallelSegment(A, B);
                // Tọa độ 4 điểm nguồn (trên hình ảnh gốc)
                Point2f[] srcPoints = {
                            new Point2f((float)B.x, (float)B.y), // Điểm trên trái
                            new Point2f((float)A.x, (float)A.y), // Điểm trên phải
                            new Point2f((float)result.A1.x, (float)result.A1.y), // Điểm dưới phải
                            new Point2f((float)result.B1.x, (float)result.B1.y)   // Điểm dưới trái
                        };
                // Tọa độ 4 điểm đích (hình chữ nhật phẳng)
                double dw = (A.x - B.x) * (A.x - B.x) + (A.y - B.y) * (A.y - B.y);
                double dh = (result.A1.x - A.x) * (result.A1.x - A.x) + (result.A1.y - A.y) * (result.A1.y - A.y);
                int width = (int)Math.Sqrt(dw);  // Chiều rộng hình chữ nhật đích
                int height = (int)Math.Sqrt(dh); // Chiều cao hình chữ nhật đích
                Point2f[] dstPoints = {
                            new Point2f(0, 0),               // Góc trên trái
                            new Point2f(width - 1, 0),       // Góc trên phải
                            new Point2f(width - 1, height - 1), // Góc dưới phải
                            new Point2f(0, height - 1)      // Góc dưới trái
                        };
                Mat perspectiveMatrix = Cv2.GetPerspectiveTransform(srcPoints, dstPoints);
                Cv2.WarpPerspective(_img, ImgWarp, perspectiveMatrix, new OpenCvSharp.Size(width, height));
                OpenCvSharp.Rect rectWarp = new OpenCvSharp.Rect((int)(width * 0.01), 0, width - (int)((width * 0.01) * 2), height);
                ImgWarp = new Mat(ImgWarp, rectWarp);
            }
            catch (Exception ex)
            {
                clsLog.WriteLog_txt(clsDefine.PATH_LOG_ROOT_HISTORY_APP, "Lỗi Warp Image:" + ex.ToString());
            }
            return ImgWarp;
        }

        ((double x, double y) A1, (double x, double y) B1) FindParallelSegment((double x, double y) A, (double x, double y) B)
        {
            // Vectơ chỉ phương của AB
            var u = (x: B.x - A.x, y: B.y - A.y);
            // Chọn điểm A' sao cho AA' vuông góc với AB
            // Dịch điểm A' một khoảng cố định theo phương vuông góc với AB
            var v = (x: -u.y, y: u.x); // Vectơ vuông góc với u
            double scale = -0.45; // Khoảng cách từ A đến A'
            var A1 = (x: A.x + scale * v.x, y: A.y + scale * v.y);
            // Hệ số k để tính B' (k = 1 cho đơn giản)
            double k = 1;
            // Tọa độ điểm B'
            var B1 = (x: A1.x + k * u.x, y: A1.y + k * u.y);
            return (A1, B1);
        }
   

        public static void ShowImage(string _name, Mat _imgShow, bool _wait = true, double _scale = 0.3)
        {
            if (_imgShow != null && _imgShow.Empty() == false)
            {
                Mat imgShow = _imgShow.Clone();
                Cv2.Resize(imgShow, imgShow, new Size(imgShow.Width * _scale, imgShow.Height * _scale));
                Cv2.ImShow(_name, imgShow);
                if (_wait) Cv2.WaitKey();
            }
        }

        #region Loại bỏ bóng keo
        public Mat ShifX(Mat _img)
        {
            if (_img == null || _img.Empty()) return _img;

            float shifX = 1f;
            Mat translationMatrix_Right = Mat.Eye(2, 3, MatType.CV_32F);
            translationMatrix_Right.Set(0, 2, shifX); // Dịch theo trục +X 
            translationMatrix_Right.Set(1, 2, 0);

            Mat translationMatrix_Left = Mat.Eye(2, 3, MatType.CV_32F);
            translationMatrix_Left.Set(0, 2, -shifX); // Dịch theo trục -X 
            translationMatrix_Left.Set(1, 2, 0);

            Mat imgShift_Right = _img.Clone();
            Mat imgShift_Left = _img.Clone();

            for (int i = 0; i < _img.Width * 0.2; i++)
            {
                Mat dst_Right = new Mat();
                Cv2.WarpAffine(imgShift_Right, dst_Right, translationMatrix_Right, _img.Size());
                imgShift_Right = imgShift_Right.BitwiseOr(dst_Right);

                Mat dst_Left = new Mat();
                Cv2.WarpAffine(imgShift_Left, dst_Left, translationMatrix_Left, _img.Size());
                imgShift_Left = imgShift_Left.BitwiseOr(dst_Left);

                //Console.WriteLine("Matrix Left:");
                //Console.WriteLine(translationMatrix_Left.Dump());
                //Console.WriteLine("Matrix Right:");
                //Console.WriteLine(translationMatrix_Right.Dump());
            }

            //clsVision.ShowImage("imgShift_Right", imgShift_Right, true, 1);
            //clsVision.ShowImage("imgShift_Left", imgShift_Left, true, 1);

            imgShift_Right = imgShift_Right.BitwiseAnd(imgShift_Left);
            return imgShift_Right;
        }

        public Mat ShifY(Mat _img)
        {
            if (_img == null || _img.Empty()) return _img;

            float shifY = 1f;
            Mat translationMatrix_Right = Mat.Eye(2, 3, MatType.CV_32F);
            translationMatrix_Right.Set(0, 2, 0);
            translationMatrix_Right.Set(1, 2, shifY);  // Dịch theo trục +Y

            Mat translationMatrix_Left = Mat.Eye(2, 3, MatType.CV_32F);
            translationMatrix_Left.Set(0, 2, 0); 
            translationMatrix_Left.Set(1, 2, -shifY); // Dịch theo trục -Y

            //Console.WriteLine("Matrix Left:");
            //Console.WriteLine(translationMatrix_Left.Dump());
            //Console.WriteLine("Matrix Right:");
            //Console.WriteLine(translationMatrix_Right.Dump());

            Mat imgShift_Right = _img.Clone();
            Mat imgShift_Left = _img.Clone();

            for (int i = 0; i < _img.Height * 0.2; i++)
            {
                Mat dst_Right = new Mat();
                Cv2.WarpAffine(imgShift_Right, dst_Right, translationMatrix_Right, _img.Size());
                imgShift_Right = imgShift_Right.BitwiseOr(dst_Right);

                Mat dst_Left = new Mat();
                Cv2.WarpAffine(imgShift_Left, dst_Left, translationMatrix_Left, _img.Size());
                imgShift_Left = imgShift_Left.BitwiseOr(dst_Left);        
            }

            //clsVision.ShowImage("imgShift_Right", imgShift_Right, false, 1);
            //clsVision.ShowImage("imgShift_Left", imgShift_Left, true, 1);

            imgShift_Right = imgShift_Right.BitwiseAnd(imgShift_Left);
            return imgShift_Right;
        }

        #endregion Loại bỏ bóng keo




    }
}
