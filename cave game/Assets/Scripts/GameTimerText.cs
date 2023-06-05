using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameTimerText : MonoBehaviour
{
    public SinglePlayerManager sp;
    public MultiplayerManager mp;
    // Update is called once per frame
    private void Awake()
    {
        if (GameObject.Find("SinglePlayerManager") != null)
        {
            sp = GameObject.Find("SinglePlayerManager").GetComponent<SinglePlayerManager>();
        }
        else if(GameObject.Find("MultiplayerManager") != null)
        {
            mp = GameObject.Find("MultiplayerManager").GetComponent<MultiplayerManager>();
        }
    }
    void Update()
    {
        double seconds = 0;
        
        if(sp != null)
        {
            seconds = (double)sp.GameTimer;
        }
        if(mp != null)
        {
            seconds = (double)mp.GameTimer;
        }
        TimeSpan t = TimeSpan.FromSeconds(seconds);
        /*if (t.Minutes == 0 && t.Hours == 0)
        {
            if (t.Seconds < 10)
            {
                transform.GetChild(0).GetComponent<TMP_Text>().text = "00:00:0" + t.Seconds.ToString();
            }
            else
            {
                transform.GetChild(0).GetComponent<TMP_Text>().text = "00:00:" + t.Seconds.ToString();
            }
        }
        else if (t.Hours == 0)
        {
            if (t.Minutes > 0)
            {
                if (GetComponent<RectTransform>().sizeDelta.x != 140f && GetComponent<RectTransform>().sizeDelta.y != 100f)
                {
                    GetComponent<RectTransform>().sizeDelta = new Vector2(140f, 100f);
                }

                if (t.Minutes < 10)
                {
                    if (t.Seconds < 10)
                    {
                        transform.GetChild(0).GetComponent<TMP_Text>().text = "00:0" + t.Minutes.ToString("") + ":0" + t.Seconds.ToString();
                    }
                    else
                    {
                        transform.GetChild(0).GetComponent<TMP_Text>().text = "00:0" + t.Minutes.ToString("") + ":" + t.Seconds.ToString();
                    }
                }
                else
                {
                    if (t.Seconds < 10)
                    {
                        transform.GetChild(0).GetComponent<TMP_Text>().text = "00:" + t.Minutes.ToString("") + ":0" + t.Seconds.ToString();
                    }
                    else
                    {
                        transform.GetChild(0).GetComponent<TMP_Text>().text = "00:" + t.Minutes.ToString("") + ":" + t.Seconds.ToString();
                    }
                }
            }
        }
        else
        {
            if (t.Hours > 0)
            {
                if (t.Hours < 10)
                {
                    transform.GetChild(0).GetComponent<TMP_Text>().text = "0" + t.Hours.ToString() + ":" + t.Minutes.ToString("") + ":" + t.Seconds.ToString();
                }
                else
                {
                    transform.GetChild(0).GetComponent<TMP_Text>().text = t.Hours.ToString() + ":" + t.Minutes.ToString() + ":" + t.Seconds.ToString();
                }
                transform.GetChild(0).GetComponent<TMP_Text>().text = t.Hours.ToString() + " " + t.Minutes.ToString() + " " + t.Seconds.ToString();
            }
        }*/

        string TimerSeconds = "";
        string TimerMinutes = "";
        string TimerHours = "";

        if (t.Seconds < 10)
        {
            TimerSeconds = "0" + t.Seconds.ToString();
        }
        else
        {
            TimerSeconds = t.Seconds.ToString();
        }

        if(t.Minutes < 10)
        {
            TimerMinutes = "0" + t.Minutes.ToString();
        }
        else
        {
            TimerMinutes = t.Minutes.ToString();
        }

        if(t.Hours < 10)
        {
            TimerHours = "0" + t.Hours.ToString();
        }
        else
        {
            TimerHours = t.Hours.ToString();
        }


        transform.GetChild(0).GetComponent<TMP_Text>().text = TimerHours + ":" + TimerMinutes + ":" + TimerSeconds;
    }
}
