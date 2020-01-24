using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network.IO;
using System.IO;

// Frank
namespace Network.Messages
{
    public class RotationMessage : AMessageBase
    {
        public int PlayerID { get; set; }
        public PlayerController PlayerController { get; set; }

        // A (left) or D (right)
        public KeyCode PressedKey { get; set; }

        public Quaternion Rotation { get; set; }


        public override byte[] Serialize(out int _bytes)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (NetworkWriter nw = new NetworkWriter(ms))
                {
                    nw.Write((short)EMessageType.ROTATION_CHANGE);

                    nw.Write(PlayerID);
                    nw.Write((short)PressedKey);
                    nw.Write(Rotation);

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
                    Rotation = nr.ReadQuaternion();

                    PlayerController = nr.ReadNetworkIdentity().GetComponent<PlayerController>();
                }
            }
        }

        public override void Use()
        {
            //// Validierung
            //GameObject go = NetworkManager.Instantiate(ItemPrefab);
            //Item item = go.GetComponent<Item>();
            //item.OwnerID = PlayerID;
            //item.SetIsDirty();

            //GameObject go = NetworkManager.Instantiate(PlayerController);
            //PlayerController player = go.GetComponent<PlayerController>();
            //player

            //NetworkManager.Instance.SpawnGameObject(go);

            if (PressedKey == KeyCode.A)
            {
                // - y -> Inverse richtig?!
                //m_rotation = Quaternion.Inverse(m_rotation) * Quaternion.Euler(0.0f, 5.0f, 0.0f);
                //m_rotation = transform.rotation * Quaternion.Euler(0.0f, -5.0f, 0.0f);
                PlayerController.transform.Rotate(0.0f, -1.0f, 0.0f);
            }

            if (PressedKey == KeyCode.D)
            {
                // + y
                // m_rotation = transform.rotation * Quaternion.Euler(0.0f, 5.0f, 0.0f);
               PlayerController.transform.Rotate(0.0f, 1.0f, 0.0f);
            }
                        
            PlayerController.SetIsDirty();

        }
    }
}