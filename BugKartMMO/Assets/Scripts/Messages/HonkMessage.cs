using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network.IO;
using System.IO;

namespace Network.Messages
{
    public class HonkMessage : AMessageBase
    {
        public int ClipID { get; set; }

        public override byte[] Serialize(out int _bytes)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (NetworkWriter nw = new NetworkWriter(ms))
                {
                    nw.Write((short)EMessageType.HONK_MESSAGE);
                    nw.Write(ClipID);

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
                    ClipID = nr.ReadInt32();
                }
            }
        }

        public override void Use()
        {
            if (NetworkManager.Instance.IsServer)
            {
                AudioManager.Instance.PlayClip(ClipID);
                NetworkManager.Instance.SendMessageToClients(this);
            }
            else
            {
                AudioManager.Instance.PlayClip(ClipID);
            }
        }
    }
}