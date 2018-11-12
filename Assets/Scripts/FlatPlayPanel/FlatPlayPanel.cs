using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.IO;

public partial class FlatPlayPanel : MonoBehaviour
{
    Transform _measures;

    private void Awake()
    {
        _measures = transform.Find("_measures");    
        init(Application.dataPath + "/../test.jason");
    }

    int _measuresCount = 0;
    private void init(string jsonPath)
    {
        string text = File.ReadAllText(jsonPath);
        JSONNode jo = JSON.Parse(text); 
        JSONArray measures = jo["tracks"][0]["measures"] as JSONArray;
        _measuresCount = measures.Count;
        for(int i = 0; i < _measuresCount; ++i)
        {

        }
    }
}
