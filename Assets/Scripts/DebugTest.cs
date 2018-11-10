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
        readgp5();
	}

    void OnGUI()
    {
   
    }

    static void readgp5()
    {
        string path = Application.dataPath + "/../ALL OF THEM WITCHES.bin";
        GP5Reader reader = new GP5Reader(path);
    }

    //static void test()
    //{
    //    string writePath = Application.dataPath + "/test.bin";
    //    FileStream fs = new FileStream(writePath, FileMode.OpenOrCreate);
    //    BinaryWriter writer = new BinaryWriter(fs);
    //    FifthGuitarProSongWriter songWriter = new FifthGuitarProSongWriter(writer);
    //    songWriter.debugTest();
    //    fs.Close();
    //    writer.Close();

    //    GP5Reader reader = new GP5Reader(writePath);
    //}


}
