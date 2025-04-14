using System.Collections.Generic;
using System.Xml.Serialization;

namespace CreatePlaylist.Features;
[XmlRoot("PatternList")]
public class PatternLoaderModel {
    [XmlElement("Anime")]
    public List<Anime> Animes { get; set; }
}

public struct Anime {
    [XmlAttribute]
    public string Name;
    [XmlElement("NamePattern")]
    public List<NamePatern> NamePatterns;
}

public struct NamePatern {
    public string Pattern;
    public string SeasonTitle;
    [XmlElement("SeasonNumberPattern")]
    public List<string> SeasonNumberPatterns;
    [XmlElement("EpisodeNumberPattern")]
    public List<string> EpisodeNumberPatterns;
}