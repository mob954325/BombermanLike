using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    TextMeshProUGUI timerText;
    private float timer;
    [SerializeField] private float maxTime = 180f;



    private void Awake()
    {
        timerText = GetComponentInChildren<TextMeshProUGUI>();
        Init();
    }

    private void FixedUpdate()
    {
        timer -= Time.fixedDeltaTime;
        timerText.text = $"{timer / 60:F0} : {timer % 60:F0}";
    }

    /// <summary>
    /// 초기화 함수
    /// </summary>
    private void Init()
    {
        timer = maxTime;
        timerText.text = $"{99:F0} : {99:F0}";
    }
}