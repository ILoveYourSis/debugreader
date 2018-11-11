using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using SimpleJSON;

public class DebugTest : MonoBehaviour {

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
        string path = Application.dataPath + "/../output.json";
        string text = File.ReadAllText(path);
        JSONNode jo = JSON.Parse(text);
        foreach(string k in jo.Keys)
        {
            Logger.Log("gp5", k);
        }
        //GP5Reader reader = new GP5Reader(path);
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
