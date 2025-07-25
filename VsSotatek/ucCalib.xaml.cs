using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.IO;
using ViSolution;
using ViSolution.Tool;
using OpenCvSharp;
using ViSolution.Object;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using ViSolution.Display;
using System.Windows.Forms.Integration;

namespace VsSotatek
{
    /// <summary>
    /// Interaction logic for ucCalib.xaml
    /// </summary>
    public partial class ucCalib : System.Windows.Controls.UserControl
    {
        VMucCalib ViewModel;
        string PathImage;
        VsJob job;
        VsInputTool inputTool;
        clsVision job_Inspect;
        List<double> lstOffset = new List<double>();
        List<double> lstResolution = new List<double>();
        double Resolution;
        double LengthFpcb_mm;
        double LengthFpcb_Pixel;
        List<DataCalib> lstDataCalib = new List<DataCalib>();

        public ucCalib()
        {
            InitializeComponent();
            try
            {
                job = (new VsSerializer()).LoadJob(clsDefine.PATH_JOB);
                inputTool = job.ToolList[0] as VsInputTool;
                job_Inspect = new clsVision();
            }
            catch (Exception ex)
            {
                job = null;
                System.Windows.Forms.MessageBox.Show("lỗi Load Tool: " + ex.ToString());
            }

            ViewModel = new VMucCalib();
            this.DataContext = ViewModel;

            ViewModel.SetOffsetY = clsDefine.CAL_OFFSET_Y;

            //Initial Display Align
            ViewModel.DisplayAlign = new VsDisplayEdit();
            ViewModel.DisplayAlign.Dock = System.Windows.Forms.DockStyle.Fill;
            WindowsFormsHost hostAlign = new WindowsFormsHost();
            hostAlign.Child = ViewModel.DisplayAlign;
            ViewAlign.Child = hostAlign;

            //Initial Display Offset
            ViewModel.DisplayOffset = new VsDisplayEdit();
            ViewModel.DisplayOffset.Dock = System.Windows.Forms.DockStyle.Fill;
            WindowsFormsHost hostResult = new WindowsFormsHost();
            hostResult.Child = ViewModel.DisplayOffset;
            viewResult.Child = hostResult;
        }

