using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViSolution;
using ViSolution.Job;
using ViSolution.Object;
using ViSolution.Tool;

namespace VsSotatek
{
    public class ViResult
    {
        public VsImage InputImage;
        public VsImage OriginalImage;
        public VsImage ResultImage;
        public Dictionary<int, VsImageInfor> ResultImageList = new Dictionary<int, VsImageInfor>();
        public List<Dictionary<Type, List<VsToolInfo>>> JobInfo = new List<Dictionary<Type, List<VsToolInfo>>>();


        public List<double> Scores = new List<double>();
        public List<double> Means = new List<double>();
        public List<double> ResultValues = new List<double>();

    }

    public class VsToolInfo
    {
        public VsBaseTool Tool;
        public VsToolInfo(VsBaseTool _tool)
        {
            Tool = _tool; 
        }
    }
}
