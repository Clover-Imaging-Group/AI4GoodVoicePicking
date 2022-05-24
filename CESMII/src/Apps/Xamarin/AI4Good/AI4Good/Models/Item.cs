using System;
namespace AI4Good.Models
{
    public class Item
    {
        public Item()
        {
        }

        public int ItemId { get; set; }
        public string Name { get; set; }
        public string SKU { get; set; }
        public string Description { get; set; }
    }
}
