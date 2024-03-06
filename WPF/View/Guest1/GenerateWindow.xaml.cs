using SimsProject.WPF.ViewModel.Guest1;
using System.Windows;
using SimsProject.Domain.Model;

namespace SimsProject.WPF.View.Guest1
{
    public partial class GenerateWindow : Window
    {
        public GenerateWindow(User currentUser)
        {
            InitializeComponent();
            DataContext = new GenerateWindowViewModel(this, currentUser);
        }
    }
}
