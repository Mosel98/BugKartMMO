using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network.IO; 
using System.IO;

namespace Network.Messages
{
    public class HandbreakMessage : AMessageBase // muss von AMessageBase erben
    {

        // Variablen
        public int PlayerID { get; set; }
        public PlayerController PlayerController { get; set; }

        public float Acceleration { get; set; }

        public override byte[] Serialize(out int _bytes)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (NetworkWriter nw = new NetworkWriter(ms))
                {
                    nw.Write((short)EMessageType.CONTROL_CHANGE); // Messagetype ändern

                    nw.Write(PlayerID);
                    nw.Write(Acceleration);

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
                    Acceleration = nr.ReadSingle();

                    PlayerController = nr.ReadNetworkIdentity().GetComponent<PlayerController>();
                }
            }
        }

        public override void Use()
        {
            Acceleration = 0.0f;
            PlayerController.SetIsDirty();
        }
    }
}
