
namespace VsSotatek
{
    partial class ucAlignEdit
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pl_View = new System.Windows.Forms.Panel();
            this.pl_Control = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lstB_PathImage = new System.Windows.Forms.ListBox();
            this.btn_runjob = new System.Windows.Forms.Button();
            this.btn_openFile = new System.Windows.Forms.Button();
            this.btn_openFolder = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lstB_PathJob = new System.Windows.Forms.ListBox();
            this.btn_loadJob = new System.Windows.Forms.Button();
            this.btn_saveJob = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.pl_Control.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.OutsetDouble;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 210F));
            this.tableLayoutPanel1.Controls.Add(this.pl_View, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.pl_Control, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1201, 731);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // pl_View
            // 
            this.pl_View.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pl_View.Location = new System.Drawing.Point(6, 6);
            this.pl_View.Name = "pl_View";
            this.pl_View.Size = new System.Drawing.Size(976, 719);
            this.pl_View.TabIndex = 0;
            // 
            // pl_Control
            // 
            this.pl_Control.Controls.Add(this.groupBox2);
            this.pl_Control.Controls.Add(this.groupBox1);
            this.pl_Control.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pl_Control.Location = new System.Drawing.Point(991, 6);
            this.pl_Control.Name = "pl_Control";
            this.pl_Control.Size = new System.Drawing.Size(204, 719);
            this.pl_Control.TabIndex = 1;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lstB_PathImage);
            this.groupBox2.Controls.Add(this.btn_runjob);
            this.groupBox2.Controls.Add(this.btn_openFile);
            this.groupBox2.Controls.Add(this.btn_openFolder);
            this.groupBox2.Location = new System.Drawing.Point(3, 154);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(193, 562);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "File Image";
            // 
            // lstB_PathImage
            // 
            this.lstB_PathImage.FormattingEnabled = true;
            this.lstB_PathImage.HorizontalScrollbar = true;
            this.lstB_PathImage.Location = new System.Drawing.Point(6, 19);
            this.lstB_PathImage.Name = "lstB_PathImage";
            this.lstB_PathImage.Size = new System.Drawing.Size(178, 264);
            this.lstB_PathImage.TabIndex = 1;
            this.lstB_PathImage.SelectedIndexChanged += new System.EventHandler(this.lstB_PathImage_SelectedIndexChanged);
            // 
            // btn_runjob
            // 
            this.btn_runjob.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_runjob.Location = new System.Drawing.Point(6, 367);
            this.btn_runjob.Name = "btn_runjob";
            this.btn_runjob.Size = new System.Drawing.Size(178, 33);
            this.btn_runjob.TabIndex = 0;
            this.btn_runjob.Text = "RUN JOB";
            this.btn_runjob.UseVisualStyleBackColor = true;
            this.btn_runjob.Click += new System.EventHandler(this.btn_runjob_Click);
            // 
            // btn_openFile
            // 
            this.btn_openFile.Location = new System.Drawing.Point(6, 289);
            this.btn_openFile.Name = "btn_openFile";
            this.btn_openFile.Size = new System.Drawing.Size(178, 33);
            this.btn_openFile.TabIndex = 0;
            this.btn_openFile.Text = "Open File";
            this.btn_openFile.UseVisualStyleBackColor = true;
            this.btn_openFile.Click += new System.EventHandler(this.btn_openFile_Click);
            // 
            // btn_openFolder
            // 
            this.btn_openFolder.Location = new System.Drawing.Point(6, 328);
            this.btn_openFolder.Name = "btn_openFolder";
            this.btn_openFolder.Size = new System.Drawing.Size(178, 33);
            this.btn_openFolder.TabIndex = 0;
            this.btn_openFolder.Text = "Open Folder";
            this.btn_openFolder.UseVisualStyleBackColor = true;
            this.btn_openFolder.Click += new System.EventHandler(this.btn_openFolder_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lstB_PathJob);
            this.groupBox1.Controls.Add(this.btn_loadJob);
            this.groupBox1.Controls.Add(this.btn_saveJob);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(193, 145);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "File Job";
            // 
            // lstB_PathJob
            // 
            this.lstB_PathJob.FormattingEnabled = true;
            this.lstB_PathJob.Location = new System.Drawing.Point(6, 19);
            this.lstB_PathJob.Name = "lstB_PathJob";
            this.lstB_PathJob.Size = new System.Drawing.Size(178, 43);
            this.lstB_PathJob.TabIndex = 1;
            // 
            // btn_loadJob
            // 
            this.btn_loadJob.Location = new System.Drawing.Point(6, 68);
            this.btn_loadJob.Name = "btn_loadJob";
            this.btn_loadJob.Size = new System.Drawing.Size(178, 33);
            this.btn_loadJob.TabIndex = 0;
            this.btn_loadJob.Text = "Load Job";
            this.btn_loadJob.UseVisualStyleBackColor = true;
            this.btn_loadJob.Click += new System.EventHandler(this.btn_loadJob_Click);
            // 
            // btn_saveJob
            // 
            this.btn_saveJob.Location = new System.Drawing.Point(6, 106);
            this.btn_saveJob.Name = "btn_saveJob";
            this.btn_saveJob.Size = new System.Drawing.Size(178, 33);
            this.btn_saveJob.TabIndex = 0;
            this.btn_saveJob.Text = "Save Job";
            this.btn_saveJob.UseVisualStyleBackColor = true;
            this.btn_saveJob.Click += new System.EventHandler(this.btn_saveJob_Click);
            // 
            // ucAlignEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ucAlignEdit";
            this.Size = new System.Drawing.Size(1201, 731);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.pl_Control.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel pl_View;
        private System.Windows.Forms.Panel pl_Control;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btn_loadJob;
        private System.Windows.Forms.Button btn_saveJob;
        private System.Windows.Forms.ListBox lstB_PathJob;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox lstB_PathImage;
        private System.Windows.Forms.Button btn_openFile;
        private System.Windows.Forms.Button btn_openFolder;
        private System.Windows.Forms.Button btn_runjob;
    }
}
