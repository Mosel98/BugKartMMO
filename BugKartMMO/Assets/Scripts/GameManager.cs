using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network;

public class GameManager : NetworkBehaviour
{
    // Bool = is finished
    // for each player, check if finished
    // if isfinished == true --> game stop // scene change to highscore or Win List
    // Player send back to lobby

    // if Player disconnects == game stop --> back to lobby
    // 


    protected override void Update()
    {

    }
}
