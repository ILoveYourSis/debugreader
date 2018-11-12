using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using SimpleJSON;

public class DebugTest : MonoBehaviour {
    const string tag = "gp5";
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
        string path        = Application.dataPath + "/../test_output.json";
        string text        = File.ReadAllText(path);
        JSONNode jo        = JSON.Parse(text);
        JSONArray measures = jo["tracks"][0]["measures"] as JSONArray;
        JSONArray beats    = measures[0]["beats"] as JSONArray;
        JSONArray voices   = beats[0]["voices"]as JSONArray;
        JSONArray notes    = voices[0]["notes"] as JSONArray;
        Logger.Log(tag, string.Format("str:{0} val:{1}", notes[0]["string"], notes[0]["value"]));
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
