using EOM.TSHotelManagement.Data;
using EOM.TSHotelManagement.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EOM.TSHotelManagement.Service
{
    internal sealed class RoomResolveResult
    {
        public Room Room { get; init; }

        public bool IsAmbiguous { get; init; }
    }

    internal static class RoomLocatorHelper
    {
        public static RoomResolveResult Resolve(
            GenericRepository<Room> repository,
            int? roomId,
            string roomNumber,
            string roomArea,
            int? roomFloor)
        {
            if (roomId.HasValue && roomId.Value > 0)
            {
                var room = repository.GetFirst(a => a.Id == roomId.Value);

                return new RoomResolveResult { Room = room, IsAmbiguous = false };
            }

            if (string.IsNullOrWhiteSpace(roomNumber))
            {
                return new RoomResolveResult();
            }

            var normalizedNumber = roomNumber.Trim();
            var normalizedArea = NormalizeArea(roomArea);
            var candidates = repository.GetList(a => a.RoomNumber == normalizedNumber);

            if (!candidates.Any())
            {
                return new RoomResolveResult();
            }

            var hasArea = !string.IsNullOrWhiteSpace(normalizedArea);
            var hasFloor = roomFloor.HasValue;
            if (!hasArea && !hasFloor)
            {
                return candidates.Count == 1
                    ? new RoomResolveResult { Room = candidates[0], IsAmbiguous = false }
                    : new RoomResolveResult { IsAmbiguous = true };
            }

            var exactMatches = candidates
                .Where(a => (!hasArea || string.Equals(NormalizeArea(a.RoomArea), normalizedArea, StringComparison.OrdinalIgnoreCase))
                    && (!hasFloor || a.RoomFloor == roomFloor))
                .Take(2)
                .ToList();

            if (exactMatches.Count == 1)
            {
                return new RoomResolveResult { Room = exactMatches[0], IsAmbiguous = false };
            }

            return new RoomResolveResult { IsAmbiguous = exactMatches.Count > 1 };
        }

        public static string NormalizeArea(string area)
        {
            return string.IsNullOrWhiteSpace(area) ? string.Empty : area.Trim();
        }

        public static string BuildLocator(string roomArea, int? roomFloor, string roomNumber)
        {
            var parts = new List<string>();

            if (!string.IsNullOrWhiteSpace(roomArea))
            {
                parts.Add(roomArea.Trim());
            }

            if (roomFloor.HasValue)
            {
                parts.Add($"{roomFloor.Value}F");
            }

            if (!string.IsNullOrWhiteSpace(roomNumber))
            {
                parts.Add(roomNumber.Trim());
            }

            return string.Join(" / ", parts);
        }
    }
}
