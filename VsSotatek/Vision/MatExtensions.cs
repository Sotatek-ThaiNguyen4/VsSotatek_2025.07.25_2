using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace VsSotatek
{
    public static class MatExtensions
    {

        public static Mat BitwiseNot(this Mat mat)
        {
            if (mat == null || mat.Empty())
                return null;

            Mat result = new Mat();
            Cv2.BitwiseNot(mat, result);
            return result;
        }

        public static Mat BitwiseAnd(this Mat mat1, Mat mat2)
        {
            if (ValidateInputs(mat1, mat2) == false) return null;
            Mat result = new Mat();
            Cv2.BitwiseAnd(mat1, mat2, result);
            return result;
        }

        public static Mat BitwiseOr(this Mat mat1, Mat mat2)
        {
            if (ValidateInputs(mat1, mat2) == false) return null;
            Mat result = new Mat();
            Cv2.BitwiseOr(mat1, mat2, result);
            return result;
        }

        private static bool ValidateInputs(Mat mat1, Mat mat2)
        {
            if (mat1 == null || mat1.Empty())
                return false;
            if (mat2 == null || mat2.Empty())
                return false;

            if (mat1.Size() != mat2.Size())
                return false;
            if (mat1.Type() != mat2.Type())
                return false;

            return true;
        }

    }
}
