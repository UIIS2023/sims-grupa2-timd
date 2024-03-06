using System.Windows.Controls;
using MainPanelViewModel = SimsProject.WPF.ViewModel.Guide.MainPanelViewModel;

namespace SimsProject.WPF.View.Guide
{
    /// <summary>
    /// Interaction logic for MainPanel.xaml
    /// </summary>
    public partial class MainPanel : UserControl
    {
        public MainPanel(MainPanelViewModel mainPanelViewModel)
        {
            InitializeComponent();
            DataContext = mainPanelViewModel;
        }
    }
}
