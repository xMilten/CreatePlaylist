using System.Windows;

namespace CreatePlaylist {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private CreatePlaylistUserControl _createPlaylistUserControl = null;
        private SortEpisodesUserControl _sortEpisodesUserControl = null;

        public MainWindow() {
            InitializeComponent();
        }

        private void CreatePlaylist_Click(object sender, RoutedEventArgs e) {
            if (_createPlaylistUserControl == null) {
                _createPlaylistUserControl = new CreatePlaylistUserControl();
            }

            contentControl.Content = _createPlaylistUserControl;
        }

        private void CreateSortEpisodes_Click(object sender, RoutedEventArgs e) {
            if (_sortEpisodesUserControl == null) {
                _sortEpisodesUserControl = new SortEpisodesUserControl();
            }

            contentControl.Content = _sortEpisodesUserControl;
        }
    }
}
