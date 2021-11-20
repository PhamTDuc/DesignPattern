using UnityEngine;
using Zenject;
using Guinea.Core.UI;
using Guinea.Core.Inventory;
using Guinea.Core.Components;

namespace Guinea.Core
{
    public class EntityBuilder : MonoInstaller
    {
        [SerializeField] ItemUI m_itemUI;
        [TextArea(10, 20)]
        [SerializeField] string m_inventoryJson;
        public override void InstallBindings()
        {
            Container.Bind<TabMenu>().FromComponentInParents();
            Container.BindFactory<TabButton, UnityEngine.Transform, string, TabButton, TabButton.Factory>().FromFactory<TabButtonFactory>();

            Container.Bind<MenuManager>().FromComponentInHierarchy().AsSingle();
            Container.BindFactory<MenuBase, UnityEngine.Transform, MenuBase, MenuBase.Factory>().FromFactory<MenuBaseFactory>();

            Container.Bind<ItemUIDrag>().FromComponentInHierarchy().AsSingle();
            Container.BindFactory<ItemType, UnityEngine.Transform, Sprite, int, ItemUI, ItemUI.Factory>().FromIFactory(x => x.To<ItemUIFactory>().AsSingle().WithArguments<ItemUI>(m_itemUI));

            Container.Bind<Components.Context>().AsSingle().NonLazy();
            Container.Bind<SharedInteraction>().FromComponentInHierarchy().AsSingle();
            Container.Bind<OperatorManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<ManipulationManager>().FromComponentInHierarchy().AsSingle();

            // Container.Bind<Inventory.Inventory>().AsSingle().WithArguments(m_inventoryJson);
            Container.Bind<Inventory.Inventory>().FromComponentInHierarchy().AsSingle();
            Container.BindFactory<ItemType, Transform, AddComponentCommand, AddComponentCommand.Factory>();

            Init();
            Commons.Logger.Log("EntityBuilder::InstallBindings()");
        }

        private void Init()
        {
            InputManager.SetActionMap(InputManager.Map.EntityBuilder, true);
        }
    }
}