using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.IO;
using System;

public partial class FlatPlayPanel : MonoBehaviour
{
    struct Chord
    {
        public string chordName;
        public int[] frets;
        public bool isInChord(int guitarStr, int value) { return frets[guitarStr] == value; }
    }

    /// <summary>
    /// all the chords used in this song
    /// </summary>
    static Dictionary<string, Chord> _name2Chord = null;


    static Transform[] copyChild(Transform temp, int count)
    {
        Transform[] o = new Transform[count];
        temp.localPosition = Vector3.zero;
        temp.localRotation = Quaternion.identity;
        temp.localScale = Vector3.one;
        temp.gameObject.SetActive(true);
        o[0] = temp;
        for (int i = 1; i < count; ++i)
        {
            Transform t = Instantiate<Transform>(temp, temp.parent);
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
            t.gameObject.SetActive(true);
            o[i] = t;
        }
        return o;
    }

    private void Awake()
    {
        init(Application.dataPath + "/../test_output.json");
    }


    private void init(string jsonPath)
    {
        Transform measuresMt = transform.Find("_measuresMt");
        string text         = File.ReadAllText(jsonPath);
        _name2Chord         = new Dictionary<string, Chord>();
        Chord[] chords = parseChords(text);
        for(int i = 0; i < chords.Length; ++i)
            if(!_name2Chord.ContainsKey(chords[i].chordName))_name2Chord.Add(chords[i].chordName, chords[i]);

        JSONNode jo         = JSON.Parse(text); 
        JSONArray tracks   = jo["tracks"] as JSONArray;
        JSONArray measures = tracks[0]["measures"] as JSONArray;
        int measuresCount  = measures.Count;
        List<Measure> measuresList = new List<Measure>(measuresCount);

        Transform temp = measuresMt.GetChild(0);
        //init before copy
        temp.Find("_bk/_start").gameObject.SetActive(false);
        temp.Find("_bk/_end").gameObject.SetActive(false);
        float posX = 0;
        Transform[] measturesT = copyChild(temp, measuresCount);
        for(int i = 0; i < measuresCount; ++i)
        {
            Transform t     = measturesT[i];
            t.localPosition = new Vector3(posX, 0, 0);
            Measure measure = t.gameObject.AddComponent<Measure>();
            measure.init(measures[i] as JSONObject);
            posX += measure.getWidth();
            measuresList.Add(measure);
            if(i == 0) t.Find("_bk/_start").gameObject.SetActive(true);
            if(i == measuresCount - 1) t.Find("_bk/_end").gameObject.SetActive(true);
        }
    }

    Chord[] parseChords(string txt)
    {
        if(!txt.StartsWith("Chord:")) return null;
        List<Chord> chordsList = new List<Chord>();
        int tabStartIdx = txt.IndexOf('{');
        string chordString = txt.Substring(0, tabStartIdx - 1);
        string[] splits = chordString.Split('\n');
        for(int i= 0; i< splits.Length; i += 8)
        {
            Chord chord = new Chord();
            chord.chordName = splits[i].Replace("Chord:", "").Trim();
            chord.frets = new int[7];
            for(int strIdx = 0; strIdx < 7; ++strIdx)
            {
                string fret = splits[i + 1 + strIdx];
                Logger.Log("gp5", fret);
                chord.frets[strIdx] = int.Parse(fret.Replace("fret:", "").Trim());
            }
            chordsList.Add(chord);
        }
        return chordsList.ToArray();
    }
}
