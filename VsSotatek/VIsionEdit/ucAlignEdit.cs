using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ViSolution;
using ViSolution.Job;
using ViSolution.Tool;
using ViSolution.ToolEdit;
using System.IO;
using ViSolution.Object;
using OpenCvSharp;

namespace VsSotatek
{
    public partial class ucAlignEdit : UserControl
    {
        VsJob job;
        VsJobEdit jobEdit;
        VsInputTool inputTool;

        List<string> lst_PathImage = new List<string>();
        string PathImage;

        public ucAlignEdit()
        {
            InitializeComponent();
            jobEdit = new VsJobEdit();
            jobEdit.Dock = DockStyle.Fill;
            pl_View.Controls.Add(jobEdit);
        }

        private void btn_loadJob_Click(object sender, EventArgs e)
        {
            try
            {
                lstB_PathJob.Items.Clear();
                lstB_PathJob.Items.Add(clsDefine.PATH_JOB);
                if (File.Exists(clsDefine.PATH_JOB))
                {
                    job = (new VsSerializer()).LoadJob(clsDefine.PATH_JOB);
                    jobEdit.Job = job;
                    inputTool = job.ToolList[0] as VsInputTool;
                }
                else
                {
                    MessageBox.Show("Không tìm thấy file: " + clsDefine.PATH_JOB);
                }
            }
            catch (Exception ex)
            {
                job = null;
                MessageBox.Show(ex.ToString());
            }
        }

        private void btn_saveJob_Click(object sender, EventArgs e)
        {
            if (job != null)
            {
                VsSerializer serial = new VsSerializer();
                serial.SaveJob(job, clsDefine.PATH_JOB);
                MessageBox.Show("Save Job OK");
                serial = null;
                GC.Collect();
            }
        }

        private void btn_openFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openfile = new OpenFileDialog())
            {
                lst_PathImage.Clear();
                openfile.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                openfile.Title = "Chọn một file ảnh";
                openfile.Multiselect = true;
                if (openfile.ShowDialog() == DialogResult.OK)
                {
                    lst_PathImage = openfile.FileNames.ToList();
                    this.Invoke(new Action(Update_PathImage));
                }
            }
        }

        private void btn_openFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowser = new FolderBrowserDialog())
            {
                lst_PathImage.Clear();
                folderBrowser.Description = "Chọn thư mục chứa ảnh";
                if (folderBrowser.ShowDialog() == DialogResult.OK)
                {
                    string selectedFolder = folderBrowser.SelectedPath;
                    string[] imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };
                    lst_PathImage = Directory.GetFiles(selectedFolder)
                            .Where(file => imageExtensions.Contains(Path.GetExtension(file).ToLower()))
                            .ToList();
                    this.Invoke(new Action(Update_PathImage));
                }
            }
        }

        private void Update_PathImage()
        {
            lstB_PathImage.Items.Clear();
            foreach (var item in lst_PathImage)
            {
                lstB_PathImage.Items.Add(item);
            }
            if (lstB_PathImage.Items.Count >0)
            {
                lstB_PathImage.SelectedIndex = 0;
            }
        }

        private void btn_runjob_Click(object sender, EventArgs e)
        {
            if (job != null && lst_PathImage.Count > 0)
            {
                try
                {
                    if (File.Exists(PathImage))
                    {
                        Mat img = new Mat(PathImage);
                        inputTool.InputImage.Mat = img;
                        jobEdit.Run();
                    }
                    else
                    {
                        MessageBox.Show("File Không tồn tại");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            else
            {
                MessageBox.Show("Kiểm tra lại job và ảnh");
            }
        }

        private void lstB_PathImage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lst_PathImage.Count > 0)
            {
                PathImage = lst_PathImage[lstB_PathImage.SelectedIndex];
                btn_runjob_Click(sender, e);
            }
        }
    }
}
