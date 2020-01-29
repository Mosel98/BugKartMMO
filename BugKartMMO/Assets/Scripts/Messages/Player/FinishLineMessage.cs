using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network.IO;
using System.IO;

// Frank
namespace Network.Messages
{
    public class FinishLineMessage : AMessageBase
    {
        public int PlayerID { get; set; }
        public PlayerController PlayerController { get; set; }

        public override byte[] Serialize(out int _bytes)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (NetworkWriter nw = new NetworkWriter(ms))
                {
                    nw.Write((short)EMessageType.ACCELERATION_CHANGE);

                    nw.Write(PlayerID);
                    nw.Write(PlayerController.GetComponent<NetworkIdentity>());

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
                }
            }
        }

        public override void Use()
        {
            // Player finished the race
            PlayerController.m_FinishedRace = true;
            PlayerController.SetIsDirty();
        }
    }
}