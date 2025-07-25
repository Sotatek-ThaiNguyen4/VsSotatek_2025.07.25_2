using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ViSolution;
using ViSolution.Job;
using ViSolution.Tool;
using ViSolution.Object;
using System.Windows.Media;
using OpenCvSharp;
using static OpenCvSharp.ConnectedComponents;
using System.Drawing;
using System.Windows.Documents;

namespace VsSotatek
{
    public abstract class Thread_Base : IDisposable
    {
        public enum eStatus
        {
            READY = 0,
            WAIT_START_REQ = 1,
            START = 2,
            JUDGEMENT = 3,
            SEND_RESULT = 4,
        }

        public Thread_Base()
        {

        }

        //====================================================================== Property
        #region Property
        protected string pathJob = clsDefine.PATH_JOB;
        public string PathJob
        {
            get { return pathJob; }
            set { pathJob = value; }
        }

        protected string name = "Base";
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public VMucAuto ViewModel
        {
            set
            {
                mViewModel = value;
                mUpdateDisplayCode = mViewModel.UpdateDisplayCode;
            }
            get { return mViewModel; }
        }
        VMucAuto mViewModel;

        protected ViResult mAlignResult = new ViResult();
        public ViResult AlignResult { get { return mAlignResult; } }
        #endregion Property

        //====================================================================== JOB
        #region JOB ALIGN
        protected VsJob mJob;
        public VsJob Job { get { return mJob; } }

