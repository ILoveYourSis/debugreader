using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using UnityEngine.UI;
public partial class FlatPlayPanel
{
    class Measure: MonoBehaviour
    {
        const string tag = "gp5";

        /// <summary>
        /// stringIdx 0-5
        /// </summary>
        /// <param name="stringIdx"></param>
        /// <returns></returns>
        static string getStringName(int stringIdx)
        {
            string[] names = { "0", "1", "2", "3", "4", "5"};
            return names[stringIdx];
        }

        /// <summary>
        /// strIdx 0-5;
        /// </summary>
        /// <param name="stringIdx"></param>
        /// <returns></returns>
        float getStringPosY(int stringIdx)
        {
            const float Y0 = 63;
            const float INTERVAL = 2 * Y0 / 5;
            return Y0 - INTERVAL * stringIdx;
        }

            const int BEAT0_OFFSET       = 25;
            const int BEAT_DEFAULT_WIDTH = 60;
        public void init(JSONObject measure)
        {
            JSONArray beatsArr = measure["beats"] as JSONArray;
            int beatsCount = beatsArr.Count;
            Transform BeatTemp = transform.Find("_beat");
            //init temp before copy
            BeatTemp.Find("_chord").GetComponent<Text>().text = "";
            //init ends
            Transform[] beatsT = copyChild(BeatTemp, beatsCount);

            int beatOffset = BEAT0_OFFSET;

            for(int beatIdx = 0; beatIdx < beatsCount; ++beatIdx)
            {
                Transform beatT     = beatsT[beatIdx];
                beatT.localPosition = new Vector3(beatOffset, 0, 0);
                JSONObject beat     = beatsArr[beatIdx] as JSONObject;
                JSONArray voicesArr = beat["voices"] as JSONArray;
                JSONObject voices_0 = voicesArr[0] as JSONObject;
                int duration        = voices_0["duration"];
                JSONArray notesArr  = voices_0["notes"] as JSONArray;

                string beatLog = "";

                Transform[] notesT = copyChild(beatT.Find("0"), notesArr.Count);

                for(int noteIdx = 0; noteIdx < notesArr.Count; ++noteIdx)
                {
                    JSONObject note = notesArr[noteIdx] as JSONObject;
                    int gStr  = note["string"];
                    int value = note["value"];
                    beatLog += string.Format("{0}:{1} ", gStr, value);

                    Transform strT = notesT[noteIdx];
                    initEffect(strT.Find("_effects"), note["effect"] as JSONObject);
                    int gStrIdx = gStr - 1;
                    strT.localPosition = new Vector3(0, getStringPosY(gStrIdx), 0);
                    strT.Find("_value").GetComponent<Text>().text = value.ToString();
                }
                beatOffset += BEAT_DEFAULT_WIDTH;
                beatLog += string.Format("duration:{0} ", duration);
                JSONObject chord = beat["chord"] as JSONObject;
                if (chord != null)
                {
                    beatLog += string.Format("chord:{0} ", chord["name"]);
                    beatT.Find("_chord").GetComponent<Text>().text = chord["name"];
                }
                Logger.Log(tag, beatLog);
            }
        }

        void initEffect(Transform effectsMt, JSONObject effect)
        {
            bool hammer = effect["hammer"] == true;
            bool slide  = effect["slide"]  == true;
            bool pull   = effect["pull"]   == true;
            Transform smoothT = effectsMt.Find("_smooth");
            bool smooth = hammer || slide || pull;
            smoothT.gameObject.SetActive(smooth);
            if(smooth)
            {
                RectTransform rt = smoothT.Find("_img").GetComponent<Image>().GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(BEAT_DEFAULT_WIDTH, rt.sizeDelta.y);
                Text text = smoothT.Find("_text").GetComponent<Text>();
                if(hammer) text.text = "H";
                else if(slide) text.text = "sl.";
                else if(pull) text.text = "P";
                rt = text.GetComponent<RectTransform>();
                rt.localPosition = new Vector3(BEAT_DEFAULT_WIDTH/2, rt.localPosition.y, 0);
            }
        }


        public float getWidth()
        {
            return transform.GetChild(0).GetComponent<Image>().rectTransform.sizeDelta.x;
        }
    }
}
