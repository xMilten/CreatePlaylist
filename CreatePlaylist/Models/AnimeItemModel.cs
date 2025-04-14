using System.Collections.Generic;

namespace CreatePlaylist.Models {
    public class AnimeItemModel {
        public string Title { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public List<EpisodesItemModel> Episodes { get; set; } = new List<EpisodesItemModel>();
        public int Count { get => Episodes.Count; }
    }
}