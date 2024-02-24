
namespace BNS360.Core.CustemExceptions
{
    public class ItemExsists : Exception
    {
        public ItemExsists(string? message = null)
            : base(message ?? nameof(ItemExsists)) 
        {
            
        }
    }
}
