using Network.IO;
using Network.Lobby;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Network.Messages.Lobby
{
    public class LobbyAcceptJoinMessage : AMessageBase
    {
        public override void Deserialize(int _sender, byte[] _data, int _receivedBytes)
        {
            base.Deserialize(_sender, _data, _receivedBytes);
        }

        public override byte[] Serialize(out int _bytes)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (NetworkWriter nw = new NetworkWriter(ms))
                {
                    nw.Write((short)EMessageType.LOBBY_ACCEPT_JOIN);
                    _bytes = (int)ms.Position;
                    return ms.ToArray();
                }
            }
        }

        public override void Use()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(((LobbyManager)NetworkManager.Instance).m_LobbyScene);
        }
    }
}