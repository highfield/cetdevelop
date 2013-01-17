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

namespace Cet.IO.DemoModbusNetduino
{
    /// <summary>
    /// Interaction logic for PageSelectRole.xaml
    /// </summary>
    public partial class PageSelectRole : Page
    {
        public PageSelectRole()
        {
            InitializeComponent();
        }


        private void MasterRoleClick(object sender, RoutedEventArgs e)
        {
            HardwareModel.Instance.Role = Role.Master;
            this.GoNextPage();
        }


        private void SlaveRoleClick(object sender, RoutedEventArgs e)
        {
            HardwareModel.Instance.Role = Role.Slave;
            this.GoNextPage();
        }


        private void GoNextPage()
        {
            this.NavigationService
                .Navigate(new Uri("/Pages/PageMediumType.xaml", UriKind.Relative));
        }

    }
}
