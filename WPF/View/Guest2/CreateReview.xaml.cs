using SimsProject.Domain.Model;
using SimsProject.WPF.ViewModel.Guest2;
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

namespace SimsProject.WPF.View.Guest2
{
    /// <summary>
    /// Interaction logic for ReviewTourForm.xaml
    /// </summary>
    public partial class CreateReview : Page
    {
        public CreateReview(User currentUser,TourReservation reservation,Action updateTable)
        {
            InitializeComponent();
            DataContext = new CreateReviewViewModel(currentUser,reservation,this,updateTable);
        }
    }
}
