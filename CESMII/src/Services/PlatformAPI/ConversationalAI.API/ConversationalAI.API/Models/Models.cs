using System.Collections.Generic;
using Newtonsoft.Json;

namespace ConversationalAI.API.Models
{
    public class WarehouseEquipmentList
    {
        [JsonProperty("attributes")]
        public List<WarehouseEquipment> WarehouseEquipment { get; set; }
    }

    public class Item
    {
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
    }
    public class WarehouseEquipment
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("onEquipment")]
        public Item Equipment { get; set; }
        public string DisplayName => Equipment.DisplayName;

        [JsonProperty("description")]
        public string Description { get; set; }

        private int _quantity;

        [JsonProperty("intValue")]
        public int? Quantity
        {
            get => _quantity;
            set => _quantity = value ?? 0;
        }
    }

    public class EquipmentQuantityUpdateRequest
    {
        public int Id { get; set; }
        public int Value { get; set; }
    }
}