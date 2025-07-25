using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace VsSotatek
{
    public partial class FrmMain : Form
    {
        ucMain main;
        public FrmMain()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            main = new ucMain();
            ElementHost host = new ElementHost();
            host.Dock = DockStyle.Fill;
            host.Child = main;
            pl_frmMain.Controls.Add(host);
            timerFrmMain.Start();
        }

        public static void Exit()
        {
            System.Diagnostics.Process.GetCurrentProcess().Close();
            Application.Exit();
        }

        private void timerFrmMain_Tick(object sender, EventArgs e)
        {
            clsSupport.MakeFolder();
        }
    }

    public abstract class BindingObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }

    public class ViewModelBase : BindingObject
    {

    }
}
