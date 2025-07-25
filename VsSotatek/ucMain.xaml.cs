using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms.Integration;
using System.ComponentModel;

namespace VsSotatek
{
    /// <summary>
    /// Interaction logic for ucMain.xaml
    /// </summary>
    public partial class ucMain : UserControl
    {
        VMFrmMain ViewModel;
        ucAuto Auto;
        ucVision Vision;
        ucCalib Calib;

        public ucMain()
        {
            InitializeComponent();
            ViewModel = new VMFrmMain();
            this.DataContext = ViewModel;
            Auto = new ucAuto();
            MainView.Child = Auto;
            Calib = new ucCalib();
            Vision = new ucVision(Auto);

            ViewModel.Version = clsDefine._VERSION_;
            ViewModel.DateMake = clsDefine._DATE_MAKE_;

        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            MainView.Child = null;

            if (ViewModel.IsAuto)
            {
                MainView.Child = Auto;
            }
            else if (ViewModel.IsVision)
            {
                MainView.Child = Vision;
            }
            else if (ViewModel.IsCalib)
            {
                MainView.Child = Calib;
            }
            else if (ViewModel.IsData)
            {

            }
            else
            {

            }
            GC.Collect();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Program Exit?.", "Exit", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                Auto.AutoThreadStop();
                FrmMain.Exit();
            }
        }
    }

    public class VMFrmMain : ViewModelBase
    {
        bool mIsAuto;
        public bool IsAuto
        {
            set
            {
                if (mIsAuto != value)
                {
                    mIsAuto = value;
                    NotifyPropertyChanged("IsAuto");
                }
            }
            get { return mIsAuto; }
        }

        bool mIsVision;
        public bool IsVision
        {
            set
            {
                if (mIsVision != value)
                {
                    mIsVision = value;
                    NotifyPropertyChanged("IsVision");
                }
            }
            get { return mIsVision; }
        }

        bool mIsCalib;
        public bool IsCalib
        {
            set
            {
                if (mIsCalib != value)
                {
                    mIsCalib = value;
                    NotifyPropertyChanged("IsCalib");
                }
            }
            get { return mIsCalib; }
        }

        bool mIsData;
        public bool IsData
        {
            set
            {
                if (mIsData != value)
                {
                    mIsData = value;
                    NotifyPropertyChanged("IsData");
                }
            }
            get { return mIsData; }
        }

        bool mIsLog;
        public bool IsLog
        {
            set
            {
                if (mIsLog != value)
                {
                    mIsLog = value;
                    NotifyPropertyChanged("IsLog");
                }
            }
            get { return mIsLog; }
        }

        string version;
        public string Version
        {
            get { return version; }
            set { version = value; }
        }

        string dateMake;
        public string DateMake
        {
            get { return dateMake; }
            set { dateMake = value; }
        }

        public VMFrmMain()
        {

        }

    }
}
