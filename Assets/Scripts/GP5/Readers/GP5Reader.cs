using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using PhoneGuitarTab.Tablatures.Models;
using Color = PhoneGuitarTab.Tablatures.Models.Color;
using SimpleJson;
public class GP5JaonReader
{
}

public class GP5Reader : GuitarProSongReader
{
    const int Version5   = 50;
    const int Version5_1 = 51;
    const string TAG = "gp5";
    static readonly string[] PageSetupLines =
    {
        "%TITLE%", "%SUBTITLE%", "%ARTIST%", "%ALBUM%", "Words by %WORDS%",
        "Music by %MUSIC%", "Words & Music by %WORDSMUSIC%",
        "Copyright %COPYRIGHT%",
        "All Rights Reserved - International Copyright Secured",
        "Page %N%/%P%", "Moderate"
    };

    public GP5Reader(string filePath)
    {
        FileStream fs = new FileStream(filePath, FileMode.Open);
        _br = new BinaryReader(fs);
        readSong();
    }

    void dumpMem()
    {
        for(int i = 0; i < 30; ++i)
            Logger.Log(TAG, string.Format("idx:{0} value:{1}", i, readInt()));
    }

    Song _song;
    int version;
    Channel[] channels;
    public void readSong()
    {
        _song = new Song();
        string versionInfo = readStringByte(30);
        Logger.Log("gp5", "versionInfo:" + versionInfo);
        version = versionInfo.Contains("5.10")? Version5_1:Version5;
        Logger.Log("gp5", "version:" + version);
        readInfo();
        int lyricTrack = readInt();
        string lyric = readLyrics();
        //ok
        readPageSetUp();
        int tempoValue = readInt();
        Logger.Log(TAG, "tempo:" + tempoValue);
        if(version > Version5) skip(1);
        int keySigNature = readKeySignature();
        Logger.Log("gp5", "keySig:" + keySigNature);
        skip(3);
        //octvave 
        byte oc = readByte();
        channels = readChannels();

        skip(42);

        int measures = readInt();
        int trackCount = readInt();
        Logger.Log("gp5", string.Format("measures:{0} trackCount:{1}", measures, trackCount));
        //dump mem OK
        if(_song.MeasureHeaders == null) _song.MeasureHeaders = new List<MeasureHeader>();
        TimeSignature timeSignature = new TimeSignature();
        for(int i = 0; i < measures; ++i)
        {
            if(i > 0) skip(1);
            byte flags = readyUnsignedByte();
            MeasureHeader header = new MeasureHeader();
            header.Number = i + 1;
            header.Start = 0;
            //header.Tempo = 120;
            header.IsRepeatOpen = (flags & 0x04) != 0;
            if((flags & 0x01) != 0) timeSignature.Numerator = readByte();
            if((flags & 0x02) != 0) timeSignature.Denominator.Value = readByte();
            //parse timesignature
            if((flags & 0x08) != 0) header.RepeatClose = (readByte() & 0xff) - 1;
            if((flags & 0x20) != 0)
            {
                Marker m = new Marker();
                m.Measure = header.Number;
                m.Title = readStringByteSizeOfInteger();
                m.Color = readColor();
            }
            if ((flags & 0x10) != 0) header.RepeatAlternative = readyUnsignedByte();
            if ((flags & 0x40) != 0) {
              int ks = readKeySignature();
              skip(1);
            }
            if ((flags & 0x01) != 0 || (flags & 0x02) != 0) skip(4);
            if ((flags & 0x10) == 0) skip(1);
            byte tripletFeel = readByte();
            if (tripletFeel == 1) header.TripletFeel = MeasureHeader.TripletFeelEighth;
            else if (tripletFeel == 2) header.TripletFeel = MeasureHeader.TripletFeelSixteenth;
            else header.TripletFeel = MeasureHeader.TripletFeelNone;
            _song.MeasureHeaders.Add(header);
        }
        //dumpmem OK
        if(_song.Tracks == null) _song.Tracks = new List<Track>();
        for(int i = 1; i <= trackCount; ++i)
        {
            Track track = new Track();
            readyUnsignedByte();
            if (i == 1 || version == Version5) skip(1);
            track.Number = i;
            track.lyric = i == lyricTrack ? lyric : null;
            track.Name = readStringByte(40);
            if(track.Strings == null) track.Strings = new List<GuitarString>();
            var stringCount = readInt();
            for (int j = 0; j < 7; ++j) {
                var tuning = readInt();
                if (stringCount > j) {
                GuitarString gs = new GuitarString();
                gs.Number = j + 1;
                gs.Value = tuning;
                    track.Strings.Add(gs);
                }
            }
            readInt();
            readChannel(track);
            readInt();
            track.Offset = readInt();
            track.Color = readColor();
            if(track.Measures == null) track.Measures = new List<Measure>();
            skip(version > Version5 ? 49 : 44);
            if (version > Version5) {
                readStringByteSizeOfInteger();
                readStringByteSizeOfInteger();
            }
            _song.Tracks.Add(track);
        }
        skip(version == Version5? 2:1);
        //dumpMem(); ok
        int temp = tempoValue;
        float start = Duration.QuarterTime;
        for(int i = 0; i < measures; ++i)
        {
            MeasureHeader header = _song.MeasureHeaders[i];
            //header.Start = start;
            for(int j = 0; j < trackCount; ++j)
            {
                Track track = _song.Tracks[j];
                Measure measure = new Measure();
                measure.Header = header;
                measure.start = start;
                measure.Beats = new List<Beat>();
                readMeasure(measure, track, temp);
                skip(1);
            }
            header.Tempo.Value = temp;
            start += header.getLength();
        }
       

        _br.Close();
    }

