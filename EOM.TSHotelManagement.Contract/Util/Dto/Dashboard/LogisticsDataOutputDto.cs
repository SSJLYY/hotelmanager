using System.Text.Json.Serialization;

namespace EOM.TSHotelManagement.Contract
{
    public class LogisticsDataOutputDto
    {
        [JsonPropertyName("totalProducts")]
        public int TotalProducts { get; set; }

        [JsonPropertyName("inventoryWarning")]
        public TempInventoryWarning InventoryWarning { get; set; }

        [JsonPropertyName("recentRecords")]
        public List<TempInventoryRecord> RecentRecords { get; set; }
    }

    public class TempInventoryWarning
    {
        [JsonPropertyName("percent")]
        public int Percent { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        public List<string> LowStockProducts { get; set; }
    }

    public class TempInventoryRecord
    {
        [JsonPropertyName("id")]
        public string RecordId { get; set; }

        [JsonPropertyName("type")]
        public TempInventoryOperationType OperationType { get; set; }

        [JsonPropertyName("productName")]
        public string ProductName { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
    }

    public enum TempInventoryOperationType
    {
        Outbound = 1,
        Inbound = 2
    }

}
