using SimsProject.WPF.ViewModel.Guest1;
using System.Windows;

namespace SimsProject.WPF.View.Guest1
{
    public partial class HelpWindow : Window
    {
        public HelpWindow()
        {
            InitializeComponent();
            DataContext = new HelpViewModel(this);
        }
    }
}
