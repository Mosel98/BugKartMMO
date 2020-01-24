using Network;
using UnityEngine;

public class Item : NetworkBehaviour
{
    [SyncVar]
    public int OwnerID;
}