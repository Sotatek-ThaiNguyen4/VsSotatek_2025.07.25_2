namespace VsSotatek.VIsionEdit
{
    partial class ucInspectEdit
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
            this.btn_run = new System.Windows.Forms.Button();
            this.btn_openFile = new System.Windows.Forms.Button();
            this.btn_openFolder = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.pl_Control.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.InsetDouble;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 340F));
            this.tableLayoutPanel1.Controls.Add(this.pl_View, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.pl_Control, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1359, 696);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // pl_View
            // 
            this.pl_View.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pl_View.Location = new System.Drawing.Point(6, 6);
            this.pl_View.Name = "pl_View";
            this.pl_View.Size = new System.Drawing.Size(1004, 684);
            this.pl_View.TabIndex = 0;
            // 
            // pl_Control
            // 
            this.pl_Control.Controls.Add(this.groupBox2);
            this.pl_Control.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pl_Control.Location = new System.Drawing.Point(1019, 6);
            this.pl_Control.Name = "pl_Control";
            this.pl_Control.Size = new System.Drawing.Size(334, 684);
            this.pl_Control.TabIndex = 1;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lstB_PathImage);
            this.groupBox2.Controls.Add(this.btn_run);
            this.groupBox2.Controls.Add(this.btn_openFile);
            this.groupBox2.Controls.Add(this.btn_openFolder);
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(328, 247);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "File Image";
            // 
            // lstB_PathImage
            // 
            this.lstB_PathImage.FormattingEnabled = true;
            this.lstB_PathImage.HorizontalScrollbar = true;
            this.lstB_PathImage.Location = new System.Drawing.Point(6, 19);
            this.lstB_PathImage.Name = "lstB_PathImage";
            this.lstB_PathImage.Size = new System.Drawing.Size(316, 134);
            this.lstB_PathImage.TabIndex = 1;
            this.lstB_PathImage.SelectedIndexChanged += new System.EventHandler(this.lstB_PathImage_SelectedIndexChanged);
            // 
            // btn_run
            // 
            this.btn_run.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_run.Location = new System.Drawing.Point(6, 198);
            this.btn_run.Name = "btn_run";
            this.btn_run.Size = new System.Drawing.Size(316, 33);
            this.btn_run.TabIndex = 0;
            this.btn_run.Text = "RUN";
            this.btn_run.UseVisualStyleBackColor = true;
            this.btn_run.Click += new System.EventHandler(this.btn_run_Click);
            // 
            // btn_openFile
            // 
            this.btn_openFile.Location = new System.Drawing.Point(6, 159);
            this.btn_openFile.Name = "btn_openFile";
            this.btn_openFile.Size = new System.Drawing.Size(119, 33);
            this.btn_openFile.TabIndex = 0;
            this.btn_openFile.Text = "Open File";
            this.btn_openFile.UseVisualStyleBackColor = true;
            this.btn_openFile.Click += new System.EventHandler(this.btn_openFile_Click);
            // 
            // btn_openFolder
            // 
            this.btn_openFolder.Location = new System.Drawing.Point(203, 159);
            this.btn_openFolder.Name = "btn_openFolder";
            this.btn_openFolder.Size = new System.Drawing.Size(119, 33);
            this.btn_openFolder.TabIndex = 0;
            this.btn_openFolder.Text = "Open Folder";
            this.btn_openFolder.UseVisualStyleBackColor = true;
            this.btn_openFolder.Click += new System.EventHandler(this.btn_openFolder_Click);
            // 
            // ucInspectEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ucInspectEdit";
            this.Size = new System.Drawing.Size(1359, 696);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.pl_Control.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel pl_View;
        private System.Windows.Forms.Panel pl_Control;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox lstB_PathImage;
        private System.Windows.Forms.Button btn_run;
        private System.Windows.Forms.Button btn_openFile;
        private System.Windows.Forms.Button btn_openFolder;
    }
}
