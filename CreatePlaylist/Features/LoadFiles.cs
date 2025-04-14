using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Xml;
using System.Xml.Serialization;

namespace CreatePlaylist.Features;
public class LoadFiles {
    private const string DOWNLOADPATH = @"F:\Download\";
    private const string XMLPATH = @"F:\anime-loads.org\";
    private const string PATTERNPATH = XMLPATH + "Patterns.xml";
    private readonly string _anime;
    private readonly XmlSerializer _serializerEpisodes;
    private readonly XmlSerializer _serializerPatterns;

    public LoadFiles(string anime) {
        _anime = anime;
        _serializerEpisodes = new(typeof(EpisodeListLoaderModel));
        _serializerPatterns = new(typeof(PatternLoaderModel));
    }

    public List<EpisodePathAndNumber> GetEpisodePathAndNumbers() {
        List<EpisodePathAndNumber> episodePathAndNumbers = new();
        List<NamePatern> patterns = GetAnimeNamePatternList();

        if (patterns.Count < 1) return null;

        string[] downloadedFolders = Directory.GetDirectories(DOWNLOADPATH);

        foreach (string downloadedFolder in downloadedFolders) {
            foreach (NamePatern pattern in patterns) {
                if (Regex.IsMatch(Path.GetFileName(downloadedFolder), pattern.Pattern, RegexOptions.IgnoreCase)) {
                    string episodePath = GetEpisode(downloadedFolder);

                    if (episodePath == string.Empty) {
                        MessageBoxResult result = MessageBox.Show(
                            $"Die Datei wurde im folgenden Pfad nicht gefunden:\n{downloadedFolder}\n\nSoll der Ordner gelöscht werden?",
                            "Keine Datei im Pfad gefunden!",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question
                        );
                        switch (result) {
                            case MessageBoxResult.Yes:
                                FileSystem.DeleteDirectory(downloadedFolder, UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
                                break;
                            case MessageBoxResult.No:
                                break;
                        }
                        break;
                    }

                    string seasonTitle = pattern.SeasonTitle;
                    string seasonNr = GetNumber(episodePath, pattern.SeasonNumberPatterns);
                    string episodeNr = GetNumber(episodePath, pattern.EpisodeNumberPatterns);

                    EpisodePathAndNumber episodePathAndNumber = new() {
                        Path = episodePath,
                        SeasonTitle = seasonTitle,
                        EpisodeNr = episodeNr
                    };
                    episodePathAndNumber.SeasonNr = seasonNr == string.Empty ? string.Empty : seasonNr;

                    episodePathAndNumbers.Add(episodePathAndNumber);
                    break;
                }
            }
        }

        return episodePathAndNumbers;
    }

    private List<NamePatern> GetAnimeNamePatternList() {
        PatternLoaderModel patternLoader;
        using (StreamReader reader = new(PATTERNPATH)) {
            patternLoader = (PatternLoaderModel)_serializerPatterns.Deserialize(reader);
        }

        return patternLoader.Animes.Find(anime => anime.Name == _anime).NamePatterns;
    }

    public List<Season> GetSeasonsWithEpisodes() {
        EpisodeListLoaderModel episodeListLoader = null;
        try {
            using (StreamReader reader = new(XMLPATH + _anime + ".xml")) {
                episodeListLoader = (EpisodeListLoaderModel)_serializerEpisodes.Deserialize(reader);
            }
        } catch (Exception e) {
            string exceptionMessage = XMLPATH + _anime + ".xml\n\n";
            exceptionMessage += e.GetBaseException().Message;
            MessageBox.Show(exceptionMessage);
        }

        return episodeListLoader.Seasons;
    }

    private static string GetEpisode(string folder) {
        string episode = string.Empty;
        string[] files = Directory.GetFiles(folder);
        string[] folders = Directory.GetDirectories(folder);

        if (Directory.GetFiles(folder).Length > 0) {
            episode = GetFile(files);
        }
        if (Directory.GetDirectories(folder).Length > 0 && episode == string.Empty) {
            episode = GetFileFromFolders(folders);
        }
        return episode;
    }

    private static string GetFile(string[] files) {
        string[] formats = { ".mp4", ".mkv", ".avi" };
        string episode = string.Empty;

        foreach (string file in files) {
            if (formats.Any(format => file.Contains(format) && !file.Contains("sample"))) {
                episode = file;
                break;
            }
        }
        return episode;
    }

    private static string GetFileFromFolders(string[] folders) {
        string episode = string.Empty;

        foreach (string folder in folders) {
            string[] files = Directory.GetFiles(folder);
            string[] subFolders = Directory.GetDirectories(folder);

            if (Directory.GetFiles(folder).Length > 0) {
                episode = GetFile(files);
                break;
            }
            if (Directory.GetDirectories(folder).Length > 0 && episode == string.Empty) {
                episode = GetFileFromFolders(subFolders);
            }
        }

        return episode;
    }

    private string GetNumber(string episodePath, List<string> numberPatterns) {
        string seasonNr = string.Empty;
        foreach (string pattern in numberPatterns) {
            if (Regex.IsMatch(episodePath, pattern)) {
                seasonNr = Regex.Match(episodePath, pattern).Groups[1].ToString();
                break;
            }
        }
        return seasonNr;
    }

    public struct EpisodePathAndNumber {
        public string Path;
        public string SeasonTitle;
        public string SeasonNr;
        public string EpisodeNr;
    }
}