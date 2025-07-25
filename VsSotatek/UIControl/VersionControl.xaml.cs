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

namespace VsSotatek.UIControl
{
    /// <summary>
    /// Interaction logic for VersionControl.xaml
    /// </summary>
    public partial class VersionControl : UserControl
    {
        public VersionControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty VersionProperty =
            DependencyProperty.Register("Version", typeof(string), typeof(VersionControl), new PropertyMetadata(null));

        public static readonly DependencyProperty DateMakeProperty =
            DependencyProperty.Register("DateMake", typeof(string), typeof(VersionControl), new PropertyMetadata(null));

        public string Version
        {
            get { return (string)GetValue(VersionProperty); }
            set { SetValue(VersionProperty, value); }
        }

        public object DateMake
        {
            get { return (string)GetValue(DateMakeProperty); }
            set { SetValue(DateMakeProperty, value); }
        }
    }
}
