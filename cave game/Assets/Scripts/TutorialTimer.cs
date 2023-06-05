using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTimer : MonoBehaviour
{
    public bool StartTimer = false;

    public float timer = 0;
    private void Update()
    {
        if (StartTimer)
        {
            timer += Time.deltaTime;
        }
    }
}
