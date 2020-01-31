using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network.IO;
using System.IO;

// Frank
namespace Network.Messages
{
    public class EndscreenMessage : AMessageBase
    {
        public int PlayerID { get; set; }
        public PlayerController PlayerController { get; set; }


        public override byte[] Serialize(out int _bytes)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (NetworkWriter nw = new NetworkWriter(ms))
                {
                    nw.Write((short)EMessageType.ENDSCREEN);


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

                    PlayerController = nr.ReadNetworkIdentity().GetComponent<PlayerController>();
                }
            }
        }

        public override void Use()
        {

            GameObject.Find("GameManager").GetComponent<GameManager>().ShowEndscreen();

            GameObject.Find("GameManager").GetComponent<GameManager>().SetIsDirty();

        }
    }
}