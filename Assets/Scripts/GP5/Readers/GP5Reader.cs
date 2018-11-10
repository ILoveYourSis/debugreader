using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using PhoneGuitarTab.Tablatures.Models;

public class GP5Reader : GuitarProSongReader
{
    public GP5Reader(string filePath)
    {
        FileStream fs = new FileStream(filePath, FileMode.Open);
        _br = new BinaryReader(fs);
        readSong();
    }

    Song _song;
    public void readSong()
    {
        _song = new Song();
        SkipBytes(1);
        SkipBytes(30);//gp5 version
        readInfo();

        _br.Close();
    }

    public void readInfo()
    {
        _song.Name = readStringByteSizeOfInteger();
        SkipBytes(5);
        _song.Artist = readStringByteSizeOfInteger();
        _song.Album = readStringByteSizeOfInteger();
        _song.Author = readStringByteSizeOfInteger();
        SkipBytes(5);
        _song.Copyright = readStringByteSizeOfInteger();
        _song.Writer = readStringByteSizeOfInteger();
        SkipBytes(5);
        int commentsCount = readInt();
        string[] comments = new string[commentsCount];
        for(int i = 0; i < commentsCount; ++i)
            comments[i] = readStringByteSizeOfInteger();
        _song.Comments = toComments(comments);
        Logger.Log("copyright:" + _song.Copyright);

    }

    private string toComments(string[] comments)
    {
        if(comments == null || comments.Length == 0) return "";
        StringBuilder sb = new StringBuilder();
        for(int i = 0; i < comments.Length; ++i) 
            sb.Append(comments[i]);
        return sb.ToString();
    }
}
