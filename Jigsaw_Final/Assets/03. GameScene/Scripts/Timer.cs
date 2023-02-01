using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    float time;
    public float curTime
    {
        get
        {
            return time;
        }
    }

    Coroutine curCO;
    public TextMeshProUGUI timeText;

    private void Awake()
    {
        time = 0;
    }

    [ContextMenu("Start")]
    public void start()
    {
        if (curCO != null) return;
        else curCO = StartCoroutine(CO_Timer());
    }

    [ContextMenu("Stop")]
    public void stop()
    {
        StopCoroutine(curCO);
        curCO = null;
    }

    IEnumerator CO_Timer()
    {
        while(true)
        {
            time += Time.deltaTime;
            if (timeText != null) timeText.text = "½Ã°£: " + time.ToString("N");

            yield return null;
        }
    }



}
