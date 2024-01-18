using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Core.CustemExceptions
{
    public class ItemNotFoundException : Exception
    {
        public ItemNotFoundException(string? message) : base(message) { }
        public static void Throw(string? message) =>
            throw new ItemNotFoundException(message);
   

        public static void ThrowIfNull([NotNull] object? item, string paramName)
        {
            if (item is null)
                Throw($"{paramName} Not Found");
        }


    }
}
