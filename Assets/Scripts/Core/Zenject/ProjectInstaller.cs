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
        [SerializeField] Inventory.Inventory m_inventoryPrefab;
        // [SerializeField] GameManager m_gameManagerPrefab;
        public override void InstallBindings()
        {
            Container.Bind<SettingManager>().FromComponentInNewPrefab(m_settingManagerPrefab).AsSingle().NonLazy();
            Container.Bind<AudioManager>().FromComponentInNewPrefab(m_audioManagerPrefab).AsSingle().NonLazy();
            Container.Bind<LevelManager>().FromComponentInNewPrefab(m_levelManagerPrefab).AsSingle().NonLazy();
            Container.Bind<Inventory.InventoryLoader>().FromComponentInNewPrefab(m_inventoryLoaderPrefab).AsSingle().NonLazy();
            Container.Bind<Inventory.Inventory>().FromComponentInNewPrefab(m_inventoryPrefab).AsSingle().NonLazy();
            // Container.Bind<GameManager>().FromComponentInNewPrefab(m_gameManagerPrefab).AsSingle().NonLazy();
            Commons.Logger.Log("ProjectInstaller::InstallBindings()");
            Initialize();
        }

        private static void Initialize()
        {
            InputManager.Initialize();
        }
    }
}