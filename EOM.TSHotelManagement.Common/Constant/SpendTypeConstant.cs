using EOM.TSHotelManagement.Infrastructure;

namespace EOM.TSHotelManagement.Common
{
    public class SpendTypeConstant : CodeConstantBase<SpendTypeConstant>
    {
        public static readonly SpendTypeConstant Product = new SpendTypeConstant("Goods", "商品");
        public static readonly SpendTypeConstant Room = new SpendTypeConstant("Room", "房间");
        public static readonly SpendTypeConstant Other = new SpendTypeConstant("Other", "其他");

        protected SpendTypeConstant(string code, string description) : base(code, description)
        {
        }
    }
}
