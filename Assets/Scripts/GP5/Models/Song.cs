using System;
using System.Collections.Generic;
using System.Linq;

namespace PhoneGuitarTab.Tablatures.Models
{
    public class Song
    {
        public string Name { get; set; }

        public string SubTitle { get; set; }

        public string Artist { get; set; }

        public string Album { get; set; }

        public string LyricAuthor { get; set; }

        public string MusicAuthor { get; set; }

        public string Date { get; set; }

        public string Copyright { get; set; }

        public string Tab { get; set; }

        public string Transcriber { get; set; }

        public string Comments { get; set; }

        public List<Track> Tracks { get; set; }

        public List<MeasureHeader> MeasureHeaders { get; set; }

        public Song()
        {
            Name = String.Empty;
            Artist = String.Empty;
            Album = String.Empty;
            LyricAuthor = String.Empty;
            Date = String.Empty;
            Copyright = String.Empty;
            Tab = String.Empty;
            Transcriber = String.Empty;
            Comments = String.Empty;
            Tracks = new List<Track>();
            MeasureHeaders = new List<MeasureHeader>();
        }

        public bool IsEmpty
        {
            get { return (!Tracks.Any() || !MeasureHeaders.Any()); }
        }
    }
}