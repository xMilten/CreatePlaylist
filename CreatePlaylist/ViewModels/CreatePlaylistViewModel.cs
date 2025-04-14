using CreatePlaylist.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace CreatePlaylist.ViewModels;
public class CreatePlaylistViewModel : INotifyPropertyChanged {
    private string _animePath = string.Empty;
    private int _episodes = 0;

    public ObservableCollection<AnimeItemModel> Animes { get; set; }
    public ObservableCollection<EpisodesItemModel> Playlist { get; set; }

    public string AnimePath {
        get => _animePath;
        set {
            if (_animePath != value) {
                _animePath = value;
                NotifyPropertyChanged(nameof(AnimePath));
            }
        }
    }

    public int Episodes {
        get => _episodes;
        set {
            if (_episodes != value) {
                _episodes = value;
                NotifyPropertyChanged(nameof(Episodes));
            }
        }
    }

    public CreatePlaylistViewModel() {
        Animes = new ObservableCollection<AnimeItemModel>();
        Playlist = new ObservableCollection<EpisodesItemModel>();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void NotifyPropertyChanged(string propertyName) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void AddAnime() {
        using System.Windows.Forms.FolderBrowserDialog openFileDialog = new();
        openFileDialog.InitialDirectory = @"F:\Serien";

        if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            AnimePath = openFileDialog.SelectedPath;

        if (IsTextEmpty()) return;
        if (IsAnimeAlreadyInList()) return;

        AnimeItemModel anime = new() {
            Title = Path.GetFileName(AnimePath),
            Path = AnimePath
        };

        if (!IsAnimeHaveEpisodes(anime)) return;

        Animes.Add(anime);
        AnimePath = string.Empty;
    }

    public void CreatePlaylist() {
        if (Playlist.Count > 0) {
            if (MessageBox.Show("Möchtest du die Liste aktualisieren?", "Achtung!", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
            Playlist.Clear();
        }

        if (Animes.Count > 0) {
            for (int i = 0; i < Episodes; i++) {
                foreach (AnimeItemModel anime in Animes) {
                    if (anime.Episodes.Count > i) {
                        Playlist.Add(anime.Episodes[i]);
                    }
                }
            }
        } else {
            MessageBox.Show("Es befinden sich keine Animes in der Liste");
        }
    }

    public void CreateVlc() {
        if (Playlist.Count > 0) {
            if (MessageBox.Show("Möchtest du die Playlist erstellen?", "Achtung!", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
            string playlistPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Playlist.xspf";

            using (StreamWriter streamWriter = File.CreateText(playlistPath)) {
                streamWriter.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                streamWriter.WriteLine("<playlist xmlns=\"http://xspf.org/ns/0/\" xmlns:vlc=\"http://www.videolan.org/vlc/playlist/ns/0/\" version=\"1\">");
                streamWriter.WriteLine("\t<title>Wiedergabeliste</title>");
                streamWriter.WriteLine("\t<trackList>");

                streamWriter.Write(GetTracks());

                streamWriter.WriteLine("\t</trackList>");
                streamWriter.WriteLine("\t<extension application=\"http://www.videolan.org/vlc/playlist/0\">");

                for (int i = 0; i < Playlist.Count; i++) {
                    streamWriter.WriteLine("\t\t<vlc:item tid=\"" + i + "\"/>");
                }

                streamWriter.WriteLine("\t</extension>");
                streamWriter.Write("</playlist>");
            }

            MessageBox.Show("Playlist erstellt:\n\n" + playlistPath);

            Animes.Clear();
            Playlist.Clear();
        }
    }

    public void ClearAnimes() {
        if (Animes.Count > 0) {
            Animes.Clear();
        }
    }

    private bool IsTextEmpty() {
        if (AnimePath == string.Empty) {
            MessageBox.Show("Das Eingabefeld ist leer");
            return true;
        }

        return false;
    }

    private bool IsAnimeAlreadyInList() {
        if (Animes.Count > 0) {
            foreach (AnimeItemModel anime in Animes) {
                if (anime.Title.Equals(Path.GetFileName(AnimePath)!, StringComparison.OrdinalIgnoreCase)) {
                    MessageBox.Show("Der Anime \"" + Path.GetFileName(AnimePath)! + "\" befindet sich bereits in der Liste");
                    return true;
                }
            }
        }

        return false;
    }

    private bool IsAnimeHaveEpisodes(AnimeItemModel anime) {
        FillFilesFromPath(anime.Path, anime);

        if (anime.Episodes.Count > 0) {
            return true;
        }

        MessageBox.Show("Es wurden keine Folgen gefunden.\n" + anime.Path);
        return false;
    }

    private void FillFilesFromPath(string parentFolder, AnimeItemModel anime) {
        foreach (string folder in Directory.GetDirectories(parentFolder)) {
            FillFiles(folder, anime);
            if (Directory.GetDirectories(folder).Length > 0) {
                FillFilesFromPath(folder, anime);
            }
        }
    }

    private void FillFiles(string folder, AnimeItemModel anime) {
        foreach (string file in Directory.GetFiles(folder)) {
            anime.Episodes.Add(new EpisodesItemModel {
                Title = Path.GetFileName(file)[..Path.GetFileName(file).LastIndexOf(".")],
                Path = file,
                Anime = anime.Title
            });
        }
    }

    private string GetTracks() {
        string track = "";

        for (int i = 0; i < Playlist.Count; i++) {
            track += "\t\t<track>\n";
            track += "\t\t\t<location>";

            track += ParsePath(Playlist[i].Path);

            track += "</location>\n";
            track += "\t\t\t<extension application=\"http://www.videolan.org/vlc/playlist/0\">\n";
            track += "\t\t\t\t<vlc:id>" + i + "</vlc:id>\n";
            track += "\t\t\t</extension>\n";
            track += "\t\t</track>\n";
        }

        return track;
    }

    private static string ParsePath(string path) {
        path = path.Replace("\\", "/");
        path = path.Replace(" ", "%20");
        path = path.Replace("ä", "%C3%A4");
        path = path.Replace("ü", "%C3%BC");
        path = path.Replace("ö", "%C3%B6");
        path = path.Replace("ß", "%C3%9F");
        path = path.Replace("´", "%C2%B4");
        path = path.Replace("&", "&amp;");
        path = path.Replace("'", "&#39;");

        string preString = "file:///";

        return preString + path;
    }
}