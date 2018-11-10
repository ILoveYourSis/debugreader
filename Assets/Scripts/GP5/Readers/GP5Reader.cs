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
        SkipBytes(30);//gp5 version
        readInfo();
    }

    public void readInfo()
    {
        _song.Name = readStringByteSizeOfInteger();
        SkipBytes(1);
        _song.Artist = readStringByteSizeOfInteger();
        _song.Album = readStringByteSizeOfInteger();
        _song.Author = readStringByteSizeOfInteger();
        SkipBytes(1);
        _song.Copyright = readStringByteSizeOfInteger();
        _song.Writer = readStringByteSizeOfInteger();
        SkipBytes(1);
        int commentsCount = readInt();
        string[] comments = new string[commentsCount];
        for(int i = 0; i < commentsCount; ++i)
            comments[i] = readStringByteSizeOfInteger();
        _song.Comments = toComments(comments);

        Logger.Log("gp5", string.Format(@"Name:{0} Artist:{1} Album:{2}
 Author:{3} cpRight:{4} writer:{5} comments:{6}", _song.Name, _song.Artist, _song.Album,
 _song.Author, _song.Copyright, _song.Writer, _song.Comments));

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
