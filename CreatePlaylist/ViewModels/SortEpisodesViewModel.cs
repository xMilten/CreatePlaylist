using CreatePlaylist.Features;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace CreatePlaylist.ViewModels;

public static class SortEpisodesViewModel {
    private const string PATH = @"F:\Download\";
    private static double _percentage = 0;

    public static List<string> Animes { get; } = new List<string>();
    public static ComboBox ComboBoxAnimes { get; set; }
    public static ObservableCollection<MoveModel> Files { get; } = new();

    public static double Percentage {
        get => _percentage;
        set {
            if (_percentage != value) {
                _percentage = value;
                NotifyStaticPropertyChanged();
            }
        }
    }

    public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

    public static void NotifyStaticPropertyChanged([CallerMemberName] string propertyName = "") {
        StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(propertyName));
    }

    public static void Reset() {
        if (Percentage > 0) {
            Percentage = 0;
        }
        if (Files.Count > 0) {
            Files.Clear();
        }
    }

    public static void FillComboBox() {
        string[] files = Directory.GetFiles(@"F:\anime-loads.org");
        foreach (string file in files) {
            if (file.Contains(".xml") && !file.Contains("Patterns.xml")) {
                Animes.Add(Path.GetFileNameWithoutExtension(file));
            }
        }
    }

    public static void Go() {
        if (IsComboBoxUnselected()) return;
        Reset();
        SortEpisodes sortEpisodes = new(ComboBoxAnimes.Text);
        sortEpisodes.Start();
    }

    private static bool IsComboBoxUnselected() {
        if (ComboBoxAnimes.SelectedIndex == -1) {
            MessageBox.Show("Es wurde kein Anime im Dropdown-Menü ausgewählt");
            return true;
        }

        return false;
    }
}