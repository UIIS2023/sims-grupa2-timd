using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using SimsProject.Domain.Model;
using SimsProject.WPF.ViewModel.Guest2;

namespace SimsProject.WPF.View.Guest2
{
    /// <summary>
    /// Interaction logic for PartialRequest.xaml
    /// </summary>
    public partial class PartialRequest : Page
    {
        public PartialRequest(User LoggedInUser , ObservableCollection<TourRequest> Requests )
        {
            InitializeComponent();
            this.DataContext = new PartialRequestViewModel(LoggedInUser,Requests,this);

        }
    }
}
