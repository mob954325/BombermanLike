using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfoPanel : MonoBehaviour
{
    PlayerCard[] playerCards = new PlayerCard[4];

    private void Awake()
    {
        for(int i = 0; i < playerCards.Length; i++)
        {
            playerCards[i] = transform.GetChild(i).GetComponent<PlayerCard>();
        }
    }

    public void Init(NetworkRunner runner)
    {
        int index = 0;
        foreach(var player in runner.ActivePlayers)
        {
            PlayerData data = GameManager.instance.GetPlayerData(player, runner);

            playerCards[index].Init(data);
            index++;
        }

        foreach(var card in playerCards)
        {
            if(!card.CheckData())
            {
                card.gameObject.SetActive(false);
            }
        }
    }
}