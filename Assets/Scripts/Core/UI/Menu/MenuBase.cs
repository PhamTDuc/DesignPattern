using UnityEngine;
using Zenject;

namespace Guinea.Core.UI
{

    public abstract class MenuBase : MonoBehaviour
    {
        [SerializeField] MenuType m_type;
        [SerializeField] bool m_destroyWhenClose;
        [SerializeField] bool m_disableUnderneath;
        [SerializeField] bool m_disableSameLayer;
        [SerializeField] int m_layer;
        public MenuType Type { get => m_type; }
        public bool DestroyWhenClose { get => m_destroyWhenClose; }
        public bool DisableUnderneath { get => m_disableUnderneath; }
        public bool DisableSameLayer { get => m_disableSameLayer; }
        public int Layer { get => m_layer; }

        protected MenuManager m_menuManager;

        [Inject]
        public void Initialize(MenuManager menuManager)
        {
            m_menuManager = menuManager;
        }

        public abstract void OnBackKeyEvent();
        public abstract void OnOpenMenu();
        public abstract void OnCloseMenu();

        public class Factory : PlaceholderFactory<MenuBase, Transform, MenuBase> { }
    }
}