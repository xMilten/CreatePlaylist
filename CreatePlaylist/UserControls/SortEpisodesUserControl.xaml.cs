using CreatePlaylist.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace CreatePlaylist {
    /// <summary>
    /// Interaction logic for CreateSortEpisodesUserControl.xaml
    /// </summary>
    public partial class SortEpisodesUserControl : UserControl {
        public SortEpisodesUserControl() {
            InitializeComponent();

            SortEpisodesViewModel.FillComboBox();
            SortEpisodesViewModel.ComboBoxAnimes = cmbAnimes;
            /*
            dgMoves.ItemsSource = viewModel.Files;
            */
        }

        private void Go_Click(object sender, RoutedEventArgs e) {
            SortEpisodesViewModel.Go();
        }
    }
}
