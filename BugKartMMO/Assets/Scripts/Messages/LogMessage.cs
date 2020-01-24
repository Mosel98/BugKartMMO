using Network.IO;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Network.Messages
{
    public class LogMessage : AMessageBase
    {
        public LogMessage()
        {

        }

        public LogMessage(string _message)
        {
            Message = _message;
        }

        public string Message { get; private set; }

        public override void Deserialize(int _senderID, byte[] _data, int _receivedBytes)
        {
            base.Deserialize(_senderID, _data, _receivedBytes);
            using (MemoryStream ms = new MemoryStream(_data, 0, _receivedBytes))
            {
                using (NetworkReader nr = new NetworkReader(ms))
                {
                    nr.ReadUInt16();
                    Message = nr.ReadString();
                }
            }
        }

        public override byte[] Serialize(out int _bytes)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (NetworkWriter nw = new NetworkWriter(ms))
                {
                    nw.Write((ushort)EMessageType.LOG);
                    nw.Write(Message);
                    _bytes = (int)ms.Position;
                    return ms.ToArray();
                }
            }
        }

        public override void Use()
        {
            Debug.Log(Message);
        }
    }
}