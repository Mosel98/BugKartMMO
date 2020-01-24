using Network.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Network
{
    [DisallowMultipleComponent]
    public class NetworkTransform : NetworkBehaviour
    {
        [SerializeField]
        [Range(1, 60)]
        private float m_syncInterval = 10;

        [SerializeField]
        private float m_extrapolationTime = 0.1f;

        private Rigidbody m_rigidbody;
        private Vector3 m_lastPosition;
        private Vector3 m_nextPosition;

        private Quaternion m_lastRotation;
        private Quaternion m_nextRotation;

        private float m_nextTime;

        protected override void Start()
        {
            base.Start();
            StartCoroutine(AsyncWaitForNetIdInit());
        }

        protected override void Update()
        {
            base.Update();

            if (IsServer)
            {
                if (!IsLocalPlayer && gameObject.CompareTag("Player"))
                {
                    if (Time.time < m_nextTime + m_extrapolationTime)
                    {
                        transform.position = Vector3.LerpUnclamped(m_lastPosition, m_nextPosition, Time.time / m_nextTime);
                        transform.rotation = Quaternion.LerpUnclamped(m_lastRotation, m_nextRotation, Time.time / m_nextTime);
                    }
                    else
                    {
                        transform.position = m_nextPosition;
                        transform.rotation = m_nextRotation;
                    }
                }

                if (Time.frameCount % m_syncInterval == 0)
                {
                    UpdatePositionMessage message = new UpdatePositionMessage(this.gameObject);
                    NetworkManager.Instance.SendMessageToClients(message);
                }

            }
            else if (IsLocalPlayer)
            {
                if (Time.frameCount % m_syncInterval == 0)
                {
                    UpdatePositionMessage message = new UpdatePositionMessage(this.gameObject);
                    NetworkManager.Instance.SendMessageToServer(message);
                }
            }
            else
            {
                if (Time.time < m_nextTime + m_extrapolationTime)
                {
                    transform.position = Vector3.LerpUnclamped(m_lastPosition, m_nextPosition, Time.time / m_nextTime);
                    transform.rotation = Quaternion.LerpUnclamped(m_lastRotation, m_nextRotation, Time.time / m_nextTime);
                }
                else
                {
                    transform.position = m_nextPosition;
                    transform.rotation = m_nextRotation;
                }
            }
        }

        public void ReceivedNewPosition(Vector3 _position, float _timeStamp)
        {
            m_lastPosition = transform.position;
            m_nextPosition = _position;
            m_nextTime = Time.time + 0.2f;
        }

        public void ReceivedNewRotation(Quaternion _rotation, float _timeStamp)
        {
            m_lastRotation = transform.rotation;
            m_nextRotation = _rotation;
        }

        private IEnumerator AsyncWaitForNetIdInit()
        {
            do
            {
                if (NetId is null)
                {
                    yield break;
                }

                yield return null;
            } while (!NetId.IsInitialized);

            if (IsServer || IsLocalPlayer)
            {
                yield break;
            }

            m_rigidbody = GetComponent<Rigidbody>();
            if (m_rigidbody is object)
            {
                m_rigidbody.isKinematic = true;
            }
        }
    }
}