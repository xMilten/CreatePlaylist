using System.Collections.Generic;
using System.Xml.Serialization;

namespace CreatePlaylist.Features;
[XmlRoot("EpisodeList")]
public class EpisodeListLoaderModel {
    [XmlElement("Season")]
    public List<Season> Seasons { get; set; }
}

public class Season {
    [XmlAttribute]
    public string Nr;
    [XmlAttribute]
    public string Title;
    [XmlElement("Episode")]
    public List<Episode> Episodes;
}

public class Episode {
    [XmlAttribute]
    public string Nr;
    [XmlAttribute]
    public string Title;
}