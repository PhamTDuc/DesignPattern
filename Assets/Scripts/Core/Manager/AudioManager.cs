using UnityEngine;

namespace Guinea.Core
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] AudioSourceDictionary m_audioSourceDictionary;

        public void PlayOneShot(SourceType sourceType, ClipType clipType)
        {
            if (m_audioSourceDictionary.TryGetValue(sourceType, out Sound sound))
            {
                sound.PlayOneShot(clipType);
            }
            else
            {
                Commons.Logger.LogWarning($"AudioManager::{sourceType} SourceType not found");
            }
        }

        public void AddAudioSource(SourceType sourceType, Sound sound)
        {
            if (!m_audioSourceDictionary.ContainsKey(sourceType))
            {
                m_audioSourceDictionary.Add(sourceType, sound);
            }
            else
            {
                Commons.Logger.LogWarning($"AudioManager::Can't not add {sourceType} SourceType. It already exists");
            }
        }

        [System.Serializable]
        public class AudioSourceDictionary : SerializableDictionary<SourceType, Sound> { }
    }
}