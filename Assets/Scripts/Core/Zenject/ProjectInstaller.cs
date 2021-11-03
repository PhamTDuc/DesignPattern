using UnityEngine;
using Zenject;

namespace Guinea.Core
{
    public class ProjectInstaller : MonoInstaller
    {
        [SerializeField] AudioManager m_audioManager;
        [SerializeField] SettingManager m_settingManager;
        [SerializeField] LevelManager m_levelManager;

        public override void InstallBindings()
        {
            Container.Bind<AudioManager>().FromComponentInNewPrefab(m_audioManager).AsSingle();
            Container.Bind<SettingManager>().FromComponentInNewPrefab(m_settingManager).AsSingle();
            Container.Bind<LevelManager>().FromComponentInNewPrefab(m_levelManager).AsSingle();

            Initialize();
        }

        // * This will called to initialize static class when no constructor specified
        private static void Initialize()
        {
            // * InputManager only enables UI mapping when game starts, other mappings will be enabled manually when in use
            InputManager.Initialize();
        }
    }
}