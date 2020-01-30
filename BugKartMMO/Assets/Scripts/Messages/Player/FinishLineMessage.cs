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
        public int SlotID { get; set; }
        
        public override byte[] Serialize(out int _bytes)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (NetworkWriter nw = new NetworkWriter(ms))
                {
                    nw.Write((short)EMessageType.FINISH_LINE);

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
            // player finished race
            PlayerController.m_FinishedPlayers[SlotID] = true;

            // set bool to true, when every player has finished the race and can go to endscreen
            if (!PlayerController.m_FinishedPlayers.ContainsValue(false))
            {
                PlayerController.m_FinishedRace = true;
            }

            // counts the finished player
            int _finishCount = 0;

            // counts the amount of players which finished the race and set the finish place to this count
            for (int i= 0; i < PlayerController.m_FinishedPlayers.Count; i++)
            {
                if(PlayerController.m_FinishedPlayers[i] == true)
                {
                _finishCount++;
                }
            }

            // set finish place of player 
            PlayerController.m_finishPlace = _finishCount;
            PlayerController.SetIsDirty();
        }
    }
}