        public abstract bool Do_InitJob();
        public bool InitJob()
        {
            try
            {
                mJob = (new VsSerializer()).LoadJob(pathJob);
                if (mJob == null) return false;
                mAlignResult.JobInfo.Clear();
                mResultImageList.Clear();
                if (!GetJobInfo(mJob)) return false;
                if (!Do_InitJob()) return false;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public bool GetJobInfo(VsJob _job, int _blockindex = 0)
        {
            if (_job == null) return false;
            try
            {
                mAlignResult.JobInfo.Add(new Dictionary<Type, List<VsToolInfo>>());
                foreach (VsBaseTool toolbase in _job.ToolList)
                {
                    if (!mAlignResult.JobInfo[_blockindex].ContainsKey(toolbase.GetType()))
                        mAlignResult.JobInfo[_blockindex].Add(toolbase.GetType(), new List<VsToolInfo>());

                    mAlignResult.JobInfo[_blockindex][toolbase.GetType()].Add(new VsToolInfo(toolbase));
                    if (toolbase is VsToolBock)
                    {
                        GetJobInfo((toolbase as VsToolBock).ToolJob, mAlignResult.JobInfo[_blockindex][typeof(VsToolBock)].Count);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public abstract bool Do_CheckVisionJob();
        #endregion JOB ALIGN

        //====================================================================== Hiển thị
        #region IMAGE - GRAPHIC
        protected List<Action> mUpdateDisplayCode;
        List<KeyValuePair<VsBaseTool, string>> mResultImageList = new List<KeyValuePair<VsBaseTool, string>>();
        private void DisplayResultImage()
        {
            try
            {
                for (int i = 0; i < mAlignResult.ResultImageList.Count; i++)
                {
                    VsImageInfor info = mAlignResult.ResultImageList[i];
                    if (info.Image != null) info.Image.Dispose();
                }
                mAlignResult.ResultImageList.Clear();

                for (int i = 0; i < mResultImageList.Count; i++)
                {
                    var kv = mResultImageList[i];
                    VsImage image = null;
                    if (kv.Key.inParams.ContainsKey(kv.Value))
                        image = (VsImage)(kv.Key.GetInParam(kv.Value) as VsImage);
                    else if (kv.Key.outParams.ContainsKey(kv.Value))
                        image = (VsImage)(kv.Key.outParams[kv.Value] as VsImage);

                    if (image == null)
                        continue;

                    VsImageInfor info = new VsImageInfor(kv.Key.ToString() + "." + kv.Value);
                    info.Image = image.Clone();
                    info.Image.TransformMat = Mat.Eye(3, 3, MatType.CV_32FC1);
                    mAlignResult.ResultImageList.Add(i, info);
                }

                mUpdateDisplayCode.Insert(0, new Action(() =>
                {
                    mViewModel.DisplayAlign.CustomImageList = mAlignResult.ResultImageList;
                    mViewModel.DisplayAlign.RefreshImage();
                }));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return;
            }
        }

        public bool SetResultImage(Type _type, int _blockindex, int _index, string _name = "InputImage")
        {
            /* Add ảnh imageInfor vào list
             */
            try
            {
                VsBaseTool resultImageTool = mAlignResult.JobInfo[_blockindex][_type][_index].Tool;
                string resultImageName = _name;
                mResultImageList.Add(new KeyValuePair<VsBaseTool, string>(resultImageTool, resultImageName));
                return true;
            }
            catch
            {
                return false;
            }
        }


        protected bool CheckTool(VsBaseTool tool, int blockindex = 0)
        {
            if (tool is VsTemplateTool) return CheckTool(tool as VsTemplateTool, blockindex);
            else if (tool is VsIntersectLineTool) return CheckTool(tool as VsIntersectLineTool, blockindex);
            else return false;
        }

        protected virtual bool CheckTool(VsTemplateTool templateTool, int displayindex = 0, bool isDraw = true)
        {
            if (templateTool.lastRunSuccess && isDraw)
            {
                mAlignResult.Scores.Add(templateTool.Score);
                mAlignResult.ResultValues.Add(templateTool.Score);

                Point2d TMcenter = new Point2d(templateTool.TranslateX, templateTool.TranslateY);
                if (double.IsNaN(TMcenter.X) || double.IsNaN(TMcenter.Y))
                    TMcenter = new Point2d(templateTool.failResultBox.CP.X, templateTool.failResultBox.CP.Y - 500);
                Point2d[] resultBox = templateTool.ResultBox;
                System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.GreenYellow, 2f);
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDot;

                this.mUpdateDisplayCode.Add(new Action(() =>
                {
                    mViewModel.DisplayAlign.CustomImageList[displayindex].drawingFunc +=
                    (display) =>
                    {
                        display.DrawPolyLines(pen, resultBox);
                    };
                }));
            }
            return templateTool.lastRunSuccess;
        }

        protected virtual bool CheckTool(VsIntersectLineTool intersectTool, int displayindex = 0, bool isDraw = true)
        {
            if (intersectTool.lastRunSuccess && isDraw)
            {
                if (intersectTool.IntersectPoint == null) return false;
                Point2d intersectpoint = VsFunc.Map2D_Fix2Img(intersectTool.IntersectPoint.Point2d, intersectTool.IntersectPoint.TransformMat);

                this.mUpdateDisplayCode.Add(new Action(() =>
                {
                    mViewModel.DisplayAlign.CustomImageList[displayindex].drawingFunc +=
                    (display) =>
                    {
                        System.Drawing.Pen CyanPen = new System.Drawing.Pen(System.Drawing.Color.Cyan, 1);
                        System.Drawing.Pen RedPen = new System.Drawing.Pen(System.Drawing.Color.Red, 3);
                        display.DrawLine(CyanPen, intersectTool.LineA.SP, intersectTool.LineA.EP);
                        display.DrawLine(CyanPen, intersectTool.LineB.SP, intersectTool.LineB.EP);
                    };
                }));

            }
            return intersectTool.lastRunSuccess;
        }

        #endregion IMAGE - GRAPHIC


        //===============================***************** ************** Thread Run ***********
        protected Thread mThread = null;
        public bool mThreadFlag = false;
        eStatus eThreadstatus;

        public string Path_File = string.Empty;
        public List<string> Path_Folder = new List<string>();

        public void ThreadStart()
        {
            if (mThreadFlag)
            {
                ThreadStop();
                Thread.Sleep(100);
            }
            mThreadFlag = true;
            mThread = new Thread(new ThreadStart(ThreadFunction));
            mThread.Start();
        }

        public void ThreadStop()
        {
            mThreadFlag = false;
            if (mThread != null)
            {
                mThread.Join();
                mThread.Abort();
                Thread.Sleep(50);
            }
        }

        IAsyncResult asyncWriteBit;
        public bool StartVision = false;

        public void ThreadFunction()
        {
            Func<bool> RunTool = Do_CheckVisionJob;
            eThreadstatus = eStatus.READY;
            while (mThreadFlag)
            {
                switch (eThreadstatus)
                {
                    case eStatus.READY:
                        eThreadstatus = eStatus.WAIT_START_REQ;
                        if (Path_Folder.Count >0) //sử dụng để Run folder
                            StartVision = true;
                        break;
                    case eStatus.WAIT_START_REQ:
                        if (StartVision)
                        {
                            StartVision = false;
                            eThreadstatus = eStatus.START;
                        }
                        break;
                    case eStatus.START:
                        asyncWriteBit = RunTool.BeginInvoke(null, null);
                        bool IsVisionOK = RunTool.EndInvoke(asyncWriteBit);
                        asyncWriteBit = null;
                        DisplayResultImage();
                        mViewModel.UpdateDisplay();
                        eThreadstatus = eStatus.JUDGEMENT;
                        break;
                    case eStatus.JUDGEMENT:
                        eThreadstatus = eStatus.SEND_RESULT;
                        break;
                    case eStatus.SEND_RESULT:
                        eThreadstatus = eStatus.READY;
                        break;
                }
                Thread.Sleep(1);
            }
        }

        public void Dispose()
        {
            try
            {
                ThreadStop();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return;
            }
        }
    }

    public class BaseLog
    {
        string mTime;
        public SolidColorBrush TextColor { get; set; }
        public string Time
        {
            set { mTime = value; }
            get { return mTime; }
        }
        string mMessage;
        public string Message
        {
            set { mMessage = value; }
            get { return mMessage; }
        }
        public BaseLog(string time, string msg, SolidColorBrush color = null)
        {
            if (color == null)
                color = System.Windows.Media.Brushes.Black;
            Time = time;
            Message = msg;
            TextColor = color;
        }
    }

    public class ResultLog
    {
        public SolidColorBrush TextColor { get; set; }

        string time;
        public string Time
        {
            set { time = value; }
            get { return time; }
        }

        string processTime;
        public string ProcessTime
        {
            set { processTime = value; }
            get { return processTime; }
        }

        string result;
        public string Result
        {
            set { result = value; }
            get { return result; }
        }

        string caseNG;
        public string CaseNG
        {
            set { caseNG = value; }
            get { return caseNG; }
        }

        string length_Fpcb;
        public string Length_Fpcb
        {
            set { length_Fpcb = value; }
            get { return length_Fpcb; }
        }

        string length_Epoxy;
        public string Length_Epoxy
        {
            set { length_Epoxy = value; }
            get { return length_Epoxy; }
        }

        string width_Epoxy;
        public string Width_Epoxy
        {
            set { width_Epoxy = value; }
            get { return width_Epoxy; }
        }


        public ResultLog(string[] _value, SolidColorBrush color = null)
        {
            if (color == null)
                color = System.Windows.Media.Brushes.Black;
            Time = _value[0];
            ProcessTime = _value[1];
            Result = _value[2];
            CaseNG = _value[3];
            Length_Fpcb = _value[4];
            Length_Epoxy = _value[5];
            Width_Epoxy = _value[6];
            TextColor = color;
        }
    }



}
