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

    public static GameModes GameMode
    {
        get
        {
            GameManager m = FindObjectOfType<GameManager>();
            if (m is object)
            {
                return m.m_gameMode;
            }
            else
            {
                return GameModes.MENU;
            }
        }
        set
        {
            FindObjectOfType<GameManager>().m_gameMode = value;
        }
    }

    [SyncVar]
    public GameModes m_gameMode;
    private GameObject[] m_allPlayers;

    public TextMeshProUGUI m_countdownText;
    private bool m_canCount = true;

    [SyncVar("countdownchanged")]
    private float m_countdown = 5.0f;

    private float m_nextSync = 5;


    protected virtual void Awake()
    {
        //DontDestroyOnLoad(this.gameObject);
    }

    protected override void Update()
    {
        base.Update();

        if (IsServer)
        {
            switch (GameMode)
            {
                case GameModes.MENU:
                    if (PlayerController.IsInGame() == true)
                    {
                        Debug.Log("Mode Menu");
                        GameMode = GameModes.START_GAME;
                    }
                    break;
                case GameModes.START_GAME:
                    Debug.Log("Mode Start Game");
                    StartCountdown();
                    break;
                case GameModes.DRIVE:
                    Debug.Log("Mode Drive");
                    break;
                case GameModes.CLIENT_DISCONNECT:
                    Debug.Log("Mode Client Disconnect");
                    // Button Lobby hit
                    //if ()
                    //{
                    //    m_gameMode = GameModes.CLIENT_DISCONNECT;
                    //}
                    break;
                case GameModes.ENDSCREEN:
                    Debug.Log("Mode Endscreen");
                    
                      //  GameMode = GameModes.RESET;
                    
                    break;
               //case GameModes.RESET:
               //    Debug.Log("Mode Reset");
               //    GameMode = GameModes.MENU;
               //    break;
                default:
                    break;

            }
            SetIsDirty();
        }
    }


    // Frank
    private void StartCountdown()
    {
        if (m_countdown >= 0.0F && m_canCount == true)
        {
            m_countdown -= Time.deltaTime;
            m_countdownText.text = m_countdown.ToString("0");
            SetIsDirty();

            if (m_nextSync < m_countdown)
            {
                SetIsDirty();
                m_nextSync -= 1;
            }
        }

        else if (m_countdown <= 0.0F && m_canCount == true)
        {
            m_countdownText.text = "0";
            m_countdown = 0.0F;
            m_countdownText.enabled = false;

            GameMode = GameModes.DRIVE;
            SetIsDirty();
        }

        // PlayerController.m_Countdown = _countdown;
    }

    void countdownchanged(float _c)
    {
        m_countdownText = GameObject.Find("Player UI/Canvas/Countdown_Text")?.GetComponent<TextMeshProUGUI>();
        if (m_countdownText is object)
            m_countdownText.text = _c.ToString("0") + " :-)";
    }


}
