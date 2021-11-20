using UnityEngine;
using Zenject;

namespace Guinea.Core.UI
{
    public class TabButtonFactory : IFactory<TabButton, Transform, string, TabButton>, IValidatable
    {
        DiContainer m_container;

        public TabButtonFactory(DiContainer container)
        {
            m_container = container;
        }

        public TabButton Create(TabButton prefab, Transform parent, string label)
        {
            TabButton instance = GameObject.Instantiate(prefab, parent).GetComponent<TabButton>();
            m_container.Inject(instance);
            instance.SetLabel(label);
            return instance;
        }

        public void Validate()
        {
            m_container.InstantiateComponent<TabButton>(new GameObject("TestTabButtonFactory"));
        }
    }
}