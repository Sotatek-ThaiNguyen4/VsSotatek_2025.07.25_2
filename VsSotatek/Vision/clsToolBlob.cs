using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VsSotatek
{
    internal class clsToolBlob
    {
        public class BlobResult
        {
            public Rect Bouding { get; set; }
        }

        public static Rect Blob(Mat _imgBin, Mat _imgSource, out List<List<Rect>> rectsBuble, out Mat _imgCheckConnect)
        {
            _imgCheckConnect = new Mat();
            OpenCvSharp.Point[][] contours;
            HierarchyIndex[] hierarchyIndices;
            Cv2.FindContours(_imgBin, out contours, out hierarchyIndices, RetrievalModes.External, ContourApproximationModes.ApproxSimple);
            // Lấy danh sách các boundingRect và sắp xếp theo diện tích giảm dần
            List<Rect> sortedRects = contours
                .Select(Cv2.BoundingRect)                      // Lấy tất cả boundingRect từ contours
                .Where(rect => rect.Y < _imgBin.Height * 0.1 && rect.Width > _imgBin.Width * 0.1 
                                    && rect.Height > 0.1/clsDefine.CAL_RESOLUTION //0.1 mm độ rộng tối thiểu của Epoxy
                                    && rect.Width * rect.Height > (clsDefine.MASTER_LENGHT_FPCB_MM/clsDefine.CAL_RESOLUTION * clsDefine.MASTER_WIDTH_MM/clsDefine.CAL_RESOLUTION) *0.05) //tính là Keo phải lớn hơn 5% diện tích tối data của keo
                .OrderByDescending(rect => rect.Width * rect.Height) // Sắp xếp theo diện tích giảm dần
                .Take(3) // Chỉ lấy 3 phần tử đầu tiên (lớn nhất)
                .ToList();                                     // Chuyển sang danh sách
            Rect BlobGlue = new Rect();

            if (sortedRects.Count > 0) BlobGlue = sortedRects[0]; //na

            //if (sortedRects.Count == 1) //na bỏ đi
            //{
            //    BlobGlue = sortedRects[0];
            //}
            //else if (sortedRects.Count > 1)
            //{
            //    int x = sortedRects.Min(r => r.X);
            //    int y = sortedRects.Min(r => r.Y);
            //    int right = sortedRects.Max(r => r.Right);
            //    int bottom = sortedRects.Max(r => r.Bottom);
            //    BlobGlue = new Rect(x, y, right - x, bottom - y);// Vùng rect bao trọn cả Rect lớn nhất
            //}

            #region CROP BUBBLE
            //Phần này đánh giá thêm để cắt vùng sáng ở ngoài
            List<List<Rect>> sortedRects_bubble_Result = new List<List<Rect>>();
            if (sortedRects.Count > 0)
            {
                Mat mask = Mat.Zeros(_imgBin.Rows, _imgBin.Cols, MatType.CV_8UC1);
                // Vẽ các contour bên ngoài lên mask với màu trắng
                double maxArea = 0;
                int maxAreaIndex = -1;
                for (int i = 0; i < contours.Length; i++)  // Duyệt qua tất cả các contour để tìm contour lớn nhất
                {
                    double area = Cv2.ContourArea(contours[i]); // Tính diện tích contour
                    if (area > maxArea)
                    {
                        maxArea = area;
                        maxAreaIndex = i;
                    }
                }
                if (maxAreaIndex != -1)
                {
                    // Vẽ contour lớn nhất lên mask
                    Cv2.DrawContours(mask, contours, maxAreaIndex, Scalar.White, -1); // Tô kín vùng trắng
                    _imgCheckConnect = mask;
                    //Cv2.ImShow("mask", mask);
                    //Cv2.WaitKey();
                }

                #region tìm vùng bao
                // Tính khoảng cách từ mỗi pixel đến contour gần nhất
                Mat contourMask = new Mat(_imgBin.Size(), MatType.CV_8UC1, Scalar.All(0));
                Cv2.DrawContours(contourMask, contours, maxAreaIndex, new Scalar(255), thickness: 1);
                Mat distTransform = new Mat();
                Cv2.DistanceTransform(~contourMask, distTransform, DistanceTypes.L2, DistanceTransformMasks.Mask5);
                Cv2.Normalize(distTransform, distTransform, 0, 255, NormTypes.MinMax); // Chuẩn hóa
                //Cv2.ImShow("distTransform", distTransform);
                //SaveMatToCsv(distTransform, "D:\\distTransform.csv");
                #endregion
                //Loại bỏ vùng trắng bên ngoài => chuyển thành màu đen, vùng trắng bên trong Epoxy chuyển thành màu đen tí nữa sẽ bắt bob ở trong Epoxy
                //chỗ này cần lấy ảnh gốc và áp threshold khác để bắt được các bóng trắng mờ bên trong
                Mat imgCrop = new Mat();
                _imgSource.CopyTo(imgCrop, mask);
                //Cv2.ImShow("imgCrop", imgCrop);
                Mat outImgConvert = new Mat();
                imgCrop.ConvertTo(outImgConvert, imgCrop.Type(), 3, -45);
                //Cv2.ImShow("outImgConvert", outImgConvert);
                //Cv2.WaitKey();
                Mat outImgThreshold = new Mat();
                Cv2.Threshold(imgCrop, outImgThreshold, 40, 255, ThresholdTypes.Binary);
                //Cv2.ImShow("outImgThreshold", outImgThreshold);
                //Cv2.WaitKey();
                //Tìm vùng countour bên rong epoxy
                List<Rect> sortedRects_bubble_1 = new List<Rect>();
                OpenCvSharp.Point[][] contours_1;
                HierarchyIndex[] hierarchyIndices_1;
                Cv2.FindContours(outImgThreshold, out contours_1, out hierarchyIndices_1, RetrievalModes.External, ContourApproximationModes.ApproxSimple);


                #region tìm vùng đốm sáng
                List<Point[]> validBlobs = new List<Point[]>();
                // Kiểm tra khoảng cách từ đốm sáng đến contour
                foreach (var blob in contours_1)
                {
                    bool isNearContour = false;

                    foreach (var pt in blob)
                    {
                        // Lấy giá trị khoảng cách từ distance transform
                        int x = Math.Max(0, Math.Min(pt.X, distTransform.Cols - 1));
                        int y = Math.Max(0, Math.Min(pt.Y, distTransform.Rows - 1));
                        if (y < 7) y = 7; // k hạn chế 3 chiều của Epoxy
                        if (x < 7) x = 7;
                        if (x > distTransform.Width - 7) x = distTransform.Width - 7;

                        float distance = distTransform.At<float>(y, x);
                        if (distance < 4) // Nếu khoảng cách nhỏ hơn 10 pixels (có thể điều chỉnh)
                        {
                            isNearContour = true;
                            break;
                        }
                    }

                    // Nếu đốm sáng không gần viền, giữ lại
                    if (!isNearContour)
                    {
                        //validBlobs.Add(blob);
                        //Kiểm tra diện tích
                        if (Cv2.ContourArea(blob) > 3)
                        {
                            validBlobs.Add(blob);
                        }
                    }
                }

                // Vẽ các đốm sáng hợp lệ lên ảnh         
                //Mat resultImage = new Mat(_imgSource.Size(), MatType.CV_8UC3, Scalar.All(0));
                //Cv2.DrawContours(resultImage, validBlobs, -1, new Scalar(0, 255, 0), 2);
                //Cv2.DrawContours(resultImage, contours, maxAreaIndex, Scalar.Red, 2);
                //foreach (var blob in validBlobs)
                //{
                //    Cv2.Polylines(resultImage, new[] { blob }, true, new Scalar(0, 0, 255), 2);
                //}

                // Hiển thị kết quả
                //Cv2.ImShow("Result", resultImage);
                //Cv2.WaitKey();

                //Phân nhóm => kiểm tra diện tích nhóm
                double eps = 30.0; // Ngưỡng khoảng cách giữa các Rect để coi là gần nhau
                int minPts = 0; // Số lượng điểm tối thiểu để tạo thành cụm
                var clusters = DBSCAN(validBlobs, eps, minPts);


                foreach (var _cluster in clusters)
                {
                    List<Rect> _rectCluster = new List<Rect>();
                    _rectCluster = _cluster.Select(Cv2.BoundingRect).ToList();
                    //kiểm tra điều kiện
                    int minX = _rectCluster.Min(r => r.X);
                    int minY = _rectCluster.Min(r => r.Y);
                    int CheckWidth = _rectCluster.Max(r => r.Right) - minX;
                    int CheckHeight = _rectCluster.Max(r => r.Bottom) - minY;

                    if (CheckWidth > 10 && CheckHeight > 10 && minY < 40)
                    {
                        sortedRects_bubble_Result.Add(_rectCluster);
                    }
                }


                // Vẽ các đốm sáng hợp lệ lên ảnh         
                Mat resultImage = new Mat(_imgSource.Size(), MatType.CV_8UC3, Scalar.All(0));
                Cv2.DrawContours(resultImage, contours, maxAreaIndex, Scalar.Green, 2);
                Scalar[] nhomMau = new Scalar[] { Scalar.Red, Scalar.Blue, Scalar.Yellow, Scalar.YellowGreen, Scalar.Green, Scalar.Orange, Scalar.Turquoise, Scalar.Violet, Scalar.BlueViolet, Scalar.Tomato };
                int CountMau = 0;

                foreach (var _clusterIdx in sortedRects_bubble_Result)
                {
                    foreach (var blob in _clusterIdx)
                    {
                        Cv2.Rectangle(resultImage, blob, nhomMau[CountMau]);
                        //Cv2.Polylines(resultImage, new[] { blob }, true, nhomMau[CountMau], 2);
                    }
                    CountMau++;
                }

                // Hiển thị kết quả
                //Cv2.ImShow("Result", resultImage);
                //Cv2.WaitKey();

                #endregion
                // Lấy danh sách các boundingRect và sắp xếp theo diện tích giảm dần
                //sortedRects_bubble_1 = contours_1
                //    .Select(Cv2.BoundingRect)                      // Lấy tất cả boundingRect từ contours
                //    .OrderByDescending(rect => rect.Width * rect.Height) // Sắp xếp theo diện tích giảm dần
                //    .ToList();                                     // Chuyển sang danh sách 

                //List<Rect> sortedRects_bubble_2 = new List<Rect>();
                ////Kiểm tra diện tích chỉ lấy kích thước các vùng lớn
                //for (int i = 0; i < sortedRects_bubble_1.Count; i++)
                //{
                //    if (sortedRects_bubble_1[i].Width * sortedRects_bubble_1[i].Height>10)
                //    {
                //        sortedRects_bubble_2.Add(sortedRects_bubble_1[i]);
                //    }
                //}

                //double eps = 70.0; // Ngưỡng khoảng cách giữa các Rect để coi là gần nhau
                //int minPts = 0; // Số lượng điểm tối thiểu để tạo thành cụm
                //var clusters = DBSCAN(sortedRects_bubble_2, eps, minPts);

                // Hiển thị kết quả
                //for (int i = 0; i < clusters.Count; i++)
                //{
                //    Console.WriteLine($"Nhóm {i + 1}:");
                //    foreach (var rect in clusters[i])
                //    {
                //        Console.WriteLine($"    Rect(X:{rect.X}, Y:{rect.Y}, W:{rect.Width}, H:{rect.Height})");
                //    }
                //}
                //sortedRects_bubble_Result = clusters;
            }
            rectsBuble = sortedRects_bubble_Result;
            #endregion
            return BlobGlue;
        }

        public static List<Rect> BlobFPCB(Mat _imgWarp)
        {
            Mat outImgConvert = new Mat();
            _imgWarp.ConvertTo(outImgConvert, _imgWarp.Type(), clsDefine.VI_CONTRAST_OVER, clsDefine.VI_BRIGHTNESS_OVER);
            //clsVision.ShowImage("outImgConvert", outImgConvert, true, 1);
            //- Close
            Mat invertedImage_1 = new Mat();
            Cv2.BitwiseNot(outImgConvert, invertedImage_1);
            Mat closedImage = new Mat(); // Thực hiện phép Close
            Mat kernel_close = Cv2.GetStructuringElement(MorphShapes.Ellipse, new OpenCvSharp.Size(5, 5));
            Cv2.MorphologyEx(invertedImage_1, closedImage, MorphTypes.Close, kernel_close);
            //clsVision.ShowImage("closedImage1", closedImage, true, 1);
            Mat outImgClose = new Mat();
            Cv2.BitwiseNot(closedImage, outImgClose);
            //clsVision.ShowImage("closedImage", closedImage, true, 1);
            //- Threshold
            Mat outImgThreshold = new Mat();
            Cv2.Threshold(outImgClose, outImgThreshold, clsDefine.VI_THRESHOLD_OVER, 255, ThresholdTypes.Binary);
            //- Blob
            Mat invertedImage_2 = new Mat();
            Cv2.BitwiseNot(outImgThreshold, invertedImage_2);
            //clsVision.ShowImage("outImgThreshold", outImgThreshold, true, 1);

            List<Rect> sortedRects_bubble = new List<Rect>();
            OpenCvSharp.Point[][] contours;
            HierarchyIndex[] hierarchyIndices;
            Cv2.FindContours(invertedImage_2, out contours, out hierarchyIndices, RetrievalModes.External, ContourApproximationModes.ApproxSimple);
            // Lấy danh sách các boundingRect và sắp xếp theo diện tích giảm dần
            List<Rect> sortedRects = contours
                .Select(Cv2.BoundingRect)
                .Where(rect => rect.Width > invertedImage_2.Width * 0.015 && rect.Height > invertedImage_2.Width * 0.015) //Chỗ này lấy tiêu chuẩn invertedImage_2.Width*0.01 vì chiều Height ngắn nên lấy cạnh dài nhất làm tiêu chuẩn
                .OrderByDescending(rect => rect.Width * rect.Height) // Sắp xếp theo diện tích giảm dần
                .ToList();                                     // Chuyển sang danh sách

            return sortedRects;
        }

        public static Rect BlobBigest(Mat _imgBin)
        {
            OpenCvSharp.Point[][] contours;
            HierarchyIndex[] hierarchyIndices;
            Cv2.FindContours(_imgBin, out contours, out hierarchyIndices, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

            // Tìm boundingRect lớn nhất
            Rect largestRect = contours
                .Select(Cv2.BoundingRect)                      // Lấy tất cả boundingRect từ contours
                .OrderByDescending(rect => rect.Width * rect.Height) // Sắp xếp theo diện tích giảm dần
                .FirstOrDefault();                             // Lấy boundingRect lớn nhất
            return largestRect;
        }

        public static double Blob_Bottom(Mat _imgBin)
        {
            if (_imgBin == null || _imgBin.Empty()) return 0;

            _imgBin = _imgBin.BitwiseNot();
            //clsVision.ShowImage("_imgBin", _imgBin, true, 1);

            //Kiểm tra độ rộng keo có dàn đều trên FPCB hay không
            OpenCvSharp.Point[][] contours;
            HierarchyIndex[] hierarchyIndices;
            Cv2.FindContours(_imgBin, out contours, out hierarchyIndices, RetrievalModes.External, ContourApproximationModes.ApproxSimple);
            // Lấy danh sách các boundingRect và sắp xếp theo X tăng dần
            List<Rect> sortedRects_X = contours
                .Select(Cv2.BoundingRect)   
                .OrderBy(rect => rect.X) // Sắp xếp theo X tăng dần
                .ToList();

            List<Rect> sortedRects_right = contours
             .Select(Cv2.BoundingRect)
             .OrderBy(rect => rect.Right) // Sắp xếp theo right tăng dần
             .ToList();

            double LenghtEpoxy = 0;
            if (sortedRects_X.Count == 1)
            {
                LenghtEpoxy = sortedRects_X[0].Width;
            }
            else if (sortedRects_X.Count > 1)
            {
                LenghtEpoxy = sortedRects_right[sortedRects_right.Count-1].Right - sortedRects_X[0].X;
            }

            return LenghtEpoxy;
        }

        #region PHAN CUM
        static List<List<Rect>> DBSCAN(List<Rect> rects, double eps, int minPts)
        {
            int[] labels = Enumerable.Repeat(-1, rects.Count).ToArray();
            int clusterId = 0;

            for (int i = 0; i < rects.Count; i++)
            {
                if (labels[i] != -1) continue; // Đã được phân cụm

                List<int> neighbors = GetNeighbors(rects, i, eps);
                if (neighbors.Count < minPts) continue; // Không đủ điểm để tạo cụm

                // Tạo cụm mới
                labels[i] = clusterId;
                int index = 0;
                while (index < neighbors.Count)
                {
                    int neighborIdx = neighbors[index];
                    if (labels[neighborIdx] == -1) labels[neighborIdx] = clusterId;
                    if (labels[neighborIdx] == -1)
                    {
                        labels[neighborIdx] = clusterId;
                        List<int> newNeighbors = GetNeighbors(rects, neighborIdx, eps);
                        if (newNeighbors.Count >= minPts)
                            neighbors.AddRange(newNeighbors);
                    }
                    index++;
                }
                clusterId++;
            }

            // Gom nhóm các Rects theo clusterId
            var clusters = new List<List<Rect>>();
            for (int i = 0; i < clusterId; i++)
                clusters.Add(new List<Rect>());

            for (int i = 0; i < rects.Count; i++)
            {
                if (labels[i] != -1)
                    clusters[labels[i]].Add(rects[i]);
            }

            return clusters;
        }

        static List<int> GetNeighbors(List<Rect> rects, int index, double eps)
        {
            List<int> neighbors = new List<int>();
            for (int i = 0; i < rects.Count; i++)
            {
                if (i != index && Distance(rects[index], rects[i]) < eps)
                    neighbors.Add(i);
            }
            return neighbors;
        }

        static double Distance(Rect a, Rect b)
        {
            double centerX1 = a.X + a.Width / 2.0;
            double centerY1 = a.Y + a.Height / 2.0;
            double centerX2 = b.X + b.Width / 2.0;
            double centerY2 = b.Y + b.Height / 2.0;
            return Math.Sqrt(Math.Pow(centerX1 - centerX2, 2) + Math.Pow(centerY1 - centerY2, 2));
        }

        #endregion

        #region PHAN CUM 2
        static List<List<Point[]>> DBSCAN(List<Point[]> blobs, double eps = 5, int minPts = 1)
        {
            int n = blobs.Count;
            int[] labels = Enumerable.Repeat(-1, n).ToArray(); // -1: chưa gán cụm
            int clusterId = 0;

            List<List<Point[]>> clusters = new List<List<Point[]>>();

            for (int i = 0; i < n; i++)
            {
                if (labels[i] != -1) continue; // Đã gán cụm rồi
                List<int> neighborIndices = GetNeighbors(blobs, i, eps);

                if (neighborIndices.Count < minPts)
                {
                    labels[i] = -2; // Đánh dấu là nhiễu
                    continue;
                }

                // Tạo cụm mới
                List<Point[]> cluster = new List<Point[]>();
                clusters.Add(cluster);
                ExpandCluster(blobs, labels, i, neighborIndices, cluster, clusterId, eps, minPts);
                clusterId++;
            }

            return clusters;
        }

        static void ExpandCluster(List<Point[]> blobs, int[] labels, int index, List<int> neighbors, List<Point[]> cluster, int clusterId, double eps, int minPts)
        {
            cluster.Add(blobs[index]); // Thêm blob hiện tại vào cụm
            labels[index] = clusterId; // Gán nhãn cụm

            int i = 0;
            while (i < neighbors.Count)
            {
                int neighborIdx = neighbors[i];

                if (labels[neighborIdx] == -2) // Nếu là nhiễu, gán vào cụm
                    labels[neighborIdx] = clusterId;

                if (labels[neighborIdx] == -1) // Nếu chưa thuộc cụm nào
                {
                    labels[neighborIdx] = clusterId;
                    cluster.Add(blobs[neighborIdx]); // Thêm vào cụm

                    // Tìm thêm hàng xóm
                    List<int> newNeighbors = GetNeighbors(blobs, neighborIdx, eps);
                    if (newNeighbors.Count >= minPts) // Nếu có đủ điểm để mở rộng cụm
                        neighbors.AddRange(newNeighbors);
                }
                i++;
            }
        }

        static List<int> GetNeighbors(List<Point[]> blobs, int idx, double eps)
        {
            List<int> neighbors = new List<int>();

            for (int i = 0; i < blobs.Count; i++)
            {
                if (i == idx) continue;
                double minDist = double.MaxValue;// Hàm tìm khoảng cách nhỏ nhất giữa 2 contours
                foreach (var p1 in blobs[idx])
                {
                    foreach (var p2 in blobs[i])
                    {
                        double dist = Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
                        minDist = Math.Min(minDist, dist);
                    }
                }
                if (minDist < eps)
                    neighbors.Add(i);
            }
            return neighbors;
        }

        #endregion

    }
}
