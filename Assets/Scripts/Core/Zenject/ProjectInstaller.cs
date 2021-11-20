using UnityEngine;
using Zenject;

namespace Guinea.Core
{
    public class ProjectInstaller : MonoInstaller
    {
        [SerializeField] SettingManager m_settingManagerPrefab;
        [SerializeField] AudioManager m_audioManagerPrefab;
        [SerializeField] LevelManager m_levelManagerPrefab;
        [SerializeField] Inventory.InventoryLoader m_inventoryLoaderPrefab;
        public override void InstallBindings()
        {
            Container.Bind<SettingManager>().FromComponentInNewPrefab(m_settingManagerPrefab).AsSingle().NonLazy();
            Container.Bind<AudioManager>().FromComponentInNewPrefab(m_audioManagerPrefab).AsSingle().NonLazy();
            Container.Bind<LevelManager>().FromComponentInNewPrefab(m_levelManagerPrefab).AsSingle().NonLazy();
            Container.Bind<Inventory.InventoryLoader>().FromComponentInNewPrefab(m_inventoryLoaderPrefab).AsSingle().NonLazy();
            Commons.Logger.Log("ProjectInstaller::InstallBindings()");
            Init();
        }

        private static void Init()
        {
            InputManager.Init();
        }
    }
}