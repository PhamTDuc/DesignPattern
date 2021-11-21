using UnityEngine;
using Zenject;
using Guinea.Core;
using Guinea.Core.UI;
using Guinea.Core.Inventory;
using Guinea.Core.Components;

namespace Guinea.Test
{
    public class TestSceneContext : MonoInstaller
    {
        [SerializeField] ItemUI m_itemUI;

        [TextArea(10, 20)]
        [SerializeField] string m_inventoryJson;

        // * DiContainer always inject yourself
        public override void InstallBindings()
        {
            Container.Bind<MenuManager>().FromComponentInHierarchy().AsSingle();
            Container.BindFactory<MenuBase, UnityEngine.Transform, MenuBase, MenuBase.Factory>().FromFactory<MenuBaseFactory>();

            Container.Bind<TabMenu>().FromComponentInParents(true);
            Container.BindFactory<TabButton, UnityEngine.Transform, string, TabButton, TabButton.Factory>().FromFactory<TabButtonFactory>();

            Container.Bind<ItemUIDrag>().FromComponentInHierarchy().AsSingle();
            Container.BindFactory<ItemType, UnityEngine.Transform, Sprite, int, ItemUI, ItemUI.Factory>().FromIFactory(x => x.To<ItemUIFactory>().AsSingle().WithArguments<ItemUI>(m_itemUI));

            Container.Bind<SharedInteraction>().FromComponentInHierarchy().AsSingle();
            Container.Bind<OperatorManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<InventoryUI>().FromComponentInHierarchy().AsSingle();
            Container.Bind<Inventory>().AsSingle().WithArguments(m_inventoryJson);


            #region Initialize
            InputManager.SetActionMap(InputManager.Map.EntityBuilder, true);
            InputManager.SetActionMap(InputManager.Map.Player, true);
            // InputManager.SetCursor(false);
            #endregion
        }
    }
}