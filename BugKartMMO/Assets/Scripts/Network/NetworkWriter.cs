using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Network.IO
{
    public class NetworkWriter : BinaryWriter
    {
        public NetworkWriter(Stream _stream) : base(_stream) { }

        public void Write(Vector2 _vector)
        {
            Write(_vector.x);
            Write(_vector.y);
        }

        public void Write(Vector3 _vector)
        {
            Write(_vector.x);
            Write(_vector.y);
            Write(_vector.z);
        }

        public void Write(Quaternion _quaternion)
        {
            Write(_quaternion.x);
            Write(_quaternion.y);
            Write(_quaternion.z);
            Write(_quaternion.w);
        }

        public void Write(NetworkIdentity _identity)
        {
            if (_identity.NetID == 0 && _identity.PrefabID >= 0)
            {
                Write(_identity.PrefabID);
            }
            else
            {
                Write(_identity.NetID);
            }
        }

        public void Write(GameObject _gameObject)
        {
            NetworkIdentity identity = _gameObject.GetComponent<NetworkIdentity>();

            if (identity is null)
            {
                Debug.Log(new MissingComponentException("NetworkIdentity ist nicht " +
                   "auf dem Objekt vorhanden!"), _gameObject);
                return;
            }
            Write(identity);
        }

        public void Write(Transform _transform)
        {
            if (_transform is null)
            {
                Write(-1);
            }
            else
            {
                Write(_transform.gameObject);
            }
        }

        public void Write(Vector4 _vector)
        {
            Write(_vector.x);
            Write(_vector.y);
            Write(_vector.z);
            Write(_vector.w);
        }

        public void Write(Color _color)
        {
            Write(_color.r);
            Write(_color.g);
            Write(_color.b);
            Write(_color.a);
        }

        public void Write(Color32 _color)
        {
            Write(_color.r);
            Write(_color.g);
            Write(_color.b);
            Write(_color.a);
        }
    }
}