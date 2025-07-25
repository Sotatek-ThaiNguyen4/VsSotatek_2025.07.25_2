using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using VsSotatek.VIsionEdit;

namespace VsSotatek
{
    /// <summary>
    /// Interaction logic for ucVision.xaml
    /// </summary>
    public partial class ucVision : UserControl
    {
        WindowsFormsHost host;
        ucAlignEdit AlignEdit;
        ucInspectEdit InspectEdit;
        ucAuto mAuto;

        public ucVision(ucAuto _mAuto)
        {
            InitializeComponent();
            host = new WindowsFormsHost();
            AlignEdit = new ucAlignEdit();
            InspectEdit = new ucInspectEdit(mAuto);
            MenuChange(AlignEdit);
            this.mAuto = _mAuto;
        }

        private void btnAlignEdit_Click(object sender, RoutedEventArgs e)
        {
            MenuChange(AlignEdit);
        }

        private void btnInspectEdit_Click(object sender, RoutedEventArgs e)
        {
            MenuChange(InspectEdit);
        }

        private void btnUpdateEdit_Click(object sender, RoutedEventArgs e)
        {
            //? Update để sau k cần reset
        }



        private void MenuChange(System.Windows.Forms.Control _control)
        {
            if (MainView.Child == host && host.Child == _control) return;
            if (host.Child is Component oldComponent)
                oldComponent.Dispose();
            MainView.Child = null;
            host.Child = null;

            if (_control is ucAlignEdit)
            {
                host.Child = new ucAlignEdit();
                MainView.Child = host;
            }
            else if (_control is ucInspectEdit)
            {
                host.Child = new ucInspectEdit(mAuto);
                MainView.Child = host;
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
