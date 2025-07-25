using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ViSolution;
using ViSolution.Display;
using ViSolution.Object;
using ViSolution.Tool;

namespace VsSotatek.VIsionEdit
{
    public partial class ucInspectEdit : UserControl
    {
        ucAuto mUcAuto;
        VsDisplayEdit Display;
        List<string> lst_PathImage = new List<string>();
        string PathImage;
        VsImage ImageInput;

        Dictionary<int, VsImageInfor> dict_Image = new Dictionary<int, VsImageInfor>();

        public ucInspectEdit(ucAuto _ucAuto)
        {
            InitializeComponent();
            Display = new VsDisplayEdit();
            Display.Dock = DockStyle.Fill;
            pl_View.Controls.Add(Display);
            this.mUcAuto = _ucAuto;
        }

        #region Button Control
        private void btn_run_Click(object sender, EventArgs e)
        {
            dict_Image.Clear();
            ImageInput = new VsImage(PathImage);
            mUcAuto.thrLG.Do_CheckVisionJob_EDIT(PathImage);
            foreach (var item in mUcAuto.thrLG.InspectImageList)
            {
                dict_Image.Add(item.Key, item.Value);
            }
            Display.ClearImage();
            Display.CustomImageList = dict_Image;
            Display.RefreshImage();
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

        private void lstB_PathImage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lst_PathImage.Count > 0)
            {
                PathImage = lst_PathImage[lstB_PathImage.SelectedIndex];
                btn_run_Click(sender, e);
            }
        }
        #endregion Button Control

        private void Update_PathImage()
        {
            lstB_PathImage.Items.Clear();
            foreach (var item in lst_PathImage)
            {
                lstB_PathImage.Items.Add(item);
            }
            if (lstB_PathImage.Items.Count > 0)
            {
                lstB_PathImage.SelectedIndex = 0;
            }
        }

    }
}
