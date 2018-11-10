using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using PhoneGuitarTab.Tablatures.Models;

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
    public void readSong()
    {
        _song = new Song();
        string versionInfo = readStringByte(30);
        Logger.Log("gp5", "versionInfo:" + versionInfo);
        version = versionInfo.Contains("5.10")? Version5_1:Version5;
        Logger.Log("gp5", "version:" + version);
        readInfo();
        readLyrics();
        //ok
        readPageSetUp();
        int tempoValue = readInt();
        Logger.Log(TAG, "tempo:" + tempoValue);
        if(version > Version5) SkipBytes(1);
        int keySigNature = readKeySignature();
        Logger.Log("gp5", "keySig:" + keySigNature);
        SkipBytes(3);
        //octvave 
        byte oc = readByte();
        Channel[] channels = readChannels();

        SkipBytes(42);

        int measures = readInt();
        int trackCount = readInt();
        Logger.Log("gp5", string.Format("measures:{0} trackCount:{1}", measures, trackCount));

        _br.Close();
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

    void readLyrics()
    {
        int lyricTrack = readInt();
        int from = readInt();
        string lyric = readStringInteger();
        Logger.Log(TAG, string.Format("lt:{0} from:{1} lyric:{2}", lyricTrack, from, lyric));
        for(int i = 0; i < 4; ++i)
        {
            int neo = readInt();
            string l = readStringInteger();
        }
    }

    void readPageSetUp()
    {
        SkipBytes(version > Version5? 49:30);
        for(int i = 0; i < 11; ++i)
        {
            SkipBytes(4);
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
            SkipBytes(2);
        }
        return o;
    }
}
