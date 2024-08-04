using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class EndGamePanel : MonoBehaviour
{
    private TextMeshProUGUI wonText;
    private Button button;

    private void Awake()
    {
        Transform child = transform.GetChild(0);
        wonText = child.GetComponent<TextMeshProUGUI>();
        
        child = transform.GetChild(1);
        button = child.GetComponent<Button>();

        gameObject.SetActive(false);
    }

    public void ShowPanel(PlayerData data)
    {
        string str = null;

        if (data != null)
        {
            str = data.nickName.ToString();
        }

        gameObject.SetActive(true);
        wonText.text = data != null ? $"Player[{str}] Won !!!" : $"Draw !!!";
        button.onClick.AddListener(GameManager.instance.ExitSession); // 버튼 이벤트 추가 (게임 세션 나가기)
    }
}