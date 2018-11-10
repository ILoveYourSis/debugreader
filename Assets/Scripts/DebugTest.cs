using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class DebugTest : MonoBehaviour {

    AndroidJavaObject jo;
	// Use this for initialization
	void Start () {
		//jo = new AndroidJavaObject("com.example.mylibrary.JarDebug");

	}

    void OnGUI()
    {
   
    }

    [UnityEditor.MenuItem("Tools/readgp5")]
    static void readgp5()
    {
        string path = Application.dataPath + "/ALL OF THEM WITCHES.gp5";
        GP5Reader reader = new GP5Reader(path);
    }

    [UnityEditor.MenuItem("Tools/DebugTest")]
    static void test()
    {
        string writePath = Application.dataPath + "/test.bin";
        FileStream fs = new FileStream(writePath, FileMode.OpenOrCreate);
        BinaryWriter writer = new BinaryWriter(fs);
        FifthGuitarProSongWriter songWriter = new FifthGuitarProSongWriter(writer);
        songWriter.debugTest();
        fs.Close();
        writer.Close();

        GP5Reader reader = new GP5Reader(writePath);
    }


}
