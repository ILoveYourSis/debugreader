using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.IO;

public partial class FlatPlayPanel : MonoBehaviour
{
    Transform _measures;

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
        _measures = transform.Find("_measuresMt");    
        init(Application.dataPath + "/../test_output.json");
    }

    int _measuresCount = 0;
    private void init(string jsonPath)
    {
        string text         = File.ReadAllText(jsonPath);
        JSONNode jo         = JSON.Parse(text); 
        JSONArray tracks   = jo["tracks"] as JSONArray;
        JSONArray measures = tracks[0]["measures"] as JSONArray;
        _measuresCount = measures.Count;
        List<Measure> measuresList = new List<Measure>(_measuresCount);

        Transform temp = _measures.GetChild(0);
        //init before copy
        temp.Find("_bk/_start").gameObject.SetActive(false);
        temp.Find("_bk/_end").gameObject.SetActive(false);
        float posX = 0;
        Transform[] measturesT = copyChild(temp, _measuresCount);
        for(int i = 0; i < _measuresCount; ++i)
        {
            Transform t     = measturesT[i];
            t.localPosition = new Vector3(posX, 0, 0);
            Measure measure = t.gameObject.AddComponent<Measure>();
            measure.init(measures[i] as JSONObject);
            posX += measure.getWidth();
            measuresList.Add(measure);
            if(i == 0) t.Find("_bk/_start").gameObject.SetActive(true);
            if(i == _measuresCount - 1) t.Find("_bk/_end").gameObject.SetActive(true);
        }

        for(int i = 0; i < measuresList.Count; ++i)
        {
        }
    }
}