        private void btnLoadImg_Click(object sender, RoutedEventArgs e)
        {
            using (OpenFileDialog openfile = new OpenFileDialog())
            {
                openfile.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                openfile.Title = "Chọn một file ảnh";
                if (openfile.ShowDialog() == DialogResult.OK)
                {
                    PathImage = openfile.FileName;
                }
            }
        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            if (job != null && job_Inspect != null && File.Exists(PathImage))
            {
                Mat img = new Mat(PathImage);
                inputTool.InputImage.Mat = img;
                job.Run();
                List<VsPoint> lstPoint = new List<VsPoint>();
                foreach (var tool in job.ToolList)
                {
                    if (tool is VsIntersectLineTool)
                    {
                        VsIntersectLineTool toolInter = tool as VsIntersectLineTool;
                        VsPoint point = toolInter.IntersectPoint;
                        lstPoint.Add(point);
                    }
                }

                if (lstPoint.Count != 2)
                {
                    System.Windows.Forms.MessageBox.Show("Run Lỗi Point != 2");
                    return;
                }
                else
                {
                    Mat imgWarp = job_Inspect.WarpImage(img, lstPoint[0], lstPoint[1]);
                    LengthFpcb_mm = clsDefine.MASTER_LENGHT_FPCB_MM;
                    LengthFpcb_Pixel = imgWarp.Width;
                    Resolution = LengthFpcb_mm / LengthFpcb_Pixel;

                    //Image và Draw Item
                    Dictionary<int, VsImageInfor> ResultImageList = new Dictionary<int, VsImageInfor>();
                    VsImageInfor inputInfor = new VsImageInfor("Image Input");
                    inputInfor.Image = inputTool.OutputImage;
                    ResultImageList.Add(0, inputInfor);
                    ViewModel.UpdateDisplayAlign_Code.Insert(0, new Action(() =>
                    {
                        ViewModel.DisplayAlign.CustomImageList = ResultImageList;
                        ViewModel.DisplayAlign.RefreshImage();
                    }));

                    //Check Tool Align
                    bool isDraw = true;
                    foreach (VsBaseTool toolbase in job.ToolList)
                    {
                        if (!toolbase.lastRunSuccess || !isDraw) continue;
                        if (toolbase is VsTemplateTool)
                        {
                            VsTemplateTool tool = (VsTemplateTool)toolbase;
                            Point2d TMcenter = new Point2d(tool.TranslateX, tool.TranslateY);
                            if (double.IsNaN(TMcenter.X) || double.IsNaN(TMcenter.Y))
                                TMcenter = new Point2d(tool.failResultBox.CP.X, tool.failResultBox.CP.Y - 500);
                            Point2d[] resultBox = tool.ResultBox;
                            System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.GreenYellow, 2f);
                            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDot;

                            ViewModel.UpdateDisplayAlign_Code.Add(new Action(() =>
                            {
                                ViewModel.DisplayAlign.CustomImageList[0].drawingFunc +=
                                (display) =>
                                {
                                    display.DrawPolyLines(pen, resultBox);
                                };
                            }));
                        }
                        else if (toolbase is VsIntersectLineTool)
                        {
                            VsIntersectLineTool intersectTool = (VsIntersectLineTool)toolbase;
                            if (intersectTool.IntersectPoint == null) continue;
                            Point2d intersectpoint = VsFunc.Map2D_Fix2Img(intersectTool.IntersectPoint.Point2d, intersectTool.IntersectPoint.TransformMat);
                            ViewModel.UpdateDisplayAlign_Code.Add(new Action(() =>
                            {
                                ViewModel.DisplayAlign.CustomImageList[0].drawingFunc +=
                                (display) =>
                                {
                                    System.Drawing.Pen CyanPen = new System.Drawing.Pen(System.Drawing.Color.Cyan, 1);
                                    System.Drawing.Pen RedPen = new System.Drawing.Pen(System.Drawing.Color.Red, 3);
                                    display.DrawLine(CyanPen, intersectTool.LineA.SP, intersectTool.LineA.EP);
                                    display.DrawLine(CyanPen, intersectTool.LineB.SP, intersectTool.LineB.EP);
                                };
                            }));
                        }
                    }

                    //Draw Offset
                    Dictionary<int, VsImageInfor> OffsetImageList = new Dictionary<int, VsImageInfor>();
                    VsImageInfor offsetInfor = new VsImageInfor("Image Offset");
                    offsetInfor.Image = new VsImage(imgWarp);
                    OffsetImageList.Add(0, offsetInfor);
                    ViewModel.UpdateDisplayOffet_Code.Insert(0, new Action(() =>
                    {
                        ViewModel.DisplayOffset.CustomImageList = OffsetImageList;
                        ViewModel.DisplayOffset.RefreshImage();
                        ViewModel.DisplayOffset.CustomImageList[0].drawingFunc +=
                              (display) =>
                              {
                                  System.Drawing.Pen CyanPen = new System.Drawing.Pen(System.Drawing.Color.Cyan, 1);
                                  PointF pt1 = new PointF(0, (float)ViewModel.SetOffsetY);
                                  PointF pt2 = new PointF(imgWarp.Width, (float)ViewModel.SetOffsetY);
                                  display.DrawLine(CyanPen, pt1, pt2);
                              };
                    }));
                }

                ViewModel.UpdateDisplay();
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Hãy kiểm tra lại job và ảnh đầu vào");
            }
        }


        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            DataCalib newData = new DataCalib(lstDataCalib.Count , LengthFpcb_mm, LengthFpcb_Pixel, ViewModel.SetOffsetY, Resolution);
            lstDataCalib.Add(newData);

            lstOffset.Add(newData.OffsetY);
            lstResolution.Add(newData.Resolution);

            ViewModel.AvgOffsetY = lstOffset.Average();
            ViewModel.AvgResolution = lstResolution.Average();

