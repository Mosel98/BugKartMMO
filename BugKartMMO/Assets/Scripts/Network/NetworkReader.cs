using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Network.IO
{
    public class NetworkReader : BinaryReader
    {
        public NetworkReader(Stream _input) : base(_input) { }
        public NetworkReader(Stream _input, System.Text.Encoding _encoding)
            : base(_input, _encoding) { }
        public NetworkReader(Stream _input, System.Text.Encoding _encoding, bool _leaveOpen)
            : base(_input, _encoding, _leaveOpen) { }

        public Vector2 ReadVector2()
        {
            return new Vector2(ReadSingle(), ReadSingle());
        }

        public Vector3 ReadVector3()
        {
            return new Vector3(ReadSingle(), ReadSingle(), ReadSingle());
        }

        public Quaternion ReadQuaternion()
        {
            return new Quaternion(ReadSingle(), ReadSingle(),
                ReadSingle(), ReadSingle());
        }

        public NetworkIdentity ReadNetworkIdentity()
        {
            uint id = ReadUInt32();
            return NetworkManager.Instance.GetIdentity(id);
        }

        public GameObject ReadGameObject()
        {
            return ReadNetworkIdentity()?.gameObject;
        }

        public Transform ReadTransform()
        {
            return ReadNetworkIdentity()?.transform;
        }

        public GameObject ReadPrefab()
        {
            int prefabId = ReadInt32();
            return NetworkManager.Instance.GetPrefabIdentity(prefabId)?.gameObject;
        }

        public Color ReadColor()
        {
            return new Color(ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle());
        }

        public Color32 ReadColor32()
        {
            return new Color32(ReadByte(), ReadByte(), ReadByte(), ReadByte());
        }

        public Vector4 ReadVector4()
        {
            return new Vector4(ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle());
        }
    }
}