    void readMeasure(Measure measure, Track track, int temp)
    {
        for(int voice = 0; voice < 2; ++voice)
        {
            float start = measure.start;
            int beats = readInt();
            for(int i = 0; i < beats; ++i)
            {
                //start += readBeat(start, measure, track, temp, voice);
            }
        }
    }

    Beat readBeat(float start, Measure measure, Track track, int tempo, int voice)
    {
        return null;
    }

    void readChannel(Track track)
    {
        var gmChannel1 = readInt() -1;
        var gmChannel2 = readInt() -1;
        Logger.Log(TAG, "TODO readChannel");
    }

    public void readInfo()
    {
        _song.Name = readStringByteSizeOfInteger();
        _song.SubTitle = readStringByteSizeOfInteger();
        _song.Artist = readStringByteSizeOfInteger();
        _song.Album = readStringByteSizeOfInteger();
        _song.LyricAuthor = readStringByteSizeOfInteger();
        _song.MusicAuthor = readStringByteSizeOfInteger();
        _song.Copyright = readStringByteSizeOfInteger();
        _song.Tab = readStringByteSizeOfInteger();
        Logger.Log(TAG, "tab:" + _song.Tab);
        string instructions = readStringByteSizeOfInteger();
        int commentsCount = readInt();
        string[] comments = new string[commentsCount];
        for(int i = 0; i < commentsCount; ++i)
            comments[i] = readStringByteSizeOfInteger();
        _song.Comments = toComments(comments);
        Logger.Log("gp5", string.Format("lAuthor:{0} mAuthor:{1} cp:{2}", _song.LyricAuthor, _song.MusicAuthor, _song.Copyright));
    }

    string readLyrics()
    {
        int from = readInt();
        string lyric = readStringInteger();
        for(int i = 0; i < 4; ++i)
        {
            int neo = readInt();
            string l = readStringInteger();
        }
        return lyric;
    }

    void readPageSetUp()
    {
        skip(version > Version5? 49:30);
        for(int i = 0; i < 11; ++i)
        {
            skip(4);
            readStringByte(0);
        }
    }

    int readKeySignature()
    {
        int k = readByte();
        if(k < 0) k = 7 - k;
        return k;
    }

    private string toComments(string[] comments)
    {
        if(comments == null || comments.Length == 0) return "";
        StringBuilder sb = new StringBuilder();
        for(int i = 0; i < comments.Length; ++i) 
            sb.Append(comments[i]);
        return sb.ToString();
    }

    Channel[] readChannels()
    {
        Channel[] o = new Channel[64];
        for(int i = 0; i < 64; ++i)
        {
            Channel channel = new Channel();
            channel.Program = readInt();
            channel.Volume = readByte();
            channel.Balance = readByte();
            channel.Chorus = readByte();
            channel.Reverb = readByte();
            channel.Phaser = readByte();
            channel.Tremolo = readByte();
            channel.bank = i == 9? "default percussion bank": "default bank";
            if(channel.Program < 0) channel.Program = 0;
            o[i] = channel;
            //Logger.Log(TAG, string.Format("%{0} %{1}", i, channel.toString()));
            skip(2);
        }
        return o;
    }

    Color readColor()
    {
        Color o = new Color();
        o.R = readyUnsignedByte();
        o.G = readyUnsignedByte();
        o.B = readyUnsignedByte();
        skip(1);
        return o;
    }
}
