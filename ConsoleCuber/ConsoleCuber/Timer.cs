using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCuber
{
    class Timer
    {
        public static double Now()
        {
            return DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }
    }
}
