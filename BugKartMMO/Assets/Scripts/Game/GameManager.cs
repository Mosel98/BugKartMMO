using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network;
using System;
using UnityEngine.UI;
using TMPro;

public class GameManager : NetworkBehaviour
{
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

    public TextMeshProUGUI m_FinishText;

    [SyncVar("countdownchanged")]
    private float m_countdown = 5.0f;

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
                case GameModes.ENDSCREEN:
                    Debug.Log("Mode Endscreen");
                    StartEndScreen();
                    break;
                default:
                    break;

            }
            SetIsDirty();
        }
    }

    private void StartEndScreen()
    {
        m_FinishText = GameObject.Find("Player UI/Canvas/Win_Text")?.GetComponent<TextMeshProUGUI>();
        m_FinishText.enabled = true;
    }


    // Frank
    private void StartCountdown()
    {
        if (m_countdown >= 0.0F && m_canCount == true)
        {
            m_countdown -= Time.deltaTime;
            m_countdownText.text = m_countdown.ToString("0");
            SetIsDirty();
        }

        else if (m_countdown <= 0.0F && m_canCount == true)
        {
            m_countdownText.text = "";
            m_countdown = 0.0F;
            m_countdownText.enabled = false;

            GameMode = GameModes.DRIVE;
            SetIsDirty();
        }
    }

    void countdownchanged(float _c)
    {
        m_countdownText = GameObject.Find("Player UI/Canvas/Countdown_Text")?.GetComponent<TextMeshProUGUI>();
        if (m_countdownText is object)
            m_countdownText.text = _c.ToString("0");
    }


}
