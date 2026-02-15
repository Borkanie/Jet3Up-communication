using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jet3UpCommLib.Helpers
{
    static public class Commons
    {        
        /// <inheritdoc/>
        static public int NumberOfDigitsInInt(int expectedQuantity)
        {
            int result = 0;
            while (expectedQuantity > 0)
            {
                expectedQuantity = expectedQuantity / 10;
                result++;
            }
            return result;
        }

    }
}
