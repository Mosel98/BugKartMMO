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


    protected override void Update()
    {

    }
}
