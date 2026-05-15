using EOM.TSHotelManagement.Data;
using EOM.TSHotelManagement.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EOM.TSHotelManagement.Service
{
    internal static class RoomReferenceHelper
    {
        public static RoomResolveResult Resolve(GenericRepository<Room> repository, int? roomId, string roomNumber)
        {
            return RoomLocatorHelper.Resolve(repository, roomId, null, null, null);
        }

        public static List<Room> LoadRooms(GenericRepository<Room> repository, IEnumerable<int?> roomIds, IEnumerable<string> roomNumbers)
        {
            var ids = roomIds?
                .Where(a => a.HasValue && a.Value > 0)
                .Select(a => a.Value)
                .Distinct()
                .ToList() ?? new List<int>();

            if (ids.Count == 0)
            {
                return new List<Room>();
            }

            return repository.AsQueryable()
                .Where(a => a.IsDelete != 1 && ids.Contains(a.Id))
                .ToList();
        }

        public static Room FindRoom(IEnumerable<Room> rooms, int? roomId, string roomNumber)
        {
            if (!roomId.HasValue || roomId.Value <= 0)
            {
                return null;
            }

            return rooms.FirstOrDefault(a => a.Id == roomId.Value);
        }

        public static string GetRoomArea(Room room)
        {
            return room?.RoomArea ?? string.Empty;
        }

        public static int? GetRoomFloor(Room room)
        {
            return room?.RoomFloor;
        }

        public static string GetRoomLocator(Room room, string fallbackRoomNumber = null)
        {
            if (room != null)
            {
                return RoomLocatorHelper.BuildLocator(room.RoomArea, room.RoomFloor, room.RoomNumber);
            }

            return string.Empty;
        }
    }
}
