using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network.IO;
using System.IO;

// Frank
namespace Network.Messages
{
    public class CountdownMessage : AMessageBase
    {
        public int PlayerID { get; set; }
        public PlayerController PlayerController { get; set; }
        public float Countdown { get; set; }

        public override byte[] Serialize(out int _bytes)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (NetworkWriter nw = new NetworkWriter(ms))
                {
                    nw.Write((short)EMessageType.COUNTDOWN);

                    nw.Write(PlayerID);
                    nw.Write(PlayerController.GetComponent<NetworkIdentity>());
                    nw.Write(Countdown);

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

                    PlayerID = nr.ReadInt32();
                    PlayerController = nr.ReadNetworkIdentity().GetComponent<PlayerController>();
                    Countdown = nr.ReadSingle();
                }
            }
        }

        public override void Use()
        {
            PlayerController.m_Countdown = Countdown;
            PlayerController.SetIsDirty();
        }
    }
}