//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Network.IO; // notwendig
//using System.IO;


//// MESSAGE ÜBERFLÜSSIG?!

//namespace Network.Messages
//{
//    public class ControlMessage : AMessageBase // muss von AMessageBase erben
//    {
//        public int PlayerID { get; set; }

//        // A (left) or D (right)
//        public KeyCode PressedKey { get; set; }

//        public Quaternion Rotation { get; set; }

//        public override byte[] Serialize(out int _bytes)
//        {
//            using (MemoryStream ms = new MemoryStream())
//            {
//                using (NetworkWriter nw = new NetworkWriter(ms))
//                {
//                    nw.Write((short)EMessageType.CONTROL_CHANGE); // Messagetype ändern

//                    nw.Write(PlayerID);
//                    nw.Write((short)PressedKey);
//                    nw.Write(Rotation);

//                    _bytes = (int)ms.Position;
//                    return ms.ToArray();
//                }
//            }
//        }

//        public override void Deserialize(int _senderID, byte[] _data, int _receivedBytes)
//        {
//            base.Deserialize(_senderID, _data, _receivedBytes);

//            using (MemoryStream ms = new MemoryStream(_data, 0, _receivedBytes))
//            {
//                using (NetworkReader nr = new NetworkReader(ms))
//                {
//                    nr.ReadInt16();

//                    PlayerID = nr.ReadInt32();
//                    PressedKey = (KeyCode)nr.ReadInt16();
//                    Rotation = nr.ReadQuaternion();
//                }
//            }
//        }

//        public override void Use()
//        {
//            if (PressedKey == KeyCode.A)
//            {
//                // - y -> Inverse richtig?!
//                Rotation = Quaternion.Inverse(Rotation) * Quaternion.Euler(0.0f, 1.0f, 0.0f);
//            }

//            if (PressedKey == KeyCode.D)
//            {
//                // + y
//                Rotation = Rotation * Quaternion.Euler(0.0f, 1.0f, 0.0f);
//            }
//        }
//    }
//}

