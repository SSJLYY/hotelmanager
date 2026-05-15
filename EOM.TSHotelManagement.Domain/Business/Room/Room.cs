using SqlSugar;
using System;

namespace EOM.TSHotelManagement.Domain
{
    [SugarTable("room", "Hotel room information")]
    public class Room : SoftDeleteEntity
    {
        [SugarColumn(ColumnName = "id", IsIdentity = true, IsPrimaryKey = true, IsNullable = false, ColumnDescription = "ID")]
        public int Id { get; set; }

        [SugarColumn(
            ColumnName = "room_no",
            ColumnDescription = "Room number",
            IsNullable = false,
            Length = 128,
            UniqueGroupNameList = new[] { "UK_room_number_area_floor" }
        )]
        public string RoomNumber { get; set; }

        [SugarColumn(
            ColumnName = "room_area",
            ColumnDescription = "Room area",
            IsNullable = true,
            Length = 128,
            UniqueGroupNameList = new[] { "UK_room_number_area_floor" }
        )]
        public string RoomArea { get; set; }

        [SugarColumn(
            ColumnName = "room_floor",
            ColumnDescription = "Room floor",
            IsNullable = true,
            UniqueGroupNameList = new[] { "UK_room_number_area_floor" }
        )]
        public int? RoomFloor { get; set; }

        [SugarColumn(
            ColumnName = "room_type",
            ColumnDescription = "Room type ID",
            IsNullable = false
        )]
        public int RoomTypeId { get; set; }

        [SugarColumn(
            ColumnName = "custo_no",
            ColumnDescription = "Linked customer number",
            IsNullable = true,
            Length = 128
        )]
        public string CustomerNumber { get; set; }

        [SugarColumn(IsIgnore = true)]
        public string CustomerName { get; set; }

        [SugarColumn(
            ColumnName = "check_in_time",
            ColumnDescription = "Last check-in time",
            IsNullable = true
        )]
        public DateTime? LastCheckInTime { get; set; }

        [SugarColumn(
            ColumnName = "check_out_time",
            ColumnDescription = "Last check-out time",
            IsNullable = true
        )]
        public DateTime? LastCheckOutTime { get; set; }

        [SugarColumn(
            ColumnName = "room_state_id",
            ColumnDescription = "Room state ID",
            IsNullable = false
        )]
        public int RoomStateId { get; set; }

        [SugarColumn(IsIgnore = true)]
        public string RoomState { get; set; }

        [SugarColumn(
            ColumnName = "room_rent",
            ColumnDescription = "Room rent",
            IsNullable = false,
            DecimalDigits = 2
        )]
        public decimal RoomRent { get; set; }

        [SugarColumn(
            ColumnName = "room_deposit",
            ColumnDescription = "Room deposit",
            IsNullable = false,
            DecimalDigits = 2,
            DefaultValue = "0.00"
        )]
        public decimal RoomDeposit { get; set; }

        [SugarColumn(
            ColumnName = "applied_room_rent",
            ColumnDescription = "Applied room rent for current stay",
            IsNullable = false,
            DecimalDigits = 2,
            DefaultValue = "0.00"
        )]
        public decimal AppliedRoomRent { get; set; }

        [SugarColumn(
            ColumnName = "applied_room_deposit",
            ColumnDescription = "Applied room deposit for current stay",
            IsNullable = false,
            DecimalDigits = 2,
            DefaultValue = "0.00"
        )]
        public decimal AppliedRoomDeposit { get; set; }

        [SugarColumn(
            ColumnName = "pricing_code",
            ColumnDescription = "Applied pricing code",
            IsNullable = true,
            Length = 64
        )]
        public string RoomPricingCode { get; set; }

        [SugarColumn(
            ColumnName = "pricing_name",
            ColumnDescription = "Applied pricing name",
            IsNullable = true,
            Length = 128
        )]
        public string RoomPricingName { get; set; }

        [SugarColumn(
            ColumnName = "pricing_stay_hours",
            ColumnDescription = "Applied pricing allowed stay hours",
            IsNullable = true
        )]
        public int? PricingStayHours { get; set; }

        [SugarColumn(
            ColumnName = "pricing_start_time",
            ColumnDescription = "Applied pricing timing start time",
            IsNullable = true
        )]
        public DateTime? PricingStartTime { get; set; }

        [SugarColumn(
            ColumnName = "room_location",
            ColumnDescription = "Room location",
            IsNullable = false,
            Length = 200
        )]
        public string RoomLocation { get; set; }

        [SugarColumn(IsIgnore = true)]
        public string CustomerTypeName { get; set; }

        [SugarColumn(IsIgnore = true)]
        public string RoomName { get; set; }

        [SugarColumn(IsIgnore = true)]
        public string RoomLocator { get; set; }

        [SugarColumn(IsIgnore = true)]
        public string LastCheckInTimeFormatted { get; set; }
    }
}
