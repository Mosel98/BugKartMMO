using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network;

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
    public GameModes m_gameMode;
    private GameObject[] m_allPlayers;



    // protected override void Update()
    // {
    //     if (IsServer)
    //     {
    //         switch (m_gameMode)
    //         {
    //             case GameModes.MENU:
    //                 if (FindObjectsOfTypeAll<PlayerController>())
    //                 { }
    //                     break;
    //             case GameModes.START_GAME:
    //                 if (PlayerController.GetCanStart() == true)
    //                 {
    //                     m_gameMode = GameModes.DRIVE;
    //                 }
    //                 break;
    //             case GameModes.DRIVE:
    //                 if (PlayerController.GetIsFinished() == true)
    //                 {
    //                     m_gameMode = GameModes.ENDSCREEN;
    //                 }
    //                 break;
    //             case GameModes.CLIENT_DISCONNECT:
    //                 if (// Button Lobby hit)
    //                 {
    //                     m_gameMode = GameModes.CLIENT_DISCONNECT;
    //                 }
    //                 break;
    //             case GameModes.ENDSCREEN:
    //                 if (// Button hit)
    //                 {
    //                     m_gameMode = GameModes.RESET;
    //                 }
    //                 break;
    //             case GameModes.RESET:
    //         
    //                 m_gameMode = GameModes.MENU;
    //                 break;
    //             default:
    //                 break;
    //         
    //         }
    //         SetIsDirty();
    //     }
    // }
}
