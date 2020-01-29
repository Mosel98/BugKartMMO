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
        public PlayerController PlayerController { get; set; }

        // W (increase) or S (decrease)
        public KeyCode PressedKey { get; set; }

        public float Acceleration { get; set; }
        public float Speed { get; set; }


        public override byte[] Serialize(out int _bytes)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (NetworkWriter nw = new NetworkWriter(ms))
                {
                    nw.Write((short)EMessageType.ACCELERATION_CHANGE);

                    nw.Write(PlayerID);
                    nw.Write((short)PressedKey);
                    nw.Write(Acceleration);
                    nw.Write(Speed);

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
                    Acceleration = nr.ReadSingle();
                    Speed = nr.ReadSingle();

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

            if (PressedKey == KeyCode.W)
            {
                Acceleration += 0.5f * Time.deltaTime;
            }

            if (PressedKey == KeyCode.S)
            {
                Acceleration -= 0.5f * Time.deltaTime;
            }

            if (Acceleration > Speed)
            {
                Acceleration = Speed;
            }

            if (Acceleration < -0.5f * Speed)
            {
                Acceleration = -0.5f * Speed;
            }

            if (PressedKey == KeyCode.None)
            {
                if (Acceleration > 0)
                    Acceleration -= 0.5f * Time.deltaTime;

                else if (Acceleration < 0)
                    Acceleration += 0.5f * Time.deltaTime;

                else
                {
                    Acceleration = 0.0f;
                }
            }

            PlayerController.SetIsDirty();
            
        }
    }
}