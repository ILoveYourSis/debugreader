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

    protected int readInt() { return _br.ReadInt32(); }

    protected byte readByte() { return _br.ReadByte(); } 

    protected byte readyUnsignedByte() {return _br.ReadByte(); }
    protected string readStringByte(int size)
    {
        return readString(size, readyUnsignedByte());
    }

    protected string readString(int size, byte len)
    {
        byte[] bytes  = _br.ReadBytes(size > 0? size: len);
        return Encoding.UTF8.GetString(bytes);
    }

    protected string readStringInteger()
    {
        int size = readInt();
        byte[] bytes = _br.ReadBytes(size);
        return Encoding.UTF8.GetString(bytes);
    }

    protected string readStringByteSizeOfInteger()
    {
        return readStringByte(readInt() - 1);
    }

    public void debugTest()
    {
        Debug.Log("read");
    }
}
