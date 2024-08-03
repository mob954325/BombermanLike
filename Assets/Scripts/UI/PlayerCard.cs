using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerCard : MonoBehaviour
{
    /// <summary>
    /// 해당 플레이어 카드의 플레이어 데이터
    /// </summary>
    PlayerData playerData;

    private TextMeshProUGUI playerName;
    private GameObject[] lifes = new GameObject[3];

    /// <summary>
    /// 활성화된 이미지의 마지막 인덱스
    /// </summary>
    private int activeLifeImgLastIndex = -1;

    private void Awake()
    {
        Transform child = transform.GetChild(0);
        playerName = child.GetComponent<TextMeshProUGUI>();

        child = transform.GetChild(1);
        for(int i = 0; i < lifes.Length; i++)
        {
            lifes[i] = child.GetChild(i).gameObject;
        }

        activeLifeImgLastIndex = lifes.Length - 1;
    }

    public void Init(PlayerData data)
    {
        playerName.text = data.nickName.ToString();
        playerData = data;

        data.Instance.GetComponent<PlayerBehaviour>().OnHit = OnPlayerHit;
    }

    /// <summary>
    /// 데이터가 있는지 확인하는 함수
    /// </summary>
    /// <returns>데이터가 있으면 true 아니면 false</returns>
    public bool CheckData()
    {
        bool result = false;

        if(playerData != null)
        {
            result = true;
        }

        return result;
    }

    private void OnPlayerHit()
    {
        if(activeLifeImgLastIndex > -1)
        {
            lifes[activeLifeImgLastIndex--].SetActive(false);
        }
    }
}