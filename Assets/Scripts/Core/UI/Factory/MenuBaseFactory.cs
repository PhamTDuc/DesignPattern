using UnityEngine;
using Zenject;

namespace Guinea.Core.UI
{
    public class MenuBaseFactory : IFactory<MenuBase, Transform, MenuBase>, IValidatable
    {
        DiContainer m_container;

        [Inject]
        public MenuBaseFactory(DiContainer container)
        {
            m_container = container;
        }

        public MenuBase Create(MenuBase prefab, Transform parent)
        {
            MenuBase instance = GameObject.Instantiate(prefab, parent).GetComponent<MenuBase>();
            m_container.Inject(instance);
            return instance;
        }

        public void Validate()
        {
            m_container.InstantiateComponent<Menu>(new GameObject("TestMenuBaseFactory"));
        }
    }
}