using CreatePlaylist.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace CreatePlaylist {
    /// <summary>
    /// Interaction logic for CreatePlaylistUserControl.xaml
    /// </summary>
    public partial class CreatePlaylistUserControl : UserControl {
        public CreatePlaylistUserControl() {
            InitializeComponent();

            DataContext = new CreatePlaylistViewModel();
        }

        private void Add_Click(object sender, RoutedEventArgs e) {
            if (DataContext is CreatePlaylistViewModel viewModel) {
                viewModel.AddAnime();
            }
        }

        private void Create_Click(object sender, RoutedEventArgs e) {
            if (DataContext is CreatePlaylistViewModel viewModel) {
                viewModel.CreatePlaylist();
            }
        }

        private void CreateVlc_Click(object sender, RoutedEventArgs e) {
            if (DataContext is CreatePlaylistViewModel viewModel) {
                viewModel.CreateVlc();
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e) {
            if (DataContext is CreatePlaylistViewModel viewModel) {
                viewModel.ClearAnimes();
            }
        }
    }
}