            ViewModel.AppendDataCalib(newData);
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lstDataCalib.Count >0 && lstOffset.Count > 0 && lstResolution.Count > 0)
                {
                    lstOffset.RemoveAt(lstOffset.Count - 1);
                    lstResolution.RemoveAt(lstResolution.Count - 1);
                    lstDataCalib.RemoveAt(lstDataCalib.Count - 1);
                    ViewModel.RemoveDataCalib();
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Lỗi cập nhập Data: " + ex.ToString());
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            clsDefine.CAL_RESOLUTION = ViewModel.AvgResolution;
            clsDefine.CAL_OFFSET_Y = (int)ViewModel.AvgOffsetY;
            clsDefine.WriteDefine();
            System.Windows.Forms.MessageBox.Show("Save Data Calib thành công");
        }

        private void txtOffsetResolution_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (lstResolution.Count>0)
            {
                ViewModel.AvgResolution = lstResolution.Average();
            }     
        }
    }

    public class VMucCalib : ViewModelBase
    {
        public ObservableCollection<DataCalib> DataCalibList { get; set; }
        Dispatcher dsp = (new UIElement()).Dispatcher;
        public VMucCalib()
        {
            DataCalibList = new ObservableCollection<DataCalib>();
        }

        double setOffsetResolution;
        public double SetOffsetResolution
        {
            get
            {
                return setOffsetResolution;
            }
            set
            {
                if (setOffsetResolution == value) return;
                setOffsetResolution = value;
                NotifyPropertyChanged("SetOffsetResolution");
            }
        }

        double setOffsetY;
        public double SetOffsetY
        {
            get
            {
                return setOffsetY;
            }
            set
            {
                if (setOffsetY == value) return;
                setOffsetY = value;
                NotifyPropertyChanged("SetOffsetY");
            }
        }

        double avgOffsetY;
        public double AvgOffsetY
        {
            get
            {
                return avgOffsetY;
            }
            set
            {
                if (avgOffsetY == value) return;
                avgOffsetY = Math.Round(value, 8);
                NotifyPropertyChanged("AvgOffsetY");
            }
        }

        double avgResolution;
        public double AvgResolution
        {
            get
            {
                return avgResolution;
            }
            set
            {
                if (avgResolution == value) return;
                avgResolution = Math.Round(value, 8) + SetOffsetResolution;
                NotifyPropertyChanged("AvgResolution");
            }
        }

        public void AppendDataCalib(DataCalib _data)
        {
            dsp.BeginInvoke(new Action(() =>
            {
                DataCalibList.Insert(0, _data);
            }));
        }

        public void RemoveDataCalib()
        {
            dsp.BeginInvoke(new Action(() =>
            {
                DataCalibList.RemoveAt(0);
            }));
        }


        public VsDisplayEdit DisplayAlign;
        public List<Action> UpdateDisplayAlign_Code = new List<Action>();
        public VsDisplayEdit DisplayOffset;
        public List<Action> UpdateDisplayOffet_Code = new List<Action>();
        object Updatelock = new object();
        public void UpdateDisplay()
        {
            lock (Updatelock)
            {
                dsp.Invoke(new Action(() =>
                {
                    foreach (Action action in UpdateDisplayAlign_Code)
                    {
                        try
                        {
                            action.Invoke();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                            continue;
                        }
                    }
                    UpdateDisplayAlign_Code.Clear();
                    //int i = DisplayAlign.imageListComboBox.SelectedIndex;
                    VsImageInfor imageInfo = (VsImageInfor)DisplayAlign.imageListComboBox.SelectedItem;
                    if (imageInfo != null && imageInfo.drawingFunc != null && DisplayAlign.Display.Image != null)
                        imageInfo.drawingFunc(DisplayAlign.Display);

                    foreach (Action action in UpdateDisplayOffet_Code)
                    {
                        try
                        {
                            action.Invoke();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                            continue;
                        }
                    }
                    UpdateDisplayOffet_Code.Clear();
                    VsImageInfor imageInfo_offset = (VsImageInfor)DisplayOffset.imageListComboBox.SelectedItem;
                    if (imageInfo_offset != null && imageInfo_offset.drawingFunc != null && DisplayOffset.Display.Image != null)
                        imageInfo_offset.drawingFunc(DisplayOffset.Display);
                }));
            }
        }

    }

    public class DataCalib
    {
        public DataCalib(int stt, double lengthFPCB_mm, double lengthFCB_pixel, double offsetY, double resolution)
        {
            STT = stt;
            LengthFPCB_mm = lengthFPCB_mm;
            LengthFCB_pixel = lengthFCB_pixel;
            OffsetY = offsetY;
            Resolution = resolution;
        }

        int stt;
        public int STT
        {
            get { return stt; }
            set
            {
                if (stt == value) return;
                stt = value;
            }
        }

        double lengthFPCB_mm;
        public double LengthFPCB_mm
        {
            get { return lengthFPCB_mm; }
            set
            {
                if (lengthFPCB_mm == value) return;
                lengthFPCB_mm = value;
            }
        }

        double lengthFCB_pixel;
        public double LengthFCB_pixel
        {
            get { return lengthFCB_pixel; }
            set
            {
                if (lengthFCB_pixel == value) return;
                lengthFCB_pixel = value;
            }
        }

        double offsetY;
        public double OffsetY
        {
            get { return offsetY; }
            set
            {
                if (offsetY == value) return;
                offsetY = value;
            }
        }

        double resolution;
        public double Resolution
        {
            get { return resolution; }
            set
            {
                if (resolution == value) return;
                resolution = value;
            }
        }
    }
}
