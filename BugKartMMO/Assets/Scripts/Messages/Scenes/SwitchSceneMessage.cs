using Network.IO;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Network.Messages
{
    public class SwitchSceneMessage : AMessageBase
    {
        public string ScenePath { get; set; }

        public override void Deserialize(int _senderID, byte[] _data, int _receivedBytes)
        {
            base.Deserialize(_senderID, _data, _receivedBytes);
            using (MemoryStream ms = new MemoryStream(_data, 0, _receivedBytes))
            {
                using (NetworkReader nr = new NetworkReader(ms))
                {
                    nr.ReadInt16();
                    ScenePath = nr.ReadString();
                }
            }
        }

        public override byte[] Serialize(out int _bytes)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (NetworkWriter nw = new NetworkWriter(ms))
                {
                    nw.Write((short)EMessageType.SWITCH_SCENE);
                    nw.Write(ScenePath);
                    _bytes = (int)ms.Position;
                    return ms.ToArray();
                }
            }
        }

        public override void Use()
        {
            if (SceneManager.GetActiveScene().path == ScenePath)
                return;

            UnityEngine.SceneManagement.SceneManager.LoadScene(ScenePath);
        }
    }
}