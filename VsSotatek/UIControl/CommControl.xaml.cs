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
    /// Interaction logic for CommControl.xaml
    /// </summary>
    public partial class CommControl : UserControl
    {
        public CommControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty PCAliveProperty =
            DependencyProperty.Register("PCAlive", typeof(bool), typeof(CommControl));

        public bool PCAlive
        {
            get { return (bool)GetValue(PCAliveProperty); }
            set { SetValue(PCAliveProperty, value); }
        }




    }
}
