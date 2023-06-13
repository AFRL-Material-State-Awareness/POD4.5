using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POD
{
    [Serializable]
    public class FlipBinarySign : IFlipBinarySign
    {
        public int FlipBits(int inputBinary)
        {
            return ~inputBinary;
        }
    }
    public interface IFlipBinarySign
    {
        int FlipBits(int inputBinary);
    }

}
