using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class GuitarProSongReader
{
    protected BinaryReader _br;

    protected void SkipBytes(int count)
    {
        _br.ReadBytes(count);
    }

    protected int readInt()
    {
        return _br.ReadInt32();
    }

    protected void readStringByte(out string s, out int size)
    {
        size = _br.ReadByte();
        byte[] bytes = _br.ReadBytes(size);
        s = Encoding.UTF8.GetString(bytes);
    }

    protected string readStringByteSizeOfInteger()
    {
        int length = readInt() - 1;//never use this value
        string str;
        int size;
        readStringByte(out str, out size);
        return str;
    }

    public void debugTest()
    {
        Debug.Log("read");
    }
}
