using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network;
using System;
using UnityEngine.UI;
using TMPro;

public class GameManager : NetworkBehaviour
{
    // Bool = is finished
    // --> m_FinishedRace (PlayerController)
    // for each player, check if finished 
    // if isfinished == true --> game stop // scene change to highscore or Win List 
    // --> Load Scene: "Endscreen"
    // Player send back to lobby
    // --> Load Scene: "Lobby"

    // if Player disconnects == game stop --> back to lobby
    // 

    [SyncVar]
    public static GameModes m_gameMode;
    private GameObject[] m_allPlayers;

    public TextMeshProUGUI m_countdownText;
    private bool m_canCount = true;

    protected virtual void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    protected override void Update()
    {
        if (IsServer)
        {
            switch (m_gameMode)
            {
                case GameModes.MENU:
                    if (PlayerController.IsInGame() == true)
                    {
                        m_gameMode = GameModes.START_GAME;
                    }
                    break;
                case GameModes.START_GAME:
                    if (PlayerController.GetCanStart() == true)
                    {
                        StartCountdown();
                        //m_gameMode = GameModes.DRIVE; --> steht unten
                    }
                    break;
                case GameModes.DRIVE:
                    if (PlayerController.GetIsFinished() == true)
                    {
                        m_gameMode = GameModes.ENDSCREEN;
                    }
                    break;
                case GameModes.CLIENT_DISCONNECT:
                    // Button Lobby hit
                    //if ()
                    //{
                    //    m_gameMode = GameModes.CLIENT_DISCONNECT;
                    //}
                    break;
                case GameModes.ENDSCREEN:
                    if (PlayerController.GetIsFinished() == true)
                    {
                        m_gameMode = GameModes.RESET;
                    }
                    break;
                case GameModes.RESET:

                    m_gameMode = GameModes.MENU;
                    break;
                default:
                    break;

            }
            SetIsDirty();
        }
    }


    // Frank
    private void StartCountdown()
    {
        float _countdown = PlayerController.m_Countdown;

        if (_countdown >= 0.0F && m_canCount == true)
        {
            _countdown -= Time.deltaTime;
            m_countdownText.text = _countdown.ToString("0");
        }

        else if (_countdown <= 0.0F && m_canCount == true)
        {
            m_countdownText.text = "0";
            _countdown = 0.0F;
            m_countdownText.enabled = false;

            m_gameMode = GameModes.DRIVE;
        }

        PlayerController.m_Countdown = _countdown;
        SetIsDirty();
    }
}
