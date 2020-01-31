using Network;
using Network.IO;
using Network.Messages;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ChangeColorMessage : AMessageBase
{
    public PlayerController Player { get; set; }
    public Color Color { get; set; }

    public override byte[] Serialize(out int _bytes)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            using (NetworkWriter nw = new NetworkWriter(ms))
            {
                nw.Write((short)EMessageType.CHANGE_COLOR);
                nw.Write(Player);
                nw.Write(Color);

                _bytes = (int)ms.Position;
                return ms.ToArray();

            }
        }
    }

    public override void Deserialize(int _senderID, byte[] _data, int _receivedBytes)
    {
        base.Deserialize(_senderID, _data, _receivedBytes);
        using (MemoryStream ms = new MemoryStream(_data, 0, _receivedBytes))
        {
            using (NetworkReader nr = new NetworkReader(ms))
            {
                nr.ReadInt16();
                Player = nr.ReadNetworkIdentity().GetComponent<PlayerController>();
                Color = nr.ReadColor();
            }
        }
    }

    public override void Use()
    {
        Renderer[] rend = Player.GetComponentsInChildren<Renderer>();

        foreach (Renderer r in rend)
        {
            r.material.color = Color;
        }
        
    }
}
