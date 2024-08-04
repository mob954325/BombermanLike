using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerPanel : MonoBehaviour
{
    TextMeshProUGUI timerText;
    private float timer;
    private float maxTime = 10;

    private void Awake()
    {
        timerText = GetComponentInChildren<TextMeshProUGUI>();
        Init();
    }

    private void FixedUpdate()
    {
        if (timer < 0)
        {
            timer = 0f;
        }

        timer -= Time.fixedDeltaTime;

        int min = (int)(timer / 60f);
        int sec = (int)(timer % 60f);
        timerText.text = $"{min:F0} : {sec:D2}";
    }

    /// <summary>
    /// 초기화 함수
    /// </summary>
    private void Init()
    {
        timer = maxTime;
        timerText.text = $"{99:F0} : {99:D2}";
    }

    /// <summary>
    /// 타이머 값을 반환하는 함수
    /// </summary>
    /// <returns>반환할 float형 타이머 값</returns>
    public float GetTimeValue()
    {
        return timer;
    }
}