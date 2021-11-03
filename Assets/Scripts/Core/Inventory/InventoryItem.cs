
namespace Guinea.Core.Inventory
{
    public class InventoryItem
    {
        private ItemType m_itemType;
        public ItemType Type => m_itemType;
        private int m_quantity;
        public int Quantity => m_quantity;
        public void Add(int quantity) => m_quantity += quantity;
        public void Use(int quantity) => m_quantity = m_quantity <= quantity ? 0 : m_quantity - quantity;
    }
}