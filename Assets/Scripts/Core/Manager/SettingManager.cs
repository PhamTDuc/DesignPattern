using UnityEngine;
using UnityEngine.Audio;

namespace Guinea.Core
{
    public class SettingManager : MonoBehaviour
    {
        [SerializeField]AudioMixer audioMixer;

        public static readonly string VOLUME = "volume";
        
        public void SetVolume(float volume)
        {
            audioMixer.SetFloat(VOLUME, volume);
        }
    }
}
