using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Guinea.Core;
using Guinea.Core.UI;

namespace Guinea.Test.UI
{
    public class SettingMenu : MenuBase
    {
        [SerializeField] Slider m_volumeSlider;
        SettingManager m_settingManager;

        [Inject]
        void Initialize(SettingManager settingManager)
        {
            m_settingManager = settingManager;
        }

        public override void OnBackKeyEvent()
        {
            m_menuManager.CloseTopMenu();
        }

        public override void OnOpenMenu() { }

        public override void OnCloseMenu() { }
        public void OnConfirm()
        {
            m_settingManager.SetVolume(m_volumeSlider.value);
        }

    }
}
