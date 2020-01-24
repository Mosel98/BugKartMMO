using Network.Messages;
using UnityEngine;
using UnityEngine.AI;

// Mario
namespace Network
{
    [DisallowMultipleComponent]
    public class NetworkItemHandeling : NetworkBehaviour
    {
        public int m_SyncInterval;

        [SerializeField]
        private GameObject m_greenShell;
        [SerializeField]
        private GameObject m_redShell;
        [SerializeField]
        private GameObject m_köttel;

        private GameObject m_gameObject;

        private float m_speed { get; set; }
        private float m_accel { get; set; }

        [SerializeField]
        private float m_boostTime = 1.0f;
        private float m_timer = 0.0f;

        private float m_tmpAccel;
        private bool m_boostSpeed = false;

        private GameObject m_player;

        #region UseItem
        public void UseItem(GameObject _go, float _item)
        {
            m_gameObject = _go;

            switch ((EItems) _item)
            {
                case EItems.COIN:
                    ConstantSpeed();
                    break;
                case EItems.GREENSHELL:
                    SpawnItem(m_greenShell, 1.0f);
                    break;
                case EItems.REDSHELL:
                    SpawnItem(m_redShell, 1.0f);
                    break;
                case EItems.MUSHROOM:
                    BoostSpeed();
                    break;
                case EItems.KÖTTEL:
                    SpawnItem(m_köttel, -1.0f);
                    break;
            }
        }

        private void SpawnItem(GameObject _item, float _spawnDist)
        {
            Vector3 spawnPos = m_gameObject.transform.position + m_gameObject.transform.forward * _spawnDist;

            GameObject tmp = Instantiate(_item, spawnPos, m_gameObject.transform.rotation, transform.parent);

            if (tmp == m_greenShell)
            {
                tmp.GetComponent<Rigidbody>().AddForce(tmp.transform.forward * 100.0f);
            }
            else if (tmp == m_redShell)
            {
                // ToDo: Get Position of Nearest Next Player!
                tmp.GetComponent<NavMeshAgent>().SetDestination(m_gameObject.transform.position);
            }
        }
        #endregion

        #region ItemCollision
        public void ItemBoxCheck(GameObject _go)
        {
            int item = Random.Range(0, 4);

            UpdateVariableMessage message = new UpdateVariableMessage(_go, item);
            NetworkManager.Instance.SendMessageToClients(message, UnityEngine.Networking.QosType.StateUpdate);
            Debug.Log("Send Updated Item To Client");
        }

        public void CollisionCheck(GameObject _go, string _tag, float _speed, float _accel)
        {
            string tag = _tag;
            m_gameObject = _go;
            m_speed = _speed;
            m_accel = _accel;

            switch (tag)
            {
                case "Coin":
                    ConstantSpeed();
                    break;
                case "Shell":
                    StopPlayer();
                    break;
                case "Boost":
                    BoostSpeed();
                    break;
            }
        }

        private void ConstantSpeed()
        {
            m_speed += 0.1f;

            UpdateVariable();
        }

        private void BoostSpeed()
        {
            m_tmpAccel = m_accel;
            m_accel += 1.5f;

            m_boostSpeed = true;

            UpdateVariable();
        }

        private void StopPlayer()
        {
            m_accel = 0.0f;

            UpdateVariable();
        }

        protected override void Update()
        {
            if (m_boostSpeed)
            {
                m_timer += Time.deltaTime;

                if (m_timer >= m_boostTime)
                {
                    m_accel = m_tmpAccel;

                    m_timer = 0.0f;
                    m_boostSpeed = false;

                    UpdateVariable();
                }
            }
        }
        #endregion

        private void UpdateVariable()
        {
            UpdateVariableMessage message = new UpdateVariableMessage(m_gameObject, m_speed, m_accel);
            NetworkManager.Instance.SendMessageToClients(message, UnityEngine.Networking.QosType.StateUpdate);
            Debug.Log("Send Updated Variable To Client");
        }
    }
}
