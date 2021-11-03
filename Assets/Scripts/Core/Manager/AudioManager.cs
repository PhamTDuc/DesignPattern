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
                Debug.LogWarning($"AudioManager::{sourceType} SourceType not found");
            }
        }

        [System.Serializable]
        public class AudioSourceDictionary : SerializableDictionary<SourceType, Sound> { }
    }
}