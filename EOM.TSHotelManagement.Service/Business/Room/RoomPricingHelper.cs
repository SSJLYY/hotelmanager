using EOM.TSHotelManagement.Contract;
using System.Text.Json;

namespace EOM.TSHotelManagement.Service
{
    internal static class RoomPricingHelper
    {
        public const string DefaultPricingCode = "STANDARD";
        public const string DefaultPricingName = "标准价";

        public static List<RoomTypePricingItemDto> BuildPricingItems(decimal roomRent, decimal roomDeposit, string pricingItemsJson)
        {
            var items = new List<RoomTypePricingItemDto>
            {
                CreateDefaultPricingItem(roomRent, roomDeposit)
            };

            items.AddRange(DeserializeAdditionalPricingItems(pricingItemsJson));
            return items;
        }

        public static RoomTypePricingItemDto CreateDefaultPricingItem(decimal roomRent, decimal roomDeposit)
        {
            return new RoomTypePricingItemDto
            {
                PricingCode = DefaultPricingCode,
                PricingName = DefaultPricingName,
                RoomRent = roomRent,
                RoomDeposit = roomDeposit,
                Sort = 0,
                IsDefault = true
            };
        }

        public static string SerializeAdditionalPricingItems(IEnumerable<RoomTypePricingItemDto>? pricingItems)
        {
            var normalized = NormalizeAdditionalPricingItems(pricingItems).ToList();
            return normalized.Count == 0 ? "[]" : JsonSerializer.Serialize(normalized);
        }

        public static RoomTypePricingItemDto? ResolvePricingItem(decimal roomRent, decimal roomDeposit, string pricingItemsJson, string? pricingCode)
        {
            var normalizedCode = NormalizePricingCode(pricingCode);
            var items = BuildPricingItems(roomRent, roomDeposit, pricingItemsJson);
            if (string.IsNullOrWhiteSpace(normalizedCode))
            {
                return items.FirstOrDefault();
            }

            return items.FirstOrDefault(a => string.Equals(a.PricingCode, normalizedCode, StringComparison.OrdinalIgnoreCase));
        }

        public static string NormalizePricingCode(string? pricingCode)
        {
            return string.IsNullOrWhiteSpace(pricingCode)
                ? string.Empty
                : pricingCode.Trim().ToUpperInvariant();
        }

        private static List<RoomTypePricingItemDto> DeserializeAdditionalPricingItems(string pricingItemsJson)
        {
            if (string.IsNullOrWhiteSpace(pricingItemsJson))
            {
                return new List<RoomTypePricingItemDto>();
            }

            try
            {
                var items = JsonSerializer.Deserialize<List<RoomTypePricingItemDto>>(pricingItemsJson) ?? new List<RoomTypePricingItemDto>();
                return NormalizeAdditionalPricingItems(items).ToList();
            }
            catch
            {
                return new List<RoomTypePricingItemDto>();
            }
        }

        private static IEnumerable<RoomTypePricingItemDto> NormalizeAdditionalPricingItems(IEnumerable<RoomTypePricingItemDto>? pricingItems)
        {
            if (pricingItems == null)
            {
                yield break;
            }

            var seenCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in pricingItems.OrderBy(a => a?.Sort ?? 0))
            {
                if (item == null)
                {
                    continue;
                }

                var code = NormalizePricingCode(item.PricingCode);
                if (string.IsNullOrWhiteSpace(code) || string.Equals(code, DefaultPricingCode, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (!seenCodes.Add(code))
                {
                    continue;
                }

                yield return new RoomTypePricingItemDto
                {
                    PricingCode = code,
                    PricingName = string.IsNullOrWhiteSpace(item.PricingName) ? code : item.PricingName.Trim(),
                    RoomRent = item.RoomRent,
                    RoomDeposit = item.RoomDeposit,
                    StayHours = item.StayHours > 0 ? item.StayHours : null,
                    Sort = item.Sort,
                    IsDefault = false
                };
            }
        }
    }
}
