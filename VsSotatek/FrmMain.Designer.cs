
namespace VsSotatek
{
    partial class FrmMain
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pl_frmMain = new System.Windows.Forms.Panel();
            this.timerFrmMain = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // pl_frmMain
            // 
            this.pl_frmMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pl_frmMain.Location = new System.Drawing.Point(0, 0);
            this.pl_frmMain.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pl_frmMain.Name = "pl_frmMain";
            this.pl_frmMain.Size = new System.Drawing.Size(2075, 1088);
            this.pl_frmMain.TabIndex = 0;
            // 
            // timerFrmMain
            // 
            this.timerFrmMain.Tick += new System.EventHandler(this.timerFrmMain_Tick);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2075, 1088);
            this.ControlBox = false;
            this.Controls.Add(this.pl_frmMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "FrmMain";
            this.Text = "Main";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pl_frmMain;
        private System.Windows.Forms.Timer timerFrmMain;
    }
}

