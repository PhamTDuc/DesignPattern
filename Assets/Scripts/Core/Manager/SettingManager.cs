using UnityEngine;
using UnityEngine.Audio;

namespace Guinea.Core
{
    public class SettingManager : MonoBehaviour
    {
        [SerializeField] AudioMixer m_audioMixer;

        public Setting SettingValues { get; private set; } = new Setting(10f, 0.2f, 0.5f);

        public static readonly string VOLUME = "volume";

        public void SetVolume(float volume)
        {
            m_audioMixer.SetFloat(VOLUME, volume);
        }

        public struct Setting
        {
            public float volume;
            public float cameraSpeed;
            public float zoomSpeed;
            public Setting(float volume, float cameraSpeed, float zoomSpeed)
            {
                this.volume = volume;
                this.cameraSpeed = cameraSpeed;
                this.zoomSpeed = zoomSpeed;
            }
        }
    }
}
