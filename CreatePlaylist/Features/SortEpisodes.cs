using CreatePlaylist.ViewModels;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using static CreatePlaylist.Features.LoadFiles;

namespace CreatePlaylist.Features;
public class SortEpisodes {
    private int episodeCount;
    private readonly string _anime;
    private const string DESTPATH = @"F:\Serien\";

    public SortEpisodes(string anime) {
        _anime = anime;
    }

    public void Start() {
        LoadFiles();
    }

    private void LoadFiles() {
        LoadFiles loadFiles = new(_anime);

        List<EpisodePathAndNumber> episodePathsAndNumbers = loadFiles.GetEpisodePathAndNumbers();

        if (episodePathsAndNumbers.Count < 1) {
            MessageBox.Show($"Die Pfade von {_anime} konnten nicht gefunden werden.");
            return;
        }

        episodeCount = episodePathsAndNumbers.Count;

        EpisodeListLoaderModel episodeListLoader = new() { Seasons = loadFiles.GetSeasonsWithEpisodes() };

        if (episodeListLoader.Seasons.Count < 1) {
            MessageBox.Show($"Die Episoden Title von {_anime} konnten aus dessen Liste nicht geladen werden.");
            return;
        }

        foreach (EpisodePathAndNumber episodePathAndNumber in episodePathsAndNumbers) {
            RenameEpisodeBySeasonNr(episodePathAndNumber, episodeListLoader);
        }
    }

    private void RenameEpisodeBySeasonNr(EpisodePathAndNumber episodePathAndNumber, EpisodeListLoaderModel episodeListLoader) {
        Season season = episodeListLoader.Seasons.FirstOrDefault(season => season.Title.Equals(episodePathAndNumber.SeasonTitle));

        season ??= episodePathAndNumber.SeasonNr == string.Empty ?
                FindSeasonByEpisodeNr(episodePathAndNumber, episodeListLoader) :
                episodeListLoader.Seasons.FirstOrDefault(season => season.Nr.Equals(episodePathAndNumber.SeasonNr));


        if (season == null) {
            MessageBox.Show($"Die Staffel-Nummer \"{episodePathAndNumber.SeasonNr}\" konnte nicht gefunden werden.\n\n{episodePathAndNumber.Path}");
            return;
        }

        Episode episode = season.Episodes.FirstOrDefault(episode => episode.Nr.Equals(episodePathAndNumber.EpisodeNr));

        if (episode == null) {
            MessageBox.Show($"Die Episoden-Nummer \"{episodePathAndNumber.EpisodeNr}\" konnte nicht gefunden werden.\n\n{episodePathAndNumber.Path}");
            return;
        }

        string seasonTitle = RemoveIllegalMarks(season.Title);
        string episodeTitle = RemoveIllegalMarks(episode.Title);
        string episodePath = episodePathAndNumber.Path;
        string format = episodePath.Substring(episodePath.Length - 4);
        string renamedEpisode = episode.Nr + " - " + episodeTitle + format;
        string destPath = DESTPATH + _anime + "\\" + season.Nr + " - " + seasonTitle + "\\";
        if (!Directory.Exists(destPath)) {
            Directory.CreateDirectory(destPath);
        }
        string episodeDestPath = destPath + renamedEpisode;

        MoveEpisode(episodePath, episodeDestPath);
    }

    private Season FindSeasonByEpisodeNr(EpisodePathAndNumber episodePathAndNumber, EpisodeListLoaderModel episodeListLoader) {
        foreach (Season season in episodeListLoader.Seasons) {
            if (season.Episodes.Any(episode => episode.Nr.Equals(episodePathAndNumber.EpisodeNr))) {
                return season;
            }
        }

        return null;
    }

    private string RemoveIllegalMarks(string title) {
        Dictionary<string, string> illegalMarks = new() {
            { "\\", "" },
            { "/", "" },
            { ":", "" },
            { "*", "" },
            { "\"", "" },
            { "<", "" },
            { ">", "" },
            { "|", "" }
        };

        foreach (string key in illegalMarks.Keys) {
            title = title.Replace(key, "");
        }

        title = title.Replace("?", "¿");

        return title;
    }

    private void MoveEpisode(string source, string dest) {
        Dictionary<string, string> bla = new() { { source, dest } };

        Dispatcher dispatcher = Application.Current.Dispatcher;
        dispatcher.BeginInvoke(() => {
            try {
                // Ladebalken
                SortEpisodesViewModel.Percentage += (double)100 / episodeCount;
                SortEpisodesViewModel.Files.Add(new MoveModel { Source = source, Destination = dest });
                File.Move(source, dest);
                string mainFolder = Path.GetDirectoryName(source);
                while (true) {
                    if (!Path.GetDirectoryName(mainFolder).Equals(@"F:\Download"))
                        mainFolder = Path.GetDirectoryName(mainFolder);
                    else
                        break;
                }
                FileSystem.DeleteDirectory(mainFolder, UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
            } catch (Exception ex) {
                MessageBox.Show($"{ ex}\n\n{source}\n{dest}");
            }
        });
    }
}

public class MoveModel {
    public string Source { get; set; }
    public string Destination { get; set; }
}