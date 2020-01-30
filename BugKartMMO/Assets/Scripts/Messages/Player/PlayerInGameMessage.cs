using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network.IO;
using System.IO;

// Frank
namespace Network.Messages
{
    public class PlayerInGameMessage : AMessageBase
    {
        public int PlayerID { get; set; }
        public PlayerController PlayerController { get; set; }
        public int SlotID { get; set; }

        public override byte[] Serialize(out int _bytes)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (NetworkWriter nw = new NetworkWriter(ms))
                {
                    nw.Write((short)EMessageType.PLAYER_IN_GAME);

                    nw.Write(PlayerID);
                    nw.Write(PlayerController.GetComponent<NetworkIdentity>());
                    nw.Write(SlotID);

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
                    SlotID = nr.ReadInt32();
                }
            }
        }

        public override void Use()
        {
            // GameManager.SetGameMode(GameModes.START_GAME);
            // GameManager.SetIsDirty();

            PlayerController.m_AllPlayersReady[SlotID] = true;

            if (!PlayerController.m_AllPlayersReady.ContainsValue(false))
            {
                PlayerController.m_isInGame = true;
            }
            PlayerController.SetIsDirty();
        }
    }
}