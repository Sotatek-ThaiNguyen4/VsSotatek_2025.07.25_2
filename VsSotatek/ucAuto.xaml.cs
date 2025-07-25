using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ViSolution;
using ViSolution.Display;
using ViSolution.Object;
using ViSolution.Tool;
using System.Windows.Forms.Integration;
using System.Windows.Forms;
using System.IO;

namespace VsSotatek
{
    /// <summary>
    /// Interaction logic for ucAuto.xaml
    /// </summary>
    public partial class ucAuto : System.Windows.Controls.UserControl
    {
        VMucAuto ViewModel;
        public Thread_LG thrLG;
        string PathFile_Manual;
        public ucAuto()
        {
            InitializeComponent();
            ViewModel = new VMucAuto();
            this.DataContext = ViewModel;

            //Initial Display Align
            ViewModel.DisplayAlign = new VsDisplayEdit();
            ViewModel.DisplayAlign.Dock = System.Windows.Forms.DockStyle.Fill;
            WindowsFormsHost hostAlign = new WindowsFormsHost();
            hostAlign.Child = ViewModel.DisplayAlign;
            ViewAlign.Child = hostAlign;

            //Initial Display Inspect
            ViewModel.DisplayInspect = new VsDisplayEdit();
            ViewModel.DisplayInspect.Dock = System.Windows.Forms.DockStyle.Fill;
            WindowsFormsHost hostResult = new WindowsFormsHost();
            hostResult.Child = ViewModel.DisplayInspect;
            viewResult.Child = hostResult;

            thrLG = new Thread_LG(clsDefine.PATH_JOB);
            thrLG.ViewModel = ViewModel;
            AutoThreadStart();           
        }

        private void AutoThreadStart()
        {
            ViewModel.AppendSequenceLog("Thread Start");
            thrLG.ThreadStart();
            thrLG.mThreadFlag = true;
        }
        public void AutoThreadStop()
        {
            ViewModel.AppendSequenceLog("Thread Stop");
            thrLG.mThreadFlag = false;
            thrLG.ThreadStop();
        }


        public static void DisposeAutoThread()
        {
           
        }

        private void btnManual_Click(object sender, RoutedEventArgs e)
        {
            thrLG.Path_File = PathFile_Manual;
            thrLG.StartVision = true;
        }

        private void btnCheckFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openfile = new OpenFileDialog();
            if (openfile.ShowDialog() == DialogResult.OK)
            {
                ViewModel.AppendSequenceLog("Check File");
                PathFile_Manual = thrLG.Path_File = openfile.FileName;
                thrLG.StartVision = true;
            }
        }

        private void btnCheckFolder_Click(object sender, RoutedEventArgs e)
        {
            List<string> lst_imagePathIn = new List<string>();
            using (FolderBrowserDialog folderBrowser = new FolderBrowserDialog())
            {
                if (folderBrowser.ShowDialog() == DialogResult.OK)
                {
                    string[] paths = Directory.GetFiles(folderBrowser.SelectedPath, "*", SearchOption.AllDirectories);
                    if (paths == null || paths.Length == 0) return;
                    string pathFolderImg_IN = folderBrowser.SelectedPath;
                    for (int i = 0; i < paths.Length; i++)
                    {
                        lst_imagePathIn.Add(paths[i]);
                    }
                }
            }

            thrLG.Path_Folder = lst_imagePathIn;
            ViewModel.AppendSequenceLog("Check Folder");
            thrLG.StartVision = true;
        }
    }

    public class VMucAuto : ViewModelBase
    {
        bool _isConnect;
        public bool IsConnect
        {
            set
            {
                if (_isConnect != value)
                {
                    _isConnect = value;
                }
            }
            get { return _isConnect; }
        }
        public SolidColorBrush ConnectStatus { get { return IsConnect ? Brushes.YellowGreen : Brushes.DimGray; } }

        public ObservableCollection<BaseLog> SequenceLogList { get; set; }
        public ObservableCollection<ResultLog> ResultLogList { get; set; }

        Dispatcher dsp = (new UIElement()).Dispatcher;

        public VMucAuto()
        {
            SequenceLogList = new ObservableCollection<BaseLog>();
            ResultLogList = new ObservableCollection<ResultLog>();
        }
        public void AppendSequenceLog(string log)
        {
            dsp.BeginInvoke(new Action(() => { SequenceLogList.Add(new BaseLog(DateTime.Now.ToString("HH:mm:ss"), log)); }));
        }

        public void AppendResultLog(List<ResultLog> lstLog)
        {
            dsp.BeginInvoke(new Action(() =>
            {             
                foreach (var item in lstLog)
                {
                    ResultLogList.Insert(0,item);
                    if (ResultLogList.Count > 100)
                    {
                        ResultLogList.RemoveAt(ResultLogList.Count - 1);
                    }
                }   
            }));     
        }

        VsPoint point1;
        public VsPoint Point1
        {
            get { return point1; }
            set
            {
                if (point1 == value) return;
                point1 = value;
                NotifyPropertyChanged("Point1");
            }
        }

        VsPoint point2;
        public VsPoint Point2
        {
            get { return point2; }
            set
            {
                if (point2 == value) return;
                point2 = value;
                NotifyPropertyChanged("Point2");
            }
        }

        string visionResult;
        public string VisionResult
        {
            get { return visionResult; }
            set
            {
                if (visionResult != value)
                {
                    visionResult = value;
                    NotifyPropertyChanged("VisionResult");
                    NotifyPropertyChanged("ResultColor");
                }
            }
        }

        public Brush ResultColor { get { return VisionResult == "OK" ? Brushes.Blue : Brushes.Red; } }


        public VsDisplayEdit DisplayAlign;
        public List<Action> UpdateDisplayCode = new List<Action>();
        object Updatelock = new object();
        public void UpdateDisplay()
        {
            lock (Updatelock)
            {
                dsp.Invoke(new Action(() =>
                {
                    foreach (Action action in UpdateDisplayCode)
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

                    UpdateDisplayCode.Clear();
                    int i = DisplayAlign.imageListComboBox.SelectedIndex;
                    VsImageInfor imageInfo = (VsImageInfor)DisplayAlign.imageListComboBox.SelectedItem;
                    if (imageInfo != null && imageInfo.drawingFunc != null && DisplayAlign.Display.Image != null)
                        imageInfo.drawingFunc(DisplayAlign.Display);

                    foreach (Action action in UpdateDisplay_Inspect)
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
                    UpdateDisplay_Inspect.Clear();
                }));
            }
        }

        public VsDisplayEdit DisplayInspect;
        public List<Action> UpdateDisplay_Inspect = new List<Action>();

    }

}
