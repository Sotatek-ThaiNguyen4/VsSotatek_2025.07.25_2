using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ViSolution;
using ViSolution.Job;
using ViSolution.Object;
using ViSolution.Tool;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.IO;

namespace VsSotatek
{
    public class Thread_LG : Thread_Base
    {
        public clsVision job_Inspect;
        VsInputTool ToolImgInput;
        public bool IsReady;
        public Dictionary<int, VsImageInfor> InspectImageList = new Dictionary<int, VsImageInfor>();

        public Thread_LG(string _pathJob)
        {
            pathJob = _pathJob;
            IsReady = InitJob();
            if (IsReady)
            {
                ToolImgInput = mJob.ToolList[0] as VsInputTool;
                job_Inspect = new clsVision();
                job_Inspect.LoadJob();
            }
            this.Name = "EPOXY LGI";
        }

        public override bool Do_InitJob()
        {
            bool initOK = true;
            mAlignResult.ResultImageList.Clear();
            SetResultImage(typeof(VsInputTool), 0, 0, "OutputImage"); //Add Image vào Display Align
            return initOK;
        }

        public override bool Do_CheckVisionJob()
        {
            bool visionResult = false;
            InspectImageList.Clear();
            Mat imgInput = new Mat();
            string imagePath = "";

            try
            {
                if (File.Exists(Path_File) == true)
                {
                    imgInput = new Mat(Path_File);
                    imagePath = Path_File;
                    Path_File = string.Empty;
                }
                else if (Path_Folder.Count > 0)
                {
                    if (File.Exists(Path_Folder[0]) == true)
                    {
                        imgInput = new Mat(Path_Folder[0]);
                        imagePath = Path_Folder[0];
                        Path_Folder.RemoveAt(0);
                    }
                }

                if (imgInput == null) return visionResult;

                ToolImgInput.InputImage.Mat = imgInput;
                mJob.Run();
                visionResult = CheckVisionJob(mJob);
                if (visionResult)
                {
                    job_Inspect.Run(imgInput, ViewModel.Point1, ViewModel.Point2, imagePath);
                    try
                    {
                        VsImageInfor imgConvert = new VsImageInfor("Image.outImgConvert");
                        imgConvert.Image = new VsImage(job_Inspect.dic_ImgMat["outImgConvert"]);
                        VsImageInfor outImgThreshold = new VsImageInfor("Image.outImgThreshold");
                        outImgThreshold.Image = new VsImage(job_Inspect.dic_ImgMat["outImgThreshold"]);
                        VsImageInfor imgInspect = new VsImageInfor("Image.Result");
                        imgInspect.Image = new VsImage(job_Inspect.Img_OutputResult);

                        InspectImageList.Add(InspectImageList.Count, imgConvert);
                        InspectImageList.Add(InspectImageList.Count, outImgThreshold);
                        InspectImageList.Add(InspectImageList.Count, imgInspect);
                        ViewModel.UpdateDisplay_Inspect.Insert(0, new Action(() =>
                        {
                            ViewModel.DisplayInspect.ClearImage();
                            ViewModel.DisplayInspect.CustomImageList = InspectImageList;
                            ViewModel.DisplayInspect.RefreshImage();
                        }));

                        SaveImage(job_Inspect.Img_OutputResult, job_Inspect.Folder_Log_Vision + "\\Result.bmp");
                        var dic_ImgMatList = job_Inspect.dic_ImgMat.ToList();
                        for (int i = 0; i < dic_ImgMatList.Count; i++)
                        {
                            if (dic_ImgMatList[i].Value.Width != 0 || dic_ImgMatList[i].Value.Height != 0)
                                dic_ImgMatList[i].Value.SaveImage(string.Format("{0}\\{1}_{2}.bmp", job_Inspect.Folder_Log_Vision, i, dic_ImgMatList[i].Key));

                            if (clsDefine.M_RUN_SIMULATION_BUBBLE)
                            {
                                if (dic_ImgMatList[i].Key.Contains("imgBubble_") && dic_ImgMatList[i].Value.Width != 0 && dic_ImgMatList[i].Value.Height != 0)
                                {
                                    dic_ImgMatList[i].Value.SaveImage(string.Format("{0}\\{1}_{2}.bmp", clsDefine.PATH_LOG_FOLDER_IMG_DATA_WARP, job_Inspect.RESULT_VISION.tmp_Time, dic_ImgMatList[i].Key));
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        VsImageInfor imgIn = new VsImageInfor("Image.InputImage");
                        imgIn.Image = new VsImage(imgInput);
                        InspectImageList.Add(0, imgIn);
                        ViewModel.UpdateDisplay_Inspect.Insert(0, new Action(() =>
                        {
                            ViewModel.DisplayInspect.ClearImage();
                            ViewModel.DisplayInspect.CustomImageList = InspectImageList;
                            ViewModel.DisplayInspect.RefreshImage();
                        }));
                    }

                    ViewModel.VisionResult = job_Inspect.RESULT_VISION.Check_Result;
                    List<ResultLog> lstResult = new List<ResultLog>();
                    lstResult.Add(new ResultLog(job_Inspect.RESULT_VISION.LOG_RESULT));
                    ViewModel.AppendResultLog(lstResult);
                }
                else
                {
                    ViewModel.VisionResult = "NG";
                    List<ResultLog> lstResult = new List<ResultLog>();
                    string[] tmpLog = new string[] { DateTime.Now.ToString("HH.mm.ss.fff"), "-", "ERROR", "-", "0", "0" };
                    lstResult.Add(new ResultLog(tmpLog));
                    ViewModel.AppendResultLog(lstResult);
                }
            }
            catch (Exception ex)
            {
                visionResult = false;
                clsLog.WriteLog_txt(clsDefine.PATH_LOG_ROOT_HISTORY_APP, "Lỗi Run Job:" + ex.ToString());
            }

            return visionResult;
        }

        bool CheckVisionJob(VsJob _job)
        {
            if (_job == null) return false;
            ViewModel.VisionResult = string.Empty;
            bool checkjobResult = true;
            List<VsPoint> lstPoint = new List<VsPoint>();

            foreach (var tool in _job.ToolList)
            {
                if (tool is VsIntersectLineTool)
                {
                    VsIntersectLineTool toolInter = tool as VsIntersectLineTool;
                    VsPoint point = toolInter.IntersectPoint;
                    lstPoint.Add(point);
                }
            }

            if (lstPoint.Count == 2)
            {
                ViewModel.Point1 = lstPoint[0];
                ViewModel.Point2 = lstPoint[1];
            }
            else
            {
                checkjobResult = false;
            }

            //Kiểm tra xem các tool có lỗi hay không
            foreach (var checkinfo in mAlignResult.JobInfo[0].Values)
            {
                for (int k = 0; k < checkinfo.Count; k++)
                {
                    if (!_job.ToolList.Contains(checkinfo[k].Tool))
                        continue;
                    else if (checkinfo[k].Tool is VsTemplateTool)
                        checkjobResult &= CheckTool(checkinfo[k].Tool);
                    else if (checkinfo[k].Tool is VsIntersectLineTool)
                        checkjobResult &= CheckTool(checkinfo[k].Tool);
                }
            }
            return checkjobResult;
        }

        private void SaveImage(Mat _img, string _path)
        {
            try
            {
                if (_img == null) return;
                _img.ToBitmap().Save(_path);
            }
            catch (Exception) { }
        }

        public bool Do_CheckVisionJob_EDIT(string _pathImage)
        {
            bool visionResult = false;
            InspectImageList.Clear();
            Mat imgInput = new Mat();
            string imagePath = "";

            try
            {
                if (File.Exists(_pathImage) == true)
                {
                    imgInput = new Mat(_pathImage);
                    imagePath = _pathImage;
                    _pathImage = string.Empty;
                }
                if (imgInput == null) return visionResult;

                ToolImgInput.InputImage.Mat = imgInput;
                mJob.Run();
                visionResult = CheckVisionJob(mJob);
                if (visionResult)
                {
                    job_Inspect.Run(imgInput, ViewModel.Point1, ViewModel.Point2, imagePath);
                    try
                    {
                        VsImageInfor imgIn = new VsImageInfor("Image.InputImage");
                        imgIn.Image = new VsImage(imgInput);
                        InspectImageList.Add(0, imgIn);
                        foreach (var item in job_Inspect.dic_ImgMat)
                        {
                            VsImageInfor imgInfor = new VsImageInfor(item.Key);
                            imgInfor.Image = new VsImage(item.Value);
                            InspectImageList.Add(InspectImageList.Count, imgInfor);
                        }
                    }
                    catch (Exception){}
                    ViewModel.VisionResult = job_Inspect.RESULT_VISION.Check_Result;
                }
                else
                {
                    ViewModel.VisionResult = "NG";
                }
            }
            catch (Exception ex)
            {
                visionResult = false;
                clsLog.WriteLog_txt(clsDefine.PATH_LOG_ROOT_HISTORY_APP, "Lỗi Run Job:" + ex.ToString());
            }

            return visionResult;
        }

    }
}
