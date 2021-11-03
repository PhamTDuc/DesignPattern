using UnityEngine;
using UnityEngine.Audio;

namespace Guinea.Core
{
    public class Sound: MonoBehaviour
    {
        AudioSource m_audioSource;
        [SerializeField] AudioClip m_default;
        [SerializeField]ClipDictionary m_clipDictionary;

        void Awake()
        {
            m_audioSource = GetComponent<AudioSource>();
        }
        
        public void Pause() => m_audioSource.Pause();
        public void Stop()  => m_audioSource.Stop();

        public void PlayOneShot(ClipType clipType)
        {
            AudioClip audioClip;
            if(!m_clipDictionary.TryGetValue(clipType, out audioClip))
            {    
                audioClip = m_default;
                Debug.LogWarning($"Sound::{clipType} ClipType not found. Using default sound");
            }
            m_audioSource.PlayOneShot(audioClip);
        }

        public void Play(ClipType clipType)
        {
            if(m_clipDictionary.TryGetValue(clipType, out AudioClip audioClip))
            {
                m_audioSource.clip = audioClip;
            }
            else
            {
                m_audioSource.clip = m_default;
            }

            if(!m_audioSource.isPlaying)
            {
                m_audioSource.Play();
            }
        }

        [System.Serializable]
        public class ClipDictionary : SerializableDictionary<ClipType, AudioClip>{}
    }
}