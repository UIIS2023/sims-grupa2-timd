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
using System.Windows.Shapes;
using SimsProject.Domain.Model;
using SimsProject.WPF.ViewModel.Domain;

namespace SimsProject.WPF.View.Guest2
{
    /// <summary>
    /// Interaction logic for AcceptedRequestInfo.xaml
    /// </summary>
    public partial class AcceptedRequestInfo : Window
    {
        public AcceptedRequestInfo(Tour tour)
        {
            InitializeComponent();
            DataContext = new TourViewModel(tour);
        }
    }
}
