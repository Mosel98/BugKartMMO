using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network.IO;
using System.IO;

// Frank
namespace Network.Messages
{
    public class AccelerationMessage : AMessageBase
    {
        public int PlayerID { get; set; }
        public KeyCode PressedKey { get; set; }
        public bool PressedDown { get; set; }
        public PlayerController PlayerController { get; set; }

        public override byte[] Serialize(out int _bytes)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (NetworkWriter nw = new NetworkWriter(ms))
                {
                    nw.Write((short)EMessageType.ACCELERATION_CHANGE);

                    nw.Write(PlayerID);
                    nw.Write((short)PressedKey);
                    nw.Write(PressedDown);
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
                    PressedKey = (KeyCode)nr.ReadInt16();
                    PressedDown = nr.ReadBoolean();
                    PlayerController = nr.ReadNetworkIdentity().GetComponent<PlayerController>();
                }
            }
        }

        public override void Use()
        {
            PlayerController.m_keysPressed[PressedKey] = PressedDown;
        }
    }
}