using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Guinea.Core.Inventory
{
    public class ItemUIFactory : IFactory<ItemType, UnityEngine.Transform, Sprite, ItemUI>
    {
        DiContainer m_container;
        ItemUI m_prefab;
        [Inject]

        void Initialize(DiContainer container)
        {
            m_container = container;
        }

        public ItemUIFactory(ItemUI prefab)
        {
            m_prefab = prefab;
        }

        public ItemUI Create(ItemType itemType, Transform parent = null, Sprite sprite = null)
        {
            ItemUI item = GameObject.Instantiate<ItemUI>(m_prefab, parent);
            m_container.Inject(item);
            item.SetItemType(itemType);
            item.transform.GetChild(0).GetComponent<Image>().sprite = sprite;
            return item;
        }
    }
}