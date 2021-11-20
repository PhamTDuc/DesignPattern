using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Guinea.Core.Inventory
{
    public class ItemUIFactory : IFactory<ItemType, UnityEngine.Transform, Sprite, int, ItemUI>
    {
        ItemUI m_prefab;
        DiContainer m_container;

        public ItemUIFactory(DiContainer container, ItemUI prefab)
        {
            m_prefab = prefab;
            m_container = container;
        }

        public ItemUI Create(ItemType itemType, Transform parent = null, Sprite sprite = null, int count = 0)
        {
            ItemUI item = GameObject.Instantiate<ItemUI>(m_prefab, parent);
            m_container.Inject(item);
            item.Init(itemType, count);
            item.transform.GetChild(0).GetComponent<Image>().sprite = sprite;
            return item;
        }
    }
}