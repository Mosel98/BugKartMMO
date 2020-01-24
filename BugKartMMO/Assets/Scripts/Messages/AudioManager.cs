using UnityEngine;

namespace Network.Messages
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        public AudioClip[] m_AudioClips;
        public AudioSource m_HonkSource;
        
        private void Awake()
        {
            Instance = this;
        }

        public void PlayClip(int _id)
        {
            m_HonkSource.PlayOneShot(m_AudioClips[_id]);
        }
    }
}