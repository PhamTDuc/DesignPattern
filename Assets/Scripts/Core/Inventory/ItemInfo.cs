namespace Guinea.Core.Inventory
{
    public class ItemInfo
    {
        public string type;
        // public string description;
        public string spriteAddress;
        public string objectAddress;


        public ItemInfo(string type, string spriteAddress, string objectAddress)
        {
            this.type = type;
            //    this.description = description;
            this.spriteAddress = spriteAddress;
            this.objectAddress = objectAddress;
        }
    }
}