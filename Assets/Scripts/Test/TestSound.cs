using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;
using Guinea.Core;

namespace Guinea.Test
{
    public class TestSound : MonoBehaviour
    {
#if DEVELOPMENT

        AudioManager m_audioManager;

        [Inject]
        void Initialize(AudioManager audioManager)
        {
            m_audioManager = audioManager;
        }

        void Update()
        {
            if (Keyboard.current[Key.G].wasPressedThisFrame)
            {
                m_audioManager.PlayOneShot(SourceType.UI, ClipType.UI_CLICK);
            }
        }
#endif
    }
}