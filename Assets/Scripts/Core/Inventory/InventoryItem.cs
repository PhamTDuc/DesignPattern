
namespace Guinea.Core.Inventory
{
    public class InventoryItem
    {
        public ItemType type;
        public int quantity;

        public InventoryItem(ItemType type, int quantity)
        {
            this.type = type;
            this.quantity = quantity;
        }
    }
}